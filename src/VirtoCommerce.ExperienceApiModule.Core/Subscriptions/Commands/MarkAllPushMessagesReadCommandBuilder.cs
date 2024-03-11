using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkAllPushMessagesReadCommandBuilder : CommandBuilder<MarkAllPushMessagesReadCommand, bool, BooleanGraphType>
    {
        protected override string Name => "pushMessagesMarkReadAll";

        public MarkAllPushMessagesReadCommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
            : base(mediator, authorizationService)
        {
        }

        protected override Task BeforeMediatorSend(IResolveFieldContext<object> context, MarkAllPushMessagesReadCommand request)
        {
            request.UserId = context.GetCurrentUserId();
            return base.BeforeMediatorSend(context, request);
        }
    }
}
