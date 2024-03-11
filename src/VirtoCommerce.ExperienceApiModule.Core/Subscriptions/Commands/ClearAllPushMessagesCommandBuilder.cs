using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class ClearAllPushMessagesCommandBuilder : CommandBuilder<ClearAllPushMessageCommand, bool, BooleanGraphType>
    {
        protected override string Name => "pushMessagesClearAll";

        public ClearAllPushMessagesCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
        }

        protected override Task BeforeMediatorSend(IResolveFieldContext<object> context, ClearAllPushMessageCommand request)
        {
            request.UserId = context.GetCurrentUserId();
            return base.BeforeMediatorSend(context, request);
        }
    }
}
