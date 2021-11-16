using System.Linq;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class GiftItemType : ExtendableGraphType<GiftItem>
    {
        public GiftItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            Field(x => x.PromotionId).Description("Promotion Id");
            Field(x => x.Quantity).Description("Quantity of gifts in the reward");
            Field(x => x.ProductId, true).Description("Product id");
            Field(x => x.CategoryId, true).Description("Product category Id");
            Field(x => x.ImageUrl, true).Description("Value of reward image absolute URL");
            Field(x => x.Name).Description("Name of the reward");
            Field(x => x.MeasureUnit, true).Description("Measure unit");
            Field(x => x.LineItemId, true).Description("ID of lineItem if gift is in cart. Otherwise null");

            AddField(new FieldType
            {
                Name = "id",
                Description = "Artificial ID for this value object",
                Type = GraphTypeExtenstionHelper.GetActualType<NonNullGraphType<StringGraphType>>(),
                Resolver = new FuncFieldResolver<GiftItem, string>(context =>
                {
                    // CacheKey as Id. CacheKey is determined by the values returned form GetEqualityComponents().
                    return context.Source.GetCacheKey();
                })
            });

            AddField(new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new FuncFieldResolver<GiftItem, IDataLoaderResult<ExpProduct>>(context =>
                {
                    if (context.Source.ProductId.IsNullOrEmpty())
                    {
                        return default;
                    }

                    var includeFields = context.SubFields.Values.GetAllNodesPaths();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("order_lineItems_products", async (ids) =>
                    {
                        //Gift is not part of cart, can't use CartAggregate. Getting store and currency from the context.
                        var request = new LoadProductsQuery
                        {
                            StoreId = context.GetValue<string>("storeId"),
                            CurrencyCode = context.GetArgumentOrValue<string>("currencyCode"),
                            ObjectIds = ids.ToArray(),
                            IncludeFields = includeFields.ToArray()
                        };

                        var response = await mediator.Send(request);

                        return response.Products.ToDictionary(x => x.Id);
                    });
                    return loader.LoadAsync(context.Source.ProductId);
                })
            });
        }
    }
}
