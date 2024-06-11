using System.Linq;
using FluentValidation;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;

namespace VirtoCommerce.XOrder.Core.Validators
{
    public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
    {
        private readonly PaymentStatus[] _availStatusesForProcessing = new[] { PaymentStatus.New, PaymentStatus.Custom };
        public ProcessPaymentRequestValidator()
        {
            RuleFor(x => x.Order).NotNull();
            RuleFor(x => x.Payment).NotNull().WithMessage(OrderErrorDescriber.PaymentNotFound());
            RuleFor(x => ((PaymentIn)x.Payment).PaymentMethod).NotNull().WithMessage(x => OrderErrorDescriber.PaymentMethodNotFound(((PaymentIn)x.Payment).GatewayCode));
            RuleFor(x => x.Store).NotNull().WithMessage(OrderErrorDescriber.StoreNotFound());
            When(x => x.BankCardInfo != null, () =>
            {
                RuleFor(x => x.BankCardInfo.BankCardNumber).NotNull().NotEmpty().CreditCard();
                RuleFor(x => x.BankCardInfo.BankCardCVV2).NotNull().NotEmpty();
                RuleFor(x => x.BankCardInfo.BankCardType).NotNull().NotEmpty();
                RuleFor(x => x.BankCardInfo.BankCardYear).NotNull().NotEmpty();
                RuleFor(x => x.BankCardInfo.BankCardMonth).NotNull().NotEmpty();
                RuleFor(x => x.BankCardInfo.CardholderName).NotNull().NotEmpty();
            });
            RuleFor(x => x).Custom((request, context) =>
            {
                var payment = request.Payment as PaymentIn;
                if (!_availStatusesForProcessing.Contains(payment.PaymentStatus))
                {
                    context.AddFailure(OrderErrorDescriber.InvalidStatus(payment.PaymentStatus, _availStatusesForProcessing));
                }
            });
        }
    }
}
