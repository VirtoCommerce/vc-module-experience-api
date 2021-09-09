using System.Linq;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class GiftItemType : ExtendableGraphType<LineItem>
    {
        public GiftItemType(IMediator mediator, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new FuncFieldResolver<LineItem, IDataLoaderResult<ExpProduct>>(context =>
                {
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
            };
            AddField(productField);

            Field(x => x.ImageUrl, nullable: true).Description("Value of line item image absolute URL");
            Field(x => x.Name, nullable: true).Description("Value of line item name");
            Field(x => x.ProductId, nullable: true).Description("Value of product id");
            Field(x => x.Quantity, nullable: true).Description("Value of line item quantity");
            Field(x => x.Sku, nullable: true).Description("Value of product SKU");
            Field(x => x.ThumbnailImageUrl, nullable: true).Description("Value of line item thumbnail image absolute URL");
        }
    }
}
