using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
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

        protected string ValidateRequest(PaymentInfo payment, PaymentCommandBase command)
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

        protected T ErrorResult<T>(string error) where T : PaymentResult, new()
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

    public class AuthorizePaymentCommandHandler : PaymentCommandHandlerBase, IRequestHandler<AuthorizePaymentCommand, AuthorizePaymentResult>
    {
        private readonly ICrudService<CustomerOrder> _customerOrderService;

        public AuthorizePaymentCommandHandler(
            ICrudService<CustomerOrder> customerOrderService,
            ICrudService<PaymentIn> paymentService,
            ICrudService<Store> storeService)
            : base(customerOrderService, paymentService, storeService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await GetPaymentInfo(request);

            var validationResult = ValidateRequest(payment, request);
            if (validationResult != null)
            {
                return ErrorResult<AuthorizePaymentResult>(validationResult);
            }

            var parameters = GetParameters(request);

            var validateResult = payment.Payment.PaymentMethod.ValidatePostProcessRequest(parameters);
            if (!validateResult.IsSuccess)
            {
                return ErrorResult<AuthorizePaymentResult>(validateResult.ErrorMessage);
            }

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                OrderId = payment.CustomerOrder.Id,
                Order = payment.CustomerOrder,
                PaymentId = payment.Payment.Id,
                Payment = payment.Payment,
                StoreId = payment.CustomerOrder.StoreId,
                Store = payment.Store,
                OuterId = validateResult.OuterId,
                Parameters = parameters
            };

            var processPaymentRequestResult = payment.Payment.PaymentMethod.PostProcessPayment(postProcessPaymentRequest);

            var result = new AuthorizePaymentResult
            {
                IsSuccess = processPaymentRequestResult.IsSuccess,
                ErrorMessage = processPaymentRequestResult.ErrorMessage,
            };

            if (result.IsSuccess)
            {
                await _customerOrderService.SaveChangesAsync(new[] { payment.CustomerOrder });
            }

            return result;
        }
    }
}
