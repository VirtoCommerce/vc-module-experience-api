using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        public readonly IMediator _mediator;

        public OrderSchema(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            _ = schema.Query.AddField(new FieldType
            {
                Name = "order",
                Arguments = new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }, new QueryArgument<StringGraphType> { Name = "number" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CustomerOrderType>(),
                Resolver = new AsyncFieldResolver<object>(async context => {
                    var orderAggregate = await _mediator.Send(new GetOrderQuery(context.GetArgument<string>("id"), context.GetArgument<string>("number")));
                    //store order aggregate in the user context for future usage in the graph types resolvers
                    AddOrderToContext(context, orderAggregate);

                    return orderAggregate;
                })
            });

            var orderConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CustomerOrderType, object>()
                .Name("orders")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<StringGraphType>("language", "")
                .Unidirectional()
                .PageSize(20);

            orderConnectionBuilder.ResolveAsync(async context => await ResolveConnectionAsync(_mediator, context));

            schema.Query.AddField(orderConnectionBuilder.FieldType);


            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                            .Name("updateOrder")
                            .Argument<NonNullGraphType<InputUpdateOrderType>>(_commandName)
                            .ResolveAsync(async context => {
                                var arg = context.GetArgument<UpdateOrderCommand>(_commandName);
                                return await _mediator.Send(arg);
                                })
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

            var response = await mediator.Send(request);
            foreach (var customerOrderAggregate in response.Results)
            {
                AddOrderToContext(context, customerOrderAggregate);
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


        //TODO
        private void AddOrderToContext(IResolveFieldContext context, CustomerOrderAggregate customerOrderAggregate)
        {
            context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{customerOrderAggregate.Order.Id}", customerOrderAggregate);

            foreach (var discount in customerOrderAggregate.Order.Discounts)
            {
                context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{discount.Id}", customerOrderAggregate);
            }

            foreach (var discount in customerOrderAggregate.Order.Items.SelectMany(x => x.Discounts))
            {
                context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{discount.Id}", customerOrderAggregate);
            }

            foreach (var discount in customerOrderAggregate.Order.Shipments.SelectMany(x => x.Discounts))
            {
                context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{discount.Id}", customerOrderAggregate);
            }

            foreach (var taxDetail in customerOrderAggregate.Order.TaxDetails)
            {
                context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{string.Join(":", taxDetail.Name, taxDetail.Rate, taxDetail.Amount)}", customerOrderAggregate);
            }

            foreach (var taxDetail in customerOrderAggregate.Order.Items.SelectMany(x => x.TaxDetails))
            {
                context.UserContext.Add($"{nameof(CustomerOrderAggregate).ToCamelCase()}:{string.Join(":", taxDetail.Name, taxDetail.Rate, taxDetail.Amount)}", customerOrderAggregate);
            }
        }
    }
}
