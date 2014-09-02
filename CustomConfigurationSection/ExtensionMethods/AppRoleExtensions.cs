namespace CustomConfigurationSection.ExtensionMethods
{
    public static class AppRoleExtensions
    {
        public static string GetDisplayName(this AppRole appRole)
        {
            switch (appRole)
            {
                case AppRole.ApplicationAdmin:
                    return "Application Administrator";
                case AppRole.WebsiteAdmin:
                    return "Website Administrator";
            }

            return appRole.ToString("F");
        }
    }
}
