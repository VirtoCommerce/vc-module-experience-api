using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommand : CartCommand
    {
        public AddOrUpdateCartPaymentCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public AddOrUpdateCartPaymentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartPayment payment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Payment = payment;
        }

        public ExpCartPayment Payment { get; set; }
    }
}
