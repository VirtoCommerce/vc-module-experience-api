using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries;

public class ShipmentStatusesQueryBuilder : LocalizedSettingQueryBuilder<ShipmentStatusesQuery>
{
    public ShipmentStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "ShipmentStatuses";
}
