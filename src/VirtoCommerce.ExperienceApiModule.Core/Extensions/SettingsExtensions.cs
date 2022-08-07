
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public class SettingsExtensions
    {
        private readonly ICrudService<Store> _storeService;
        public SettingsExtensions()
        {

        }
        public SettingsExtensions(IStoreService storeService)
        {
            _storeService = (ICrudService<Store>)storeService;
        }
        public async Task<T> GetSettingFromStore<T>(string storeId, SettingDescriptor settingDescriptor)
        {
            var store = await _storeService.GetByIdAsync(storeId, StoreResponseGroup.None.ToString());
            var taxCalculation = store.Settings.GetSettingValue<T>(settingDescriptor.Name, (T)settingDescriptor.DefaultValue);
            return taxCalculation;
        }
    }
}
