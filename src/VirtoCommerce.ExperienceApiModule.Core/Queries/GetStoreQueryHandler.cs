using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;
using StoreSettingGeneral = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings.General;
using StoreSettingSeo = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings.SEO;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetStoreQueryHandler : IQueryHandler<GetStoreQuery, StoreResponse>
    {
        private readonly IStoreService _storeService;
        private readonly IStoreCurrencyResolver _storeCurrencyResolver;

        public GetStoreQueryHandler(IStoreService storeService, IStoreCurrencyResolver storeCurrencyResolver)
        {
            _storeService = storeService;
            _storeCurrencyResolver = storeCurrencyResolver;
        }

        public async Task<StoreResponse> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId, clone: false);

            if (store == null)
            {
                return null;
            }

            var cultureName = request.CultureName ?? store.DefaultLanguage;

            var allCurrencies = await _storeCurrencyResolver.GetAllStoreCurrenciesAsync(store.Id, cultureName);
            var availableCurrencies = allCurrencies.Where(x => store.Currencies.Contains(x.Code)).ToList();
            var defaultCurrency = await _storeCurrencyResolver.GetStoreCurrencyAsync(store.DefaultCurrency, store.Id, cultureName);

            var defaultLanguage = store.DefaultLanguage != null ? new Language(store.DefaultLanguage) : Language.InvariantLanguage;
            var availableLanguages = !store.Languages.IsNullOrEmpty() ? store.Languages.Select(x => new Language(x)).ToList() : new List<Language>();

            var response = new StoreResponse
            {
                StoreId = store.Id,
                StoreName = store.Name,
                CatalogId = store.Catalog,
                StoreUrl = store.Url,
                DefaultCurrency = defaultCurrency,
                AvailableCurrencies = availableCurrencies,
                DefaultLanguage = defaultLanguage,
                AvailableLanguages = availableLanguages,
            };

            if (!store.Settings.IsNullOrEmpty())
            {
                response.Settings = new StoreSettings
                {
                    IsSpa = store.Settings.GetValue<bool>(StoreSettingGeneral.IsSpa),
                    TaxCalculationEnabled = store.Settings.GetValue<bool>(StoreSettingGeneral.TaxCalculationEnabled),
                    AnonymousUsersAllowed = store.Settings.GetValue<bool>(StoreSettingGeneral.AllowAnonymousUsers),
                    EmailVerificationEnabled = store.Settings.GetValue<bool>(StoreSettingGeneral.EmailVerificationEnabled),
                    EmailVerificationRequired = store.Settings.GetValue<bool>(StoreSettingGeneral.EmailVerificationRequired),
                    SeoLinkType = store.Settings.GetValue<string>(StoreSettingSeo.SeoLinksType),
                    CreateAnonymousOrderEnabled = store.Settings.GetValue<bool>(ModuleConstants.Settings.General.CreateAnonymousOrder),

                    QuotesEnabled = store.Settings.GetValue<bool>(new SettingDescriptor { Name = "Quotes.EnableQuotes" }),
                    SubscriptionEnabled = store.Settings.GetValue<bool>(new SettingDescriptor { Name = "Subscription.EnableSubscriptions" }),
                };
            }

            return response;
        }
    }
}
