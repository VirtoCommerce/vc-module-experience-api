using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Queries.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries;

public abstract class LocalizedSettingQueryBuilder<TQuery> : QueryBuilder<TQuery, LocalizedSettingResponse, LocalizedSettingResponseType>
    where TQuery : LocalizedSettingQuery
{
    protected LocalizedSettingQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }
}
