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
            var payment = await GetPaymentInfo(request);

            var validationResult = ValidateRequest(payment, request);
            if (validationResult != null)
            {
                return ErrorResult<InitializePaymentResult>(validationResult);
            }

            var paymentRequest = new ProcessPaymentRequest
            {
                OrderId = payment.CustomerOrder.Id,
                Order = payment.CustomerOrder,
                PaymentId = payment.Payment.Id,
                Payment = payment.Payment,
                StoreId = payment.Store.Id,
                Store = payment.Store,
            };

            var result = await InitializePaymentAsync(paymentRequest);

            return result;
        }

        private Task<InitializePaymentResult> InitializePaymentAsync(ProcessPaymentRequest request)
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
                result.PublicParameters = processPaymentResult.PublicParameters;
                result.PaymentMethodCode = payment.PaymentMethod.Code;
                result.PaymentActionType = payment.PaymentMethod.PaymentMethodType.ToString();
            }

            return Task.FromResult(result);
        }
    }
}
