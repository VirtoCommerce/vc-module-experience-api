using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class XOrderConstants
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

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return CreateAnonymousOrder;
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
