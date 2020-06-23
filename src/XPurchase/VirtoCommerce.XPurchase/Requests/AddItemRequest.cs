using MediatR;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Requests
{
    public class AddItemRequest : IRequest<AddItemResponse>
    {
        public ShoppingCartContext CartContext { get; set; }
        public AddCartItem CartItem { get; set; }
    }
}
