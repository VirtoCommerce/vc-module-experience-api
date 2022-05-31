namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class PaymentCommandBase
    {
        public string OrderId { get; set; }

        public string PaymentId { get; set; }
    }
}
