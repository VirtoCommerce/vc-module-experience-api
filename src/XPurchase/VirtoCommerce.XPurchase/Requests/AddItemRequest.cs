using MediatR;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Requests
{
    public class AddItemRequest : IRequest<AddItemResponse>
    {
        public string StoreId { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }
        public string Type { get; set; }
        public AddCartItem CartItem { get; set; }
    }
}
