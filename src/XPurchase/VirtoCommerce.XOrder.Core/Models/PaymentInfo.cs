using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XOrder.Core.Models
{
    public class PaymentInfo
    {
        public CustomerOrder CustomerOrder { get; set; }

        public PaymentIn Payment { get; set; }

        public Store Store { get; set; }
    }
}
