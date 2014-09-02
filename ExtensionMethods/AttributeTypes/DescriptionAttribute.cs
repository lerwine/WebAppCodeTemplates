using System;

namespace Erwine.Leonard.T.ExtensionMethods.AttributeTypes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class DescriptionAttribute : Attribute
    {
        private string _description = "";

        public string Description
        {
            get { return _description; }
            set { this._description = (value == null) ? "" : value; }
        }

        public DescriptionAttribute() : base() { }

        public DescriptionAttribute(string description) : base() { this._description = description; }
    }
}
