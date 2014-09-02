using System;
using System.Collections.Generic;
using System.Linq;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    /// <summary>
    /// Represents results from Take2 extension methods which inserts one collection into another
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public class Take2InsertResult<T> : IEnumerable<T>
    {
        private Take2ParseResult<T> _take2;
        private IEnumerable<T> _result = null;
        private IEnumerable<T> _inserted;

        /// <summary>
        /// Collection which was parsed
        /// </summary>
        public IEnumerable<T> Source { get { return this._take2.Source; } }

        /// <summary>
        /// Elements which sequentially occur before the inserted elements.
        /// </summary>
        public IEnumerable<T> BeforeInserted { get { return this._take2.Select(e => e); } }

        /// <summary>
        /// Elements which have been inserted.
        /// </summary>
        public IEnumerable<T> Inserted
        {
            get
            {
                if (this._inserted == null)
                    this._inserted = new T[0];

                return this._inserted;
            }
        }

        /// <summary>
        /// Items which sequentially occur after the inserted elements.
        /// </summary>
        public IEnumerable<T> AfterInserted { get { return this._take2.Remaining; } }

        /// <summary>
        /// Elements new Take2ResultCollection object.
        /// </summary>
        /// <param name="take2">Object which represents the collection being inserted into.</param>
        /// <param name="inserted">Elements being inserted.</param>
        public Take2InsertResult(Take2ParseResult<T> take2, IEnumerable<T> inserted)
        {
            this._take2 = take2;
            this._inserted = inserted;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the result collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator&lt;T&gt;"/> that can be used to iterate through the result collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this._result == null)
            {
                if (this.Inserted is T[] && (this.Inserted as T[]).Length == 0)
                    this._result = this._take2.Source;
                else if (this._take2.Source is T[] && (this._take2.Source as T[]).Length == 0)
                    this._result = this.Inserted;
                else
                    this._result = this._take2.Concat(this.Inserted).Concat(this._take2.Remaining);
            }

            return this._result.GetEnumerator();
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
