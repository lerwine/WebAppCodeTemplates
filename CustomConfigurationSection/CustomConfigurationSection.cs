using System;
using System.Configuration;

namespace CustomConfigurationSection
{
    public class CustomConfigurationSection : ConfigurationSection
    {
        public const string Default_ElementName = "myApp.settings";

        [ConfigurationProperty(NotificationEmailElementCollection.Default_ElementName)]
        public NotificationEmailElementCollection NotificationEmails
        {
            get
            {
                return (NotificationEmailElementCollection)(this[NotificationEmailElementCollection.Default_ElementName]) ??
                    new NotificationEmailElementCollection();
            }
        }

        [ConfigurationProperty(RoleGroupMappingElementCollection.Default_ElementName)]
        public RoleGroupMappingElementCollection RoleGroupMappings
        {
            get
            {
                return (RoleGroupMappingElementCollection)(this[RoleGroupMappingElementCollection.Default_ElementName]) ??
                    new RoleGroupMappingElementCollection();
            }
        }

        public static bool GetBestBooleanValue(object input, bool defaultValue)
        {
            bool b;
            int i;
            return (input == null) ? NotificationEmailConfigElement.DefaultValue_Disabled :
                ((input is bool) ? (bool)input :
                ((input is string) ? ((Boolean.TryParse((input as string).Trim(), out b)) ? b :
                    ((Int32.TryParse((input as string).Trim(), out i)) ? i != 0 :
                        NotificationEmailConfigElement.DefaultValue_Disabled)) :
                    ((input is int) ? ((int)input != 0) : NotificationEmailConfigElement.DefaultValue_Disabled)));
        }
    }
}