using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

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
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CountryType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetCountriesQuery());

                    return result.Countries;
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

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     checkUsernameUniqueness(username: "testUser")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            _ = schema.Query.AddField(new FieldType
            {
                Name = "checkUsernameUniqueness",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "username" }),
                Type = GraphTypeExtenstionHelper.GetActualType<BooleanGraphType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new CheckUsernameUniquenessQuery
                    {
                        Username = context.GetArgument<string>("username"),
                    });

                    return result.IsUnique;
                })
            });

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     checkEmailUniqueness(email: "user@email")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            _ = schema.Query.AddField(new FieldType
            {
                Name = "checkEmailUniqueness",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "email" }),
                Type = GraphTypeExtenstionHelper.GetActualType<BooleanGraphType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new CheckEmailUniquenessQuery
                    {
                        Email = context.GetArgument<string>("email"),
                    });

                    return result.IsUnique;
                })
            });
        }
    }
}
