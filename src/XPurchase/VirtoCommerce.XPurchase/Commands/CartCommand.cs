using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class CartCommand : IRequest<CartAggregate>
    {
        protected CartCommand()
        {

        }
        protected CartCommand(string storeId, string type, string cartName, string userId, string currency, string language)
        {
            StoreId = storeId;
            CartType = type;
            CartName = cartName;
            UserId = userId;
            Currency = currency;
            Language = language;
        }
        public string StoreId { get;  set; }
        public string CartType { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
    }
}
