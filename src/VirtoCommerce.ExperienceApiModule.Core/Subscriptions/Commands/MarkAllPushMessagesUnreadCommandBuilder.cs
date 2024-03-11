using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkAllPushMessagesUnreadCommandBuilder : CommandBuilder<MarkAllPushMessagesUnreadCommand, bool, BooleanGraphType>
    {
        protected override string Name => "pushMessagesMarkUneadAll";

        public MarkAllPushMessagesUnreadCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
        }

        protected override Task BeforeMediatorSend(IResolveFieldContext<object> context, MarkAllPushMessagesUnreadCommand request)
        {
            request.UserId = context.GetCurrentUserId();
            return base.BeforeMediatorSend(context, request);
        }
    }
}
