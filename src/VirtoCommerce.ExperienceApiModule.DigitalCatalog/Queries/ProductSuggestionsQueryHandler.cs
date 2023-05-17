using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Search.Indexed;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQueryHandler : IQueryHandler<ProductSuggestionsQuery, ProductSuggestionsQueryResponse>
{
    private readonly IProductSuggestionService _productSuggestionService;

    public ProductSuggestionsQueryHandler(IProductSuggestionService productSuggestionService)
    {
        _productSuggestionService = productSuggestionService;
    }

    public async Task<ProductSuggestionsQueryResponse> Handle(ProductSuggestionsQuery query, CancellationToken cancellationToken)
    {
        var result = AbstractTypeFactory<ProductSuggestionsQueryResponse>.TryCreateInstance();

        if (string.IsNullOrWhiteSpace(query.Query) || query.Size < 1)
        {
            return result;
        }

        var request = AbstractTypeFactory<SuggestionRequest>.TryCreateInstance();
        request.Query = query.Query;
        request.Fields = query.Fields;
        request.Size = query.Size;

        var response = await _productSuggestionService.GetSuggestionsAsync(request);

        if (response.Suggestions != null)
        {
            result.Suggestions.AddRange(response.Suggestions);
        }

        return result;
    }
}
