using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQueryResponseType : ExtendableGraphType<ChildCategoriesQueryResponse>
{
    public ChildCategoriesQueryResponseType()
    {
        ExtendableField<ListGraphType<CategoryType>>(
            nameof(ChildCategoriesQueryResponse.ChildCategories),
            resolve: context => context.Source.ChildCategories);
    }
}
