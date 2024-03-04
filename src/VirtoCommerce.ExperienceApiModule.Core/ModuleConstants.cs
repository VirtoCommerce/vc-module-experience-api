using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    public static class ModuleConstants
    {
        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor CreateAnonymousOrder { get; } = new SettingDescriptor
                {
                    Name = "XOrder.CreateAnonymousOrderEnabled",
                    ValueType = SettingValueType.Boolean,
                    GroupName = "Orders|General",
                    DefaultValue = true
                };

                // FOR TESTING PURPOSES ONLY
                public static readonly SettingDescriptor EnableScheduledNotifications = new SettingDescriptor
                {
                    Name = "XNotification.EnableScheduledNotifications",
                    GroupName = "Notifications|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static readonly SettingDescriptor ScheduledNotificationsCron = new SettingDescriptor
                {
                    Name = "XNotification.ScheduledNotificationscron",
                    GroupName = "Notifications|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "*/10 * * * * *"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return CreateAnonymousOrder;

                        yield return EnableScheduledNotifications;
                        yield return ScheduledNotificationsCron;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> StoreLevelSettings
            {
                get
                {
                    yield return General.CreateAnonymousOrder;
                }
            }
        }
    }
}
