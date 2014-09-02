using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TextProcessing
{
    /// <summary>
    /// Text writer that supports indenting for string creation
    /// </summary>
    public class IndentableTextWriter : TextWriter
    {
        private StringBuilder _innerStringBuilderx = null;
        private StringBuilder _associatedStringBuilder = null;
        private char[] _currentLineContent = null; // Needs to be initially set to null to indicate this object has not been inialized
        private char[] _currentLineIndentx = new char[0]; // needs to be initially set to zero-length value to indicate that object is not disposed
        private string _indentText = "\t";

        protected bool IsDisposed
        {
            get { return this._currentLineIndentx == null; }
            private set
            {
                if (!value)
                    return;

                this._innerStringBuilderx = null;
                this._associatedStringBuilder = null;
                this._currentLineContent = null;
                this._currentLineIndentx = null;
                this._indentText = null;
            }
        }

        public bool IsClosed
        {
            get { return this._innerStringBuilderx == null; }
            private set
            {
                if (!value)
                    return;

                this._innerStringBuilderx = null;
                this._associatedStringBuilder = null;
            }
        }

        public StringBuilder AssociatedStringBuilder
        {
            get { return this._associatedStringBuilder; }
            private set { this._associatedStringBuilder = value; }
        }

        protected StringBuilder InnerStringBuilder
        {
            get { return this._innerStringBuilderx; }
            private set { this._innerStringBuilderx = (value == null) ? new StringBuilder() : value; }
        }

        protected char[] CurrentLineIndent
        {
            get { return this._currentLineIndentx; }
            private set { this._currentLineIndentx = (value == null) ? new char[0] : value; }
        }

        /// <summary>
        /// Returns the System.Text.Encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding { get { return Encoding.Unicode; } }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        public override string NewLine
        {
            get { return base.NewLine; }
            set { base.NewLine = (value == null) ? "" : value; }
        }

        /// <summary>
        /// Gets or sets the text used for indenting lines
        /// </summary>
        public string IndentText
        {
            get { return this._indentText; }
            set
            {
                string s = (value == null) ? "" : value;

                if (this._indentText == s)
                    return;
                
                this._indentText = s;

                if (this.CurrentLineIndent != null) // Prevents a property from throwing an exception after it has been closed
                    this.UpdateCurrentLineIndent();
            }
        }

        /// <summary>
        /// The current nubmer of times each line is indented
        /// </summary>
        public int IndentLevel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessing.IndentableTextWriter"/> class.
        /// </summary>
        public IndentableTextWriter() : this(null as StringBuilder) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessing.IndentableTextWriter"/> class that writes to the specified <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The StringBuilder to write to.</param>
        public IndentableTextWriter(StringBuilder sb)
            : base()
        {
            this.OpenWrite(sb, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessing.IndentableTextWriter"/> class with the specified format provider.
        /// </summary>
        /// <param name="formatProvider">An <see cref="System.IFormatProvider"/> object that controls formatting.</param>
        public IndentableTextWriter(IFormatProvider formatProvider) : this(null, formatProvider) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProcessing.IndentableTextWriter"/> class with the specified format provider that writes to the specified <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The StringBuilder to write to.</param>
        /// <param name="formatProvider">An <see cref="System.IFormatProvider"/> object that controls formatting.</param>
        public IndentableTextWriter(StringBuilder sb, IFormatProvider formatProvider)
            : base(formatProvider)
        {
            this.OpenWrite(sb, true);
        }

        private void UpdateCurrentLineIndent()
        {
            this.CurrentLineIndent = new char[0];
            if (this.IndentText.Length == 0)
                return;

            for (int i = 0; i < this.IndentLevel; i++)
                this.CurrentLineIndent = this.CurrentLineIndent.Concat(this.IndentText.ToCharArray()).ToArray();
        }

        private void AssertIsWritable()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);

            if (this.IsClosed)
                throw new ObjectDisposedException(this.GetType().FullName, "This writer has been closed and disassociated from its StringBuilder object.");
        }

        /// <summary>
        /// Increases the indent level
        /// </summary>
        /// <remarks>This only increases the indent level for lines which have not yet been written.</remarks>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public void Indent()
        {
            this.AssertIsWritable();

            this.IndentLevel++;
            this.UpdateCurrentLineIndent();
        }

        /// <summary>
        /// Decreases the indent level
        /// </summary>
        /// <remarks>This only decreases the indent level for lines which have not yet been written.</remarks>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public void Unindent()
        {
            this.AssertIsWritable();

            if (this.IndentLevel == 0)
                return;

            this.IndentLevel--;
            this.UpdateCurrentLineIndent();
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">Starting index in the buffer.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="System.ArgumentNullException">The buffer parameter is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index or count is negative.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public override void Write(char[] buffer, int index, int count)
        {
            this.AssertIsWritable();

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index out of range");

            if (index >= buffer.Length)
                return;

            if (this.NewLine.Length == 0 && this._currentLineContent.Length == 0)
            {
                this.InnerStringBuilder.Append(buffer, index, count);
                return;
            }

            char[] nl = this.NewLine.ToCharArray();

            this._currentLineContent = this._currentLineContent.Concat(buffer.Skip(index).Take(count)).ToArray();
            char[] incompleteTerminator = this._currentLineContent.GetTrailingIncompleteCharSeq(nl);
            char[][] lines = this._currentLineContent.Take(this._currentLineContent.Length - incompleteTerminator.Length).ToLines(nl, true).ToArray();
            if (lines.Length == 0)
                this._currentLineContent = incompleteTerminator;
            else if (lines.Length == 1)
                this._currentLineContent = lines[0].Concat(incompleteTerminator).ToArray();
            else
            {
                this._currentLineContent = lines.Last().Concat(incompleteTerminator).ToArray();
                foreach (char[] l in lines)
                    this.InnerStringBuilder.AppendFormat("{0}{1}", (new String(this.CurrentLineIndent.Concat(l).ToArray())).TrimEnd(), this.NewLine);
            }
        }

        /// <summary>
        ///  Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public override void Write(char value)
        {
            this.Write(new char[] { value }, 0, 1);
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the associated <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public override void Flush()
        {
            this.AssertIsWritable();

            if (this._currentLineContent.Length == 0)
                return;

            this.InnerStringBuilder.Append(this.GetFinalText());
            this._currentLineContent = new char[0];
        }

        /// <summary>
        /// Open a <see cref="System.Text.StringBuilder"/> for writing.
        /// </summary>
        /// <param name="sb">StringBuilder which this <see cref="TextProcessing.IndentableTextWriter"/> will write to.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="sb"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">Another <see cref="System.Text.StringBuilder"/> is already open for writing by this <see cref="TextProcessing.IndentableTextWriter"/>.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>Anything that had been previously written to this <see cref="TextProcessing.IndentableTextWriter"/> will be discraded.</remarks>
        public void OpenWrite(StringBuilder sb)
        {
            this.OpenWrite(sb, false);
        }

        /// <summary>
        /// Open a <see cref="System.Text.StringBuilder"/> for writing.
        /// </summary>
        /// <param name="sb">StringBuilder which this <see cref="TextProcessing.IndentableTextWriter"/> will write to.</param>
        /// <param name="force">Set to true if you wish to force it to open a new StringBuilder, even if another one had already been opened.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="sb"/> is null and <paramref name="force"/> is false.</exception>
        /// <exception cref="System.InvalidOperationException"><paramref name="force"/> is false, and another <see cref="System.Text.StringBuilder"/> is already open for writing by this <see cref="TextProcessing.IndentableTextWriter"/>.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If a new StringBuilder is opened while another had already been opened, then all output is flushed to the previous StringBuilder before it is released from writing.
        /// Otherwise, anything that had been written to this <see cref="TextProcessing.IndentableTextWriter"/> will be discraded.</remarks>
        public void OpenWrite(StringBuilder sb, bool force)
        {
            if (this._currentLineContent == null) // This will only be null when it is called for the first time, from the constructors, for initialization
            {
                if (this.IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);

                this._currentLineContent = new char[0];
            }
            else
                this.AssertIsWritable();

            if (this.AssociatedStringBuilder != null)
            {
                if (Object.ReferenceEquals(this.AssociatedStringBuilder, sb))
                    return;

                if (!force)
                    throw new InvalidOperationException("This TextWriter is already associated with a StringBuilder object.");

                this.Flush();
                this.InnerStringBuilder = sb;
                this.AssociatedStringBuilder = sb;
                this.IndentLevel = 0;
                this.UpdateCurrentLineIndent();
                return;
            }

            if (sb == null)
            {
                if (!force)
                    throw new ArgumentNullException("sb", "You must provide a StringBuilder object is 'force' is not set to true.");

                this.InnerStringBuilder = new StringBuilder();
            }
            else
            {
                if (Object.ReferenceEquals(this.InnerStringBuilder, sb))
                    return;

                this.InnerStringBuilder = sb;
            }

            this.AssociatedStringBuilder = sb;
            if (this._currentLineContent.Length > 0)
                this._currentLineContent = new char[0];
            this.IndentLevel = 0;
            this.UpdateCurrentLineIndent();
        }

        /// <summary>
        /// Stop writing to current <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <returns>StringBuilder which had been opened for writing.</returns>
        /// <exception cref="System.InvalidOperationException">No <see cref="System.Text.StringBuilder"/> was open for writing.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>Any buffered data is flushed to the opened StringBuilder before it is released.</remarks>
        public StringBuilder ReleaseStringBuilder()
        {
            this.AssertIsWritable();

            if (this.AssociatedStringBuilder == null)
                throw new InvalidOperationException("No StringBuilder object was open for writing.");

            StringBuilder result = this.AssociatedStringBuilder;
            this.Flush();
            this.IndentLevel = 0;
            this.UpdateCurrentLineIndent();
            this.AssociatedStringBuilder = null;
            this.InnerStringBuilder = new StringBuilder();

            return result;
        }

        /// <summary>
        /// Flushes any buffered data and disassociates the <see cref="System.Text.StringBuilder"/> the current writer and releases any system resources associated with the writer.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is disposed.</exception>
        public override void Close()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);

            if (!this.IsClosed)
            {
                if (this.AssociatedStringBuilder != null)
                    this.ReleaseStringBuilder();
                else
                    this.Flush();
                this._currentLineContent = this.InnerStringBuilder.ToString().ToCharArray();
                this.IsClosed = true;
            }

            base.Close();
        }

        /// <summary>
        /// Returns a hash code from everything which has been written so far.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a string containing everything which has been written so far.
        /// </summary>
        /// <returns>a string reprenting everything which has been written so far.</returns>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is disposed.</exception>
        public override string ToString()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);

            if (this.IsClosed)
                return new String(this._currentLineContent);

            return this.InnerStringBuilder.ToString() + this.GetFinalText();
        }

        private string GetFinalText()
        {
            if (this._currentLineContent.Length == 0)
                return "";

            if (this.NewLine.Length == 0)
                return new String(this.CurrentLineIndent.Concat(this._currentLineContent).ToArray()).TrimEnd();

            return String.Join(this.NewLine, this._currentLineContent.ToLines(this.NewLine.ToCharArray(), true)
                .Select(c => new String(this.CurrentLineIndent.Concat(c).ToArray()).TrimEnd()).ToArray());
        }

        /// <summary>
        /// Flushes any buffered data, releases the unmanaged resources used by the <see cref="TextProcessing.IndentableTextWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);

            if (this.AssociatedStringBuilder != null)
                this.Flush();
                
            if (disposing)
                this.IsDisposed = true;
            else
                this.IsClosed = true;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Runs the specified <see cref="System.Action&lt;IndentableTextWriter&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <param name="callback">Action to invoke</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public void RunActionAsIndented(Action<IndentableTextWriter> callback)
        {
            this.Indent();
            try
            {
                callback(this);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }
        }

        /// <summary>
        /// Runs the specified <see cref="System.Action&&lt;IndentableTextWriter, T&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="T">Type of argument to be passed.</typeparam>
        /// <param name="arg">Argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Action to invoke.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public void RunActionAsIndented<T>(T arg, Action<IndentableTextWriter, T> callback)
        {
            this.Indent();
            try
            {
                callback(this, arg);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }
        }

        /// <summary>
        /// Runs the specified <see cref="System.Action&&lt;IndentableTextWriter, TArg1, TArg2&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TArg1">First argument type to be passed.</typeparam>
        /// <typeparam name="TArg2">Second argument type to be passed.</typeparam>
        /// <param name="arg1">First argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg2">Second argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Action to invoke.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public void RunActionAsIndented<TArg1, TArg2>(TArg1 arg1, TArg2 arg2, Action<IndentableTextWriter, TArg1, TArg2> callback)
        {
            this.Indent();
            try
            {
                callback(this, arg1, arg2);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }
        }

        /// <summary>
        /// Runs the specified <see cref="System.Action&&lt;IndentableTextWriter, TArg1, TArg2, TArg3&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TArg1">First argument type to be passed.</typeparam>
        /// <typeparam name="TArg2">Second argument type to be passed.</typeparam>
        /// <typeparam name="TArg3">Third argument type to be passed.</typeparam>
        /// <param name="arg1">First argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg2">Second argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg3">Third argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Action to invoke.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public void RunActionAsIndented<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<IndentableTextWriter, TArg1, TArg2, TArg3> callback)
        {
            this.Indent();
            try
            {
                callback(this, arg1, arg2, arg3);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }
        }

        /// <summary>
        /// Runs the specified <see cref="System.Func&lt;IndentableTextWriter, TResult&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TResult">Type of value returned by <paramref name="callback"/>.</typeparam>
        /// <param name="callback">Func to invoke.</param>
        /// <returns>value which had been returnd by <paramref name="callback"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public TResult RunFuncAsIndented<TResult>(Func<IndentableTextWriter, TResult> callback)
        {
            TResult result;

            this.Indent();
            try
            {
                result = callback(this);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }

            return result;
        }

        /// <summary>
        /// Runs the specified <see cref="System.Func&lt;IndentableTextWriter, TArg, TResult&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TArg">Type of argument to be passed.</typeparam>
        /// <typeparam name="TResult">Type of value returned by <paramref name="callback"/>.</typeparam>
        /// <param name="arg">Argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Func to invoke.</param>
        /// <returns>value which had been returnd by <paramref name="callback"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public TResult RunFuncAsIndented<TArg, TResult>(TArg arg, Func<IndentableTextWriter, TArg, TResult> callback)
        {
            TResult result;

            this.Indent();
            try
            {
                result = callback(this, arg);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }

            return result;
        }

        /// <summary>
        /// Runs the specified <see cref="System.Func&lt;IndentableTextWriter, TArg1, TArg2, TResult&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TArg1">First argument type to be passed.</typeparam>
        /// <typeparam name="TArg2">Second argument type to be passed.</typeparam>
        /// <typeparam name="TResult">Type of value returned by <paramref name="callback"/>.</typeparam>
        /// <param name="arg1">First argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg2">Second argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Func to invoke.</param>
        /// <returns>value which had been returnd by <paramref name="callback"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public TResult RunFuncAsIndented<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2, Func<IndentableTextWriter, TArg1, TArg2, TResult> callback)
        {
            TResult result;

            this.Indent();
            try
            {
                result = callback(this, arg1, arg2);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }

            return result;
        }

        /// <summary>
        /// Runs the specified <see cref="System.Func&lt;IndentableTextWriter, TArg1, TArg2, TArg3, TResult&gt>"/> under an increased indent level, restoring the indent level when completed.
        /// </summary>
        /// <typeparam name="TArg1">First argument type to be passed.</typeparam>
        /// <typeparam name="TArg2">Second argument type to be passed.</typeparam>
        /// <typeparam name="TArg3">Third argument type to be passed.</typeparam>
        /// <typeparam name="TResult">Type of value returned by <paramref name="callback"/>.</typeparam>
        /// <param name="arg1">First argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg2">Second argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="arg3">Third argument to be passed to <paramref name="callback"/>.</param>
        /// <param name="callback">Func to invoke.</param>
        /// <returns>value which had been returnd by <paramref name="callback"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
        /// <exception cref="System.ObjectDisposedException">The <see cref="TextProcessing.IndentableTextWriter"/> is closed or disposed.</exception>
        /// <remarks>If <paramref name="callback"/> throws any exceptions, then the exception will be re-thrown as the writer is un-indented.</remarks>
        public TResult RunFuncAsIndented<TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Func<IndentableTextWriter, TArg1, TArg2, TArg3, TResult> callback)
        {
            TResult result;

            this.Indent();
            try
            {
                result = callback(this, arg1, arg2, arg3);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Unindent();
            }

            return result;
        }
    }
}
