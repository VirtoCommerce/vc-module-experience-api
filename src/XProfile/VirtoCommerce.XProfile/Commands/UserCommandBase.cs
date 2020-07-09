using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public abstract class UserCommandBase<T> : ICommand<T>
    {
        public string UserId { get; set; }

    }
}
