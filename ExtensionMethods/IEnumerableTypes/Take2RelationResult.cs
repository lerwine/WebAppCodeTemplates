using System;
using System.Collections.Generic;
using System.Linq;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    /// <summary>
    /// Result which enumerates parent/children relationships and contains a property for orphaned children
    /// </summary>
    /// <typeparam name="TParent">Type of parent object</typeparam>
    /// <typeparam name="TChild">Type of child object</typeparam>
    public class Take2RelationResult<TParent, TChild> : IEnumerable<Take2Relation<TParent, TChild>>
    {
        private IEnumerable<TParent> _parents;
        private IEnumerable<TChild> _children;
        private Func<TParent, TChild, bool> _predicate;
        private bool _isExclusive;
        private IEnumerable<Take2Relation<TParent, TChild>> _relationships = null;
        private IEnumerable<TChild> _orphans;

        /// <summary>
        /// All parent objects
        /// </summary>
        private IEnumerable<TParent> Parents
        {
            get
            {
                if (this._parents == null)
                    this._parents = new TParent[0];

                return this._parents;
            }
        }

        /// <summary>
        /// All child and orphan objects
        /// </summary>
        private IEnumerable<TChild> Children
        {
            get
            {
                if (this._children == null)
                    this._children = new TChild[0];

                return this._children;
            }
        }

        /// <summary>
        /// Objects which did not evaluate to having a parent object
        /// </summary>
        public IEnumerable<TChild> Orphans
        {
            get
            {
                if (this._orphans == null)
                    this.Evaluate();

                return this._orphans;
            }
        }

        /// <summary>
        /// Initializes new Parent/Children relationship result object
        /// </summary>
        /// <param name="parents">Parent objects</param>
        /// <param name="children">Potential child objects</param>
        /// <param name="predicate">Function which is used to determine if a child belongs to a parent</param>
        /// <param name="isExclusive">Whether a child can have multiple parents.</param>
        public Take2RelationResult(IEnumerable<TParent> parents, IEnumerable<TChild> children, Func<TParent, TChild, bool> predicate, bool isExclusive)
        {
            this._parents = parents;
            this._children = children;
            this._predicate = predicate;
            this._isExclusive = isExclusive;
        }

        private void Evaluate()
        {
            if (this.Parents is TParent[] && (this.Parents as TParent[]).Length == 0)
            {
                this._relationships = new Take2Relation<TParent, TChild>[0];
                this._orphans = this.Children;
                return;
            }

            if (this._predicate == null || this.Children is TChild[] && (this.Children as TChild[]).Length == 0)
            {
                this._relationships = this.Parents.Select(p => new Take2Relation<TParent, TChild>(p));
                this._orphans = this.Children;
                return;
            }

            if (this._isExclusive)
            {
                IEnumerable<TChild> remaining = this.Children;

                this._relationships = this.Parents.Select(p =>
                {
                    Take2Relation<TParent, TChild> result = new Take2Relation<TParent, TChild>(p);
                    Take2ParseResult<TChild> pr = remaining.Where2(c => this._predicate(p, c));
                    result.Children = pr.Select(c => c);
                    remaining = pr.Remaining;
                    return result;
                });

                this._orphans = remaining;

                return;
            }

            // TODO: May need to see if this gets fixed. It may be possible that in some circumstances, this can cause a double-evaluation
            this._relationships = this.Parents.Select(p => new Take2Relation<TParent, TChild>(p));

            this._orphans = this.Children.Where(child => this._relationships.Count(r =>
            {
                if (!this._predicate(r.Parent, child))
                    return false;

                r.Children = r.Children.Concat(new TChild[] { child });
                return true;
            }) == 0);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the result collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator&lt;Take2Relation&lt;TParent, TChild&gt&gt;"/> that can be used to iterate through the result collection.</returns>
        public IEnumerator<Take2Relation<TParent, TChild>> GetEnumerator()
        {
            if (this._relationships == null)
                this.Evaluate();

            return this._relationships.GetEnumerator();
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
