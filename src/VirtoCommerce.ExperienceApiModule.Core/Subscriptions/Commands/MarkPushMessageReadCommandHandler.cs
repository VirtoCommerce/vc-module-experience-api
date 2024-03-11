using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushMessageReadCommandHandler : IRequestHandler<MarkPushMessageReadCommand, bool>
    {
        public Task<bool> Handle(MarkPushMessageReadCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
