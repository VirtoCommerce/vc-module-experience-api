using System.Linq;
using VirtoCommerce.XPurchase.Models;

namespace VirtoCommerce.XPurchase.Domain.Converters
{
    public static class SettingConverter
    {
        public static SettingEntry ToSettingEntry(this VirtoCommerce.Platform.Core.Settings.ObjectSettingEntry settingDto)
        {
            var retVal = new SettingEntry
            {
                DefaultValue = settingDto.DefaultValue,
                IsArray = false,
                Name = settingDto.Name,
                Value = settingDto.Value,
                ValueType = settingDto.ValueType.ToString()
            };
            if (settingDto.AllowedValues != null)
            {
                retVal.AllowedValues = settingDto.AllowedValues.ToList();
            }
            return retVal;
        }
    }
}
