namespace VirtoCommerce.XOrder.Core.Commands
{
    public class PaymentCommandBase
    {
        public string OrderId { get; set; }

        public string PaymentId { get; set; }
    }
}
