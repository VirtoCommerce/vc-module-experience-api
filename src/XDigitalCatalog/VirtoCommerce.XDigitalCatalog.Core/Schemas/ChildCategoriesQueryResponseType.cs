using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas;

public class ChildCategoriesQueryResponseType : ExtendableGraphType<ChildCategoriesQueryResponse>
{
    public ChildCategoriesQueryResponseType()
    {
        Field<ListGraphType<CategoryType>>(
            nameof(ChildCategoriesQueryResponse.ChildCategories),
            resolve: context => context.Source.ChildCategories);
    }
}
