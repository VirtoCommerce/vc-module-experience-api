using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Validators;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class InitializePaymentCommandHandler : IRequestHandler<InitializePaymentCommand, InitializePaymentResult>
    {
        private readonly ICrudService<CustomerOrder> _customerOrderService;
        private readonly ICrudService<PaymentIn> _paymentService;
        private readonly ICrudService<Store> _storeService;

        public InitializePaymentCommandHandler(
            ICrudService<CustomerOrder> customerOrderService,
            ICrudService<PaymentIn> paymentService,
            ICrudService<Store> storeService)
        {
            _customerOrderService = customerOrderService;
            _paymentService = paymentService;
            _storeService = storeService;
        }

        public async Task<InitializePaymentResult> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
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

            var paymentRequest = new ProcessPaymentRequest
            {
                OrderId = customerOrder.Id,
                Order = customerOrder,
                PaymentId = payment.Id,
                Payment = payment,
                StoreId = store.Id,
                Store = store,
            };

            var result = await InitializePaymentAsync(paymentRequest);

            await _customerOrderService.SaveChangesAsync(new[] { customerOrder });

            return result;
        }

        protected virtual Task<InitializePaymentResult> InitializePaymentAsync(ProcessPaymentRequest request)
        {
            var order = request.Order as CustomerOrder;

            var result = new InitializePaymentResult
            {
                StoreId = request.StoreId,
                PaymentId = request.PaymentId,
                OrderId = request.OrderId,
                OrderNumber = order?.Number,
            };

            var validationResult = AbstractTypeFactory<ProcessPaymentRequestValidator>.TryCreateInstance().Validate(request);
            if (!validationResult.IsValid)
            {
                result.IsSuccess = false;
                result.ErrorMessage = string.Join(';', validationResult.Errors);
            }

            if (request.Payment is PaymentIn payment)
            {
                var processPaymentResult = payment.PaymentMethod.ProcessPayment(request);

                if (processPaymentResult.OuterId != null)
                {
                    payment.OuterId = processPaymentResult.OuterId;
                }

                result.IsSuccess = processPaymentResult.IsSuccess;
                result.ErrorMessage = processPaymentResult.ErrorMessage;
                result.ActionHtmlForm = processPaymentResult.HtmlForm;
                result.ActionRedirectUrl = processPaymentResult.RedirectUrl;
                result.PaymentMethodCode = payment.PaymentMethod.Code;
                result.PaymentActionType = payment.PaymentMethod.PaymentMethodType.ToString();
            }

            return Task.FromResult(result);
        }

        private InitializePaymentResult ErrorResult(string error)
        {
            return new InitializePaymentResult
            {
                IsSuccess = false,
                ErrorMessage = error,
            };
        }
    }
}
