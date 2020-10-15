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
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        public readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;
        private readonly Func<SignInManager<ApplicationUser>> _signInManagerFactory;
        private readonly ICurrencyService _currencyService;


        public OrderSchema(
            IMediator mediator
            , IAuthorizationService authorizationService
            , Func<SignInManager<ApplicationUser>> signInManagerFactory
            , ICurrencyService currencyService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _signInManagerFactory = signInManagerFactory;
            _currencyService = currencyService;
        }

        public void Build(ISchema schema)
        {
            _ = schema.Query.AddField(new FieldType
            {
                Name = "order",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "number" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" }
                    ),
                Type = GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var orderAggregate = await _mediator.Send(new GetOrderQuery(context.GetArgument<string>("id"), context.GetArgument<string>("number")));
                    //TODO: this authorization checks prevent of returns orders of other users very often case for b2b scenarios
                    //Need to find out other solution how to do such authorization checks
                    //await CheckAuthAsync(context, orderAggregate.Order);

                    //store order aggregate in the user context for future usage in the graph types resolvers
                    context.SetExpandedObjectGraph(orderAggregate);

                    return orderAggregate;
                })
            });

            var orderConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CustomerOrderType, object>()
                .Name("orders")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<StringGraphType>("language", "")
                .Argument<NonNullGraphType<StringGraphType>>("userId", "")
                .Unidirectional()
                .PageSize(20);

            orderConnectionBuilder.ResolveAsync(async context => await ResolveOrdersConnectionAsync(_mediator, context));

            schema.Query.AddField(orderConnectionBuilder.FieldType);


            var paymentsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<PaymentInType, object>()
             .Name("payments")
             .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
             .Argument<StringGraphType>("sort", "The sort expression")
             .Argument<StringGraphType>("language", "")
             .Argument<NonNullGraphType<StringGraphType>>("userId", "")
             .Unidirectional()
             .PageSize(20);
            paymentsConnectionBuilder.ResolveAsync(async context => await ResolvePaymentsConnectionAsync(_mediator, context));
            schema.Query.AddField(paymentsConnectionBuilder.FieldType);


            _ = schema.Mutation.AddField(FieldBuilder.Create<object, CustomerOrderAggregate>(typeof(CustomerOrderType))
                            .Name("createOrderFromCart")
                            .Argument<NonNullGraphType<InputCreateOrderFromCartType>>(_commandName)
                            .ResolveAsync(async context =>
                            {
                                var response = await _mediator.Send(context.GetArgument<CreateOrderFromCartCommand>(_commandName));
                                context.SetExpandedObjectGraph(response);
                                return response;
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                            .Name("changeOrderStatus")
                            .Argument<NonNullGraphType<InputChangeOrderStatusType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<ChangeOrderStatusCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                            .Name("confirmOrderPayment")
                            .Argument<NonNullGraphType<InputConfirmOrderPaymentType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<ConfirmOrderPaymentCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                            .Name("cancelOrderPayment")
                            .Argument<NonNullGraphType<InputCancelOrderPaymentType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CancelOrderPaymentCommand>(_commandName)))
                            .FieldType);
        }

        private async Task<object> ResolveOrdersConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var request = new SearchOrderQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                Filter = context.GetArgument<string>("filter"),
                Sort = context.GetArgument<string>("sort"),
                CultureName = context.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase())
            };

            context.UserContext.Add(nameof(Currency.CultureName).ToCamelCase(), request.CultureName);
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            context.SetCurrencies(allCurrencies, request.CultureName);

            //TODO: this authorization checks prevent of returns orders of other users very often case for b2b scenarios
            //Need to find out other solution how to do such authorization checks
            //await CheckAuthAsync(context, request);

            var response = await mediator.Send(request);

            foreach (var customerOrderAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(customerOrderAggregate);
            }

            return new PagedConnection<CustomerOrderAggregate>(response.Results, skip, Convert.ToInt32(context.After ?? 0.ToString()), response.TotalCount);
        }

        private async Task<object> ResolvePaymentsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var request = new SearchPaymentsQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                Filter = context.GetArgument<string>("filter"),
                Sort = context.GetArgument<string>("sort"),
                CultureName = context.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase())
            };

            context.UserContext.Add(nameof(Currency.CultureName).ToCamelCase(), request.CultureName);

            //TODO: this authorization checks prevent of returns orders of other users very often case for b2b scenarios
            //Need to find out other solution how to do such authorization checks
            //await CheckAuthAsync(context, request);

            var response = await mediator.Send(request);

            foreach (var payment in response.Results)
            {
                context.SetExpandedObjectGraph(payment);
            }
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            //Store all currencies in the user context for future resolve in the schema types
            context.SetCurrencies(allCurrencies, request.CultureName);

            return new PagedConnection<PaymentIn>(response.Results, skip, Convert.ToInt32(context.After ?? 0.ToString()), response.TotalCount);
        }

        private async Task CheckAuthAsync(IResolveFieldContext context, object resource)
        {
            var userId = context.GetArgument<string>("userId");

            if (userId == null)
            {
                throw new ExecutionError($"argument {nameof(userId)} is null");
            }

            var signInManager = _signInManagerFactory();
            var user = await signInManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ExecutionError($"can't find user with id:{userId}");
            }

            var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            var authorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, resource, new CanAccessOrderAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new ExecutionError($"access denied by userId:{userId}");
            }
        }
    }
}
