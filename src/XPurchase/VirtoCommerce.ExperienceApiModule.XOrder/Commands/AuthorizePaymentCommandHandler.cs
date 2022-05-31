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
    public class AuthorizePaymentCommandHandler : IRequestHandler<AuthorizePaymentCommand, AuthorizePaymentResult>
    {
        private readonly ICrudService<CustomerOrder> _customerOrderService;
        private readonly ICrudService<PaymentIn> _paymentService;
        private readonly ICrudService<Store> _storeService;

        public AuthorizePaymentCommandHandler(
            ICrudService<CustomerOrder> customerOrderService,
            ICrudService<PaymentIn> paymentService,
            ICrudService<Store> storeService)
        {
            _customerOrderService = customerOrderService;
            _paymentService = paymentService;
            _storeService = storeService;
        }

        public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
        {
            var customerOrder = default(CustomerOrder);
            var payment = default(PaymentIn);

            if (!string.IsNullOrEmpty(request.OrderId))
            {
                customerOrder = await _customerOrderService.GetByIdAsync(request.OrderId, CustomerOrderResponseGroup.Full.ToString());
                payment = customerOrder?.InPayments?.FirstOrDefault(x => x.Id == request.PaymentId);
            }
            else if (!string.IsNullOrEmpty(request.PaymentId))
            {
                payment = await _paymentService.GetByIdAsync(request.PaymentId);
                customerOrder = await _customerOrderService.GetByIdAsync(payment?.OrderId, CustomerOrderResponseGroup.Full.ToString());

                // take payment from order since Order payment contains instanced PaymentMethod (payment taken from service doesn't)
                payment = customerOrder?.InPayments?.FirstOrDefault(x => x.Id == request.PaymentId);
            }

            if (customerOrder == null)
            {
                return ErrorResult($"Cannot find order with ID {request.OrderId}");
            }

            if (payment == null)
            {
                return ErrorResult($"Cannot find payment with ID {request.PaymentId}");
            }

            if (payment.PaymentStatus == PaymentStatus.Paid)
            {
                return ErrorResult($"Document {payment.Number} is already paid");
            }

            var store = await _storeService.GetByIdAsync(customerOrder.StoreId, StoreResponseGroup.StoreInfo.ToString());
            if (store == null)
            {
                return ErrorResult($"Cannot find store with ID {customerOrder.StoreId}");
            }

            var parameters = GetParameters(request);

            var validateResult = payment.PaymentMethod.ValidatePostProcessRequest(parameters);
            if (!validateResult.IsSuccess)
            {
                return ErrorResult(validateResult.ErrorMessage);
            }

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                OrderId = customerOrder.Id,
                Order = customerOrder,
                PaymentId = payment.Id,
                Payment = payment,
                StoreId = customerOrder.StoreId,
                Store = store,
                OuterId = validateResult.OuterId,
                Parameters = parameters
            };

            var result = await AuthorizePaymentAsync(postProcessPaymentRequest);

            if (result.IsSuccess)
            {
                await _customerOrderService.SaveChangesAsync(new[] { customerOrder });
            }

            return result;
        }

        protected virtual Task<AuthorizePaymentResult> AuthorizePaymentAsync(PostProcessPaymentRequest request)
        {
            var payment = request.Payment as PaymentIn;
            var order = request.Order as CustomerOrder;

            var processPaymentRequestResult = payment.PaymentMethod.PostProcessPayment(request);

            var result = new AuthorizePaymentResult
            {
                IsSuccess = processPaymentRequestResult.IsSuccess,
                ErrorMessage = processPaymentRequestResult.ErrorMessage
            };

            return Task.FromResult(result);
        }


        private static NameValueCollection GetParameters(AuthorizePaymentCommand request)
        {
            var parameters = new NameValueCollection();
            foreach (var param in request?.Parameters ?? Array.Empty<KeyValuePair>())
            {
                parameters.Add(param.Key, param.Value);
            }

            return parameters;
        }

        private static AuthorizePaymentResult ErrorResult(string error)
        {
            return new AuthorizePaymentResult
            {
                IsSuccess = false,
                ErrorMessage = error,
            };
        }
    }
}
