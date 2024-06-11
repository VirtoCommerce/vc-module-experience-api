using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XOrder.Core.Validators
{
    public class OrderPaymentValidator : AbstractValidator<OrderPaymentValidationContext>
    {
        public OrderPaymentValidator()
        {
            RuleFor(x => x).Custom((paymentContext, context) =>
            {
                var availPaymentMethods = paymentContext.AvailPaymentMethods;
                var paymentCode = paymentContext.Payment?.GatewayCode;

                if (availPaymentMethods != null && !string.IsNullOrEmpty(paymentCode))
                {
                    var paymentMethod = availPaymentMethods.FirstOrDefault(x => paymentCode.EqualsInvariant(x.Code));
                    if (paymentMethod == null)
                    {
                        context.AddFailure(OrderErrorDescriber.PaymentMethodUnavailable(paymentCode));
                    }
                }
            });
        }
    }
}
