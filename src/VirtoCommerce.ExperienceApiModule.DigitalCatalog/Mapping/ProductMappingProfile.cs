using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Queries;
using ProductPrice = VirtoCommerce.ExperienceApiModule.Core.Models.ProductPrice;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<LoadProductsQuery, SearchProductQuery>();
            CreateMap<LoadCategoryQuery, SearchCategoryQuery>();

            CreateMap<SearchProductAssociationsQuery, ProductAssociationSearchCriteria>();

            CreateMap<SearchDocument, ExpProduct>().ConvertUsing(src => new GenericModelBinder<ExpProduct>().BindModel(src) as ExpProduct);

            CreateMap<ExpProduct, ProductPromoEntry>().ConvertUsing((src, dest, context) =>
            {
                var result = AbstractTypeFactory<ProductPromoEntry>.TryCreateInstance();
                if (!context.Items.TryGetValue("currency", out var currencyObj))
                {
                    throw new OperationCanceledException("currency must be set");
                }
                var currency = currencyObj as Currency;

                var productPrice = src.AllPrices.FirstOrDefault(x => x.Currency.Code.EqualsInvariant(currency.Code));

                result.CatalogId = src.IndexedProduct.CatalogId;
                result.CategoryId = src.IndexedProduct.CategoryId;
                result.ProductId = src.Id;
                result.Code = src.IndexedProduct.Code;
                result.Outline = src.IndexedProduct.Outline;
                if (productPrice != null)
                {
                    result.Discount = productPrice.DiscountAmount.Amount;
                    result.Price = productPrice.SalePrice.Amount;
                    result.ListPrice = productPrice.ListPrice.Amount;
                }
                result.InStockQuantity = (int)src.AvailableQuantity;
                result.Quantity = 1;

                return result;
            });

            CreateMap<ExpProduct, IEnumerable<TaxLine>>().ConvertUsing((src, dest, context) =>
            {
                var result = new List<TaxLine>();
                foreach (var price in src.AllPrices)
                {
                    result.Add(new TaxLine()
                    {
                        Id = src.Id,
                        Code = src.IndexedProduct.Code,
                        Name = src.IndexedProduct.Name,
                        TaxType = src.IndexedProduct.TaxType,
                        //Special case when product have 100% discount and need to calculate tax for old value
                        Amount = price.ActualPrice.Amount > 0 ? price.ActualPrice.Amount : price.SalePrice.Amount
                    });
                    //Need generate tax line for each tier price
                    foreach (var tierPrice in price.TierPrices)
                    {
                        result.Add(new TaxLine()
                        {
                            Id = src.Id,
                            Code = src.IndexedProduct.Code,
                            Name = src.IndexedProduct.Name,
                            TaxType = src.IndexedProduct.TaxType,
                            Quantity = (int)tierPrice.Quantity,
                            Amount = tierPrice.Price.Amount
                        });
                    }
                }
                return result.ToArray();
            });

            CreateMap<IEnumerable<Price>, IEnumerable<ProductPrice>>().ConvertUsing((src, dest, context) =>
            {
                if (!context.Items.TryGetValue("all_currencies", out var allCurrenciesObj))
                {
                    throw new OperationCanceledException("all_currencies must be set");
                }
                var result = new List<ProductPrice>();
                var allCurrencies = (allCurrenciesObj as IEnumerable<Currency>).ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);

                static IEnumerable<ProductPrice> PricesToProductPrices(IEnumerable<Price> prices, IDictionary<string, Currency> allCurrencies)
                {
                    foreach (var price in prices)
                    {
                        var currency = allCurrencies[price.Currency];
                        if (currency != null)
                        {
                            var productPrice = new ProductPrice(currency)
                            {
                                ProductId = price.ProductId,
                                PricelistId = price.PricelistId,
                                Currency = currency,
                                ListPrice = new Money(price.List, currency)
                            };
                            productPrice.SalePrice = price.Sale == null ? productPrice.ListPrice : new Money(price.Sale ?? 0m, currency);
                            productPrice.MinQuantity = price.MinQuantity;
                            yield return productPrice;
                        }
                    }
                }

                //group prices by currency
                var groupByCurrencyPrices = PricesToProductPrices(src, allCurrencies).GroupBy(x => x.Currency).Where(x => x.Any());
                foreach (var currencyGroup in groupByCurrencyPrices)
                {
                    //For each currency need get nominal price (with min qty)
                    var orderedPrices = currencyGroup.OrderBy(x => x.MinQuantity ?? 0).ThenBy(x => x.ListPrice);
                    var nominalPrice = orderedPrices.FirstOrDefault();
                    //and add to nominal price other prices as tier prices
                    nominalPrice.TierPrices.AddRange(orderedPrices.Select(x => new TierPrice(x.SalePrice, x.MinQuantity ?? 1)));
                    //Add nominal price to product prices list
                    result.Add(nominalPrice);
                }

                if (!context.Items.TryGetValue("currency", out var currentCurrency) && !(currentCurrency is string))
                {
                    return result;
                }

                // Filter by current currency
                return result.Where(x => (x.Currency == null) || x.Currency.Equals(currentCurrency)).ToList();
            });
        }
    }
}
