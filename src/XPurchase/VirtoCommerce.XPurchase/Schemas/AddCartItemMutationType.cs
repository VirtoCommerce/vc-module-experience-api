using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Interfaces;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Requests;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class AddCartItemMutationType : IBuildableMutation
    {
        public FieldType GetMutationType(IMediator mediator, IDataLoaderContextAccessor dataLoader) => new FieldType
        {
            Name = "addCartItem",
            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<CartContextInputType>> { Name = "context" },
                new QueryArgument<NonNullGraphType<AddCartItemInputType>> { Name = "cartItem" }
            ),
            Type = GraphTypeExtenstionHelper.GetActualType<IntGraphType>(),
            Resolver = new AsyncFieldResolver<int>(async context =>
            {
                var request = new AddItemRequest
                {
                    CartContext = context.GetArgument<ShoppingCartContext>("context"),
                    CartItem = context.GetArgument<AddCartItem>("cartItem")
                };

                var responce = await mediator.Send(request);

                return responce.ItemsQuantity;
            })
        };
    }
}
