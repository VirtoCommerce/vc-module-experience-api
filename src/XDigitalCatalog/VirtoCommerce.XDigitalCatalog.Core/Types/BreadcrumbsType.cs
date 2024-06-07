using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class BreadcrumbType : ObjectGraphType<Breadcrumb>
    {
        public BreadcrumbType()
        {
            Name = "Breadcrumb";
            Field(x => x.ItemId, nullable: false).Description("Id of item the breadcrumb calculated for");
            Field(x => x.Title, nullable: false).Description("Name of current breadcrumb");
            Field(x => x.TypeName, nullable: false).Description("Catalog, category or product");
            Field(x => x.SeoPath, nullable: true).Description("Full path from catalog");
            Field(x => x.SemanticUrl, nullable: true).Description("Semantic URL keyword");
        }
    }
}
