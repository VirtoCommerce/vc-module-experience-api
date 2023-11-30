using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries;

public class OrderStatusesQueryBuilder : LocalizedSettingQueryBuilder<OrderStatusesQuery>
{
    public OrderStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "OrderStatuses";
}
