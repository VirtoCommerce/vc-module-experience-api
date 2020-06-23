using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Builders;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Interfaces;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ClearCartQueryType : ISimpleQueryType
    {
        public FieldType GetQueryType(IShoppingCartAggregateFactory cartFactory) => new FieldType
        {
            Name = "clearCart",
            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cartName" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "type" }
            ),
            Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
            Resolver = new AsyncFieldResolver<ShoppingCart>(async context =>
            {
                var storeId = context.GetArgument<string>("storeId");
                var cartName = context.GetArgument<string>("cartName");
                var userId = context.GetArgument<string>("userId");
                var cultureName = context.GetArgument<string>("cultureName");
                var currencyCode = context.GetArgument<string>("currencyCode");
                var type = context.GetArgument<string>("type");

                var shoppingCartContext = CartContextBuilder.Build()
                                                            .WithStore(storeId)
                                                            .WithCartName(cartName)
                                                            .WithUser(userId)
                                                            .WithCurrencyAndLanguage(currencyCode, cultureName)
                                                            .WithCartType(type)
                                                            .GetContext();

                var cartAggregate = await cartFactory.CreateOrGetShoppingCartAggregateAsync(shoppingCartContext);

                await cartAggregate.ClearAsync();

                await cartAggregate.SaveAsync();

                return cartAggregate.Cart;
            })
        };
    }
}
