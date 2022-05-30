using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class InitializePaymentCommand : ICommand<InitializePaymentResult>
    {
        public string OrderId { get; set; }

        public string PaymentId { get; set; }
    }
}
