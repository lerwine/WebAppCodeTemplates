using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    public class NonReiteratingEnumerable<T> : IEnumerable<T>
    {
        private object _syncRoot = new object();
        private T[] _elements = new T[0];
        private IEnumerator<T> _enumerator = null;

        public int InterationCount
        {
            get
            {
                int result;
                lock (this._syncRoot) { result = this._elements.Length; }
                return result;
            }
        }

        public bool IterationComplete
        {
            get
            {
                bool result;
                lock (this._syncRoot) { result = this._enumerator == null; }
                return result;
            }
        }

        public T this[int index]
        {
            get
            {
                T result;
                lock (this._syncRoot)
                {
                    while (this._elements.Length <= index)
                    {
                        if (!this._MoveNext())
                            break;
                    }

                    result = this._elements[index];
                }
                return result;
            }
        }

        private bool _MoveNext()
        {
            if (this._enumerator == null)
                return false;

            if (!this._enumerator.MoveNext())
            {
                this._enumerator.Dispose();
                this._enumerator = null;
                return false;
            }

            int index = this._elements.Length;
            Array.Resize<T>(ref this._elements, index + 1);
            this._elements[index] = this._enumerator.Current;
            return true;
        }

        public NonReiteratingEnumerable(IEnumerable<T> source)
        {
            if (source == null)
                return;

            if (source is T[])
            {
                T[] s = source as T[];
                this._elements = new T[s.Length];
                s.CopyTo(this._elements, 0);
            }
            else
                this._enumerator = source.GetEnumerator();
        }

        public bool MoveNext()
        {
            bool result;
            lock (this._syncRoot)
                result = this._MoveNext();
            return result;
        }

        public T[] ToArray()
        {
            T[] result;
            lock (this._syncRoot)
            {
                while (this._MoveNext()) ;
                result = this._elements;
            }
            return result;
        }

        public int Count { get { return this.ToArray().Length; } }

        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorForNonIteratingEnumerable<T>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new EnumeratorForNonIteratingEnumerable<T>(this);
        }
    }
}
