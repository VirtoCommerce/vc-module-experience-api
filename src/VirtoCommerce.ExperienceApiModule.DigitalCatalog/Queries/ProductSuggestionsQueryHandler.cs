using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search.Indexed;
using VirtoCommerce.CatalogModule.Core.Search.Indexed;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQueryHandler : IQueryHandler<ProductSuggestionsQuery, ProductSuggestionsQueryResponse>
{
    private readonly IProductSuggestionService _productSuggestionService;
    private readonly ICrudService<Store> _storeService;

    public ProductSuggestionsQueryHandler(IProductSuggestionService productSuggestionService, ICrudService<Store> storeService)
    {
        _productSuggestionService = productSuggestionService;
        _storeService = storeService;
    }

    public async Task<ProductSuggestionsQueryResponse> Handle(ProductSuggestionsQuery query, CancellationToken cancellationToken)
    {
        var result = AbstractTypeFactory<ProductSuggestionsQueryResponse>.TryCreateInstance();

        if (string.IsNullOrWhiteSpace(query.Query) || query.Size < 1)
        {
            return result;
        }

        var request = AbstractTypeFactory<ProductSuggestionRequest>.TryCreateInstance();
        request.Query = query.Query;
        request.Size = query.Size;

        var store = await _storeService.GetByIdAsync(query.StoreId);
        if (store is null)
        {
            return result;
        }

        var response = await _productSuggestionService.GetSuggestionsAsync(request);

        if (response.Suggestions != null)
        {
            result.Suggestions.AddRange(response.Suggestions);
        }

        return result;
    }
}
