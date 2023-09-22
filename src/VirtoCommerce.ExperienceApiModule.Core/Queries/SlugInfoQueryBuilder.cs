using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SlugInfoQueryBuilder : QueryBuilder<SlugInfoQuery, SlugInfoResponse, SlugInfoResponseType>
    {
        public SlugInfoQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
        }

        protected override string Name => "slugInfo";
    }
}
