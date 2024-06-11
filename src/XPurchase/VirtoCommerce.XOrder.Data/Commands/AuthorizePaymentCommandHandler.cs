using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XOrder.Core.Commands;
using VirtoCommerce.XOrder.Core.Models;
using VirtoCommerce.XOrder.Core.Validators;

namespace VirtoCommerce.XOrder.Data.Commands
{
    public class AuthorizePaymentCommandHandler : PaymentCommandHandlerBase, IRequestHandler<AuthorizePaymentCommand, AuthorizePaymentResult>
    {
        private readonly ICustomerOrderService _customerOrderService;

        public AuthorizePaymentCommandHandler(
            ICustomerOrderService customerOrderService,
            IPaymentService paymentService,
            IStoreService storeService)
            : base(customerOrderService, paymentService, storeService)
        {
            _customerOrderService = customerOrderService;
        }

        public async Task<AuthorizePaymentResult> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentInfo = await GetPaymentInfo(request);

            var validationResult = AbstractTypeFactory<PaymentRequestValidator>.TryCreateInstance().Validate(paymentInfo);
            if (!validationResult.IsValid)
            {
                return ErrorResult<AuthorizePaymentResult>(validationResult.Errors.FirstOrDefault()?.ErrorMessage);
            }

            var parameters = GetParameters(request);

            var validatePostProcessResult = paymentInfo.Payment.PaymentMethod.ValidatePostProcessRequest(parameters);
            if (!validatePostProcessResult.IsSuccess)
            {
                return ErrorResult<AuthorizePaymentResult>(validatePostProcessResult.ErrorMessage);
            }

            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                OrderId = paymentInfo.CustomerOrder.Id,
                Order = paymentInfo.CustomerOrder,
                PaymentId = paymentInfo.Payment.Id,
                Payment = paymentInfo.Payment,
                StoreId = paymentInfo.CustomerOrder.StoreId,
                Store = paymentInfo.Store,
                Parameters = parameters,
            };

            var processPaymentRequestResult = paymentInfo.Payment.PaymentMethod.PostProcessPayment(postProcessPaymentRequest);

            if (processPaymentRequestResult.IsSuccess)
            {
                paymentInfo.Payment.Status = processPaymentRequestResult.NewPaymentStatus.ToString();

                await _customerOrderService.SaveChangesAsync(new[] { paymentInfo.CustomerOrder });
            }

            var result = new AuthorizePaymentResult
            {
                IsSuccess = processPaymentRequestResult.IsSuccess,
                ErrorMessage = processPaymentRequestResult.ErrorMessage,
            };

            return result;
        }
    }
}
