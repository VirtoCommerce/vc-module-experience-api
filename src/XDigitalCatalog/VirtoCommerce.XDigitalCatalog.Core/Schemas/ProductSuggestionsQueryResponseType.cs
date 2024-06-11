using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas;

public class ProductSuggestionsQueryResponseType : ExtendableGraphType<ProductSuggestionsQueryResponse>
{
    public ProductSuggestionsQueryResponseType()
    {
        Field<ListGraphType<StringGraphType>>(
            nameof(ProductSuggestionsQueryResponse.Suggestions),
            resolve: context => context.Source.Suggestions);
    }
}
