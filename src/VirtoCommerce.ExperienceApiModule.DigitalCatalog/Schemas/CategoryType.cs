using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Category";

            Field(x => x.Id, nullable: false).Description("Id of category.");
            Field(x => x.Code, nullable: false).Description("SKU of category.");
            Field(x => x.Name, nullable: false).Description("Name of category.");
            Field<StringGraphType>("slug", resolve: context => "Stub slug :)");
            // TODO: maybe we need smart loading here in case we load only parentIds
            Field<CategoryType>("parent", resolve: context => context.Source.Parent);
        }
    }
}
