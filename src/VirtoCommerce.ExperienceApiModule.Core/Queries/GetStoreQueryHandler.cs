using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using StoreSettingGeneral = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings.General;
using StoreSettingSeo = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings.SEO;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetStoreQueryHandler : IQueryHandler<GetStoreQuery, StoreResponse>
    {
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;
        private readonly IStoreCurrencyResolver _storeCurrencyResolver;
        private readonly ISettingsManager _settingsManager;
        private readonly IdentityOptions _identityOptions;
        private readonly GraphQLWebSocketOptions _webSocketOptions;
        private readonly StoresOptions _storeOptions;

        public GetStoreQueryHandler(
            IStoreService storeService,
            IStoreSearchService storeSearchService,
            IStoreCurrencyResolver storeCurrencyResolver,
            ISettingsManager settingsManager,
            IOptions<IdentityOptions> identityOptions,
            IOptions<GraphQLWebSocketOptions> webSocketOptions,
            IOptions<StoresOptions> storeOptions)

        {
            _storeService = storeService;
            _storeSearchService = storeSearchService;
            _storeCurrencyResolver = storeCurrencyResolver;
            _settingsManager = settingsManager;
            _identityOptions = identityOptions.Value;
            _webSocketOptions = webSocketOptions.Value;
            _storeOptions = storeOptions.Value;
        }

        public async Task<StoreResponse> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            Store store = null;

            if (!string.IsNullOrEmpty(request.StoreId))
            {
                store = await _storeService.GetByIdAsync(request.StoreId, clone: false);
            }
            else if (!string.IsNullOrEmpty(request.Domain))
            {
                store = await ResolveStoreByDomain(request.Domain);

                if (store == null && !string.IsNullOrEmpty(_storeOptions.DefaultStore))
                {
                    store = await _storeService.GetByIdAsync(_storeOptions.DefaultStore, clone: false);
                }
            }

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
                GraphQLSettings = new GraphQLSettings
                {
                    KeepAliveInterval = _webSocketOptions.KeepAliveInterval,
                }
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

                    EnvironmentName = _settingsManager.GetValue<string>(ModuleConstants.Settings.General.EnvironmentName),
                    PasswordRequirements = _identityOptions.Password,

                    Modules = ToModulesSettings(store.Settings)
                };
            }

            return response;
        }

        protected virtual async Task<Store> ResolveStoreByDomain(string domain)
        {
            if (_storeOptions.Domains.TryGetValue(domain, out var storeId))
            {
                return await _storeService.GetByIdAsync(storeId, clone: false);
            }

            var criteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            criteria.Domain = domain;
            criteria.Take = 1;

            var result = await _storeSearchService.SearchAsync(criteria, clone: false);
            return result.Results.FirstOrDefault();
        }

        protected virtual ModuleSettings[] ToModulesSettings(ICollection<ObjectSettingEntry> settings)
        {
            var result = new List<ModuleSettings>();

            foreach (var settingByModule in settings.Where(s => s.IsPublic).GroupBy(s => s.ModuleId))
            {
                var moduleSettings = new ModuleSettings
                {
                    ModuleId = settingByModule.Key,
                    Settings = settingByModule.Select(s => new ModuleSetting
                    {
                        Name = s.Name,
                        Value = ToSettingValue(s)
                    }).ToArray(),
                };

                if (moduleSettings.Settings.Length > 0)
                {
                    result.Add(moduleSettings);
                }
            }

            return result.ToArray();
        }

        protected virtual object ToSettingValue(ObjectSettingEntry s)
        {
            var result = s.Value ?? s.DefaultValue;

            if (result == null)
            {
                switch (s.ValueType)
                {
                    case SettingValueType.Boolean:
                        result = false;
                        break;
                }
            }

            return result;
        }
    }
}
