using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public abstract class UserCommandBase<T> : IRequest<T>
    {
        public string UserId { get; set; }

    }
}
