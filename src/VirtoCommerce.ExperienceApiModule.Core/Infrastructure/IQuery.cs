using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
