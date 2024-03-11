using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushMessageUnreadCommandBuilder : CommandBuilder<MarkAllPushMessagesUnreadCommand, bool, InputMarkPushMessageUnreadType, BooleanGraphType>
    {
        protected override string Name => "pushMessageMarkUnread";

        public MarkPushMessageUnreadCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
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
