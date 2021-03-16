using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
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

        public const string _commandName = "command";

        public PurchaseSchema(IMediator mediator, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        public void Build(ISchema schema)
        {
            //Queries
            //We can't use the fluent syntax for new types registration provided by dotnet graphql here, because we have the strict requirement for underlying types extensions
            //and must use GraphTypeExtenstionHelper to resolve the effective type on execution time
            var cartField = new FieldType
            {
                Name = "cart",
                Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId", Description = "Store Id" },
                        new QueryArgument<StringGraphType> { Name = "userId", Description = "User Id" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                        new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-Us\")" },
                        new QueryArgument<StringGraphType> { Name = "cartName", Description = "Cart name" },
                        new QueryArgument<StringGraphType> { Name = "type", Description = "Cart type" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var getCartQuery = context.GetCartQuery<GetCartQuery>();
                    getCartQuery.IncludeFields = context.SubFields.Values.GetAllNodesPaths().ToArray();
                    context.CopyArgumentsToUserContext();
                    var cartAggregate = await _mediator.Send(getCartQuery);
                    if (cartAggregate == null)
                    {
                        var createCartCommand = new CreateCartCommand(getCartQuery.StoreId, getCartQuery.CartType, getCartQuery.CartName, getCartQuery.UserId, getCartQuery.CurrencyCode, getCartQuery.CultureName);
                        cartAggregate = await _mediator.Send(createCartCommand);
                    }

                    context.SetExpandedObjectGraph(cartAggregate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(cartField);


            var orderConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CartType, object>()
                .Name("carts")
                .Argument<StringGraphType>("storeId", "")
                .Argument<StringGraphType>("userId", "")
                .Argument<StringGraphType>("currencyCode", "")
                .Argument<StringGraphType>("cultureName", "")
                .Argument<StringGraphType>("cartType", "")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            orderConnectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(_mediator, context));

            schema.Query.AddField(orderConnectionBuilder.FieldType);

            //Mutations
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddItemType!){ addItem(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "productId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "quantity": 1
            ///      }
            ///   }
            /// }
            /// </example>
            var addItemField = new EventStreamFieldType
            {
                Name = "addItem",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddItemType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<AddCartItemCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(addItemField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputClearCartType!){ clearCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart"
            ///      }
            ///   }
            /// }
            /// </example>
            var clearCartField = new EventStreamFieldType
            {
                Name = "clearCart",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputClearCartType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ClearCartCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(clearCartField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCommentType!){ changeComment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "comment": "Hi, Virto!"
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCommentField = new EventStreamFieldType
            {
                Name = "changeComment",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetQueryArguments<InputChangeCommentType>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCommentCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(changeCommentField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemPriceType!){ changeCartItemPrice(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "price": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemPriceField = new EventStreamFieldType
            {
                Name = "changeCartItemPrice",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputChangeCartItemPriceType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemPriceCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(changeCartItemPriceField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemQuantityType!){ changeCartItemQuantity(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "quantity": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemQuantityField = new EventStreamFieldType
            {
                Name = "changeCartItemQuantity",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputChangeCartItemQuantityType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemQuantityCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(changeCartItemQuantityField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemCommentType!){ changeCartItemComment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "comment": "verynicecomment"
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemCommentField = new EventStreamFieldType
            {
                Name = "changeCartItemComment",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputChangeCartItemCommentType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemCommentCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(changeCartItemCommentField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveItemType!){ removeCartItem(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCartItemField = new EventStreamFieldType
            {
                Name = "removeCartItem",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputRemoveItemType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveCartItemCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(removeCartItemField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddCouponType!){ addCoupon(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "couponCode": "verynicecouponcode"
            ///      }
            ///   }
            /// }
            /// </example>
            var addCouponField = new EventStreamFieldType
            {
                Name = "addCoupon",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddCouponType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<AddCouponCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(addCouponField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveCouponType!){ removeCoupon(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "couponCode": "verynicecouponcode"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCouponField = new EventStreamFieldType
            {
                Name = "removeCoupon",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputRemoveCouponType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveCouponCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(removeCouponField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveShipmentType!){ removeShipment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "shipmentId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeShipmentField = new EventStreamFieldType
            {
                Name = "removeShipment",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputRemoveShipmentType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveShipmentCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(removeShipmentField);

            //TODO: add shipment model to example
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddOrUpdateCartShipmentType!){ addOrUpdateCartShipment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "shipment": { }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartShipmentField = new EventStreamFieldType
            {
                Name = "addOrUpdateCartShipment",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddOrUpdateCartShipmentType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<AddOrUpdateCartShipmentCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(addOrUpdateCartShipmentField);

            //TODO: add payment model to example
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddOrUpdateCartPaymentType!){ addOrUpdateCartPayment(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "payment": { }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartPaymentField = new EventStreamFieldType
            {
                Name = "addOrUpdateCartPayment",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddOrUpdateCartPaymentType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<AddOrUpdateCartPaymentCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(addOrUpdateCartPaymentField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputValidateCouponType!){ validateCoupon(command: $command) }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "coupon": {
            ///             "code":"verynicecodeforvalidation"
            ///         }
            ///      }
            ///   }
            /// }
            /// </example>
            var validateCouponField = new EventStreamFieldType
            {
                Name = "validateCoupon",
                Type = typeof(BooleanGraphType),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputValidateCouponType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, bool>(async context =>
                    (bool)await _mediator.Send(context.GetArgument(GenericTypeHelper.GetActualType<ValidateCouponCommand>(), _commandName)))
            };

            schema.Mutation.AddField(validateCouponField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:MergeCartType!){ mergeCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "secondCartId": "7777-7777-7777-7777"
            ///      }
            ///   }
            /// }
            /// </example>
            var margeCartField = new EventStreamFieldType
            {
                Name = "mergeCart",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputMergeCartType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<MergeCartCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(margeCartField);

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
            var removeCartField = new EventStreamFieldType
            {
                Name = "removeCart",
                Type = typeof(BooleanGraphType),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputRemoveCartType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, bool>(async context =>
                    (bool)await _mediator.Send(context.GetArgument(GenericTypeHelper.GetActualType<RemoveCartCommand>(), _commandName)))
            };

            schema.Mutation.AddField(removeCartField);

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
            var clearShipmentsField = new EventStreamFieldType
            {
                Name = "clearShipments",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputClearShipmentsType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ClearShipmentsCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(clearShipmentsField);

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
            var clearPaymentsField = new EventStreamFieldType
            {
                Name = "clearPayments",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputClearPaymentsType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<ClearPaymentsCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(clearPaymentsField);


            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddOrUpdateCartAddressType!){ addOrUpdateCartAddress(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "address": {
            ///             "line1":"st street 1"
            ///         }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartAddress = new EventStreamFieldType
            {
                Name = "addOrUpdateCartAddress",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddOrUpdateCartAddressType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                {
                    //TODO: Need to refactor later to prevent ugly code duplication
                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                    var cartAggregate = await _mediator.Send(context.GetCartCommand<AddOrUpdateCartAddressCommand>());
                    //store cart aggregate in the user context for future usage in the graph types resolvers    
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
            };

            schema.Mutation.AddField(addOrUpdateCartAddress);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputRemoveCartAddressType!){ removeCartAddress(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "cartId": "7777-7777-7777-7777",
            ///          "addressId": "111"
            ///      }
            ///   }
            /// }
            /// </example>
            var removeCartAddressField = new EventStreamFieldType
            {
                Name = "removeCartAddress",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputRemoveCartAddressType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                    await _mediator.Send(context.GetCartCommand<RemoveCartAddressCommand>()))
            };

            schema.Mutation.AddField(removeCartAddressField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// mutation ($command: InputAddItemsType!) { addItemsCart(command: $command) }
            /// "variables": {
            ///    "command": {
            ///         "storeId": "Electronics",
            ///         "cartName": "default",
            ///         "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///         "currencyCode": "USD",
            ///         "cultureName": "en-US",
            ///         "cartType": "cart",
            ///         "cartId": "",
            ///         "cartItems": [{
            ///             "productId": "1111-1111-1111-1111",
            ///             "quantity": 2
            ///         },
            ///         {
            ///             "productId": "2222-2222-2222-2222",
            ///             "quantity": 5
            ///         }]
            ///     }
            /// }
            /// </example>
            var addItemsCartField = new EventStreamFieldType
            {
                Name = "addItemsCart",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddItemsType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                    await _mediator.Send(context.GetCartCommand<AddCartItemsCommand>()))
            };

            schema.Mutation.AddField(addItemsCartField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": mutation ($command: InputAddOrUpdateCartAddressType!) { addCartAddress(command: $command) }
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "address": {
            ///             "line1":"st street 1"
            ///           }
            ///       }
            ///    }
            /// }
            /// </example>
            var addCartAddressField = new EventStreamFieldType
            {
                Name = "addCartAddress",
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputAddOrUpdateCartAddressType>>(_commandName),
                Resolver = new AsyncFieldResolver<CartAggregate, CartAggregate>(async context =>
                    await _mediator.Send(context.GetCartCommand<AddCartAddressCommand>()))
            };

            schema.Mutation.AddField(addCartAddressField);
        }

        private async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = context.GetCartQuery<SearchCartQuery>();
            query.Skip = skip;
            query.Take = first ?? context.PageSize ?? 10;
            query.Sort = context.GetArgument<string>("sort");
            query.Filter = context.GetArgument<string>("filter");
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths().ToArray();

            context.CopyArgumentsToUserContext();

            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), query, new CanAccessCartAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new ExecutionError($"Access denied");
            }

            var response = await mediator.Send(query);
            foreach (var cartAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(cartAggregate);
            }
            return new PagedConnection<CartAggregate>(response.Results, skip, Convert.ToInt32(context.After ?? 0.ToString()), response.TotalCount);
        }
    }
}
