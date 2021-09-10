using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidator : AbstractValidator<CartValidationContext>
    {
        public CartValidator()
        {
            RuleFor(x => x.CartAggregate.Cart).NotNull();
            When(x => x.CartAggregate.Cart != null, () =>
            {
                RuleFor(x => x.CartAggregate.Cart.Name).NotNull().NotEmpty();
                RuleFor(x => x.CartAggregate.Cart.Currency).NotNull();
                RuleFor(x => x.CartAggregate.Cart.CustomerId).NotNull().NotEmpty();

                RuleSet("strict", () =>
                {
                    RuleFor(x => x).Custom((cartContext, context) =>
                    {
                        cartContext.CartAggregate.Cart.Items?.Apply(item =>
                        {
                            var lineItemContext = new LineItemValidationContext
                            {
                                LineItem = item,
                                AllCartProducts = cartContext.AllCartProducts ?? cartContext.CartAggregate.CartProducts.Values
                            };
                            var result = AbstractTypeFactory<CartLineItemValidator>.TryCreateInstance().Validate(lineItemContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });

                        cartContext.CartAggregate.Cart.Shipments?.Apply(shipment =>
                        {
                            var shipmentContext = new ShipmentValidationContext
                            {
                                Shipment = shipment,
                                AvailShippingRates = cartContext.AvailShippingRates
                            };
                            var result = AbstractTypeFactory<CartShipmentValidator>.TryCreateInstance().Validate(shipmentContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });

                        cartContext.CartAggregate.Cart.Payments?.Apply(payment =>
                        {
                            var paymentContext = new PaymentValidationContext
                            {
                                Payment = payment,
                                AvailPaymentMethods = cartContext.AvailPaymentMethods
                            };
                            var result = AbstractTypeFactory<CartPaymentValidator>.TryCreateInstance().Validate(paymentContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });
                    });
                });
            });
        }
    }
}
