using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XOrder.Core.Models;

namespace VirtoCommerce.XOrder.Core.Commands
{
    public class InitializePaymentCommand : PaymentCommandBase, ICommand<InitializePaymentResult>
    {
    }
}
