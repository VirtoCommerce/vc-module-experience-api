using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XOrder.Core;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Commands
{
    public class AddOrUpdateOrderPaymentCommandHandler : IRequestHandler<AddOrUpdateOrderPaymentCommand, CustomerOrderAggregate>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;

        public AddOrUpdateOrderPaymentCommandHandler(
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            ICustomerOrderService customerOrderService,
            IPaymentMethodsSearchService paymentMethodsSearchService)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderService = customerOrderService;
            _paymentMethodsSearchService = paymentMethodsSearchService;
        }

        public async Task<CustomerOrderAggregate> Handle(AddOrUpdateOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);

            var paymentId = request.Payment.Id?.Value;
            var payment = orderAggregate.Order.InPayments.FirstOrDefault(s => paymentId != null && s.Id == paymentId);
            payment = request.Payment.MapTo(payment);

            var criteria = new PaymentMethodsSearchCriteria
            {
                IsActive = true,
                StoreId = orderAggregate.Order.StoreId,
            };

            if (!string.IsNullOrEmpty(payment.GatewayCode))
            {
                criteria.Codes = new List<string> { payment.GatewayCode };
            }

            var result = await _paymentMethodsSearchService.SearchAsync(criteria);

            await orderAggregate.AddPaymentAsync(payment, result.Results);

            if (!request.Payment.DynamicProperties.IsNullOrEmpty())
            {
                await orderAggregate.UpdatePaymentDynamicProperties(paymentId, request.Payment.DynamicProperties);
            }

            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });

            return orderAggregate;
        }
    }
}
