using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Commands;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductsQueryHandler : IQueryHandler<LoadProductQuery, LoadProductResponse>
    {
        private readonly IMediator _mediator;
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;

        public LoadProductsQueryHandler(ISearchProvider searchProvider, IMapper mapper, IMediator mediator, ICartAggregateRepository cartAggregateRepository)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _mediator = mediator;
            _cartAggregateRepository = cartAggregateRepository;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductQuery request, CancellationToken cancellationToken)
        {
            var result = new LoadProductResponse();
            var requestFields = new List<string>();

            var searchRequest = new SearchRequestBuilder()
                .WithPaging(0, request.Ids.Count())
                .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                .WithIncludeFields(request.IncludeFields.Where(x => x.StartsWith("prices.")).Concat(new[] { "id" }).Select(x => "__prices." + x.TrimStart("prices.")).ToArray())
                .WithIncludeFields(request.IncludeFields.Any(x => x.StartsWith("variations."))
                    ? new[] { "__variations" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.StartsWith("category."))
                    ? new[] { "__object.categoryId" }
                    : Array.Empty<string>())
                // Add master variation fields
                .WithIncludeFields(request.IncludeFields
                    .Where(x => x.StartsWith("masterVariation."))
                    .Concat(new[] { "mainProductId" })
                    .Select(x => "__object." + x.TrimStart("masterVariation."))
                    .ToArray())
                // Add seoInfos
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("slug", StringComparison.OrdinalIgnoreCase)
                                                                || x.Contains("meta", StringComparison.OrdinalIgnoreCase)) // for metaKeywords, metaTitle and metaDescription
                    ? new[] { "__object.seoInfos" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("imgSrc", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.images" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("brandName", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.properties" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("descriptions", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.reviews" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("availabilityData", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.isActive", "__object.isBuyable", "__object.trackInventory" }
                    : Array.Empty<string>())
                .AddObjectIds(request.Ids)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            var expProducts = searchResult.Documents.Select(x => _mapper.Map<ExpProduct>(x)).ToList();
            result.Products = expProducts;

            if (expProducts.All(x => x.Prices.IsNullOrEmpty()))
            {
                return result;
            }

            // If tax evaluation requested
            if (request.IncludeFields.Where(x => x.Contains("prices")).Any(x => x.Contains("tax", StringComparison.OrdinalIgnoreCase) ||
                                                                                x.Contains("tier", StringComparison.OrdinalIgnoreCase) ||
                                                                                x.Contains("discount", StringComparison.OrdinalIgnoreCase)))
            {
                var createCartCommand = new CreateDefaultCartCommand(
                    storeId: request.StoreId,
                    userId: request.UserId,
                    currency: request.CurrencyCode,
                    lang: request.Language);

                var cart = _cartAggregateRepository.CreateDefaultShoppingCart(createCartCommand);

                var cartAggregate = await _cartAggregateRepository.GetCartForShoppingCartAsync(cart);

                var addLineItemTasks = expProducts
                    .Select(x => cartAggregate.AddItemAsync(new NewCartItem(x.CatalogProduct.Id, 1) { CartProduct = new CartProduct(x.CatalogProduct) }))
                    .ToArray();

                Task.WaitAll(addLineItemTasks);

                var context = _mapper.Map<PromotionEvaluationContext>(cartAggregate);

                context.PromoEntries = expProducts
                    .Select(x => x.CatalogProduct)
                    .Select(x => new ProductPromoEntry
                    {
                        ProductId = x.Id,
                        CatalogId = x.CatalogId,
                        CategoryId = x.CategoryId,
                        Quantity = 1,
                        Code = x.Code,
                        InStockQuantity = 0, //TODO: check this
                        Outline = x.Outline,
                        Variations = x.Variations?.Select(x => new ProductPromoEntry
                        {
                            ProductId = x.Id,
                            CatalogId = x.CatalogId,
                            CategoryId = x.CategoryId,
                            Quantity = 1,
                            Code = x.Code,
                            InStockQuantity = 0, //TODO: and this
                            Outline = x.Outline
                        }).ToList()
                    })
                    .ToList();

                var promotionResults = await cartAggregate.EvaluatePromotionsAsync(context);
                var promoRewards = promotionResults.Rewards.OfType<CatalogItemAmountReward>().ToArray();

                var language = new Language(request.Language);

                foreach (var expProduct in expProducts)
                {
                    var productId = expProduct.Id;

                    var prices = expProduct.Prices;
                    var productPrices = prices
                        .Where(price => price.ProductId == productId)
                        // Apply Prices
                        .Select(price =>
                        {
                            var currency = new Currency(language, price.Currency);

                            return new ProductPrice(currency)
                            {
                                ProductId = productId,
                                PricelistId = price.PricelistId,
                                Currency = currency,
                                ListPrice = new Money(price.List, currency),
                                SalePrice = price.Sale == null
                                     ? new Money(price.List, currency)
                                     : new Money((decimal)price.Sale, currency),
                                MinQuantity = price.MinQuantity
                            };
                        })
                        .GroupBy(x => x.Currency)
                        .Where(x => x.Any())
                        // Apply TierPrices
                        .Select(currencyGroup =>
                        {
                            var orderedPrices = currencyGroup
                                .OrderBy(x => x.MinQuantity ?? 0)
                                .ThenBy(x => x.ListPrice);

                            var nominalPrice = orderedPrices.First();

                            var tierPrices = orderedPrices.Select(x => new TierPrice(x.SalePrice, x.MinQuantity ?? 1));

                            nominalPrice.TierPrices.AddRange(tierPrices);

                            return nominalPrice;
                        })
                        .ToList();

                    productPrices.ApplyRewards(promoRewards);

                    expProduct.ProductPrices = productPrices;
                }
            }

            return result;
        }

        public class CreateDefaultCartCommand : CartCommand
        {
            public CreateDefaultCartCommand(string storeId, string userId, string currency, string lang)
            : base(storeId, "cart", "default", userId, currency, lang)
            {
            }
        }
    }
}
