using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class AuthorizePaymentCommand : ICommand<AuthorizePaymentResult>
    {
        public string OrderId { get; set; }

        public string PaymentId { get; set; }

        //public string PaymentMethodCode { get; set; }

        public KeyValuePair[] Parameters { get; set; }
    }
}
