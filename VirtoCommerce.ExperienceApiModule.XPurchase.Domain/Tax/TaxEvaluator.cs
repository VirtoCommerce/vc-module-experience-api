using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Tax
{
    public class TaxEvaluator : ITaxEvaluator
    {
        private readonly ITaxProviderSearchService _taxProviderSearchService;

        public TaxEvaluator(ITaxProviderSearchService taxProviderSearchService)
        {
            _taxProviderSearchService = taxProviderSearchService;
        }

        // TODO: TaxEvaluationContext is created from shopping cart and store (in storefront)
        public virtual async Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (owners == null)
            {
                throw new ArgumentNullException(nameof(owners));
            }

            IList<TaxRate> taxRates = new List<TaxRate>();

            if (context.StoreTaxCalculationEnabled && context.FixedTaxRate != 0)
            {
                // Do not execute platform API for tax evaluation if fixed tax rate is used
                if (context.FixedTaxRate != 0)
                {
                    foreach (var line in context.Lines ?? Enumerable.Empty<TaxLine>())
                    {
                        var rate = new TaxRate(context.Currency)
                        {
                            Rate = new Money((double)(line.Amount * context.FixedTaxRate * 0.01m).Amount, context.Currency),
                            PercentRate = 0, // DOTO: ???
                            Line = line,
                        };
                        taxRates.Add(rate);
                    }
                }
                else
                {
                    taxRates = await EvaluateTaxesAsync(context.StoreId, context);
                }
            }
            ApplyTaxRates(taxRates, owners);
        }


        protected virtual async Task<IList<TaxRate>> EvaluateTaxesAsync(string storeId, TaxEvaluationContext context)
        {
            var result = new List<TaxRate>();
            var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria { StoreIds = new[] { storeId } });

            var activeTaxProvider = storeTaxProviders.Results.FirstOrDefault(x => x.IsActive);
            if (activeTaxProvider != null)
            {
                context.StoreId = storeId;
                result.AddRange(activeTaxProvider.CalculateRates(context.ToTaxEvaluationContextDto()).Select(x => x.ToTaxRate(context.Currency)));
            }

            return result;
        }


        private static void ApplyTaxRates(IList<TaxRate> taxRates, IEnumerable<ITaxable> owners)
        {
            if (taxRates == null)
            {
                return;
            }
            var taxRatesMap = owners.Select(x => x.Currency).Distinct().ToDictionary(x => x, x => taxRates.ToArray());

            foreach (var owner in owners)
            {
                owner.ApplyTaxRates(taxRatesMap[owner.Currency]);
            }
        }
    }
}
