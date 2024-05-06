using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class InitializePaymentCommand : PaymentCommandBase, ICommand<InitializePaymentResult>
    {
    }
}
