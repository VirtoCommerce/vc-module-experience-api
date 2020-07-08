using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;

        public ProfileSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
        }

        public void Build(ISchema schema)
        {
            //Queries

            /* organization query with contacts connection filtering:
            {
              organization(id: "689a72757c754bef97cde51afc663430"){
                 id contacts(first:10, after: "0", searchPhrase: null){
                  totalCount items {id firstName}
                }
              }
            }
             */
            schema.Query.AddField(new FieldType
            {
                Name = "organization",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var organizationId = context.GetArgument<string>("id");

                    var getOrganizationByIdQuery = new GetOrganizationByIdQuery(organizationId);
                    var organizationAggregate = await _mediator.Send(getOrganizationByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("organizationAggregate", organizationAggregate);

                    return organizationAggregate;
                })
            });

            /// <example>
            /*
             {
              contact(id: "51311ae5-371c-453b-9394-e6d352f1cea7"){
                  firstName memberType organizationIds organizations { id businessCategory description emails groups memberType name outerId ownerId parentId phones seoObjectType }
                  addresses { line1 phone }
             }
            }
             */
            /// </example>
            schema.Query.AddField(new FieldType
            {
                Name = "contact",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var getContactByIdQuery = new GetContactByIdQuery(context.GetArgument<string>("id"));
                    var contactAggregate = await _mediator.Send(getContactByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("contactAggregate", contactAggregate);

                    return contactAggregate;
                })
            });

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("updateAddresses")
                            .Argument<NonNullGraphType<InputUpdateContactAddressType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateContactAddressesCommand>(_commandName)))
                            .FieldType);

            /// <example>
            ///mutation($command: OrganizationInputType!){
            ///    updateOrganization(command: $command){
            ///        name addresses { line1 }
            ///    }
            ///}
            /// </example>
            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(typeof(OrganizationType))
                            .Name("updateOrganization")
                            .Argument<NonNullGraphType<InputUpdateOrganizationType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateOrganizationCommand>(_commandName)))
                            .FieldType);

            /// <example>
            /// This is a sample mutation to createOrganization.
            /// mutation($command: createOrganization!){
            ///     createUserInvitations(command: $command){
            ///         succeeded errors { description }
            ///     }
            /// }
            /// query variables:
            /// {
            ///     "command": {

            ///     }
            /// }
            /// </example>
            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(typeof(OrganizationType))
                            .Name("createOrganization")
                            .Argument<NonNullGraphType<InputCreateOrganizationType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CreateOrganizationCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("createContact")
                            .Argument<NonNullGraphType<InputCreateContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CreateContactCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("updateContact")
                            .Argument<NonNullGraphType<InputUpdateContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateContactCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, bool>(typeof(BooleanGraphType))
                            .Name("deleteContact")
                            .Argument<NonNullGraphType<InputDeleteContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<DeleteContactCommand>(_commandName)))
                            .FieldType);
        }
    }
}
