using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace NGIC.CHAP.Diagnostics
{
    public abstract class BindingListBase<TItem> : IList<TItem>, IBindingList
        where TItem : class
    {
        [Serializable]
        public class OrderedItem : IEquatable<OrderedItem>, IComparable<OrderedItem>
        {
            public int OriginalIndex { get; private set; }

            public TItem Item { get; private set; }

            public OrderedItem(TItem item, int index)
            {
                this.OriginalIndex = index;
                this.Item = item;
            }

            public bool Equals(OrderedItem other)
            {
                return other != null && (Object.ReferenceEquals(this, other) || this.OriginalIndex == other.OriginalIndex);
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as OrderedItem);
            }

            public override int GetHashCode()
            {
                return this.OriginalIndex;
            }

            public override string ToString()
            {
                return this.OriginalIndex.ToString();
            }

            public int CompareTo(OrderedItem other)
            {
                if (other == null)
                    return 1;

                return (Object.ReferenceEquals(this, other)) ? 0 : this.OriginalIndex.CompareTo(other.OriginalIndex);
            }
        }

        public abstract class ItemPropertyComparer : IEqualityComparer<OrderedItem>, IComparer<OrderedItem>, IEquatable<ItemPropertyComparer>
        {
            public PropertyDescriptor Property { get; private set; }

            public static ItemPropertyComparer Create(PropertyDescriptor property)
            {
                if (property == null || !typeof(TItem).IsAssignableFrom(property.ComponentType))
                    return null;

                ItemPropertyComparer result = Activator.CreateInstance((typeof(ItemPropertyComparer<>)).MakeGenericType(property.PropertyType)) as ItemPropertyComparer;
                result.Property = property;
                return result;
            }

            public abstract bool ItemHasProperty(TItem item);

            public abstract bool PropertiesAreEqual(TItem x, TItem y);

            public abstract int GetPropertyHashCode(TItem item);

            public abstract int ComparePropertyValues(TItem x, TItem y);

            public bool Equals(OrderedItem x, OrderedItem y)
            {
                return (x == null) ? y == null : (y != null && (Object.ReferenceEquals(x, y) || ((this.ItemHasProperty(x.Item)) ? this.ItemHasProperty(y.Item) : this.ItemHasProperty(y.Item) &&
                    this.PropertiesAreEqual(x.Item, y.Item))));
            }

            public int GetHashCode(OrderedItem obj)
            {
                return (obj == null || !this.ItemHasProperty(obj.Item)) ? 0 : this.GetPropertyHashCode(obj.Item);
            }

            public int Compare(OrderedItem x, OrderedItem y)
            {
                if (x == null)
                    return (y == null) ? 0 : -1;

                if (y == null)
                    return 1;

                if (Object.ReferenceEquals(x, y) || Object.ReferenceEquals(x.Item, y.Item))
                    return 0;

                if (!this.ItemHasProperty(x.Item))
                    return (this.ItemHasProperty(y.Item)) ? -1 : 0;

                if (!this.ItemHasProperty(y.Item))
                    return 1;

                return this.ComparePropertyValues(x.Item, y.Item);
            }

            public bool Equals(ItemPropertyComparer other)
            {
                return other != null && (Object.ReferenceEquals(this, other) || (this.Property.ComponentType.Equals(other.Property.ComponentType) && this.Property.PropertyType.Equals(other.Property.PropertyType) &&
                    String.Compare(this.Property.Name, other.Property.Name, true) == 0));
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as ItemPropertyComparer);
            }

            public override int GetHashCode()
            {
                return this.Property.Name.GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0} => {1} {2}", this.Property.ComponentType.ToString(), this.Property.PropertyType.ToString(), this.Property.Name);
            }
        }

        [Serializable]
        public class ItemPropertyComparer<TValue> : ItemPropertyComparer
        {
            private EqualityComparer<TValue> _equalityComparer;
            private Comparer<TValue> _comparer;
            private Func<TValue, TValue, bool> _valuesEqual;
            private Func<TValue, int> _gethashCode;
            private Func<TValue, TValue, int> _compreTo;

            public ItemPropertyComparer()
            {
                Type type = typeof(TValue);
                if (type.Equals(typeof(string)))
                {
                    this._equalityComparer = StringComparer.Ordinal as EqualityComparer<TValue>;
                    this._comparer = this._equalityComparer as Comparer<TValue>;
                }
                else
                {
                    this._equalityComparer = EqualityComparer<TValue>.Default;
                    this._comparer = Comparer<TValue>.Default;

                    if (type.IsValueType)
                    {
                        this._valuesEqual = (TValue x, TValue y) => this._equalityComparer.Equals(x, y);
                        this._gethashCode = (TValue value) => this._equalityComparer.GetHashCode(value);
                        this._compreTo = (TValue x, TValue y) => this._comparer.Compare(x, y);
                        return;
                    }
                }

                this._valuesEqual = (TValue x, TValue y) => ((object)x == null) ? (object)y == null : ((object)y != null && (Object.ReferenceEquals(x, y) || this._equalityComparer.Equals(x, y)));
                this._gethashCode = (TValue value) => ((object)value == null) ? 0 : this._equalityComparer.GetHashCode(value);
                this._compreTo = (TValue x, TValue y) =>
                {
                    if ((object)x == null)
                        return ((object)y == null) ? 0 : -1;

                    if ((object)y == null)
                        return 1;

                    if (Object.ReferenceEquals(x, y))
                        return 0;

                    return this._comparer.Compare(x, y);
                };
            }

            private bool TryGetValue(TItem item, out TValue value)
            {
                bool result;

                try
                {
                    value = (TValue)(this.Property.GetValue(item));
                    result = true;
                }
                catch
                {
                    value = default(TValue);
                    result = false;
                }

                return result;
            }

            public override bool ItemHasProperty(TItem item)
            {
                return item != null && this.Property.ComponentType.IsInstanceOfType(item);
            }

            public override bool PropertiesAreEqual(TItem itemX, TItem itemY)
            {
                TValue valueX, valueY;

                if (!this.TryGetValue(itemX, out valueX))
                    return !this.TryGetValue(itemY, out valueY);

                if (!this.TryGetValue(itemY, out valueY))
                    return false;

                return this._valuesEqual(valueX, valueY);
            }

            public override int GetPropertyHashCode(TItem item)
            {
                TValue value;

                if (!this.TryGetValue(item, out value))
                    return 0;

                return this._gethashCode(value);
            }

            public override int ComparePropertyValues(TItem itemX, TItem itemY)
            {
                TValue valueX, valueY;

                if (!this.TryGetValue(itemX, out valueX))
                    return (this.TryGetValue(itemY, out valueY)) ? -1 : 0;

                if (!this.TryGetValue(itemY, out valueY))
                    return 1;

                return this._compreTo(valueX, valueY);
            }
        }

        private object _syncRoot = new object();
        private OrderedItem[] _items;
        private Dictionary<Type, bool> _validatedPropertyDescriptors = new Dictionary<Type, bool>();
        private LinkedList<Tuple<ItemPropertyComparer, ListSortDirection>> _sortStack = new LinkedList<Tuple<ItemPropertyComparer,ListSortDirection>>();

        private int _sortIndex = 0;

        public BindingListBase(params TItem[] items) : this(items as IEnumerable<TItem>) { }

        public BindingListBase(IEnumerable<TItem> items)
        {
            int index = 0;
            this._items = (items == null) ? new OrderedItem[0] : items.Where(i => i != null).Select(i => new OrderedItem(i, index++)).ToArray();
        }

        protected void RunWithLock(System.Action action)
        {
            lock (this._syncRoot)
                action();
        }

        protected T GetWithLock<T>(Func<T> func)
        {
            T result;

            lock (this._syncRoot)
                result = func();

            return result;
        }

        private void _ResetInnerSort()
        {
            this._items = this._items.OrderBy(i => i.OriginalIndex).ToArray();
            this._sortIndex = 0;
        }

        private void _EnsureSort()
        {
            if (this._items.Length < 2 || this._sortStack.Count == 0 || this._sortIndex >= this._sortStack.Count)
                return;

            foreach (Tuple<ItemPropertyComparer, ListSortDirection> sort in this._sortStack.Skip(this._sortIndex))
            {
                if (sort.Item2 == ListSortDirection.Ascending)
                    this._items = this._items.OrderBy(i => i, sort.Item1).ToArray();
                else
                    this._items = this._items.OrderByDescending(i => i, sort.Item1).ToArray();
            }

            this._sortIndex = this._items.Length;
        }

        public void EnsureSort()
        {
            this.RunWithLock(this._EnsureSort);
        }

        protected bool ValidatePropertyDescriptor(PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!this._validatedPropertyDescriptors.ContainsKey(property.ComponentType))
                this._validatedPropertyDescriptors.Add(property.ComponentType, (typeof(TItem)).IsAssignableFrom(property.ComponentType));

            return this._validatedPropertyDescriptors[property.ComponentType];
        }

        private OrderedItem[] _GetItems()
        {
            this._EnsureSort();
            return this._items;
        }

        public List<TItem> GetAllItems() { return this.GetWithLock<List<TItem>>(() => this._GetItems().Select(i => i.Item).ToList()); }

        public List<TItem> GetItems(int startIndex, int maxResults, string orderBy)
        {
            throw new NotImplementedException();
        }

        #region IList<TItem> Members

        public int IndexOf(TItem item)
        {
            return this.GetWithLock<int>(() =>
            {
                if (item == null || this._items.Length == 0)
                    return -1;

                int index = this._GetItems().TakeWhile(i => !Object.ReferenceEquals(i.Item, item)).Count();
                return (index == this._items.Length) ? -1 : index;
            });
        }

        public TItem this[int index] { get { return this.GetWithLock<TItem>(() => this._GetItems()[index].Item); } }

        #endregion

        #region ICollection<TItem> Members

        public bool Contains(TItem item) { return this.GetWithLock<bool>(() => item != null && this._GetItems().Any(i => Object.ReferenceEquals(i, item))); }

        public void CopyTo(TItem[] array, int arrayIndex) { this.RunWithLock(() => this._GetItems().Select(i => i.Item).ToList().CopyTo(array, arrayIndex)); }

        public int Count { get { return this.GetWithLock<int>(() => this._items.Length); } }

        bool ICollection<TItem>.IsReadOnly { get { return true; } }

        #endregion

        #region IEnumerator<TItem> Members

        public IEnumerator<TItem> GetEnumerator() { return this.GetWithLock<IEnumerator<TItem>>(() => this._GetItems().Select(i => i.Item).ToList().GetEnumerator()); }

        #endregion
        
        #region System.Collections.IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetWithLock<System.Collections.IEnumerator>(() => this._GetItems().Select(i => i.Item).ToArray().GetEnumerator()); }

        #endregion

        #region IBindingList Members

        bool IBindingList.AllowEdit { get { return false; } }

        bool IBindingList.AllowNew { get { return false; } }

        bool IBindingList.AllowRemove { get { return false; } }

        public void _ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            if (this._items.Length < 2)
                return;

            if (this._sortStack.Count > 0)
            {
                if (String.Compare(property.Name, this._sortStack.Last.Value.Item1.Property.Name, true) == 0)
                {
                    if (direction == this._sortStack.Last.Value.Item2)
                        return;

                    this._sortStack.RemoveLast();
                    this._sortStack.AddLast(new Tuple<ItemPropertyComparer, ListSortDirection>(ItemPropertyComparer.Create(property), direction));
                    if (this._sortIndex >= this._items.Length)
                        this._items = this._items.Reverse().ToArray();
                    return;
                }

                Tuple<ItemPropertyComparer, ListSortDirection> toRemove = this._sortStack.FirstOrDefault(i => String.Compare(i.Item1.Property.Name, property.Name, true) == 0);

                if (toRemove != null)
                {
                    this._sortStack.Remove(toRemove);
                    this._ResetInnerSort();
                }
            }

            this._sortStack.AddLast(new Tuple<ItemPropertyComparer, ListSortDirection>(ItemPropertyComparer.Create(property), direction));
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            if (!this.ValidatePropertyDescriptor(property))
                throw new ArgumentOutOfRangeException("property", String.Format("Property descriptor's component object is not of type {0}.", typeof(TItem).FullName));

            this.RunWithLock(() => this._ApplySort(property, direction));
        }

        public bool IsSorted { get { return this.GetWithLock<bool>(() => this._sortStack.Count > 0); } }

        public void RemoveSort()
        {
            this.RunWithLock(() =>
            {
                if (this._sortStack.Count == 0)
                    return;

                this._sortStack.RemoveLast();
                this._ResetInnerSort();
            });
        }

        public ListSortDirection SortDirection { get { return this.GetWithLock<ListSortDirection>(() => (this._sortStack.Count == 0) ? ListSortDirection.Ascending : this._sortStack.Last.Value.Item2); } }

        public PropertyDescriptor SortProperty { get { return this.GetWithLock<PropertyDescriptor>(() => (this._sortStack.Count == 0) ? null : this._sortStack.Last.Value.Item1.Property); } }

        bool IBindingList.SupportsChangeNotification { get { return false; } }

        bool IBindingList.SupportsSearching { get { return false; } }

        bool IBindingList.SupportsSorting { get { return true; } }

        #endregion

        #region System.Collections.IList Members

        bool System.Collections.IList.Contains(object value) { return this.Contains(value as TItem); }

        int System.Collections.IList.IndexOf(object value) { return this.IndexOf(value as TItem); }

        bool System.Collections.IList.IsFixedSize { get { return true; } }

        bool System.Collections.IList.IsReadOnly { get { return true; } }

        #endregion

        #region System.Collections.ICollection Members

        void System.Collections.ICollection.CopyTo(Array array, int index) { this.RunWithLock(() => this._GetItems().Select(i => i.Item).ToArray().CopyTo(array, index)); }

        bool System.Collections.ICollection.IsSynchronized { get { return true; } }

        public object SyncRoot { get { return this._syncRoot; } }

        #endregion

        #region Unsupported Members

        #region IList<TItem> Members

        void IList<TItem>.Insert(int index, TItem item) { throw new NotSupportedException(); }

        void IList<TItem>.RemoveAt(int index) { throw new NotSupportedException(); }

        TItem IList<TItem>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region ICollection<TItem> Members

        void ICollection<TItem>.Add(TItem item) { throw new NotSupportedException(); }

        void ICollection<TItem>.Clear() { throw new NotSupportedException(); }

        bool ICollection<TItem>.Remove(TItem item) { throw new NotSupportedException(); }

        #endregion

        #region IBindingList Members

        void IBindingList.AddIndex(PropertyDescriptor property) { throw new NotSupportedException(); }

        object IBindingList.AddNew() { throw new NotSupportedException(); }

        int IBindingList.Find(PropertyDescriptor property, object key) { throw new NotSupportedException(); }

        private event ListChangedEventHandler _listChanged;

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add { this._listChanged += value; }
            remove { this._listChanged -= value; }
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property) { throw new NotSupportedException(); }

        #endregion

        #region System.Collections.IList Members

        int System.Collections.IList.Add(object value) { throw new NotSupportedException(); }

        void System.Collections.IList.Clear() { throw new NotSupportedException(); }

        void System.Collections.IList.Insert(int index, object value) { throw new NotSupportedException(); }

        void System.Collections.IList.Remove(object value) { throw new NotSupportedException(); }

        void System.Collections.IList.RemoveAt(int index) { throw new NotSupportedException(); }

        object System.Collections.IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #endregion
    }
}
