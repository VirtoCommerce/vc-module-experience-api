
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class StoreServiceExtensions
    {
        public static async Task<T> GetSettingValue<T>(this ICrudService<Store> storeService, string storeId, SettingDescriptor settingDescriptor)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return (T)settingDescriptor.DefaultValue;
            }

            var store = await storeService.GetByIdAsync(storeId, StoreResponseGroup.None.ToString());
            var taxCalculation = store.GetSettingValue<T>(settingDescriptor);

            return taxCalculation;
        }

        public static T GetSettingValue<T>(this Store store, SettingDescriptor settingDescriptor)
        {
            var taxCalculation = store.Settings.GetSettingValue<T>(settingDescriptor.Name, (T)settingDescriptor.DefaultValue);

            return taxCalculation;
        }
    }
}
