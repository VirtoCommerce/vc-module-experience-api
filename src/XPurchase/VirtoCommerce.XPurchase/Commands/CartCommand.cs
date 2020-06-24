using MediatR;

namespace VirtoCommerce.XPurchase.Domain.Commands
{
    public abstract class CartCommand : IRequest
    {
        protected CartCommand(string storeId, string cartType, string cartName, string userId, string currency, string language)
        {
            StoreId = storeId;
            CartType = cartType;
            CartName = cartName;
            UserId = userId;
            Currency = currency;
            Language = language;
        }
        public string StoreId { get;  private set; }
        public string CartType { get; private set; }
        public string CartName { get; private set; }
        public string UserId { get; private set; }
        public string Currency { get; private set; }
        public string Language { get; private set; }
    }
}
