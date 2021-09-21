using System.Linq;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Validators
{
    public class CartValidator : AbstractValidator<CartValidationContext>
    {
        protected virtual CartLineItemValidator LineItemValidator { get; set; }
        protected virtual CartShipmentValidator ShipmentValidator { get; set; }
        protected virtual CartPaymentValidator PaymentValidator { get; set; }

        public CartValidator()
        {
            LineItemValidator = AbstractTypeFactory<CartLineItemValidator>.TryCreateInstance();
            ShipmentValidator = AbstractTypeFactory<CartShipmentValidator>.TryCreateInstance();
            PaymentValidator = AbstractTypeFactory<CartPaymentValidator>.TryCreateInstance();

            RuleFor(x => x.CartAggregate.Cart).NotNull();
            When(x => x.CartAggregate.Cart != null, () =>
            {
                RuleFor(x => x.CartAggregate.Cart.Name).NotEmpty();
                RuleFor(x => x.CartAggregate.Cart.Currency).NotEmpty();
                RuleFor(x => x.CartAggregate.Cart.CustomerId).NotEmpty();

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
                            var result = LineItemValidator.Validate(lineItemContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });

                        cartContext.CartAggregate.Cart.Shipments?.Apply(shipment =>
                        {
                            var shipmentContext = new ShipmentValidationContext
                            {
                                Shipment = shipment,
                                AvailShippingRates = cartContext.AvailShippingRates
                            };
                            var result = ShipmentValidator.Validate(shipmentContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });

                        cartContext.CartAggregate.Cart.Payments?.Apply(payment =>
                        {
                            var paymentContext = new PaymentValidationContext
                            {
                                Payment = payment,
                                AvailPaymentMethods = cartContext.AvailPaymentMethods
                            };
                            var result = PaymentValidator.Validate(paymentContext);
                            result.Errors.Apply(x => context.AddFailure(x));
                        });
                    });
                });
            });
        }
    }
}
