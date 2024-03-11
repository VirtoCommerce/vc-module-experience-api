using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class ClearAllPushMessagesCommandHandler : IRequestHandler<ClearAllPushMessageCommand, bool>
    {
        public Task<bool> Handle(ClearAllPushMessageCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
