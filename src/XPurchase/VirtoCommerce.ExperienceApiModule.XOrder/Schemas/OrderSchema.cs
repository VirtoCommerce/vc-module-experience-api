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
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XOrder.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        public readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;
        private readonly Func<SignInManager<ApplicationUser>> _signInManagerFactory;


        public OrderSchema(IMediator mediator, IAuthorizationService authorizationService, Func<SignInManager<ApplicationUser>> signInManagerFactory)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _signInManagerFactory = signInManagerFactory;
        }

        public void Build(ISchema schema)
        {
            _ = schema.Query.AddField(new FieldType
            {
                Name = "order",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "number" },
                    new QueryArgument<StringGraphType> { Name = "userId" }
                    ),
                Type = GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var orderAggregate = await _mediator.Send(new GetOrderQuery(context.GetArgument<string>("id"), context.GetArgument<string>("number")));
                    await CheckAuthAsync(context, orderAggregate.Order);
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
                .Argument<StringGraphType>("userId", "")
                .Unidirectional()
                .PageSize(20);

            orderConnectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(_mediator, context));

            schema.Query.AddField(orderConnectionBuilder.FieldType);

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

        private async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
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

            await CheckAuthAsync(context, request);

            var response = await mediator.Send(request);


            foreach (var customerOrderAggregate in response.Results)
            {
                context.SetExpandedObjectGraph(customerOrderAggregate);
            }

            var result = new Connection<CustomerOrderAggregate>()
            {
                Edges = response.Results
                    .Select((x, index) =>
                        new Edge<CustomerOrderAggregate>()
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
