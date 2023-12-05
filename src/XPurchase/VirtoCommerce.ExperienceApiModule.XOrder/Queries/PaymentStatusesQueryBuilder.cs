using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries;

public class PaymentStatusesQueryBuilder : LocalizedSettingQueryBuilder<PaymentStatusesQuery>
{
    public PaymentStatusesQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override string Name => "PaymentStatuses";
}
