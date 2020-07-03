using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
    }
}
