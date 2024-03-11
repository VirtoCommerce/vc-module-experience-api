using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkAllPushMessagesReadCommandHandler : IRequestHandler<MarkAllPushMessagesReadCommand, bool>
    {
        public Task<bool> Handle(MarkAllPushMessagesReadCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
