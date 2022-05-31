using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class AuthorizePaymentCommand : PaymentCommandBase, ICommand<AuthorizePaymentResult>
    {
        public KeyValuePair[] Parameters { get; set; }
    }
}
