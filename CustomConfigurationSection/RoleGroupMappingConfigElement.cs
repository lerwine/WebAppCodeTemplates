using System;
using System.Configuration;

namespace CustomConfigurationSection
{
    public class RoleGroupMappingConfigElement : ConfigurationElement
    {
        public const string ConfigPropertyName_RoleText = "role";
        public const string ConfigPropertyName_ADGroupName = "adGroupName";
        public const string ConfigPropertyName_Disabled = "disabled";
        public const bool DefaultValue_Disabled = false;

        [ConfigurationProperty(RoleGroupMappingConfigElement.ConfigPropertyName_RoleText, IsRequired = true, IsKey = true)]
        public string RoleText
        {
            get
            {
                string result = this[RoleGroupMappingConfigElement.ConfigPropertyName_RoleText] as string; return (result == null) ? "" : result;
            }
        }

        public AppRole Role
        {
            get
            {
                string roleText = this.RoleText.Trim();

                if (roleText.Length == 0)
                    return default(AppRole);

                AppRole result;

                if (Enum.TryParse<AppRole>(roleText, true, out result))
                    return result;

                int i;
                if (!Int32.TryParse(roleText, out i))
                    return default(AppRole);

                try
                {
                    result = (AppRole)i;
                }
                catch
                {
                    result = default(AppRole);
                }

                return result;
            }
        }

        [ConfigurationProperty(RoleGroupMappingConfigElement.ConfigPropertyName_ADGroupName, IsRequired = false, DefaultValue = "")]
        public string ADGroupName
        {
            get
            {
                string result = this[RoleGroupMappingConfigElement.ConfigPropertyName_ADGroupName] as string; return (result == null) ? "" : result;
            }
        }

        [ConfigurationProperty(RoleGroupMappingConfigElement.ConfigPropertyName_Disabled, IsRequired = false, DefaultValue = RoleGroupMappingConfigElement.DefaultValue_Disabled)]
        public bool Disabled
        {
            get
            {
                return CustomConfigurationSection.GetBestBooleanValue(this[RoleGroupMappingConfigElement.ConfigPropertyName_Disabled], RoleGroupMappingConfigElement.DefaultValue_Disabled);
            }
        }
    }
}