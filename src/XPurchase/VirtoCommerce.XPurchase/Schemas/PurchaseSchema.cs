using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.XPurchase.Authorization;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PurchaseSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;
        private readonly Func<SignInManager<ApplicationUser>> _signInManagerFactory;
        public const string _commandName = "command";

        public PurchaseSchema(IMediator mediator, IAuthorizationService authorizationService, Func<SignInManager<ApplicationUser>> signInManagerFactory)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _signInManagerFactory = signInManagerFactory;
        }

        public void Build(ISchema schema)
        {
            #region Queries

            //We can't use the fluent syntax for new types registration provided by dotnet graphql here, because we have the strict requirement for underlying types extensions
            //and must use GraphTypeExtenstionHelper to resolve the effective type on execution time
            var cartField = new FieldType
            {
                Name = "cart",
                Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId", Description = "Store Id" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId", Description = "User Id" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                        new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-Us\")" },
                        new QueryArgument<StringGraphType> { Name = "cartName", Description = "Cart name" },
                        new QueryArgument<StringGraphType> { Name = "type", Description = "Cart type" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    //TODO: Move to extension methods

                    var storeId = context.GetArgument<string>("storeId");
                    var cartName = context.GetArgument("cartName", "default");
                    var userId = context.GetArgument<string>("userId");
                    var cultureName = context.GetArgument<string>("cultureName");
                    var currencyCode = context.GetArgument<string>("currencyCode");
                    var type = context.GetArgument<string>("type");

                    var getCartQuery = new GetCartQuery(storeId, type, cartName, userId, currencyCode, cultureName);
                    var cartAggregate = await _mediator.Send(getCartQuery);
                    if (cartAggregate == null)
                    {
                        var createCartCommand = new CreateCartCommand(storeId, type, cartName, userId, currencyCode, cultureName);
                        cartAggregate = await _mediator.Send(createCartCommand);
                    }
                    await CheckAuthAsync(context, cartAggregate.Cart);
                    //store cart aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("cartAggregate", cartAggregate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(cartField);

            var cartsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CartType, object>()
                .Name("carts")
                .Argument<StringGraphType>("storeId", "")
                .Argument<StringGraphType>("userId", "")
                .Argument<StringGraphType>("currencyCode", "")
                .Argument<StringGraphType>("cultureName", "")
                .Argument<StringGraphType>("cartType", "")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            cartsConnectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(_mediator, context));

            schema.Query.AddField(cartsConnectionBuilder.FieldType);

            #endregion Queries

            #region Mutations

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#additem"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddItemType!){ addItem(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "productId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "quantity": 1
            ///      }
            ///   }
            /// }
            /// </example>
            var addItemField = FieldBuilderHelper.CreateCartAggregateMutation<InputAddItemType, AddCartItemCommand>("addItem", _mediator);

            schema.Mutation.AddField(addItemField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#clearcart"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputClearCartType!){ clearCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart"
            ///      }
            ///   }
            /// }
            /// </example>
            var clearCartField = FieldBuilderHelper.CreateCartAggregateMutation<InputClearCartType, ClearCartCommand>("clearCart", _mediator);

            schema.Mutation.AddField(clearCartField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#changecomment"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCommentType!){ changeComment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "comment": "Hi, Virto!"
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCommentField = FieldBuilderHelper.CreateCartAggregateMutation<InputChangeCommentType, ChangeCommentCommand>("clearCart", _mediator);

            schema.Mutation.AddField(changeCommentField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#changecartitemprice"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemPriceType!){ changeCartItemPrice(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "productId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "price": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemPriceField = FieldBuilderHelper.CreateCartAggregateMutation<InputChangeCartItemPriceType, ChangeCartItemPriceCommand>("changeCartItemPrice", _mediator);

            schema.Mutation.AddField(changeCartItemPriceField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#changecartitemquantity"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemQuantityType!){ changeCartItemQuantity(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "quantity": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemQuantityField = FieldBuilderHelper.CreateCartAggregateMutation<InputChangeCartItemQuantityType, ChangeCartItemQuantityCommand>("changeCartItemQuantity", _mediator);

            schema.Mutation.AddField(changeCartItemQuantityField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#changecartitemcomment"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemCommentType!){ changeCartItemComment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "comment": "verynicecomment"
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemCommentField = FieldBuilderHelper.CreateCartAggregateMutation<InputChangeCartItemCommentType, ChangeCartItemCommentCommand>("changeCartItemComment", _mediator);

            schema.Mutation.AddField(changeCartItemCommentField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#removecartitem"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveItemType!){ removeCartItem(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCartItemField = FieldBuilderHelper.CreateCartAggregateMutation<InputRemoveItemType, RemoveCartItemCommand>("removeCartItem", _mediator);

            schema.Mutation.AddField(removeCartItemField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#addcoupon"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddCouponType!){ addCoupon(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "couponCode": "verynicecouponcode"
            ///      }
            ///   }
            /// }
            /// </example>
            var addCouponField = FieldBuilderHelper.CreateCartAggregateMutation<InputAddCouponType, AddCouponCommand>("addCoupon", _mediator);

            schema.Mutation.AddField(addCouponField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#removecoupon"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveCouponType!){ removeCoupon(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "couponCode": "verynicecouponcode"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCouponField = FieldBuilderHelper.CreateCartAggregateMutation<InputRemoveCouponType, RemoveCouponCommand>("removeCoupon", _mediator);

            schema.Mutation.AddField(removeCouponField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#removeshipment"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveShipmentType!){ removeShipment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "shipmentId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeShipmentField = FieldBuilderHelper.CreateCartAggregateMutation<InputRemoveShipmentType, RemoveShipmentCommand>("removeShipment", _mediator);

            schema.Mutation.AddField(removeShipmentField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#addorupdatecartshipment"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddOrUpdateCartShipmentType!){ addOrUpdateCartShipment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "shipment": {
            ///              "fulfillmentCenterId": "7777-7777-7777-7777",
            ///              "height": 7,
            ///              "shipmentMethodCode": "code",
            ///              "currency": "USD",
            ///              "price": 98
            ///          }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartShipmentField = FieldBuilderHelper.CreateCartAggregateMutation<InputAddOrUpdateCartShipmentType, AddOrUpdateCartShipmentCommand>("addOrUpdateCartShipment", _mediator);

            schema.Mutation.AddField(addOrUpdateCartShipmentField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#addorupdatecartpayment"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddOrUpdateCartPaymentType!){ addOrUpdateCartPayment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "payment": {
            ///              "outerId": "7777-7777-7777-7777",
            ///              "paymentGatewayCode": "code",
            ///              "currency": "USD",
            ///              "price": 98,
            ///              "amount": 55
            ///          }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartPaymentField = FieldBuilderHelper.CreateCartAggregateMutation<InputAddOrUpdateCartPaymentType, AddOrUpdateCartPaymentCommand>("addOrUpdateCartPayment", _mediator);

            schema.Mutation.AddField(addOrUpdateCartPaymentField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#validatecoupon"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputValidateCouponType!){ validateCoupon(command: $command) }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "coupon": {
            ///             "code":"verynicecodeforvalidation"
            ///         }
            ///      }
            ///   }
            /// }
            /// </example>
            var validateCouponField = FieldBuilderHelper.CreateCartAggregateOperation<InputValidateCouponType, ValidateCouponCommand>("validateCoupon", _mediator);

            schema.Mutation.AddField(validateCouponField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#mergecart"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:MergeCartType!){ mergeCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "cultureName": "en-US",
            ///          "currencyCode": "USD",
            ///          "cartType": "cart",
            ///          "secondCartId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var margeCartField = FieldBuilderHelper.CreateCartAggregateMutation<InputMergeCartType, MergeCartCommand>("mergeCart", _mediator);

            schema.Mutation.AddField(margeCartField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#removecart"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveCartType!){ removeCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "cartId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCartField = FieldBuilderHelper.CreateCartAggregateOperation<InputRemoveCartType, RemoveCartCommand>("removeCart", _mediator);

            schema.Mutation.AddField(removeCartField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#clearshipments"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputClearShipmentsType!){ clearShipments(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "cartId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var clearShipmentsField = FieldBuilderHelper.CreateCartAggregateMutation<InputClearShipmentsType, ClearShipmentsCommand>("clearShipments", _mediator);

            schema.Mutation.AddField(clearShipmentsField);

            /// <seealso href="https://github.com/VirtoCommerce/vc-module-experience-api/blob/dev/docs/x-purchase-cart-reference.md#clearpayments"/>
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputClearPaymentsType!){ clearPayments(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "cartId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var clearPaymentsField = FieldBuilderHelper.CreateCartAggregateMutation<InputClearPaymentsType, ClearPaymentsCommand>("clearPayments", _mediator);

            schema.Mutation.AddField(clearPaymentsField);

            #endregion Mutations
        }

        private async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = context.GetSearchCartQuery<SearchCartQuery>();
            query.Skip = skip;
            query.Take = first ?? context.PageSize ?? 10;
            query.Sort = context.GetArgument<string>("sort");

            context.UserContext.Add(nameof(Currency.CultureName).ToCamelCase(), query.CultureName);

            await CheckAuthAsync(context, query);

            var response = await mediator.Send(query);
            foreach (var cartAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(cartAggregate);
            }

            var result = new Connection<CartAggregate>()
            {
                Edges = response.Results
                    .Select((x, index) =>
                        new Edge<CartAggregate>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = response.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount,
            };

            return result;
        }

        private async Task CheckAuthAsync(IResolveFieldContext context, object resource)
        {
            var userId = context.GetArgument<string>("userId");

            if (userId == null)
            {
                throw new ExecutionError($"argument {nameof(userId)} is null");
            }

            var signInManager = _signInManagerFactory();
            var user = await signInManager.UserManager.FindByIdAsync(userId) ?? new ApplicationUser
            {
                Id = userId,
                UserName = "Anonymous",
            };

            var userPrincipal = await signInManager.ClaimsFactory.CreateAsync(user);
            var authorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, resource, new CanAccessCartAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new ExecutionError($"access denied by userId:{userId}");
            }
        }
    }
}
