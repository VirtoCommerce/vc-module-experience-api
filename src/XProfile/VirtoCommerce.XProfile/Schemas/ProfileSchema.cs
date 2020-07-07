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
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;
        private readonly IMemberServiceX _memberService;

        public ProfileSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader, IMemberServiceX memberService)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
            _memberService = memberService;
        }

        public void Build(ISchema schema)
        {
            //Queries
            var organizationFiled = new FieldType
            {
                Name = "organization",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "organizationId" }),
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var organizationId = context.GetArgument<string>("organizationId");

                    var getOrganizationByIdQuery = new GetOrganizationByIdQuery(organizationId);
                    var organizationAggregate = await _mediator.Send(getOrganizationByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("organizationAggregate", organizationAggregate);

                    return organizationAggregate;
                })
            };
            schema.Query.AddField(organizationFiled);

            var contactField = new FieldType
            {
                Name = "contact",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "contactId" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var contactId = context.GetArgument<string>("contactId");

                    var getContactByIdQuery = new GetContactByIdQuery(contactId);
                    var contactAggregate = await _mediator.Send(getContactByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("contactAggregate", contactAggregate);

                    return contactAggregate;
                })
            };
            schema.Query.AddField(contactField);

            /// <example>
            /// {
            ///     customer(id: "9d5b5d2ee74b48ffb1212bb87d47864f"){
            ///         firstName
            ///         organization{ name }
            ///         addresses { line1 }
            ///     }
            /// }
            /// </example>
            _ = schema.Query.AddField(new FieldType
            {
                Name = "customer",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "user id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<Contact>(async context =>
                {
                    var getCartQuery = new GetContactByIdQuery(context.GetArgument<string>("id"));
                    var cartAggregate = await _mediator.Send(getCartQuery);
                    return cartAggregate.Contact;
                })
            });

            /// <example>
            /// This is a sample request for organization users (connection) query. Valid organizationId required
            ///{
            ///    organizationUsers(command: { organizationId: "2e4c562f-f51c-4d49-84c3-8ba9f661aee7", userId: "62223176-92db-4bf7-963a-15a07928095c"}){
            ///        totalCount items { contact{ firstName } userName }
            ///    }
            ///}
            /// </example>
            var connectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ContactType, object>()
              .Name("searchOrganizationMembers")
              .Argument<NonNullGraphType<InputSearchOrganizationMembersType>>(_commandName, "Query command")
              .Unidirectional()
              .PageSize(20);
            connectionBuilder.ResolveAsync(ResolveOrganizationUsersConnectionAsync);
            _ = schema.Query.AddField(connectionBuilder.FieldType);

            //_ = schema.Mutation.AddField(FieldBuilder.Create<UserUpdateInfo, Contact>(GraphTypeExtenstionHelper.GetActualType<ContactType>())
            //                .Name("updateAccount")
            //                .Argument<NonNullGraphType<UserUpdateInfoInputType>>("userUpdateInfo")
            //                .ResolveAsync(async context =>
            //                {
            //                    return await _memberService.UpdateContactAsync(context.GetArgument<UserUpdateInfo>("userUpdateInfo"));
            //                }).FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("updateAddresses")
                            .Argument<NonNullGraphType<InputUpdateContactAddressType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateContactAddressesCommand>(_commandName)))
                            .FieldType);

            /// <example>
            /// mutation ($input: OrganizationUpdateInfoInputType!){
            ///    updateOrganizationInfo(input: $input){
            ///        name addresses{ line1 }
            ///    }
            ///}
            /// </example>
            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationUpdateInfo, Organization>(GraphTypeExtenstionHelper.GetActualType<OrganizationType>())
                            .Name("updateOrganizationInfo")
                            .Argument<NonNullGraphType<InputUpdateOrganizationType>>("input")
                            .ResolveAsync(async context =>
                            {
                                return await _memberService.UpdateOrganizationAsync(
                                             context.GetArgument<OrganizationUpdateInfo>("input"));
                            }).FieldType);

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

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, bool>(typeof(ContactType))
                            .Name("deleteContact")
                            .Argument<NonNullGraphType<InputDeleteContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<DeleteContactCommand>(_commandName)))
                            .FieldType);
        }

        private async Task<object> ResolveOrganizationUsersConnectionAsync(IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var command = context.GetArgument<SearchOrganizationMembersQuery>(_commandName);
            command.Take = first ?? 20;
            command.Skip = skip;
            var response = await _mediator.Send(command);

            var result = new Connection<Member>()
            {
                Edges = response.Results.Select((x, index) =>
                        new Edge<Member>()
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
    }
}
