using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}
