using System;
using System.Configuration;

namespace CustomConfigurationSection
{
    public class NotificationEmailConfigElement : ConfigurationElement
    {
        public const string ConfigPropertyName_Address = "address";
        public const string ConfigPropertyName_DisplayName = "displayName";
        public const string ConfigPropertyName_Disabled = "disabled";
        public const bool DefaultValue_Disabled = false;

        [ConfigurationProperty(NotificationEmailConfigElement.ConfigPropertyName_Address, IsRequired = true, IsKey = true)]
        public string Address
        {
            get
            {
                string result = this[NotificationEmailConfigElement.ConfigPropertyName_Address] as string; return (result == null) ? "" : result;
            }
        }

        [ConfigurationProperty(NotificationEmailConfigElement.ConfigPropertyName_DisplayName, IsRequired = false, DefaultValue = "")]
        public string DisplayName
        {
            get
            {
                string result = this[NotificationEmailConfigElement.ConfigPropertyName_DisplayName] as string; return (result == null) ? "" : result;
            }
        }

        [ConfigurationProperty(NotificationEmailConfigElement.ConfigPropertyName_Disabled, IsRequired = false, DefaultValue = NotificationEmailConfigElement.DefaultValue_Disabled)]
        public bool Disabled
        {
            get
            {
                return CustomConfigurationSection.GetBestBooleanValue(this[NotificationEmailConfigElement.ConfigPropertyName_Disabled], NotificationEmailConfigElement.DefaultValue_Disabled);
            }
        }
    }
}
