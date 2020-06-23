using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
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
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cartName" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "type" },
                new QueryArgument<NonNullGraphType<AddCartItemType>> { Name = "cartItem" }
            ),
            Type = GraphTypeExtenstionHelper.GetActualType<IntGraphType>(),
            Resolver = new AsyncFieldResolver<int>(async context =>
            {
                var request = new AddItemRequest
                {
                    StoreId = context.GetArgument<string>("storeId"),
                    CartName = context.GetArgument<string>("cartName"),
                    UserId = context.GetArgument<string>("userId"),
                    CultureName = context.GetArgument<string>("cultureName"),
                    CurrencyCode = context.GetArgument<string>("currencyCode"),
                    Type = context.GetArgument<string>("type"),
                    CartItem = context.GetArgument<AddCartItem>("cartItem")
                };

                var responce = await mediator.Send(request);

                return responce.ItemsQuantity;
            })
        };
    }
}
