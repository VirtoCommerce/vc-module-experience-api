using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
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
