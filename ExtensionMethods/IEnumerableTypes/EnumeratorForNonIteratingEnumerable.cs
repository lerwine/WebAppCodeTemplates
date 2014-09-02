using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    public class EnumeratorForNonIteratingEnumerable<T> : IEnumerator<T>
    {
        private NonReiteratingEnumerable<T> _source;
        private int _index = 0;

        public T Current { get { return this._source[this._index]; } }

        object System.Collections.IEnumerator.Current { get { return this._source[this._index]; } }

        public EnumeratorForNonIteratingEnumerable(NonReiteratingEnumerable<T> source)
        {
            this._source = source;
        }

        public void Dispose()
        {
            this._source = null;
        }

        public bool MoveNext()
        {
            if (this._source.IterationComplete)
                return false;

            if (this._index == this._source.InterationCount)
            {
                this._source.MoveNext();
                if (this._index == this._source.InterationCount)
                    return false;
            }

            this._index++;
            return true;
        }

        public void Reset()
        {
            this._index = 0;
        }
    }
}
