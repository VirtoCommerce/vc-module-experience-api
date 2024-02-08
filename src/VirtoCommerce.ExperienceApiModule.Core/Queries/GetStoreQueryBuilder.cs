using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetStoreQueryBuilder : QueryBuilder<GetStoreQuery, StoreResponse, StoreResponseType>
    {
        protected override string Name => "store";

        public GetStoreQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
        }
    }
}
