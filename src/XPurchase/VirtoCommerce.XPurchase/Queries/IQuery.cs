using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace VirtoCommerce.XPurchase.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
