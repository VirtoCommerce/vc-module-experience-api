using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class ConfirmOrderPaymentCommand : ICommand<bool>
    {
        public ConfirmOrderPaymentCommand(PaymentIn payment)
        {
            Payment = payment;
        }

        public PaymentIn Payment { get; set; }
    }
}
