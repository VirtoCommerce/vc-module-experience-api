using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class ProductType2 : ProductType
    {
        public ProductType2(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
            : base(mediator, dataLoader)
        {
  
            Field<ListGraphType<InventoryType>>("inventories", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "warehouse", Description = "warehouse id" }), resolve: context =>
            {
                var result = ((ExpProduct2)context.Source).Inventories;
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
