using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommand : CartCommand
    {
        public AddOrUpdateCartPaymentCommand()
        {
        }

        public AddOrUpdateCartPaymentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, ExpCartPayment payment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Payment = payment;
        }

        public ExpCartPayment Payment { get; set; }
    }
}
