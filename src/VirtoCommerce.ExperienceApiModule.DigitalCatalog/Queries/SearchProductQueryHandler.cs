using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
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
using VirtoCommerce.XDigitalCatalog.Facets;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Commands;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQueryHandler :
        IQueryHandler<SearchProductQuery, SearchProductResponse>,
        IQueryHandler<LoadProductQuery, LoadProductResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IAggregationConverter _aggregationConverter;

        // TODO: we have __inventories
        public SearchProductQueryHandler(ISearchProvider searchProvider, ISearchPhraseParser searchPhraseParser, IMapper mapper, IAggregationConverter aggregationConverter, ICartAggregateRepository cartAggregateRepository)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
            _aggregationConverter = aggregationConverter;
            _cartAggregateRepository = cartAggregateRepository;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var requestFields = BuildFields(request.IncludeFields.ToArray(), out var loadPrices);

            var searchRequest = new SearchRequestBuilder(_searchPhraseParser, _aggregationConverter)
                .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                .ParseFilters(request.Filter)
                .ParseFacets(request.Facet, request.StoreId)
                .WithSearchPhrase(request.Query)
                .WithPaging(request.Skip, request.Take)
                .AddSorting(request.Sort)
                .WithIncludeFields(requestFields)
                .AddObjectIds(request.ProductIds)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            var expProducts = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList();

            // If promotion evaluation requested
            if (loadPrices && !expProducts.All(x => x.Prices.IsNullOrEmpty()))
            {
                await LoadPricesAsync(expProducts, request.CartName, request.StoreId, request.UserId, request.CultureName, request.CurrencyCode, request.CartType);
            }

            return new SearchProductResponse
            {
                Results = expProducts,
                Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductQuery request, CancellationToken cancellationToken)
        {
            var requestFields = BuildFields(request.IncludeFields.ToArray(), out var loadPrices);

            var searchRequest = new SearchRequestBuilder()
                .WithPaging(0, request.Ids.Count())
                .WithIncludeFields(requestFields)
                .AddObjectIds(request.Ids)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            var expProducts = searchResult.Documents.Select(x => _mapper.Map<ExpProduct>(x)).ToList();

            // If promotion evaluation requested
            if (loadPrices && !expProducts.All(x => x.Prices.IsNullOrEmpty()))
            {
                await LoadPricesAsync(expProducts, request.CartName, request.StoreId, request.UserId, request.Language, request.CurrencyCode, request.Type);
            }

            return new LoadProductResponse(expProducts);
        }

        protected virtual async Task LoadPricesAsync(List<ExpProduct> expProducts, string cartName, string storeId, string userId, string cultureName, string currencyCode, string type)
        {
            var cartAggregate = await _cartAggregateRepository.GetCartAsync(cartName, storeId, userId, cultureName, currencyCode, type);
            if (cartAggregate == null)
            {
                var createCartCommand = new CreateDefaultCartCommand(storeId, userId, currencyCode, cultureName);
                var cart = _cartAggregateRepository.CreateDefaultShoppingCart(createCartCommand);
                cartAggregate = await _cartAggregateRepository.GetCartForShoppingCartAsync(cart);
            }

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

            var language = new Language(cultureName);

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

        private string[] BuildFields(IReadOnlyCollection<string> includeFields, out bool loadPrices)
        {
            var result = new List<string>();

            // Add filds for __object
            result.AddRange(includeFields.Concat(new[] { "id" }).Select(x => "__object." + x));

            loadPrices = includeFields.Any(x => x.Contains("prices", StringComparison.OrdinalIgnoreCase));
            if (loadPrices)
            {
                result.Add("__prices");
            }

            if (includeFields.Any(x => x.Contains("variations", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__variations");
            }

            if (includeFields.Any(x => x.StartsWith("category", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.categoryId");
            }

            // Add master variation fields
            result.AddRange(includeFields.Where(x => x.StartsWith("masterVariation."))
                                         .Concat(new[] { "mainProductId" })
                                         .Select(x => "__object." + x.TrimStart("masterVariation.")));

            // Add metaKeywords, metaTitle and metaDescription
            if (includeFields.Any(x => x.Contains("slug", StringComparison.OrdinalIgnoreCase) || x.Contains("meta", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.seoInfos");
            }

            if (includeFields.Any(x => x.Contains("imgSrc", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.images");
            }

            if (includeFields.Any(x => x.Contains("brandName", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.properties");
            }

            if (includeFields.Any(x => x.Contains("descriptions", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.reviews");
            }

            if (includeFields.Any(x => x.Contains("availabilityData", StringComparison.OrdinalIgnoreCase)))
            {
                result.Add("__object.isActive");
                result.Add("__object.isBuyable");
                result.Add("__object.trackInventory");
            }

            return result.Distinct().ToArray();
        }

        public class CreateDefaultCartCommand : CartCommand
        {
            public CreateDefaultCartCommand(string storeId, string userId, string currency, string lang)//todo: change this to normal cartcommand
            : base(storeId, "cart", "default", userId, currency, lang)
            {
            }
        }
    }
}
