namespace VirtoCommerce.XPurchase.Domain.CartAggregate
{
    public class NewItemComment
    {
        public NewItemComment(string lineItemId, string comment)
        {
            LineItemId = lineItemId;
            Comment = comment;
        }
        public string LineItemId { get; private set; }
        public string Comment { get; private set; }
    }
}
