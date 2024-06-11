using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.XOrder.Core.Queries;

namespace VirtoCommerce.XOrder.Data.Queries;

public class OrderLineItemStatusesQueryBuilder : LocalizedSettingQueryBuilder<OrderLineItemStatusesQuery>
{
    public OrderLineItemStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "OrderLineItemStatuses";
}
