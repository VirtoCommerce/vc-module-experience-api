using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQuery : CatalogQueryBase<SearchProductResponse>, ISearchQuery
    {
        public string Keyword { get => Query; set => Query = value; }
        public string Query { get; set; }
        public bool Fuzzy { get; set; }
        public int? FuzzyLevel { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string[] ObjectIds { get; set; }
        public bool EvaluatePromotions { get; set; } = true;

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId), "The store id where products are searched");
            yield return Argument<StringGraphType>(nameof(UserId), "The customer id for search result impersonation");
            yield return Argument<StringGraphType>(nameof(CurrencyCode), "The currency for which all prices data will be returned");
            yield return Argument<StringGraphType>(nameof(CultureName), "The culture name for cart context product");

            yield return Argument<StringGraphType>(nameof(Query), "The query parameter performs the full-text search");
            yield return Argument<StringGraphType>(nameof(Filter), "This parameter applies a filter to the query results");
            yield return Argument<StringGraphType>(nameof(Facet), "Facets calculate statistical counts to aid in faceted navigation.");
            yield return Argument<BooleanGraphType>(nameof(Fuzzy), "When the fuzzy query parameter is set to true the search endpoint will also return products that contain slight differences to the search text.");
            yield return Argument<IntGraphType>(nameof(FuzzyLevel), "The fuzziness level is quantified in terms of the Damerau-Levenshtein distance, this distance being the number of operations needed to transform one word into another.");
            yield return Argument<StringGraphType>(nameof(Sort), "The sort expression");

            yield return Argument<ListGraphType<StringGraphType>>("productIds", "Product Ids");

            yield return Argument<StringGraphType>("custom", "Can be used for custom query parameters");
        }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            var productIds = context.GetArgument<List<string>>("productIds");
            if (!productIds.IsNullOrEmpty())
            {
                ObjectIds = productIds.ToArray();
                Take = productIds.Count;
            }
            else
            {
                Query = context.GetArgument<string>(nameof(Query));
                Filter = context.GetArgument<string>(nameof(Filter));
                Facet = context.GetArgument<string>(nameof(Facet));
                Fuzzy = context.GetArgument<bool>(nameof(Fuzzy));
                FuzzyLevel = context.GetArgument<int?>(nameof(FuzzyLevel));
                Sort = context.GetArgument<string>(nameof(Sort));

                if (context is IResolveConnectionContext connectionContext)
                {
                    Skip = int.TryParse(connectionContext.After, out var skip) ? skip : 0;
                    Take = connectionContext.First ?? connectionContext.PageSize ?? 20;
                }
            }
        }

        public virtual string GetResponseGroup()
        {
            var result = ExpProductResponseGroup.None;
            if (IncludeFields.Any(x => x.Contains("price")))
            {
                result |= ExpProductResponseGroup.LoadPrices;
            }
            if (IncludeFields.Any(x => x.Contains("minVariationPrice")))
            {
                result |= ExpProductResponseGroup.LoadVariationPrices;
            }
            if (IncludeFields.Any(x => x.Contains("availabilityData")))
            {
                result |= ExpProductResponseGroup.LoadInventories;
                result |= ExpProductResponseGroup.LoadPrices;
            }
            if (IncludeFields.Any(x => x.Contains("vendor")))
            {
                result |= ExpProductResponseGroup.LoadVendors;
            }
            if (IncludeFields.Any(x => x.Contains("rating")))
            {
                result |= ExpProductResponseGroup.LoadRating;
            }
            if (IncludeFields.Any(x => x.Contains("_facets")))
            {
                result |= ExpProductResponseGroup.LoadFacets;
            }
            if (IncludeFields.ContainsAny("inWishlist", "wishlistIds"))
            {
                result |= ExpProductResponseGroup.LoadWishlists;
            }
            if (IncludeFields.ContainsAny("properties", "keyProperties"))
            {
                result |= ExpProductResponseGroup.LoadPropertyMetadata;
            }
            return result.ToString();
        }

        public virtual string GetItemResponseGroup()
        {
            var result = ItemResponseGroup.None;

            if (IncludeFields.ContainsAny("assets", "images", "imgSrc"))
            {
                result |= ItemResponseGroup.WithImages;
            }

            if (IncludeFields.ContainsAny("properties", "keyProperties", "brandName"))
            {
                result |= ItemResponseGroup.WithProperties;
            }

            // "descriptions" could look redundant, but better to check it explicitly - clear approach for possible modification or different "Contains" logic
            if (IncludeFields.ContainsAny("description", "descriptions"))
            {
                result |= ItemResponseGroup.ItemEditorialReviews;
            }

            if (IncludeFields.ContainsAny("seoInfo"))
            {
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("slug"))
            {
                result |= ItemResponseGroup.WithLinks;
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("outline", "outlines", "slug", "level", "breadcrumbs"))
            {
                result |= ItemResponseGroup.WithOutlines;
                result |= ItemResponseGroup.WithSeo;
            }

            if (IncludeFields.ContainsAny("availabilityData"))
            {
                result |= ItemResponseGroup.Inventory;
            }

            return result.ToString();
        }
    }
}
