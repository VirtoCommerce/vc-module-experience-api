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
    public class InitializePaymentCommandHandler : PaymentCommandHandlerBase, IRequestHandler<InitializePaymentCommand, InitializePaymentResult>
    {
        public InitializePaymentCommandHandler(
            ICrudService<CustomerOrder> customerOrderService,
            ICrudService<PaymentIn> paymentService,
            ICrudService<Store> storeService)
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
