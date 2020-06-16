using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Services
{
    public class TaxEvaluator : ITaxEvaluator
    {
        private readonly ITaxProviderSearchService _taxProviderSearchService;

        public TaxEvaluator(ITaxProviderSearchService taxProviderSearchService)
        {
            _taxProviderSearchService = taxProviderSearchService;
        }

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

            if (!context.StoreTaxCalculationEnabled)
            {
                return;
            }

            var taxRates = new List<TaxRate>();

            //Do not execute platform API for tax evaluation if fixed tax rate is used
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
                var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria
                {
                    StoreIds = new[] { context.StoreId },
                }).ConfigureAwait(false);

                var activeTaxProvider = storeTaxProviders.Results.FirstOrDefault(x => x.IsActive);
                if (activeTaxProvider != null)
                {
                    taxRates.AddRange(activeTaxProvider
                        .CalculateRates(new VirtualTaxEvaluationContext
                        {
                            StoreId = context.StoreId,
                            Code = context.Code,
                            Type = context.Type,
                            CustomerId = context.Customer?.Id,
                            Address = context.Address,
                            Currency = context.Currency.ToString(),
                            Lines = context.Lines
                        }).Select(x => new TaxRate(context.Currency)
                        {
                            Line = x.Line != null ? new TaxLine(context.Currency)
                            {
                                Id = x.Line.Id,
                                Amount = new Money(x.Line.Amount, context.Currency),
                                Code = x.Line.Code,
                                Name = x.Line.Name,
                                Price = new Money(x.Line.Price, context.Currency),
                                Quantity = x.Line.Quantity,
                                TaxType = x.Line.TaxType,
                                TypeName = x.Line.TypeName
                            } : null,
                            PercentRate = x.PercentRate,
                            Rate = new Money(x.Rate, context.Currency)
                        }));
                }
            }

            ApplyTaxRates(taxRates, owners);
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

        public class VirtualTaxEvaluationContext : Entity, VirtoCommerce.CoreModule.Core.Common.IEvaluationContext
        {
            public string StoreId { get; set; }
            public string Code { get; set; }
            public string Type { get; set; }
            public string CustomerId { get; set; }
            public string OrganizationId { get; set; }
            public Address Address { get; set; }
            public string Currency { get; set; }
            public ICollection<TaxLine> Lines { get; set; } = new List<TaxLine>();
        }
    }
}
