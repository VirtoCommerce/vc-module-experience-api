using System.Linq;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartPaymentValidator : AbstractValidator<Payment>
    {
        public CartPaymentValidator(CartAggregate cartAggr)
        {
            RuleFor(x => x.PaymentGatewayCode).NotNull().NotEmpty();
            RuleSet("strict", () =>
            {
                RuleFor(x => x).CustomAsync(async (payment, context, cancellationToken) =>
                {
                    var paymentMethod =  (await cartAggr.GetAvailablePaymentMethodsAsync()).FirstOrDefault(x => payment.PaymentGatewayCode.EqualsInvariant(x.Code));
                    if (paymentMethod == null)
                    {
                        context.AddFailure(CartErrorDescriber.PaymentMethodUnavailable(payment, payment.PaymentGatewayCode));
                    }                 
                });
            });
        }
    }
}
