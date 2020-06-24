using GraphQL.Types;
using VirtoCommerce.XPurchase.Domain.Models;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartContextInputType : InputObjectGraphType<ShoppingCartContext>
    {
        public CartContextInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("storeId", resolve: context => context.Source.StoreId);
            Field<NonNullGraphType<StringGraphType>>("cartName", resolve: context => context.Source.CartName);
            Field<NonNullGraphType<StringGraphType>>("userId", resolve: context => context.Source.UserId);
            Field<NonNullGraphType<StringGraphType>>("cultureName", resolve: context => context.Source.CultureName);
            Field<NonNullGraphType<StringGraphType>>("currencyCode", resolve: context => context.Source.CurrencyCode);
            Field<NonNullGraphType<StringGraphType>>("type", resolve: context => context.Source.Type);
        }
    }
}
