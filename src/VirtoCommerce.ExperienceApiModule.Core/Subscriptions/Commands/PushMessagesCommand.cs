using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public abstract class PushMessagesCommand : ICommand<bool>
    {
        public string UserId { get; set; }
    }
}
