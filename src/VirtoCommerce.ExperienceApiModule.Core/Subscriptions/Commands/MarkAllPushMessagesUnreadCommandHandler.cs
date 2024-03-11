using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkAllPushMessagesUnreadCommandHandler : IRequestHandler<MarkAllPushMessagesUnreadCommand, bool>
    {
        public Task<bool> Handle(MarkAllPushMessagesUnreadCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
