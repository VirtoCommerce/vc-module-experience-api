using System;
using System.Linq;
using VirtoCommerce.OrdersModule.Data.Repositories;

namespace VirtoCommerce.XPurchase.Services
{
    public class MemberOrdersService : IMemberOrdersService
    {
        private readonly Func<IOrderRepository> _orderRepositoryFactory;

        public MemberOrdersService(Func<IOrderRepository> orderRepositoryFactory)
        {
            _orderRepositoryFactory = orderRepositoryFactory;
        }

        public bool IsFirstBuyer(string customerId)
        {
            using var orderRepository = _orderRepositoryFactory();
            return !orderRepository.CustomerOrders
                .Any(x => x.CustomerId == customerId);
            }

    }
}
