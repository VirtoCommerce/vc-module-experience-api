using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Breadcrumbs;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class BreadcrumbType : ObjectGraphType<Breadcrumb>
    {
        public BreadcrumbType()
        {
            Name = "Breadcrumb";

            Field(x => x.Title).Description("Name of current breadcrumb");
            Field(x => x.TypeName, nullable: true).Description("Catalog, category or product");
            Field(x => x.SeoPath, nullable: true).Description("Full path from catalog");
        }
    }
}
