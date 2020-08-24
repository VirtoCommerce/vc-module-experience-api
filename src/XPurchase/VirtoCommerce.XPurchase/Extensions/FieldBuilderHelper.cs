using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class FieldBuilderHelper
    {
        public static EventStreamFieldType CreateCartAggregateMutation<TArgumentType, TCartCommand>(string mutationName, IMediator mediator)
            where TArgumentType : InputCartBaseType
            where TCartCommand : CartCommand
        => FieldBuilder
            .Create<CartAggregate, CartAggregate>(typeof(CartType))
            .Name(mutationName)
            .Argument<NonNullGraphType<TArgumentType>>(PurchaseSchema._commandName)
            .ResolveAsync(async context =>
            {
                var command = context.GetCartCommand<TCartCommand>();
                var cartAggregate = await mediator.Send(command);
                context.UserContext.Add("cartAggregate", cartAggregate);
                return cartAggregate;
            })
            .FieldType;

        public static EventStreamFieldType CreateCartAggregateOperation<TArgumentType, TCartCommand>(string mutationName, IMediator mediator)
            where TArgumentType : InputObjectGraphType
            where TCartCommand : class, ICommand<bool>
        => FieldBuilder
            .Create<CartAggregate, bool>(typeof(BooleanGraphType))
            .Name(mutationName)
            .Argument<NonNullGraphType<TArgumentType>>(PurchaseSchema._commandName)
            .ResolveAsync(async context =>
            {
                var command = context.GetArgument<TCartCommand>(PurchaseSchema._commandName);
                var cartAggregate = await mediator.Send(command);
                context.UserContext.Add("cartAggregate", cartAggregate);
                return cartAggregate;
            })
            .FieldType;
    }
}
