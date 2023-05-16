using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQueryBuilder : QueryBuilder<ProductSuggestionsQuery, ProductSuggestionsQueryResponse, ProductSuggestionsQueryResponseType>
{
    protected override string Name => "ProductSuggestions";

    public ProductSuggestionsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }
}
