namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangePurchaseOrderNumberCommand : CartCommand
    {
        /// <summary>
        /// Purchase Order Number
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
    }
}
