using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushMessageUnreadCommandHandler : IRequestHandler<MarkPushMessageUnreadCommand, bool>
    {
        public Task<bool> Handle(MarkPushMessageUnreadCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
