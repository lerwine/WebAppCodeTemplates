using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CustomConfigurationSection
{
    public class RoleGroupMappingElementCollection : ConfigurationElementCollection
    {
        public const string Default_ElementName = "roleGroupMappings";

        public RoleGroupMappingConfigElement this[int index]
        {
            get { return base.BaseGet(index) as RoleGroupMappingConfigElement; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RoleGroupMappingConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RoleGroupMappingConfigElement)element).RoleText;
        }
    }
}