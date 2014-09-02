using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TextProcessing
{
    public class IndentingStringWriter : TextWriter
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        private char[] _currentLine = new char[0];
        private char[] _currentIndent = new char[0];
        private string _indent = "\t";
        private int _indentLevel = 0;

        public override string NewLine
        {
            get
            {
                return base.NewLine;
            }
            set
            {
                base.NewLine = value;
            }
        }

        public string Indent
        {
            get { return this._indent; }
            set
            {
                string s = (value == null) ? "\t" : value;

                if (this._indent == s)
                    return;

                this._indent = s;
                this.UpdateCurrentIndent();
            }
        }

        public int IndentLevel
        {
            get { return this._indentLevel; }
            private set
            {
                int i = (value < 0) ? 0 : value;
                if (this._indentLevel == i)
                    return;

                this._indentLevel = i;
                this.UpdateCurrentIndent();
            }
        }

        public override Encoding Encoding { get { return Encoding.Unicode; } }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            base.Flush();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Write(char value)
        {
            base.Write(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
        }

        private void UpdateCurrentIndent()
        {
            lock (this._stringBuilder)
            {
                this._currentIndent = new char[0];
                if (this._indent.Length > 0)
                {
                    for (int i = 0; i < this.IndentLevel; i++)
                        this._currentIndent = this._currentIndent.Concat(this._indent.ToCharArray()).ToArray();
                }
            }
        }
    }
}
