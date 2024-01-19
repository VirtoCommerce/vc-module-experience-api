using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQueryResponseType : ExtendableGraphType<ChildCategoriesQueryResponse>
{
    public ChildCategoriesQueryResponseType()
    {
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<CategoryType>>>>(
            nameof(ChildCategoriesQueryResponse.ChildCategories),
            resolve: context => context.Source.ChildCategories);
    }
}
