using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Validators;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
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
            var paymentInfo = await GetPaymentInfo(request);

            var validationResult = AbstractTypeFactory<PaymentRequestValidator>.TryCreateInstance().Validate(paymentInfo);
            if (!validationResult.IsValid)
            {
                return ErrorResult<AuthorizePaymentResult>(validationResult.Errors.FirstOrDefault().ErrorMessage);
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

            var result = new AuthorizePaymentResult
            {
                IsSuccess = processPaymentRequestResult.IsSuccess,
                ErrorMessage = processPaymentRequestResult.ErrorMessage,
            };

            if (result.IsSuccess)
            {
                await _customerOrderService.SaveChangesAsync(new[] { paymentInfo.CustomerOrder });
            }

            return result;
        }
    }
}
