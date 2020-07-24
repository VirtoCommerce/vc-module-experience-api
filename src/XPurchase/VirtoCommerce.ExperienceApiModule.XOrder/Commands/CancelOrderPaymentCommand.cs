using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class CancelOrderPaymentCommand : ICommand<bool>
    {
        public CancelOrderPaymentCommand(PaymentIn payment)
        {
            Payment = payment;
        }

        public PaymentIn Payment { get; set; }
    }
}
