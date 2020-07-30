using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ProductAssociationType : ObjectGraphType<ProductAssociation>
    {
        public ProductAssociationType(IDataLoaderContextAccessor dataLoader, IMediator mediator)
        {
            Name = "ProductAssociation";
            Description = "product association.";

            Field(d => d.Type, nullable: true);
            Field(d => d.Priority, nullable: true);
            Field("Quantity", x => x.Quantity, nullable: true, type: typeof(IntGraphType));
            Field(d => d.AssociatedObjectId, nullable: true);
            Field(d => d.AssociatedObjectType, nullable: true);
            Field<ListGraphType<StringGraphType>>("tags", resolve: context => context.Source.Tags.ToList());

            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<ProductAssociation, object>(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("associatedProductLoader", (ids) => LoadProductsAsync(mediator, ids, context));

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.AssociatedObjectId);
                    return await loadHandle;
                })
            };
            AddField(productField);
        }

        public static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var query = context.GetCatalogQuery<LoadProductQuery>();
            query.Ids = ids.ToArray();
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths();

            var response = await mediator.Send(query);
            return response.Products.ToDictionary(x => x.Id);
        }
    }
}
