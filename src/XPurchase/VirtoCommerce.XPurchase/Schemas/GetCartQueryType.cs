using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Builders;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Interfaces;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class GetCartQueryType : ISimpleQueryType
    {
        public FieldType GetQueryType(IShoppingCartAggregateFactory cartFactory) => new FieldType
        {
            Name = "cart",
            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<CartContextInputType>> { Name = "context" }
            ),
            Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
            Resolver = new AsyncFieldResolver<ShoppingCart>(async context =>
            {
                var cartContext = context.GetArgument<ShoppingCartContext>("context");

                var shoppingCartContext = CartContextBuilder.Initialize(cartContext)
                                                            .WithDefaults()
                                                            .GetContext();

                var cartAggregate = await cartFactory.CreateOrGetShoppingCartAggregateAsync(shoppingCartContext);

                await cartAggregate.EvaluateTaxesAsync();

                await cartAggregate.EvaluatePromotionsAsync();

                await cartAggregate.ValidateAsync();

                context.UserContext.Add("taxCalculationEnabled", shoppingCartContext.Store.TaxCalculationEnabled);

                context.UserContext.Add("fixedTaxRate", shoppingCartContext.Store.FixedTaxRate);

                return cartAggregate.Cart;
            })
        };
    }
}
