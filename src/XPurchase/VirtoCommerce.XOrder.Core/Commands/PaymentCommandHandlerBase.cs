using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XOrder.Core.Models;

namespace VirtoCommerce.XOrder.Core.Commands
{
    public abstract class PaymentCommandHandlerBase
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IPaymentService _paymentService;
        private readonly IStoreService _storeService;

        protected PaymentCommandHandlerBase(
            ICustomerOrderService customerOrderService,
            IPaymentService paymentService,
            IStoreService storeService)
        {
            _customerOrderService = customerOrderService;
            _paymentService = paymentService;
            _storeService = storeService;
        }

        protected async Task<PaymentInfo> GetPaymentInfo(PaymentCommandBase command)
        {
            var result = new PaymentInfo();

            if (!string.IsNullOrEmpty(command.OrderId))
            {
                result.CustomerOrder = await _customerOrderService.GetByIdAsync(command.OrderId, CustomerOrderResponseGroup.Full.ToString());
                result.Payment = result.CustomerOrder?.InPayments?.FirstOrDefault(x => x.Id == command.PaymentId);
            }
            else if (!string.IsNullOrEmpty(command.PaymentId))
            {
                result.Payment = await _paymentService.GetByIdAsync(command.PaymentId);
                result.CustomerOrder = await _customerOrderService.GetByIdAsync(result.Payment?.OrderId, CustomerOrderResponseGroup.Full.ToString());

                // take payment from order since Order payment contains instanced PaymentMethod (payment taken from service doesn't)
                result.Payment = result.CustomerOrder?.InPayments?.FirstOrDefault(x => x.Id == command.PaymentId);
            }

            result.Store = await _storeService.GetByIdAsync(result.CustomerOrder?.StoreId, StoreResponseGroup.StoreInfo.ToString());

            return result;
        }

        protected static NameValueCollection GetParameters(AuthorizePaymentCommand request)
        {
            var parameters = new NameValueCollection();
            foreach (var param in request?.Parameters ?? Array.Empty<KeyValue>())
            {
                parameters.Add(param.Key, param.Value);
            }

            return parameters;
        }

        protected static T ErrorResult<T>(string error) where T : PaymentResult, new()
        {
            return new T
            {
                IsSuccess = false,
                ErrorMessage = error,
            };
        }
    }
}
