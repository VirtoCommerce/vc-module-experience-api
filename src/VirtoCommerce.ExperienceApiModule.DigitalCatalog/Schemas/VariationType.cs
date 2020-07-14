using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<Variation>
    {
        public VariationType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Field(x => x.Id, nullable: false).Description("Id of variation.");
            Field(x => x.Code, nullable: false).Description("SKU of variation.");

            var availabilityDataField = new FieldType // TODO: rewrite this
            {
                Name = "availabilityData",
                Type = GraphTypeExtenstionHelper.GetActualType<AvailabilityDataType>(),
                Resolver = new AsyncFieldResolver<AvailabilityDataType, object>(async context =>
                {
                    return null;
                })
            };

            AddField(availabilityDataField);
        }
    }
}
