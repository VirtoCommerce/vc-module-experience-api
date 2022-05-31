using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public abstract class PaymentCommandHandlerBase
    {
        private readonly ICrudService<CustomerOrder> _customerOrderService;
        private readonly ICrudService<PaymentIn> _paymentService;
        private readonly ICrudService<Store> _storeService;

        public PaymentCommandHandlerBase(
            ICrudService<CustomerOrder> customerOrderService,
            ICrudService<PaymentIn> paymentService,
            ICrudService<Store> storeService)
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

        protected static string ValidateRequest(PaymentInfo payment, PaymentCommandBase command)
        {
            if (payment.CustomerOrder == null)
            {
                return $"Cannot find order with ID {command.OrderId}";
            }

            if (payment.Payment == null)
            {
                return $"Cannot find payment with ID {command.PaymentId}";
            }

            if (payment.Payment.PaymentStatus == PaymentStatus.Paid)
            {
                return $"Document {payment.Payment.Number} is already paid";
            }

            if (payment.Store == null)
            {
                return $"Cannot find store with ID {payment.CustomerOrder?.StoreId}";
            }

            return null;
        }

        protected static NameValueCollection GetParameters(AuthorizePaymentCommand request)
        {
            var parameters = new NameValueCollection();
            foreach (var param in request?.Parameters ?? Array.Empty<KeyValuePair>())
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

        public class PaymentInfo
        {
            public CustomerOrder CustomerOrder { get; set; }

            public PaymentIn Payment { get; set; }

            public Store Store { get; set; }
        }
    }
}
