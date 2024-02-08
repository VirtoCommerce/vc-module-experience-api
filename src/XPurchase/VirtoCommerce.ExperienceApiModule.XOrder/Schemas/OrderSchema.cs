using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Queries;
using XOrderSettings = VirtoCommerce.ExperienceApiModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        public readonly IMediator _mediator;
        private readonly ICurrencyService _currencyService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IUserManagerCore _userManagerCore;

        public OrderSchema(
            IMediator mediator,
            ICurrencyService currencyService,
            IAuthorizationService authorizationService,
            ICustomerOrderService customerOrderService,
            IUserManagerCore userManagerCore)
        {
            _mediator = mediator;
            _currencyService = currencyService;
            _authorizationService = authorizationService;
            _customerOrderService = customerOrderService;
            _userManagerCore = userManagerCore;
        }

        public void Build(ISchema schema)
        {
            ValueConverter.Register<ExpOrderAddress, Optional<ExpOrderAddress>>(x => new Optional<ExpOrderAddress>(x));

            _ = schema.Query.AddField(new FieldType
            {
                Name = "order",
                Arguments = AbstractTypeFactory<OrderQueryArguments>.TryCreateInstance(),
                Type = GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var request = context.ExtractQuery<GetOrderQuery>();

                    context.CopyArgumentsToUserContext();
                    var orderAggregate = await _mediator.Send(request);

                    if (orderAggregate == null)
                    {
                        return null;
                    }

                    await AuthorizeAsync(context, orderAggregate.Order, allowAnonymous: false);

                    var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
                    //Store all currencies in the user context for future resolve in the schema types
                    context.SetCurrencies(allCurrencies, request.CultureName);

                    //store order aggregate in the user context for future usage in the graph types resolvers
                    context.SetExpandedObjectGraph(orderAggregate);

                    return orderAggregate;
                })
            });

            var paymentsConnectionBuilder = GraphTypeExtenstionHelper
                .CreateConnection<PaymentInType, object>()
                .Name("payments")
                .PageSize(20)
                .OrderArguments();

            paymentsConnectionBuilder.ResolveAsync(async context => await ResolvePaymentsConnectionAsync(_mediator, context));
            schema.Query.AddField(paymentsConnectionBuilder.FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("createOrderFromCart")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateOrderFromCartType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<CreateOrderFromCartCommand>();
                                var command = context.GetArgument(type, _commandName) as CreateOrderFromCartCommand;

                                // check anonymous access to order create
                                var cartAggregate = await GetCartAggregateAsync(command.CartId);
                                var createAnonymousOrderEnabled = cartAggregate.Store.Settings.GetValue<bool>(XOrderSettings.CreateAnonymousOrder);
                                await AuthorizeAsync(context, cartAggregate.Cart, allowAnonymous: createAnonymousOrderEnabled);

                                var response = (CustomerOrderAggregate)await _mediator.Send(context.GetArgument(type, _commandName));
                                context.SetExpandedObjectGraph(response);
                                return response;
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                            .Name("changeOrderStatus")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputChangeOrderStatusType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<ChangeOrderStatusCommand>();
                                var command = (ChangeOrderStatusCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, ProcessPaymentRequestResult>(typeof(ProcessPaymentRequestResultType))
                            .Name("processOrderPayment")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputProcessOrderPaymentType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<ProcessOrderPaymentCommand>();
                                var command = (ProcessOrderPaymentCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .DeprecationReason("Obsolete. Use 'initializePayment' mutation")
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, InitializePaymentResult>(typeof(InitializePaymentResultType))
                            .Name("initializePayment")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputInitializePaymentType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<InitializePaymentCommand>();

                                var command = (InitializePaymentCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, AuthorizePaymentResult>(typeof(AuthorizePaymentResultType))
                            .Name("authorizePayment")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAuthorizePaymentType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<AuthorizePaymentCommand>();

                                var command = (AuthorizePaymentCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);


            _ = schema.Mutation.AddField(FieldBuilder.Create<CustomerOrderAggregate, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("updateOrderDynamicProperties")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateOrderDynamicPropertiesType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateOrderDynamicPropertiesCommand>();
                                var command = (UpdateOrderDynamicPropertiesCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<CustomerOrderAggregate, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("updateOrderItemDynamicProperties")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateOrderItemDynamicPropertiesType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateOrderItemDynamicPropertiesCommand>();
                                var command = (UpdateOrderItemDynamicPropertiesCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<CustomerOrderAggregate, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("updateOrderPaymentDynamicProperties")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateOrderPaymentDynamicPropertiesType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateOrderPaymentDynamicPropertiesCommand>();
                                var command = (UpdateOrderPaymentDynamicPropertiesCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<CustomerOrderAggregate, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("updateOrderShipmentDynamicProperties")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateOrderShipmentDynamicPropertiesType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateOrderShipmentDynamicPropertiesCommand>();
                                var command = (UpdateOrderShipmentDynamicPropertiesCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<CustomerOrderAggregate, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("addOrUpdateOrderPayment")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputAddOrUpdateOrderPaymentType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<AddOrUpdateOrderPaymentCommand>();
                                var command = (AddOrUpdateOrderPaymentCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context, command.OrderId);

                                var response = await _mediator.Send(command);

                                context.SetExpandedObjectGraph(response);

                                return response;
                            })
                            .FieldType);
        }

        private async Task<object> ResolvePaymentsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var query = context.ExtractQuery<SearchPaymentsQuery>();
            await AuthorizeAsync(context, query, allowAnonymous: true);

            context.UserContext.Add(nameof(Currency.CultureName).ToCamelCase(), query.CultureName);

            var response = await mediator.Send(query);

            foreach (var payment in response.Results)
            {
                context.SetExpandedObjectGraph(payment);
            }

            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            context.SetCurrencies(allCurrencies, query.CultureName);

            return new PagedConnection<PaymentIn>(response.Results, query.Skip, query.Take, response.TotalCount);
        }

        private async Task CheckAuthAsync(IResolveFieldContext context, string orderId)
        {
            var order = await _customerOrderService.GetByIdAsync(orderId)
                ?? throw new ArgumentException($"Order does not exist, ID: '{orderId}'", nameof(orderId));

            await AuthorizeAsync(context, order, allowAnonymous: true);
        }

        private async Task<CartAggregate> GetCartAggregateAsync(string cartId)
        {
            var cart = await _mediator.Send(new GetCartByIdQuery { CartId = cartId })
                ?? throw new ArgumentException($"Cart does not exist, ID: '{cartId}'", nameof(cartId));

            return cart;
        }

        private async Task AuthorizeAsync(IResolveFieldContext context, object resource, bool allowAnonymous)
        {
            await _userManagerCore.CheckUserState(context.GetCurrentUserId(), allowAnonymous);
            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), resource, new CanAccessOrderAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw !allowAnonymous || context.IsAuthenticated()
                    ? AuthorizationError.Forbidden()
                    : AuthorizationError.AnonymousAccessDenied();
            }
        }
    }
}
