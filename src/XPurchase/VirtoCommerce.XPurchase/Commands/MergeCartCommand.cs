using System;

namespace VirtoCommerce.XPurchase.Commands
{
    public class MergeCartCommand : CartCommand
    {
        public MergeCartCommand()
        {
        }

        [Obsolete("Use context.GetCartCommand<>() or object initializer", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public MergeCartCommand(string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName, string secondCartId)
            : base(storeId, cartType, cartName, userId, currencyCode, cultureName)
        {
            SecondCartId = secondCartId;
        }

        public string SecondCartId { get; set; }

        public bool DeleteAfterMerge { get; set; } = true;
    }
}
