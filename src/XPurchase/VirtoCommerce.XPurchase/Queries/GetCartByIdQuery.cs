using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetCartByIdQuery : IQuery<CartAggregate>
    {
        public GetCartByIdQuery()
        {
        }
        public GetCartByIdQuery(string cartId)
        {
            CartId = cartId;
        }
        public string CartId { get; set; }
       
    }
}
