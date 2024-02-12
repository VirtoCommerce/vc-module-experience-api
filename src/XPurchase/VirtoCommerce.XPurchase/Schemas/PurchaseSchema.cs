using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
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
        private readonly IShoppingCartService _cartService;
        private readonly IShoppingCartSearchService _shoppingCartSearchService;
        private readonly IDistributedLockService _distributedLockService;
        private readonly IUserManagerCore _userManagerCore;
        private readonly IMemberResolver _memberResolver;

        public const string _commandName = "command";
        public const string CartPrefix = "Cart";

        public PurchaseSchema(
            IMediator mediator,
            IAuthorizationService authorizationService,
            ICurrencyService currencyService,
            IShoppingCartService cartService,
            IShoppingCartSearchService shoppingCartSearchService,
            IDistributedLockService distributedLockService,
            IUserManagerCore userManagerCore,
            IMemberResolver memberResolver)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _cartService = cartService;
            _shoppingCartSearchService = shoppingCartSearchService;
            _distributedLockService = distributedLockService;
            _userManagerCore = userManagerCore;
            _memberResolver = memberResolver;
        }

        public void Build(ISchema schema)
        {
            ValueConverter.Register<ExpCartAddress, Optional<ExpCartAddress>>(x => new Optional<ExpCartAddress>(x));

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
            ///          "dynamicProperties": [
            ///             {
            ///                 "name": "ItemProperty",
            ///                 "value": "test value"
            ///            }]
            ///      }
            ///   }
            /// }
            /// </example>
            var addItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                           .Name("addItem")
                                           .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddItemType>>(), _commandName)
                                           //PT-5394: Write the unit-tests for successfully mapping input variable to the command
                                           .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                           {
                                               var cartCommand = context.GetCartCommand<AddCartItemCommand>();

                                               await CheckAuthByCartCommandAsync(context, cartCommand);

                                               //PT-5327: Need to refactor later to prevent ugly code duplication
                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                               var cartAggregate = await _mediator.Send(cartCommand);

                                               //store cart aggregate in the user context for future usage in the graph types resolvers
                                               context.SetExpandedObjectGraph(cartAggregate);
                                               return cartAggregate;
                                           })
                                           .FieldType;

            schema.Mutation.AddField(addItemField);

            schema.Mutation.AddField(FieldBuilder
                .Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                .Name("addGiftItems")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddGiftItemsType>>(), _commandName)
                .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                {
                    var cartCommand = context.GetCartCommand<AddGiftItemsCommand>();

                    await CheckAuthByCartCommandAsync(context, cartCommand);

                    var cartAggregate = await _mediator.Send(cartCommand);

                    //store cart aggregate in the user context for future usage in the graph types resolvers
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
                .FieldType
            );

            schema.Mutation.AddField(FieldBuilder
                .Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                .Name("rejectGiftItems")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRejectGiftItemsType>>(), _commandName)
                .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                {
                    var cartCommand = context.GetCartCommand<RejectGiftCartItemsCommand>();

                    await CheckAuthByCartCommandAsync(context, cartCommand);

                    var cartAggregate = await _mediator.Send(cartCommand);

                    //store cart aggregate in the user context for future usage in the graph types resolvers
                    context.SetExpandedObjectGraph(cartAggregate);
                    return cartAggregate;
                })
                .FieldType
            );

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
            var clearCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                             .Name("clearCart")
                                             .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputClearCartType>>(), _commandName)
                                             .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                             {
                                                 var cartCommand = context.GetCartCommand<ClearCartCommand>();

                                                 await CheckAuthByCartCommandAsync(context, cartCommand);

                                                 //PT-5327: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(cartCommand);

                                                 //store cart aggregate in the user context for future usage in the graph types resolvers
                                                 context.SetExpandedObjectGraph(cartAggregate);
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
            var changeCommentField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("changeComment")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeCommentType>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                 {
                                                     var cartCommand = context.GetCartCommand<ChangeCommentCommand>();

                                                     await CheckAuthByCartCommandAsync(context, cartCommand);

                                                     //PT-5327: Need to refactor later to prevent ugly code duplication
                                                     //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                     var cartAggregate = await _mediator.Send(cartCommand);

                                                     //store cart aggregate in the user context for future usage in the graph types resolvers
                                                     context.SetExpandedObjectGraph(cartAggregate);
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
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "price": 777
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemPriceField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                       .Name("changeCartItemPrice")
                                                       .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputChangeCartItemPriceType>>(), _commandName)
                                                       .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                       {
                                                           var cartCommand = context.GetCartCommand<ChangeCartItemPriceCommand>();

                                                           await CheckAuthByCartCommandAsync(context, cartCommand);

                                                           //PT-5327: Need to refactor later to prevent ugly code duplication
                                                           //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                           var cartAggregate = await _mediator.Send(cartCommand);

                                                           //store cart aggregate in the user context for future usage in the graph types resolvers
                                                           context.SetExpandedObjectGraph(cartAggregate);
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
            var changeCartItemQuantityField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                          .Name("changeCartItemQuantity")
                                                          .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputChangeCartItemQuantityType>>(), _commandName)
                                                          .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                          {
                                                              var cartCommand = context.GetCartCommand<ChangeCartItemQuantityCommand>();

                                                              await CheckAuthByCartCommandAsync(context, cartCommand);

                                                              //PT-5327: Need to refactor later to prevent ugly code duplication
                                                              //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                              var cartAggregate = await _mediator.Send(cartCommand);

                                                              //store cart aggregate in the user context for future usage in the graph types resolvers
                                                              context.SetExpandedObjectGraph(cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

            schema.Mutation.AddField(changeCartItemQuantityField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemCommentType!){ changeCartItemComment(command: $command) }",
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
            var changeCartItemCommentField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                          .Name("changeCartItemComment")
                                                          .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeCartItemCommentType>(), _commandName)
                                                          .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                          {
                                                              var cartCommand = context.GetCartCommand<ChangeCartItemCommentCommand>();

                                                              await CheckAuthByCartCommandAsync(context, cartCommand);

                                                              //PT-5327: Need to refactor later to prevent ugly code duplication
                                                              //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                              var cartAggregate = await _mediator.Send(cartCommand);

                                                              //store cart aggregate in the user context for future usage in the graph types resolvers
                                                              context.SetExpandedObjectGraph(cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

            schema.Mutation.AddField(changeCartItemCommentField);

            #region Change selected items

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputChangeCartItemSelectedType!){ changeCartItemSelected(command: $command) { id } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "lineItemId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "selectedForCheckout": false
            ///      }
            ///   }
            /// }
            /// </example>
            var changeCartItemSelectedField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                          .Name("changeCartItemSelected")
                                                          .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeCartItemSelectedType>(), _commandName)
                                                          .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                          {
                                                              var cartCommand = context.GetCartCommand<ChangeCartItemSelectedCommand>();

                                                              await CheckAuthByCartCommandAsync(context, cartCommand);

                                                              var cartAggregate = await _mediator.Send(cartCommand);

                                                              context.SetExpandedObjectGraph(cartAggregate);
                                                              return cartAggregate;
                                                          }).FieldType;

            schema.Mutation.AddField(changeCartItemSelectedField);

            var selectCartItems = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                              .Name("selectCartItems")
                                              .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeCartItemsSelectedType>(), _commandName)
                                              .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                              {
                                                  var cartCommand = context.GetCartCommand<ChangeCartItemsSelectedCommand>();
                                                  cartCommand.SelectedForCheckout = true;

                                                  await CheckAuthByCartCommandAsync(context, cartCommand);

                                                  var cartAggregate = await _mediator.Send(cartCommand);
                                                  context.SetExpandedObjectGraph(cartAggregate);

                                                  return cartAggregate;
                                              }).FieldType;

            schema.Mutation.AddField(selectCartItems);

            var unSelectCartItems = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                .Name("unSelectCartItems")
                                                .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeCartItemsSelectedType>(), _commandName)
                                                .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                {
                                                    var cartCommand = context.GetCartCommand<ChangeCartItemsSelectedCommand>();

                                                    await CheckAuthByCartCommandAsync(context, cartCommand);

                                                    var cartAggregate = await _mediator.Send(cartCommand);
                                                    context.SetExpandedObjectGraph(cartAggregate);

                                                    return cartAggregate;
                                                }).FieldType;

            schema.Mutation.AddField(unSelectCartItems);

            var selectAllCartItems = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("selectAllCartItems")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeAllCartItemsSelectedType>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                 {
                                                     var cartCommand = context.GetCartCommand<ChangeAllCartItemsSelectedCommand>();
                                                     cartCommand.SelectedForCheckout = true;

                                                     await CheckAuthByCartCommandAsync(context, cartCommand);

                                                     var cartAggregate = await _mediator.Send(cartCommand);
                                                     context.SetExpandedObjectGraph(cartAggregate);

                                                     return cartAggregate;
                                                 }).FieldType;

            schema.Mutation.AddField(selectAllCartItems);

            var unSelectAllCartItems = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                   .Name("unSelectAllCartItems")
                                                   .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangeAllCartItemsSelectedType>(), _commandName)
                                                   .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                   {
                                                       var cartCommand = context.GetCartCommand<ChangeAllCartItemsSelectedCommand>();

                                                       await CheckAuthByCartCommandAsync(context, cartCommand);

                                                       var cartAggregate = await _mediator.Send(cartCommand);
                                                       context.SetExpandedObjectGraph(cartAggregate);

                                                       return cartAggregate;
                                                   }).FieldType;

            schema.Mutation.AddField(unSelectAllCartItems);

            #endregion

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
            var removeCartItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                  .Name("removeCartItem")
                                                  .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveItemType>>(), _commandName)
                                                  .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<RemoveCartItemCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  }).FieldType;

            schema.Mutation.AddField(removeCartItemField);

            var removeCartItemsField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                  .Name("removeCartItems")
                                                  .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveItemsType>>(), _commandName)
                                                  .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<RemoveCartItemsCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  }).FieldType;

            schema.Mutation.AddField(removeCartItemsField);

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
            var addCouponField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                             .Name("addCoupon")
                                             .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddCouponType>>(), _commandName)
                                             .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                             {
                                                 var cartCommand = context.GetCartCommand<AddCouponCommand>();

                                                 await CheckAuthByCartCommandAsync(context, cartCommand);

                                                 //PT-5327: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(cartCommand);

                                                 //store cart aggregate in the user context for future usage in the graph types resolvers
                                                 context.SetExpandedObjectGraph(cartAggregate);
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
            var removeCouponField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                .Name("removeCoupon")
                                                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveCouponType>>(), _commandName)
                                                .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                {
                                                    var cartCommand = context.GetCartCommand<RemoveCouponCommand>();

                                                    await CheckAuthByCartCommandAsync(context, cartCommand);

                                                    //PT-5327: Need to refactor later to prevent ugly code duplication
                                                    //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                    var cartAggregate = await _mediator.Send(cartCommand);

                                                    //store cart aggregate in the user context for future usage in the graph types resolvers
                                                    context.SetExpandedObjectGraph(cartAggregate);
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
            var removeShipmentField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                  .Name("removeShipment")
                                                  .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveShipmentType>>(), _commandName)
                                                  .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<RemoveShipmentCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  }).FieldType;

            schema.Mutation.AddField(removeShipmentField);

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
            ///          "shipment": {
            ///             "id": "7557d2ee-8173-46bb-95eb-64f23e1316e3",
            ///             "fulfillmentCenterId": "tulsa-branch",
            ///             "height": 10,
            ///             "length": 10,
            ///             "measureUnit": "cm",
            ///             "shipmentMethodCode": "FixedRate",
            ///             "shipmentMethodOption": "Ground",
            //              "volumetricWeight": 10,
            ///             "weight": 50,
            ///             "weightUnit": "kg",
            ///             "width": 10,
            ///             "currency": "USD",
            ///             "price": 10,
            ///             "dynamicProperties": [
            ///                 {
            ///                     "name": "ShipmentProperty",
            ///                     "value": "test value"
            ///                 }],
            ///             "deliveryAddress": {
            ///                 "city": "CityName",
            ///                 "countryCode": "RU",
            ///                 "countryName": "Russia",
            ///                 "email": "Email@test",
            ///                 "firstName": "First test name",
            ///                 "key": "KeyTest",
            ///                 "lastName": "Last name test",
            ///                 "line1": "Address Line 1",
            ///                 "line2": "Address line 2",
            ///                 "middleName": "Test Middle Name",
            ///                 "name": "First name address",
            ///                 "organization": "OrganizationTestName",
            ///                 "phone": "88005553535",
            ///                 "postalCode": "12345",
            ///                 "regionId": "RegId",
            ///                 "regionName": "Region Name",
            ///                 "zip": "12345",
            ///                 "addressType": 2
            ///           }
            ///        }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartShipmentField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                           .Name("addOrUpdateCartShipment")
                                                           .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddOrUpdateCartShipmentType>>(), _commandName)
                                                           .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                           {
                                                               var cartCommand = context.GetCartCommand<AddOrUpdateCartShipmentCommand>();

                                                               await CheckAuthByCartCommandAsync(context, cartCommand);

                                                               //PT-5327: Need to refactor later to prevent ugly code duplication
                                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                               var cartAggregate = await _mediator.Send(cartCommand);

                                                               //store cart aggregate in the user context for future usage in the graph types resolvers
                                                               context.SetExpandedObjectGraph(cartAggregate);
                                                               return cartAggregate;
                                                           }).FieldType;

            schema.Mutation.AddField(addOrUpdateCartShipmentField);

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
            ///          "payment": {
            ///             "id": "7557d2ee-8173-46bb-95eb-64f23e1316e3",
            ///             "outerId": "ID1",
            ///             "paymentGatewayCode": "ManualTestPaymentMethod",
            ///             "dynamicProperties": [
            ///                 {
            ///                     "name": "PaymentProperty",
            ///                     "value": "test value"
            ///                 }],
            ///             "billingAddress": {
            ///                 "city": "CityName",
            ///                 "countryCode": "RU",
            ///                 "countryName": "Russia",
            ///                 "email": "Email@test",
            ///                 "firstName": "First test name",
            ///                 "key": "KeyTest",
            ///                 "lastName": "Last name test",
            ///                 "line1": "Address Line 1",
            ///                 "line2": "Address line 2",
            ///                 "middleName": "Test Middle Name",
            ///                 "name": "First name address",
            ///                 "organization": "OrganizationTestName",
            ///                 "phone": "88005553535",
            ///                 "postalCode": "12345",
            ///                 "regionId": "RegId",
            ///                 "regionName": "Region Name",
            ///                 "zip": "12345",
            ///                 "addressType": 1
            ///             },
            ///             "currency": "USD",
            ///             "price": "1001",
            ///             "amount": "1001"
            ///          }
            ///      }
            ///   }
            /// }
            /// </example>
            var addOrUpdateCartPaymentField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                          .Name("addOrUpdateCartPayment")
                                                          .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddOrUpdateCartPaymentType>>(), _commandName)
                                                           .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                           {
                                                               var cartCommand = context.GetCartCommand<AddOrUpdateCartPaymentCommand>();

                                                               await CheckAuthByCartCommandAsync(context, cartCommand);

                                                               //PT-5327: Need to refactor later to prevent ugly code duplication
                                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                               var cartAggregate = await _mediator.Send(cartCommand);

                                                               //store cart aggregate in the user context for future usage in the graph types resolvers
                                                               context.SetExpandedObjectGraph(cartAggregate);
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
            var validateCouponMutationField = FieldBuilder.Create<CartAggregate, bool>(typeof(BooleanGraphType))
                                                  .Name("validateCoupon")
                                                  .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputValidateCouponType>>(), _commandName)
                                                  .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var command = context.GetArgument<ValidateCouponCommand>(_commandName);

                                                      await CheckAuthByCartParamsAsync(context, command.StoreId, command.CartType, command.CartName, command.UserId, command.CurrencyCode, command.CultureName);

                                                      return await _mediator.Send(command);
                                                  })
                                                  .DeprecationReason("Use 'validateCoupon' query instead.")
                                                  .FieldType;

            schema.Mutation.AddField(validateCouponMutationField);

            var validateCouponQueryField = new FieldType
            {
                Name = "validateCoupon",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "cartId", Description = "Cart Id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId", Description = "Store Id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId", Description = "User Id" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-Us\")" },
                    new QueryArgument<StringGraphType> { Name = "cartName", Description = "Cart name" },
                    new QueryArgument<StringGraphType> { Name = "cartType", Description = "Cart type" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "coupon", Description = "Cart promo coupon code" }),
                Type = typeof(BooleanGraphType),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = context.GetCartQuery<ValidateCouponQuery>();
                    query.CartId = context.GetArgumentOrValue<string>("cartId");
                    query.Coupon = context.GetArgumentOrValue<string>("coupon");

                    await CheckAuthByCartParamsAsync(context, query.StoreId, query.CartType, query.CartName, query.UserId, query.CurrencyCode, query.CultureName);

                    return await _mediator.Send(query);
                })
            };
            schema.Query.AddField(validateCouponQueryField);

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
            var margeCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                             .Name("mergeCart")
                                             .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputMergeCartType>>(), _commandName)
                                             .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                             {
                                                 var cartCommand = context.GetCartCommand<MergeCartCommand>();

                                                 await CheckAuthByCartCommandAsync(context, cartCommand);

                                                 //PT-5327: Need to refactor later to prevent ugly code duplication
                                                 //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                 var cartAggregate = await _mediator.Send(cartCommand);

                                                 //store cart aggregate in the user context for future usage in the graph types resolvers
                                                 context.SetExpandedObjectGraph(cartAggregate);
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
                                              .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveCartType>>(), _commandName)
                                              .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                              {
                                                  var command = context.GetArgument(GenericTypeHelper.GetActualType<RemoveCartCommand>(), _commandName) as RemoveCartCommand;

                                                  await CheckAuthAsyncByCartId(context, command.CartId);

                                                  return await _mediator.Send(command);
                                              })
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
            var clearShipmentsField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                  .Name("clearShipments")
                                                  .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputClearShipmentsType>>(), _commandName)
                                                  .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<ClearShipmentsCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
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
            var clearPaymentsField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("clearPayments")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputClearPaymentsType>>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<ClearPaymentsCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  })
                                                 .FieldType;

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
            var addOrUpdateCartAddress = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("addOrUpdateCartAddress")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddOrUpdateCartAddressType>>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                  {
                                                      var cartCommand = context.GetCartCommand<AddOrUpdateCartAddressCommand>();

                                                      await CheckAuthByCartCommandAsync(context, cartCommand);

                                                      //PT-5327: Need to refactor later to prevent ugly code duplication
                                                      //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                                      var cartAggregate = await _mediator.Send(cartCommand);

                                                      //store cart aggregate in the user context for future usage in the graph types resolvers
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  })
                                                 .FieldType;

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
            var removeCartAddressField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                              .Name("removeCartAddress")
                                              .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveCartAddressType>>(), _commandName)
                                              .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                              {
                                                  var cartCommand = context.GetCartCommand<RemoveCartAddressCommand>();

                                                  await CheckAuthByCartCommandAsync(context, cartCommand);

                                                  var cartAggregate = await _mediator.Send(cartCommand);
                                                  context.SetExpandedObjectGraph(cartAggregate);
                                                  return cartAggregate;
                                              })
                                              .FieldType;

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
            ///             "quantity": 2,
            ///             "dynamicProperties": [
            ///                 {
            ///                     "name": "ItemProperty",
            ///                     "value": "test value"
            ///                 }]
            ///         },
            ///         {
            ///             "productId": "2222-2222-2222-2222",
            ///             "quantity": 5
            ///         }]
            ///     }
            /// }
            /// </example>
            var addItemsCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("addItemsCart")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddItemsType>>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                 {
                                                     var cartCommand = context.GetCartCommand<AddCartItemsCommand>();

                                                     await CheckAuthByCartCommandAsync(context, cartCommand);

                                                     var cartAggregate = await _mediator.Send(cartCommand);

                                                     context.SetExpandedObjectGraph(cartAggregate);

                                                     return cartAggregate;
                                                 })
                                                 .FieldType;

            schema.Mutation.AddField(addItemsCartField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// mutation ($command: InputAddItemsBulkType!) { addItemsCart(command: $command) }
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
            ///             "productSku": "1111-1111-1111-1111",
            ///             "quantity": 2
            ///         },
            ///         {
            ///             "productSku": "2222-2222-2222-2222",
            ///             "quantity": 5
            ///         }]
            ///     }
            /// }
            /// </example>
            var addItemsBulkCartField = FieldBuilder.Create<BulkCartResult, BulkCartResult>(GraphTypeExtenstionHelper.GetActualType<BulkCartType>())
                                                 .Name("addBulkItemsCart")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddBulkItemsType>>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                 {
                                                     var command = context.GetArgument<AddCartItemsBulkCommand>(_commandName);

                                                     if (!string.IsNullOrEmpty(command.CartId))
                                                     {
                                                         await CheckAuthAsyncByCartId(context, command.CartId);
                                                     }
                                                     else
                                                     {
                                                         await CheckAuthByCartParamsAsync(context, command.StoreId, command.CartType, command.CartName, command.UserId, command.CurrencyCode, command.CultureName);
                                                     }

                                                     var result = await _mediator.Send(command);

                                                     context.SetExpandedObjectGraph(result.Cart);

                                                     return result;
                                                 })
                                                 .FieldType;

            schema.Mutation.AddField(addItemsBulkCartField);

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
            var addCartAddressField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                 .Name("addCartAddress")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddOrUpdateCartAddressType>>(), _commandName)
                                                 .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                 {
                                                     var cartCommand = context.GetCartCommand<AddCartAddressCommand>();

                                                     await CheckAuthByCartCommandAsync(context, cartCommand);

                                                     var cartAggregate = await _mediator.Send(cartCommand);
                                                     context.SetExpandedObjectGraph(cartAggregate);
                                                     return cartAggregate;
                                                 })
                                                 .FieldType;

            schema.Mutation.AddField(addCartAddressField);

            var updateCartDynamicPropertiesField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                     .Name("updateCartDynamicProperties")
                                                     .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateCartDynamicPropertiesType>>(), _commandName)
                                                     .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                     {
                                                         var cartCommand = context.GetCartCommand<UpdateCartDynamicPropertiesCommand>();

                                                         await CheckAuthByCartCommandAsync(context, cartCommand);

                                                         var cartAggregate = await _mediator.Send(cartCommand);
                                                         context.SetExpandedObjectGraph(cartAggregate);
                                                         return cartAggregate;
                                                     })
                                                     .FieldType;

            schema.Mutation.AddField(updateCartDynamicPropertiesField);

            var updateCartItemDynamicPropertiesField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                        .Name("updateCartItemDynamicProperties")
                                                        .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateCartItemDynamicPropertiesType>>(), _commandName)
                                                        .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                        {
                                                            var cartCommand = context.GetCartCommand<UpdateCartItemDynamicPropertiesCommand>();

                                                            await CheckAuthByCartCommandAsync(context, cartCommand);

                                                            var cartAggregate = await _mediator.Send(cartCommand);
                                                            context.SetExpandedObjectGraph(cartAggregate);
                                                            return cartAggregate;
                                                        })
                                                        .FieldType;

            schema.Mutation.AddField(updateCartItemDynamicPropertiesField);

            var updateCartPaymentDynamicPropertiesField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                            .Name("updateCartPaymentDynamicProperties")
                                                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateCartPaymentDynamicPropertiesType>>(), _commandName)
                                                            .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                            {
                                                                var cartCommand = context.GetCartCommand<UpdateCartPaymentDynamicPropertiesCommand>();

                                                                await CheckAuthByCartCommandAsync(context, cartCommand);

                                                                var cartAggregate = await _mediator.Send(cartCommand);
                                                                context.SetExpandedObjectGraph(cartAggregate);
                                                                return cartAggregate;
                                                            })
                                                            .FieldType;

            schema.Mutation.AddField(updateCartPaymentDynamicPropertiesField);

            var updateCartShipmentDynamicPropertiesField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                                               .Name("updateCartShipmentDynamicProperties")
                                                               .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateCartShipmentDynamicPropertiesType>>(), _commandName)
                                                               .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                                               {
                                                                   var cartCommand = context.GetCartCommand<UpdateCartShipmentDynamicPropertiesCommand>();

                                                                   await CheckAuthByCartCommandAsync(context, cartCommand);

                                                                   var cartAggregate = await _mediator.Send(cartCommand);
                                                                   context.SetExpandedObjectGraph(cartAggregate);
                                                                   return cartAggregate;
                                                               })
                                                               .FieldType;

            schema.Mutation.AddField(updateCartShipmentDynamicPropertiesField);

            var changePurchaseOrderNumberField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                     .Name("changePurchaseOrderNumber")
                                     .Argument(GraphTypeExtenstionHelper.GetActualType<InputChangePurchaseOrderNumber>(), _commandName)
                                     .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                     {
                                         var cartCommand = context.GetCartCommand<ChangePurchaseOrderNumberCommand>();

                                         await CheckAuthByCartCommandAsync(context, cartCommand);

                                         var cartAggregate = await _mediator.Send(cartCommand);
                                         context.SetExpandedObjectGraph(cartAggregate);
                                         return cartAggregate;
                                     })
                                     .FieldType;

            schema.Mutation.AddField(changePurchaseOrderNumberField);

            //Mutations
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation ($command:InputAddItemType!){ refreshCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "cart",
            ///          "cartId": "",
            ///      }
            ///   }
            /// }
            /// </example>
            var refreshCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<CartType>())
                                           .Name("refreshCart")
                                           .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<RefreshCartType>>(), _commandName)
                                           //PT-5394: Write the unit-tests for successfully mapping input variable to the command
                                           .ResolveSynchronizedAsync(CartPrefix, "userId", _distributedLockService, async context =>
                                           {
                                               var cartCommand = context.GetCartCommand<RefreshCartCommand>();

                                               await CheckAuthByCartCommandAsync(context, cartCommand);

                                               //PT-5327: Need to refactor later to prevent ugly code duplication
                                               //We need to add cartAggregate to the context to be able use it on nested cart types resolvers (e.g for currency)
                                               var cartAggregate = await _mediator.Send(cartCommand);

                                               //store cart aggregate in the user context for future usage in the graph types resolvers
                                               context.SetExpandedObjectGraph(cartAggregate);
                                               return cartAggregate;
                                           })
                                           .FieldType;

            schema.Mutation.AddField(refreshCartField);


            #region Wishlists

            // Queries

            var listField = new FieldType
            {
                Name = "wishlist",
                Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "listId", Description = "List Id" },
                        new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-Us\")" }),
                Type = GraphTypeExtenstionHelper.GetActualType<WishlistType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var getListQuery = AbstractTypeFactory<GetWishlistQuery>.TryCreateInstance();
                    getListQuery.ListId = context.GetArgument<string>("listId");
                    context.CopyArgumentsToUserContext();

                    var cartAggregate = await _mediator.Send(getListQuery);

                    if (cartAggregate == null)
                    {
                        return null;
                    }

                    context.UserContext["storeId"] = cartAggregate.Cart.StoreId;

                    var wishlistUserContext = await InitializeWishlistUserContext(context, cart: cartAggregate.Cart);
                    await AuthorizeAsync(context, wishlistUserContext);

                    context.SetExpandedObjectGraph(cartAggregate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(listField);

            var listConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<WishlistType, object>()
                .Name("wishlists")
                .Argument<StringGraphType>("storeId", "Store Id")
                .Argument<StringGraphType>("userId", "User Id")
                .Argument<StringGraphType>("currencyCode", "Currency Code")
                .Argument<StringGraphType>("cultureName", "Culture Name")
                .Argument<StringGraphType>("scope", "List scope")
                .Argument<StringGraphType>("sort", "The sort expression")
                .PageSize(20);

            listConnectionBuilder.ResolveAsync(async context => await ResolveListConnectionAsync(_mediator, context));
            schema.Query.AddField(listConnectionBuilder.FieldType);

            // Mutations
            // Add list
            var addListField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                                                 .Name("createWishlist")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateWishlistType>>(), _commandName)
                                                  .ResolveAsync(async context =>
                                                  {
                                                      var commandType = GenericTypeHelper.GetActualType<CreateWishlistCommand>();
                                                      var command = (CreateWishlistCommand)context.GetArgument(commandType, _commandName);
                                                      var wishlistUserContext =
                                                          InitializeWishlistUserContext(context, userId: command.UserId);
                                                      await AuthorizeAsync(context, wishlistUserContext);
                                                      var cartAggregate = await _mediator.Send(command);
                                                      context.SetExpandedObjectGraph(cartAggregate);
                                                      return cartAggregate;
                                                  })
                                                 .FieldType;

            schema.Mutation.AddField(addListField);

            // Change list
            var changeListField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                                                 .Name("changeWishlist")
                                                 .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputChangeWishlistType>>(), _commandName)
                                                 .ResolveAsync(async context =>
                                                 {
                                                     var commandType = GenericTypeHelper.GetActualType<ChangeWishlistCommand>();
                                                     var command = (ChangeWishlistCommand)context.GetArgument(commandType, _commandName);

                                                     var wishlistUserContext = await AuthorizeByListIdAsync(context, command.ListId);
                                                     command.WishlistUserContext = wishlistUserContext;

                                                     var cartAggregate = await _mediator.Send(command);
                                                     context.SetExpandedObjectGraph(cartAggregate);
                                                     return cartAggregate;
                                                 })
                                                .FieldType;

            schema.Mutation.AddField(changeListField);

            // Rename list
            var renameListField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                                     .Name("renameWishlist")
                                     .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRenameWishlistType>>(), _commandName)
                                     .ResolveAsync(async context =>
                                     {
                                         var commandType = GenericTypeHelper.GetActualType<RenameWishlistCommand>();
                                         var command = (RenameWishlistCommand)context.GetArgument(commandType, _commandName);
                                         await AuthorizeByListIdAsync(context, command.ListId);
                                         var cartAggregate = await _mediator.Send(command);
                                         context.SetExpandedObjectGraph(cartAggregate);
                                         return cartAggregate;
                                     })
                                     .DeprecationReason("Obsolete. Use 'changeWishlist' instead.")
                                     .FieldType;

            schema.Mutation.AddField(renameListField);

            // Remove list
            var removeListField = FieldBuilder.Create<CartAggregate, bool>(typeof(BooleanGraphType))
                         .Name("removeWishlist")
                         .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveWishlistType>>(), _commandName)
                         .ResolveAsync(async context =>
                         {
                             var commandType = GenericTypeHelper.GetActualType<RemoveWishlistCommand>();
                             var command = (RemoveWishlistCommand)context.GetArgument(commandType, _commandName);
                             await AuthorizeByListIdAsync(context, command.ListId);
                             return await _mediator.Send(command);
                         })
                         .FieldType;

            schema.Mutation.AddField(removeListField);

            // Add product to list
            var addListItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                         .Name("addWishlistItem")
                         .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddWishlistItemType>>(), _commandName)
                         .ResolveAsync(async context =>
                         {
                             var commandType = GenericTypeHelper.GetActualType<AddWishlistItemCommand>();
                             var command = (AddWishlistItemCommand)context.GetArgument(commandType, _commandName);
                             var cartAggregate = await _mediator.Send(command);
                             context.UserContext["storeId"] = cartAggregate.Cart.StoreId;
                             await AuthorizeByListIdAsync(context, command.ListId);
                             context.SetExpandedObjectGraph(cartAggregate);
                             return cartAggregate;
                         })
                         .FieldType;

            schema.Mutation.AddField(addListItemField);

            // Update wishlist item
            var updateListItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                         .Name("updateWishListItems")
                         .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateWishlistItemsType>>(), _commandName)
                         .ResolveAsync(async context =>
                         {
                             var commandType = GenericTypeHelper.GetActualType<UpdateWishlistItemsCommand>();
                             var command = (UpdateWishlistItemsCommand)context.GetArgument(commandType, _commandName);
                             var cartAggregate = await _mediator.Send(command);
                             context.UserContext["storeId"] = cartAggregate.Cart.StoreId;
                             await AuthorizeByListIdAsync(context, command.ListId);
                             context.SetExpandedObjectGraph(cartAggregate);
                             return cartAggregate;
                         })
                         .FieldType;

            schema.Mutation.AddField(updateListItemField);

            // Add product to wishlists
            var addWishlistBulkItemField = FieldBuilder.Create<BulkCartAggregateResult, BulkCartAggregateResult>(GraphTypeExtenstionHelper.GetActualType<BulkWishlistType>())
                .Name("addWishlistBulkItem")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddWishlistBulkItemType>>(), _commandName)
                .ResolveAsync(async context =>
                {
                    var commandType = GenericTypeHelper.GetActualType<AddWishlistBulkItemCommand>();
                    var command = (AddWishlistBulkItemCommand)context.GetArgument(commandType, _commandName);
                    await AuthorizeByListIdsAsync(context, command.ListIds.ToList());
                    var cartAggregateList = await _mediator.Send(command);

                    foreach (var cartAggregate in cartAggregateList.CartAggregates)
                    {
                        context.SetExpandedObjectGraph(cartAggregate);
                    }
                    return cartAggregateList;
                })
                .FieldType;

            schema.Mutation.AddField(addWishlistBulkItemField);

            // Add products to wishlist
            var addWishlistItemsField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                                     .Name("addWishlistItems")
                                     .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddWishlistItemsType>>(), _commandName)
                                     .ResolveAsync(async context =>
                                     {
                                         var command = context.GetArgument<AddWishlistItemsCommand>(_commandName);
                                         await AuthorizeByListIdAsync(context, command.ListId);
                                         var result = await _mediator.Send(command);
                                         context.SetExpandedObjectGraph(result.Cart);
                                         return result;
                                     })
                                     .FieldType;

            schema.Mutation.AddField(addWishlistItemsField);

            // Remove product from list
            var removeListItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                         .Name("removeWishlistItem")
                         .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveWishlistItemType>>(), _commandName)
                         .ResolveAsync(async context =>
                         {
                             var commandType = GenericTypeHelper.GetActualType<RemoveWishlistItemCommand>();
                             var command = (RemoveWishlistItemCommand)context.GetArgument(commandType, _commandName);
                             await AuthorizeByListIdAsync(context, command.ListId);
                             var cartAggregate = await _mediator.Send(command);
                             context.SetExpandedObjectGraph(cartAggregate);
                             return cartAggregate;
                         })
                         .FieldType;

            schema.Mutation.AddField(removeListItemField);

            // Remove products from list
            var removeListItemsField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                         .Name("removeWishlistItems")
                         .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRemoveWishlistItemsType>>(), _commandName)
                         .ResolveAsync(async context =>
                         {
                             var commandType = GenericTypeHelper.GetActualType<RemoveWishlistItemsCommand>();
                             var command = (RemoveWishlistItemsCommand)context.GetArgument(commandType, _commandName);
                             await AuthorizeByListIdAsync(context, command.ListId);
                             var cartAggregate = await _mediator.Send(command);
                             context.SetExpandedObjectGraph(cartAggregate);
                             return cartAggregate;
                         })
                         .FieldType;

            schema.Mutation.AddField(removeListItemsField);

            // Move product to another list
            var moveListItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(GraphTypeExtenstionHelper.GetActualType<WishlistType>())
                     .Name("moveWishlistItem")
                     .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputMoveWishlistItemType>>(), _commandName)
                     .ResolveAsync(async context =>
                     {
                         var commandType = GenericTypeHelper.GetActualType<MoveWishlistItemCommand>();
                         var command = (MoveWishlistItemCommand)context.GetArgument(commandType, _commandName);
                         await AuthorizeByListIdsAsync(context, new List<string> { command.ListId, command.DestinationListId });
                         var cartAggregate = await _mediator.Send(command);
                         context.SetExpandedObjectGraph(cartAggregate);
                         return cartAggregate;
                     })
                     .FieldType;

            schema.Mutation.AddField(moveListItemField);

            #endregion Wishlists
        }

        private async Task<object> ResolveListConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = context.GetCartQuery<SearchWishlistQuery>();
            query.Scope = context.GetArgument<string>("scope");
            query.Skip = skip;
            query.Take = first ?? context.PageSize ?? 10;
            query.Sort = context.GetArgument<string>("sort");

            context.CopyArgumentsToUserContext();

            await AuthorizeAsync(context, query);

            var response = await mediator.Send(query);
            foreach (var cartAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(cartAggregate);
            }

            return new PagedConnection<CartAggregate>(response.Results, query.Skip, query.Take, response.TotalCount);
        }

        private async Task CheckAuthByCartCommandAsync(IResolveFieldContext context, CartCommand cartCommand)
        {
            if (!string.IsNullOrEmpty(cartCommand.CartId))
            {
                await CheckAuthAsyncByCartId(context, cartCommand.CartId);
            }
            else
            {
                await CheckAuthByCartParamsAsync(context, cartCommand.StoreId, cartCommand.CartType, cartCommand.CartName, cartCommand.UserId, cartCommand.CurrencyCode, cartCommand.CultureName);
            }
        }

        private async Task CheckAuthByCartParamsAsync(IResolveFieldContext context, string storeId, string cartType, string cartName, string userId, string currencyCode, string cultureName)
        {
            var criteria = new ShoppingCartSearchCriteria
            {
                StoreId = storeId,
                CustomerId = userId ?? AnonymousUser.UserName,
                Name = cartName,
                Currency = currencyCode,
                Type = cartType,
                LanguageCode = cultureName,
            };

            var cartSearchResult = await _shoppingCartSearchService.SearchAsync(criteria);
            var cart = cartSearchResult.Results.FirstOrDefault(x => (cartType != null) || x.Type == null);

            if (cart == null)
            {
                await AuthorizeAsync(context, userId);
            }
            else
            {
                await AuthorizeAsync(context, cart);
            }
        }

        private async Task CheckAuthAsyncByCartId(IResolveFieldContext context, string cartId)
        {
            var cart = await _cartService.GetByIdAsync(cartId, CartResponseGroup.Default.ToString());

            if (cart == null)
            {
                return;
            }

            await AuthorizeAsync(context, cart);
        }

        private async Task AuthorizeByListIdsAsync(IResolveFieldContext context, List<string> listIds)
        {
            var currentUserId = context.GetCurrentUserId();
            var contact = await _memberResolver.ResolveMemberByIdAsync(currentUserId) as Contact;
            var carts = await _cartService.GetAsync(listIds);

            foreach (var cart in carts)
            {
                var wishlistUserContext = new WishlistUserContext
                {
                    CurrentUserId = currentUserId,
                    CurrentContact = contact,
                    Cart = cart,
                };

                await AuthorizeAsync(context, wishlistUserContext);
            }
        }

        private async Task<WishlistUserContext> AuthorizeByListIdAsync(IResolveFieldContext context, string listId)
        {
            var wishlistUserContext = await InitializeWishlistUserContext(context, listId);
            await AuthorizeAsync(context, wishlistUserContext);
            return wishlistUserContext;
        }

        private async Task<WishlistUserContext> InitializeWishlistUserContext(IResolveFieldContext context, string listId = null, ShoppingCart cart = null, string userId = null)
        {
            var currentUserId = string.IsNullOrEmpty(userId)
                ? context.GetCurrentUserId()
                : userId;

            cart ??= await _cartService.GetByIdAsync(listId);
            var scope = context.GetArgument<string>("scope");

            var wishlistUserContext = new WishlistUserContext
            {
                CurrentUserId = currentUserId,
                CurrentContact = await _memberResolver.ResolveMemberByIdAsync(currentUserId) as Contact,
                Cart = cart,
                Scope = scope,
            };

            return wishlistUserContext;
        }

        private async Task AuthorizeAsync(IResolveFieldContext context, object resource)
        {
            await _userManagerCore.CheckUserState(context.GetCurrentUserId(), allowAnonymous: true);
            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), resource, new CanAccessCartAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw context.IsAuthenticated()
                    ? AuthorizationError.Forbidden()
                    : AuthorizationError.AnonymousAccessDenied();
            }
        }
    }
}
