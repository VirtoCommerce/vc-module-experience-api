using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class AuthorizePaymentCommand : PaymentCommandBase, ICommand<AuthorizePaymentResult>
    {
        public KeyValue[] Parameters { get; set; }
    }
}
