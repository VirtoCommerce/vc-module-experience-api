using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.XOrder.Core.Queries;

namespace VirtoCommerce.XOrder.Data.Queries;

public class ShipmentStatusesQueryBuilder : LocalizedSettingQueryBuilder<ShipmentStatusesQuery>
{
    public ShipmentStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "ShipmentStatuses";
}
