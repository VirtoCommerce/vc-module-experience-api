using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.XDigitalCatalog.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Core.Queries;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class ProductAssociationType : ObjectGraphType<ProductAssociation>
    {
        public ProductAssociationType(IDataLoaderContextAccessor dataLoader, IMediator mediator)
        {
            Name = "ProductAssociation";
            Description = "product association.";

            Field(d => d.Type, nullable: false);
            Field(d => d.Priority, nullable: false);
            Field("Quantity", x => x.Quantity, nullable: true, type: typeof(IntGraphType));
            Field(d => d.AssociatedObjectId, nullable: true);
            Field(d => d.AssociatedObjectType, nullable: true);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("tags", resolve: context => context.Source.Tags?.ToList() ?? new List<string>());

            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new FuncFieldResolver<ProductAssociation, IDataLoaderResult<ExpProduct>>(context =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("associatedProductLoader", (ids) => LoadProductsAsync(mediator, ids, context));
                    return loader.LoadAsync(context.Source.AssociatedObjectId);
                })
            };
            AddField(productField);
        }

        public static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var query = context.GetCatalogQuery<LoadProductsQuery>();
            query.ObjectIds = ids.ToArray();
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths(context).Select(x => x.Replace("associations.items.product", string.Empty)).ToArray();

            var response = await mediator.Send(query);
            return response.Products.ToDictionary(x => x.Id);
        }
    }
}
