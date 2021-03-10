using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommand : CartCommand
    {
        public AddOrUpdateCartPaymentCommand()
        {
        }

        public AddOrUpdateCartPaymentCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, Payment payment)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            Payment = payment;
        }

        public Payment Payment { get; set; }
    }
}
