using MediatR;

namespace VirtoCommerce.XPurchase.Queries
{
    public interface IQueryHandler<in TQuery, TResult> :
         IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
    }
}
