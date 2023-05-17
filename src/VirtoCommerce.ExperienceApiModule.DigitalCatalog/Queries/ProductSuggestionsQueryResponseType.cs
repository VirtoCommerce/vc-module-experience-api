using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQueryResponseType : ExtendableGraphType<ProductSuggestionsQueryResponse>
{
    public ProductSuggestionsQueryResponseType()
    {
        Field<ListGraphType<StringGraphType>>(
            nameof(ProductSuggestionsQueryResponse.Suggestions),
            resolve: context => context.Source.Suggestions);
    }
}
