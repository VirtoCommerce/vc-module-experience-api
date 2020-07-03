using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}
