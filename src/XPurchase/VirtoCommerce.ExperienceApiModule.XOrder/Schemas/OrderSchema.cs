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
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        public readonly IMediator _mediator;
        private readonly ICurrencyService _currencyService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICustomerOrderService _customerOrderService;

        public OrderSchema(IMediator mediator, ICurrencyService currencyService, IAuthorizationService authorizationService, ICustomerOrderService customerOrderService)
        {
            _mediator = mediator;
            _currencyService = currencyService;
            _authorizationService = authorizationService;
            _customerOrderService = customerOrderService;
        }

        public void Build(ISchema schema)
        {
            _ = schema.Query.AddField(new FieldType
            {
                Name = "order",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "number" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-US\")" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var request = new GetOrderQuery
                    {
                        Number = context.GetArgument<string>("number"),
                        OrderId = context.GetArgument<string>("id"),
                        CultureName = context.GetArgument<string>(nameof(Currency.CultureName))
                    };

                    context.CopyArgumentsToUserContext();

                    var orderAggregate = await _mediator.Send(request);

                    var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), orderAggregate.Order, new CanAccessOrderAuthorizationRequirement());

                    if (!authorizationResult.Succeeded)
                    {
                        throw new AuthorizationError($"Access denied");
                    }

                    var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
                    //Store all currencies in the user context for future resolve in the schema types
                    context.SetCurrencies(allCurrencies, request.CultureName);

                    //store order aggregate in the user context for future usage in the graph types resolvers
                    context.SetExpandedObjectGraph(orderAggregate);

                    return orderAggregate;
                })
            });

            var orderConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CustomerOrderType, object>()
                .Name("orders")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<StringGraphType>("cultureName", "Culture name (\"en-US\")")
                .Argument<StringGraphType>("userId", "")
                .PageSize(20);

            orderConnectionBuilder.ResolveAsync(async context => await ResolveOrdersConnectionAsync(_mediator, context));

            schema.Query.AddField(orderConnectionBuilder.FieldType);

            var paymentsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<PaymentInType, object>()
             .Name("payments")
             .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
             .Argument<StringGraphType>("sort", "The sort expression")
             .Argument<StringGraphType>("cultureName", "Culture name (\"en-US\")")
             .Argument<NonNullGraphType<StringGraphType>>("userId", "")
             .PageSize(20);
            paymentsConnectionBuilder.ResolveAsync(async context => await ResolvePaymentsConnectionAsync(_mediator, context));
            schema.Query.AddField(paymentsConnectionBuilder.FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, CustomerOrderAggregate>(GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>())
                            .Name("createOrderFromCart")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateOrderFromCartType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<CreateOrderFromCartCommand>();
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
        }

        private async Task<object> ResolveOrdersConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = new SearchOrderQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                Filter = context.GetArgument<string>("filter"),
                Sort = context.GetArgument<string>("sort"),
                CultureName = context.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase()),
                CustomerId = context.GetArgumentOrValue<string>("userId")
            };

            context.CopyArgumentsToUserContext();
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            context.SetCurrencies(allCurrencies, query.CultureName);

            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), query, new CanAccessOrderAuthorizationRequirement());
            if (!authorizationResult.Succeeded)
            {
                throw new AuthorizationError($"Access denied");
            }

            var response = await mediator.Send(query);

            foreach (var customerOrderAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(customerOrderAggregate);
            }

            return new PagedConnection<CustomerOrderAggregate>(response.Results, query.Skip, query.Take, response.TotalCount);
        }

        private async Task<object> ResolvePaymentsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = new SearchPaymentsQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                Filter = context.GetArgument<string>("filter"),
                Sort = context.GetArgument<string>("sort"),
                CultureName = context.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase()),
                CustomerId = context.GetArgumentOrValue<string>("userId")
            };

            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), query, new CanAccessOrderAuthorizationRequirement());
            if (!authorizationResult.Succeeded)
            {
                throw new AuthorizationError($"Access denied");
            }

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
            var order = await _customerOrderService.GetByIdAsync(orderId);

            var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), order, new CanAccessOrderAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new AuthorizationError($"Access denied");
            }
        }
    }
}
