using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries;

public class OrderLineItemStatusesQueryBuilder : LocalizedSettingQueryBuilder<OrderLineItemStatusesQuery>
{
    public OrderLineItemStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "OrderLineItemStatuses";
}
