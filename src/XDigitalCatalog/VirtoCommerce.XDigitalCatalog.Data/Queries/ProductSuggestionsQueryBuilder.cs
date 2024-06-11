using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Core.Queries;
using VirtoCommerce.XDigitalCatalog.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Data.Queries;

public class ProductSuggestionsQueryBuilder : QueryBuilder<ProductSuggestionsQuery, ProductSuggestionsQueryResponse, ProductSuggestionsQueryResponseType>
{
    protected override string Name => "ProductSuggestions";

    public ProductSuggestionsQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }
}
