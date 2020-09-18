using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class ProductType2 : ProductType
    {
        public ProductType2(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
            : base(mediator, dataLoader)
        {
            Field<ListGraphType<StringGraphType>>("searchKeywords", resolve: context => ((ExpProduct2)context.Source).SearchKeywords).RootAlias("__content"); ;
            Field<ListGraphType<InventoryType>>("inventoriesFromIndex", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "warehouse", Description = "warehouse id" }), resolve: context =>
            {
                var result = ((ExpProduct2)context.Source).InventoriesFromIndex;
                var warehouseId = context.GetArgument<string>("warehouse");
                if (warehouseId != null)
                {
                    result = result.Where(x => x.FulfillmentCenterId.EqualsInvariant(warehouseId)).ToList();
                }
                return result;
            });
        }
    }
}
