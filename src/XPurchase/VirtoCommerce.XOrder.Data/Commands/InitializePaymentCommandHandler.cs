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
    public class InitializePaymentCommandHandler : PaymentCommandHandlerBase, IRequestHandler<InitializePaymentCommand, InitializePaymentResult>
    {
        public InitializePaymentCommandHandler(
            ICustomerOrderService customerOrderService,
            IPaymentService paymentService,
            IStoreService storeService)
            : base(customerOrderService, paymentService, storeService)
        {
        }

        public async Task<InitializePaymentResult> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentInfo = await GetPaymentInfo(request);

            var validationResult = AbstractTypeFactory<PaymentRequestValidator>.TryCreateInstance().Validate(paymentInfo);
            if (!validationResult.IsValid)
            {
                return ErrorResult<InitializePaymentResult>(validationResult.Errors.FirstOrDefault()?.ErrorMessage);
            }

            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderId = paymentInfo.CustomerOrder.Id,
                Order = paymentInfo.CustomerOrder,
                PaymentId = paymentInfo.Payment.Id,
                Payment = paymentInfo.Payment,
                StoreId = paymentInfo.Store.Id,
                Store = paymentInfo.Store,
            };

            var processPaymentResult = paymentInfo.Payment.PaymentMethod.ProcessPayment(processPaymentRequest);

            var result = new InitializePaymentResult
            {
                StoreId = processPaymentRequest.StoreId,
                PaymentId = processPaymentRequest.PaymentId,
                OrderId = processPaymentRequest.OrderId,
                OrderNumber = paymentInfo.CustomerOrder.Number,
                IsSuccess = processPaymentResult.IsSuccess,
                ErrorMessage = processPaymentResult.ErrorMessage,
                ActionHtmlForm = processPaymentResult.HtmlForm,
                ActionRedirectUrl = processPaymentResult.RedirectUrl,
                PublicParameters = processPaymentResult.PublicParameters,
                PaymentMethodCode = paymentInfo.Payment.PaymentMethod.Code,
                PaymentActionType = paymentInfo.Payment.PaymentMethod.PaymentMethodType.ToString()
            };

            return result;
        }
    }
}
