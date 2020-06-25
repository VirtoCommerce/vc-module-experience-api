using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Commands;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PurchaseSchema : ISchemaBuilder
    {
        private readonly ICartAggregateRepository _cartAggrRepository;
        private readonly IMediator _mediator;
        public PurchaseSchema(IMediator mediator, ICartAggregateRepository cartAggrFactory)
        {
            _mediator = mediator;
            _cartAggrRepository = cartAggrFactory;
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
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cartName" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "type" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    //TODO: Move to extension methods
                    var storeId = context.GetArgument<string>("storeId");
                    var cartName = context.GetArgument<string>("cartName");
                    var userId = context.GetArgument<string>("userId");
                    var cultureName = context.GetArgument<string>("cultureName");
                    var currencyCode = context.GetArgument<string>("currencyCode");
                    var type = context.GetArgument<string>("type");
                                     

                    var cartAggregate = await _cartAggrRepository.GetOrCreateAsync(cartName, storeId, userId, cultureName, currencyCode, type);

                    await cartAggregate.ValidateAsync();
                    await cartAggregate.RecalculateAsync();

                    //TODO:
                    //context.UserContext.Add("taxCalculationEnabled", shoppingCartContext.Store.TaxCalculationEnabled);
                    //context.UserContext.Add("fixedTaxRate", shoppingCartContext.Store.FixedTaxRate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(cartField);

            //Mutations
            //TODO: User result type with errors 
            var clearCartField = FieldBuilder.Create<ShoppingCart, ShoppingCart>(typeof(CartType))
                                                 .Name("clearCart")
                                                 .Argument<NonNullGraphType<InputClearCartType>>("payload")
                                                 .Resolve(context =>
                                                 {
                                                     //TODO: Move to extension methods
                                                     var storeId = context.GetArgument<string>("storeId");
                                                     var cartName = context.GetArgument<string>("cartName");
                                                     var userId = context.GetArgument<string>("userId");
                                                     var cultureName = context.GetArgument<string>("cultureName");
                                                     var currencyCode = context.GetArgument<string>("currencyCode");
                                                     var type = context.GetArgument<string>("type");

                                                     _mediator.Send(new ClearCartCommand(storeId, type, cartName, userId, currencyCode, cultureName) { });
                                                     //TODO: return cart here from response
                                                     return new ShoppingCart { } ;
                                                 }).FieldType;

          
            schema.Mutation.AddField(clearCartField);
        }       
    }
}
