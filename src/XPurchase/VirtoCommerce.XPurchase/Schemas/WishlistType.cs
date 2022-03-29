using System.Linq;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class WishlistType : ExtendableGraphType<CartAggregate>
    {
        public WishlistType()
        {
            Field(x => x.Cart.Id, nullable: true).Description("Shopping cart ID");
            Field(x => x.Cart.Name, nullable: false).Description("Shopping cart name");
            Field(x => x.Cart.StoreId, nullable: true).Description("Shopping cart store ID");
            Field(x => x.Cart.CustomerId, nullable: true).Description("Shopping cart user ID");
            Field(x => x.Cart.CustomerName, nullable: true).Description("Shopping cart user name");
            Field<CurrencyType>("currency", "Currency", resolve: context => context.Source.Currency);
            ExtendableField<ListGraphType<LineItemType>>("items", "Items", resolve: context => context.Source.LineItems);
            Field<IntGraphType>("itemsCount", "Item count", resolve: context => context.Source.LineItems.Count());
        }
    }
}
