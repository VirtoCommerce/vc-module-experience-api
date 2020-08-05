using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PurchaseSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        public const string _commandName = "command";

        public PurchaseSchema(IMediator mediator)
        {
            _mediator = mediator;
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

                    //store cart aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("cartAggregate", cartAggregate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(cartField);

            var wishListsField = new FieldType()
            {
                Name = "carts",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                    new QueryArgument<StringGraphType> { Name = "cultureName" },
                    new QueryArgument<StringGraphType> { Name = "type" },
                    new QueryArgument<StringGraphType> { Name = "sort" },
                    new QueryArgument<IntGraphType> { Name = "skip" },
                    new QueryArgument<IntGraphType> { Name = "take" }),
                    
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CartDescriptionType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var type = context.GetArgument<string>("type");
                    var storeId = context.GetArgument<string>("storeId");
                    var userId = context.GetArgument<string>("userId");
                    var cultureName = context.GetArgument<string>("cultureName");
                    var currencyCode = context.GetArgument<string>("currencyCode");
                    var sort = context.GetArgument<string>("sort");
                    var skip = context.GetArgument<int>("skip");
                    var take = context.GetArgument<int>("take");

                    var cartsQuery = new SearchCartDescriptionQuery(storeId, type, userId, currencyCode, cultureName, sort, skip, take);
                    var carts = await _mediator.Send(cartsQuery);

                    return carts.Results;
                })
            };
            schema.Query.AddField(wishListsField);

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
            var addItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                           .Name("addItem")
                                           .Argument<NonNullGraphType<InputAddItemType>>(_commandName)
                                           //TODO: Write the unit-tests for successfully mapping input variable to the command
                                           .ResolveAsync(async context =>
                                           {
                                               //TODO: Need to refactor later to prevent ugly code duplication
                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                               var cartAggregate = await _mediator.Send(context.GetCartCommand<AddCartItemCommand>());
                                               context.UserContext.Add("cartAggregate", cartAggregate);
                                               return cartAggregate;
                                           })
                                           .FieldType;

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
            var clearCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                             .Name("clearCart")
                                             .Argument<NonNullGraphType<InputClearCartType>>(_commandName)
                                             .ResolveAsync(async context =>
                                             {
                                                 //TODO: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(context.GetCartCommand<ClearCartCommand>());
                                                 context.UserContext.Add("cartAggregate", cartAggregate);
                                                 return cartAggregate;
                                             }).FieldType;

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
            var changeCommentField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                 .Name("changeComment")
                                                 .Argument<InputChangeCommentType>(_commandName)
                                                 .ResolveAsync(async context =>
                                                 {
                                                     //TODO: Need to refactor later to prevent ugly code duplication
                                                     //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                     var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCommentCommand>());
                                                     context.UserContext.Add("cartAggregate", cartAggregate);
                                                     return cartAggregate;
                                                 })
                                                 .FieldType;

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
            ///          "productId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "price": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemPriceField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                       .Name("changeCartItemPrice")
                                                       .Argument<NonNullGraphType<InputChangeCartItemPriceType>>(_commandName)
                                                       .ResolveAsync(async context =>
                                                       {
                                                           //TODO: Need to refactor later to prevent ugly code duplication
                                                           //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                           var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemPriceCommand>());
                                                           context.UserContext.Add("cartAggregate", cartAggregate);
                                                           return cartAggregate;
                                                       }).FieldType;

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
            var changeCartItemQuantityField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                          .Name("changeCartItemQuantity")
                                                          .Argument<NonNullGraphType<InputChangeCartItemQuantityType>>(_commandName)
                                                          .ResolveAsync(async context =>
                                                          {
                                                              //TODO: Need to refactor later to prevent ugly code duplication
                                                              //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                              var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemQuantityCommand>());
                                                              context.UserContext.Add("cartAggregate", cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

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
            var changeCartItemCommentField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                          .Name("changeCartItemComment")
                                                          .Argument<InputChangeCartItemCommentType>(_commandName)
                                                          .ResolveAsync(async context =>
                                                          {
                                                              //TODO: Need to refactor later to prevent ugly code duplication
                                                              //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                              var cartAggregate = await _mediator.Send(context.GetCartCommand<ChangeCartItemCommentCommand>());
                                                              context.UserContext.Add("cartAggregate", cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

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
            var removeCartItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                  .Name("removeCartItem")
                                                  .Argument<NonNullGraphType<InputRemoveItemType>>(_commandName)
                                                  .ResolveAsync(async context =>
                                                  {
                                                      //TODO: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveCartItemCommand>());
                                                      context.UserContext.Add("cartAggregate", cartAggregate);
                                                      return cartAggregate;
                                                  }).FieldType;

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
            var addCouponField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                             .Name("addCoupon")
                                             .Argument<NonNullGraphType<InputAddCouponType>>(_commandName)
                                             .ResolveAsync(async context =>
                                             {
                                                 //TODO: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(context.GetCartCommand<AddCouponCommand>());
                                                 context.UserContext.Add("cartAggregate", cartAggregate);
                                                 return cartAggregate;
                                             }).FieldType;

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
            var removeCouponField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                .Name("removeCoupon")
                                                .Argument<NonNullGraphType<InputRemoveCouponType>>(_commandName)
                                                .ResolveAsync(async context =>
                                                 {
                                                     //TODO: Need to refactor later to prevent ugly code duplication
                                                     //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                     var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveCouponCommand>());
                                                     context.UserContext.Add("cartAggregate", cartAggregate);
                                                     return cartAggregate;
                                                 }).FieldType;

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
            var removeShipmentField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                  .Name("removeShipment")
                                                  .Argument<NonNullGraphType<InputRemoveShipmentType>>(_commandName)
                                                  .ResolveAsync(async context =>
                                                  {
                                                      //TODO: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(context.GetCartCommand<RemoveShipmentCommand>());
                                                      context.UserContext.Add("cartAggregate", cartAggregate);
                                                      return cartAggregate;
                                                  }).FieldType;

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
            var addOrUpdateCartShipmentField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                           .Name("addOrUpdateCartShipment")
                                                           .Argument<NonNullGraphType<InputAddOrUpdateCartShipmentType>>(_commandName)
                                                           .ResolveAsync(async context =>
                                                           {
                                                               //TODO: Need to refactor later to prevent ugly code duplication
                                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                               var cartAggregate = await _mediator.Send(context.GetCartCommand<AddOrUpdateCartShipmentCommand>());
                                                               context.UserContext.Add("cartAggregate", cartAggregate);
                                                               return cartAggregate;
                                                           }).FieldType;

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
            var addOrUpdateCartPaymentField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                          .Name("addOrUpdateCartPayment")
                                                          .Argument<NonNullGraphType<InputAddOrUpdateCartPaymentType>>(_commandName)
                                                          .ResolveAsync(async context =>
                                                          {
                                                              //TODO: Need to refactor later to prevent ugly code duplication
                                                              //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                              var cartAggregate = await _mediator.Send(context.GetCartCommand<AddOrUpdateCartPaymentCommand>());
                                                              context.UserContext.Add("cartAggregate", cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

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
            var validateCouponField = FieldBuilder.Create<CartAggregate, bool>(typeof(BooleanGraphType))
                                                  .Name("validateCoupon")
                                                  .Argument<NonNullGraphType<InputValidateCouponType>>(_commandName)
                                                  .ResolveAsync(async context => await _mediator.Send(context.GetArgument<ValidateCouponCommand>(_commandName)))
                                                  .FieldType;

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
            var margeCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                             .Name("mergeCart")
                                             .Argument<NonNullGraphType<InputMergeCartType>>(_commandName)
                                             .ResolveAsync(async context =>
                                             {
                                                 //TODO: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(context.GetCartCommand<MergeCartCommand>());
                                                 context.UserContext.Add("cartAggregate", cartAggregate);
                                                 return cartAggregate;
                                             }).FieldType;

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
            var removeCartField = FieldBuilder.Create<CartAggregate, bool>(typeof(BooleanGraphType))
                                              .Name("removeCart")
                                              .Argument<NonNullGraphType<InputRemoveCartType>>(_commandName)
                                              .ResolveAsync(async context => await _mediator.Send(context.GetArgument<RemoveCartCommand>(_commandName)))
                                              .FieldType;

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
            var clearShipmentsField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                  .Name("clearShipments")
                                                  .Argument<NonNullGraphType<InputClearShipmentsType>>(_commandName)
                                                  .ResolveAsync(async context =>
                                                  {
                                                      //TODO: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(context.GetArgument<ClearShipmentsCommand>(_commandName));
                                                      context.UserContext.Add("cartAggregate", cartAggregate);
                                                      return cartAggregate;
                                                  })
                                                  .FieldType;

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
            var clearPaymentsField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                                 .Name("clearPayments")
                                                 .Argument<NonNullGraphType<InputClearPaymentsType>>(_commandName)
                                                 .ResolveAsync(async context =>
                                                 {
                                                     //TODO: Need to refactor later to prevent ugly code duplication
                                                     //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                     var cartAggregate = await _mediator.Send(context.GetArgument<ClearPaymentsCommand>(_commandName));
                                                     context.UserContext.Add("cartAggregate", cartAggregate);
                                                     return cartAggregate;
                                                 })
                                                 .FieldType;

            schema.Mutation.AddField(clearPaymentsField);
        }
    }
}
