using System;
using System.Linq;
using FluentValidation;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.XOrder.Core.Models;

namespace VirtoCommerce.XOrder.Core.Validators
{
    public class PaymentRequestValidator : AbstractValidator<PaymentInfo>
    {
        private readonly PaymentStatus[] _invalidStates = new[] { PaymentStatus.Paid };

        public PaymentRequestValidator()
        {
            RuleFor(x => x.CustomerOrder)
                .NotNull()
                .WithMessage(OrderErrorDescriber.OrderNotFound());

            RuleFor(x => x.Payment)
                .NotNull()
                .WithMessage(OrderErrorDescriber.PaymentNotFound());

            RuleFor(x => x.Payment.PaymentMethod)
                .NotNull()
                .WithMessage(x => OrderErrorDescriber.PaymentMethodNotFound(x.Payment.GatewayCode))
                .When(x => x.Payment != null);

            RuleFor(x => x.Store)
                .NotNull()
                .WithMessage(OrderErrorDescriber.StoreNotFound());

            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    if (_invalidStates.Contains(request.Payment.PaymentStatus))
                    {
                        context.AddFailure(OrderErrorDescriber.InvalidStatus(request.Payment.PaymentStatus));
                    }
                })
                .When(x => x.Payment != null);
        }
    }
}
