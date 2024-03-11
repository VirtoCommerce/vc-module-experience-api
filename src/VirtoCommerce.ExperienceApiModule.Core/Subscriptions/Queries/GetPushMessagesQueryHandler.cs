using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Queries
{
    public class GetPushMessagesQueryHandler : IQueryHandler<GetPushMessagesQuery, PushMessagesResponse>
    {
        public Task<PushMessagesResponse> Handle(GetPushMessagesQuery request, CancellationToken cancellationToken)
        {
            var result = new PushMessagesResponse();

            for (var i = 1; i <= 10; i++)
            {
                var message = new PushMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    ShortMessage = "Test message " + i,
                    Status = i < 5 ? "Unread" : "Read",
                    CreatedDate = DateTime.UtcNow,
                    UserId = request.UserId,
                };

                result.Items.Add(message);
            }

            result.UnreadCount = result.Items.Count(x => x.Status == "Unread");

            return Task.FromResult(result);
        }
    }
}
