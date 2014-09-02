using System;
using System.Collections.Generic;

namespace Erwine.Leonard.T.ExtensionMethods.IEnumerableTypes
{
    /// <summary>
    /// Represents a parent-to-children relationship
    /// </summary>
    /// <typeparam name="TParent">Type of parent object</typeparam>
    /// <typeparam name="TChild">Type of child object</typeparam>
    public class Take2Relation<TParent, TChild>
    {
        /// <summary>
        /// Parent object
        /// </summary>
        public TParent Parent { get; set; }

        /// <summary>
        /// Child objects
        /// </summary>
        public IEnumerable<TChild> Children { get; set; }

        public Take2Relation(TParent parent)
        {
            this.Parent = parent;
            this.Children = new TChild[0];
        }
    }
}
