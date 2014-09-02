using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    /// <summary>
    /// Represents results from Take2 extension methods which parses a collection into 2 parts
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public class Take2ParseResult<T> : IEnumerable<T>
    {
        private IEnumerable<T> _source;
        private bool _isSequential;
        private Func<T, bool> _skipPredicate;
        private Func<T, int, bool> _skipPredicate2;
        private Func<T, bool> _takePredicate;
        private Func<T, int, bool> _takePredicate2;
        private bool _endOfCollection = false;
        private IEnumerable<T> _collection = null;
        private IEnumerable<T> _remaining = null;

        /// <summary>
        /// Elements remaining after evaluation
        /// </summary>
        public IEnumerable<T> Remaining
        {
            get
            {
                if (this._remaining == null)
                    this.Evaluate();

                return this._remaining;
            }
        }

        /// <summary>
        /// Collection which was parsed
        /// </summary>
        public IEnumerable<T> Source
        {
            get
            {
                if (this._source == null)
                    this._source = new T[0];

                return this._source;
            }
        }

        /// <summary>
        /// Initialize new Take2Result object.
        /// </summary>
        /// <param name="source">Collection to parse.</param>
        /// <param name="predicate">A function to test each element for a condition. Items are included sequentially in the results until this returns false.</param>
        /// <param name="isSequential">If set to true, then predicate is a sequential &quot;Take&quot; operation. Otherwise, it determines which items, across the entire source collection, are included in the result.</param>
        public Take2ParseResult(IEnumerable<T> source, Func<T, bool> predicate, bool isSequential)
        {
            this._source = source;
            this._isSequential = isSequential;
            this._skipPredicate2 = null;
            this._takePredicate2 = null;
            if (isSequential)
            {
                this._skipPredicate = null;
                this._takePredicate = predicate;
            }
            else
            {
                this._skipPredicate = predicate;
                this._takePredicate = null;
            }
        }

        /// <summary>
        /// Initialize new Take2Result object.
        /// </summary>
        /// <param name="source">Collection to parse.</param>
        /// <param name="predicate">A function to test each element for a condition. Items are included sequentially in the results until this returns false.</param>
        /// <param name="isSequential">If set to true, then predicate is a sequential &quot;Take&quot; operation. Otherwise, it determines which items, across the entire source collection, are included in the result.</param>
        public Take2ParseResult(IEnumerable<T> source, Func<T, int, bool> predicate, bool isSequential)
        {
            this._source = source;
            this._isSequential = isSequential;
            this._skipPredicate = null;
            this._takePredicate = null;
            if (isSequential)
            {
                this._skipPredicate2 = null;
                this._takePredicate2 = predicate;
            }
            else
            {
                this._skipPredicate2 = predicate;
                this._takePredicate2 = null;
            }
        }

        /// <summary>
        /// Initialize new Take2Result object.
        /// </summary>
        /// <param name="source">Collection to parse.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Items are excluded sequentially from the results until this returns false.</param>
        /// <param name="takePredicate">A function to test each element for a condition. Items are included sequentially, following the skipped items, in the results until this returns false.</param>
        public Take2ParseResult(IEnumerable<T> source, Func<T, bool> skipPredicate, Func<T, bool> takePredicate)
        {
            this._source = source;
            this._skipPredicate = skipPredicate;
            this._takePredicate = takePredicate;
            this._skipPredicate2 = null;
            this._takePredicate2 = null;
            this._isSequential = false;
        }

        /// <summary>
        /// Initialize new Take2Result object.
        /// </summary>
        /// <param name="source">Collection to parse.</param>
        /// <param name="skipPredicate">A function to test each element for a condition. Items are excluded sequentially from the results until this returns false.</param>
        /// <param name="takePredicate">A function to test each element for a condition. Items are included sequentially, following the skipped items, in the results until this returns false.</param>
        public Take2ParseResult(IEnumerable<T> source, Func<T, int, bool> skipPredicate, Func<T, int, bool> takePredicate)
        {
            this._source = source;
            this._skipPredicate2 = skipPredicate;
            this._takePredicate2 = takePredicate;
            this._skipPredicate = null;
            this._takePredicate = null;
            this._isSequential = false;
        }

        private int _index = 0;
        private void Evaluate()
        {
            this._index = 0;
            if ((this._takePredicate == null && this._takePredicate2 == null) || (this.Source is T[] && (this.Source as T[]).Length == 0))
            {
                this._collection = new T[0];
                this._remaining = this.Source;
                return;
            }

            if (!this._isSequential)
            {
                this._remaining = this._Parse();
                return;
            }

            this._endOfCollection = false;

            if (this._skipPredicate == null && this._skipPredicate2 == null)
            {
                using (IEnumerator<T> enumerator = this.Source.GetEnumerator())
                {
                    this._collection = (this._takePredicate == null) ? this._GetCollection(enumerator, this._takePredicate) : this._GetCollection(enumerator, this._takePredicate2);
                    this._remaining = (this._endOfCollection) ? new T[0] : this._GetRemaining(enumerator);
                }

                return;
            }

            using (IEnumerator<T> enumerator = this.Source.GetEnumerator())
            {
                this._remaining = (this._skipPredicate == null) ? this._GetCollection(enumerator, this._skipPredicate) : this._GetCollection(enumerator, this._skipPredicate2);

                if (this._endOfCollection)
                    this._collection = new T[0];
                else
                {
                    this._collection = (this._takePredicate == null) ? this._GetCollection(enumerator, this._takePredicate) : this._GetCollection(enumerator, this._takePredicate2);

                    if (!this._endOfCollection)
                        this._remaining = this._remaining.Concat(this._GetRemaining(enumerator));
                }
            }
        }

        private IEnumerable<T> _Parse()
        {
            Collection<T> trueElements = new Collection<T>();
            this._collection = trueElements;

            if (this._takePredicate == null)
            {
                foreach (T e in this.Source)
                {
                    if (this._takePredicate2(e, this._index))
                        trueElements.Add(e);
                    else
                        yield return e;
                    this._index++;
                }
            }
            else
            {
                foreach (T e in this.Source)
                {
                    if (this._takePredicate(e))
                        trueElements.Add(e);
                    else
                        yield return e;
                    this._index++;
                }
            }
        }

        private IEnumerable<T> _GetCollection(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current))
                    yield break;

                this._index++;
                yield return enumerator.Current;
            }

            this._endOfCollection = true;
        }

        private IEnumerable<T> _GetCollection(IEnumerator<T> enumerator, Func<T, int, bool> predicate)
        {
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current, this._index))
                    yield break;

                this._index++;
                yield return enumerator.Current;
            }

            this._endOfCollection = true;
        }

        private IEnumerable<T> _GetRemaining(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the result collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/> that can be used to iterate through the result collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this._collection == null)
                this.Evaluate();

            return this._collection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the result collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the result collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
