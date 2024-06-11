using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Services;

namespace VirtoCommerce.XOrder.Data.Commands
{
    public class ProcessOrderPaymentCommandHandler : IRequestHandler<ProcessOrderPaymentCommand, ProcessPaymentRequestResult>
    {
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IStoreService _storeService;

        public ProcessOrderPaymentCommandHandler(
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            ICustomerOrderService customerOrderService,
            IStoreService storeService)
        {
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _customerOrderService = customerOrderService;
            _storeService = storeService;
        }

        public virtual async Task<ProcessPaymentRequestResult> Handle(ProcessOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            var orderAggregate = await _customerOrderAggregateRepository.GetOrderByIdAsync(request.OrderId);
            if (orderAggregate == null)
            {
                throw new OperationCanceledException($"Can't find an order with ID: {request.OrderId}");
            }
            var store = await _storeService.GetByIdAsync(orderAggregate.Order.StoreId, StoreResponseGroup.StoreInfo.ToString());

            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderId = orderAggregate.Order.Id,
                PaymentId = request.PaymentId,
                StoreId = orderAggregate.Order.StoreId,
                Store = store,
                BankCardInfo = request.BankCardInfo
            };
            var result = orderAggregate.ProcessOrderPayment(processPaymentRequest);

            await _customerOrderService.SaveChangesAsync(new[] { orderAggregate.Order });

            return result;
        }
    }
}
