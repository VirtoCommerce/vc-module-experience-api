using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<Variation>
    {
        public VariationType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Field(x => x.Id, nullable: false).Description("Id of variation.");
            Field(x => x.Code, nullable: true).Description("SKU of variation.");
            Field<AvailabilityDataType>("availabilityData", resolve: context => null);
            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Images);
        }
    }
}
