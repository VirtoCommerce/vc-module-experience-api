using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CoreSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;

        public CoreSchema(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            #region countries query

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     countries
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion

            _ = schema.Query.AddField(new FieldType
            {
                Name = "countries",
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CountryType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetCountriesQuery());

                    return result.Countries;
                })
            });

            #region regions query

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     regions(countryId: "country code")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion

            _ = schema.Query.AddField(new FieldType
            {
                Name = "regions",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "countryId" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CountryRegionType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetRegionsQuery
                    {
                        CountryId = context.GetArgument<string>("countryId"),
                    });

                    return result.Regions;
                })
            });

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     validatePassword(password: "pswd")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Query.AddField(new FieldType
            {
                Name = "validatePassword",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }),
                Type = GraphTypeExtenstionHelper.GetActualType<PasswordValidationType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new PasswordValidationQuery
                    {
                        Password = context.GetArgument<string>("password"),
                    });

                    return result;
                })
            });

            var shippingMethodsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CoreShippingMethodType, object>()
                .Name("shippingMethods")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<NonNullGraphType<StringGraphType>>("storeId", "")
                .Unidirectional()
                .PageSize(50);
            shippingMethodsConnectionBuilder.ResolveAsync(async context => await ResolveShipmentMethodsConnectionAsync(_mediator, context));
            schema.Query.AddField(shippingMethodsConnectionBuilder.FieldType);

            var paymentMethodsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CorePaymentMethodType, object>()
                .Name("paymentMethods")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<NonNullGraphType<StringGraphType>>("storeId", "")
                .Unidirectional()
                .PageSize(50);
            paymentMethodsConnectionBuilder.ResolveAsync(async context => await ResolvePaymentMethodsConnectionAsync(_mediator, context));
            schema.Query.AddField(paymentMethodsConnectionBuilder.FieldType);
        }

        private async Task<object> ResolveShipmentMethodsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            int.TryParse(context.After, out var skip);

            var query = new SearchShippingMethodsQuery
            {
                Skip = skip,
                Take = context.First ?? context.PageSize ?? 50,
                StoreId = context.GetArgumentOrValue<string>("storeId"),
            };

            var response = await mediator.Send(query);

            return new PagedConnection<ShippingMethod>(response.Results, query.Skip, query.Take, response.TotalCount);
        }

        private async Task<object> ResolvePaymentMethodsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            int.TryParse(context.After, out var skip);

            var query = new SearchPaymentMethodsQuery
            {
                Skip = skip,
                Take = context.First ?? context.PageSize ?? 50,
                StoreId = context.GetArgumentOrValue<string>("storeId"),
            };

            var response = await mediator.Send(query);

            return new PagedConnection<PaymentMethod>(response.Results, query.Skip, query.Take, response.TotalCount);
        }
    }
}
