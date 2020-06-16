using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Builders;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Factories;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Stores;
using VirtoCommerce.ExperienceApiModule.XPurchase.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Queries
{
    public class GetCartQuery : ObjectGraphType
    {
        public GetCartQuery(IShoppingCartAggregateFactory cartFactory)
        {
            FieldAsync<CartType>(
                "cart",
                "Get cart for current user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StoreType>> { Name = "store" },
                    new QueryArgument<StringGraphType> { Name = "cartName" },
                    new QueryArgument<NonNullGraphType<UserType>> { Name = "user" },
                    new QueryArgument<NonNullGraphType<LanguageType>> { Name = "language" },
                    new QueryArgument<NonNullGraphType<CurrencyType>> { Name = "currency" },
                    new QueryArgument<StringGraphType> { Name = "type" }
                ),
                resolve: async context =>
                {
                    var store = context.GetArgument<Store>("store");
                    var cartName = context.GetArgument<string>("cartName");
                    var user = context.GetArgument<User>("user");
                    var language = context.GetArgument<Language>("language");
                    var currency = context.GetArgument<Currency>("currency");
                    var type = context.GetArgument<string>("type");

                    var shoppingCartContext = CartContextBuilder.Build()
                                                                .WithStore(store)
                                                                .WithCartName(cartName)
                                                                .WithUser(user)
                                                                .WithLanguage(language)
                                                                .WithCurrency(currency)
                                                                .WithCartType(type)
                                                                .GetContext();

                    var cartAggregate = await cartFactory.CreateOrGetShoppingCartAggregateAsync(shoppingCartContext);

                    await cartAggregate.ValidateAsync();

                    return cartAggregate.Cart;
                });
        }
    }
}
