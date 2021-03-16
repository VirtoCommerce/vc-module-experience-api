using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Authorization;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;
        private readonly Func<SignInManager<ApplicationUser>> _signInManagerFactory;

        public ProfileSchema(IMediator mediator, IAuthorizationService authorizationService, Func<SignInManager<ApplicationUser>> signInManagerFactory)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _signInManagerFactory = signInManagerFactory;
        }

        public void Build(ISchema schema)
        {
            schema.Query.AddField(new FieldType
            {
                Name = "me",
                Type = GraphTypeExtenstionHelper.GetActualType<UserType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var userName = ((GraphQLUserContext)context.UserContext).User?.Identity.Name;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var result = await _mediator.Send(new GetUserQuery
                        {
                            UserName = userName
                        });
                        return result;
                    }
                    return AnonymousUser.Instance;
                })
            });

            //Queries

            #region organization query

            /* organization query with contacts connection filtering:
            {
              organization(id: "689a72757c754bef97cde51afc663430"){
                 id contacts(first:10, after: "0", searchPhrase: null){
                  totalCount items {id firstName}
                }
              }
            }
             */

            #endregion
            schema.Query.AddField(new FieldType
            {
                Name = "organization",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "userId" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var organizationId = context.GetArgument<string>("id");
                    var query = new GetOrganizationByIdQuery(organizationId);
                    var organizationAggregate = await _mediator.Send(query);

                    await CheckAuthAsync(context.GetCurrentUserId(), organizationAggregate);
                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("organizationAggregate", organizationAggregate);

                    return organizationAggregate;
                })
            });

            #region contact query
            /// <example>
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         {
                          contact(id: "51311ae5-371c-453b-9394-e6d352f1cea7"){
                              firstName memberType organizationIds organizations { id businessCategory description emails groups memberType name outerId ownerId parentId phones seoObjectType }
                              addresses { line1 phone }
                         }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            /// </example>

            #endregion
            schema.Query.AddField(new FieldType
            {
                Name = "contact",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "userId" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var query = new GetContactByIdQuery(context.GetArgument<string>("id"));
                    var contactAggregate = await _mediator.Send(query);
                    await CheckAuthAsync(context.GetCurrentUserId(), contactAggregate);
                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("contactAggregate", contactAggregate);

                    return contactAggregate;
                })
            });

            #region updateAddressMutation

            /// sample code for updating addresses:
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                        mutation updateAddresses($command: InputUpdateContactAddressType!){
                        contact: updateAddresses(command: $command)
                          {
                            firstName lastName
                            addresses { key city countryCode countryName email firstName  lastName line1 line2 middleName name phone postalCode regionId regionName zip }
                          }
                        }
                        query variables:
                        {
                            "command": {
                              "contactId": "acc3b262-a21e-45f9-a612-b4b1530d27ef",
                              "addresses": [{"addressType": "Shipping", "name": "string", "countryCode": "string", "countryName": "string", "city": "string", "postalCode": "string", "line1": "string", "regionId": "string", "regionName": "string", "firstName": "string", "lastName": "string", "phone": "string", "email": "string", "regionId": "string"
                                }]
                            }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion

            var updateAddressesField = new EventStreamFieldType
            {
                Name = "updateAddresses",
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdateContactAddressType>>(_commandName),
                Resolver = new AsyncFieldResolver<ContactAggregate, ContactAggregate>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdateContactAddressesCommand>();
                    var command = (UpdateContactAddressesCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(updateAddressesField);

            var updateOrganizationField = new EventStreamFieldType
            {
                Name = "updateOrganization",
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdateOrganizationType>>(_commandName),
                Resolver = new AsyncFieldResolver<OrganizationAggregate, OrganizationAggregate>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdateOrganizationCommand>();
                    var command = (UpdateOrganizationCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command, CustomerModule.Core.ModuleConstants.Security.Permissions.Update);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(updateOrganizationField);

            var createOrganizationField = new EventStreamFieldType
            {
                Name = "createOrganization",
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputCreateOrganizationType>>(_commandName),
                Resolver = new AsyncFieldResolver<OrganizationAggregate, OrganizationAggregate>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<CreateOrganizationCommand>();
                    var command = (CreateOrganizationCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(createOrganizationField);

            var createContactField = new EventStreamFieldType
            {
                Name = "createContact",
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputCreateContactType>>(_commandName),
                Resolver = new AsyncFieldResolver<ContactAggregate, ContactAggregate>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<CreateContactCommand>();
                    var command = (CreateContactCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);

                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(createContactField);

            var updateContactField = new EventStreamFieldType
            {
                Name = "updateContact",
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdateContactType>>(_commandName),
                Resolver = new AsyncFieldResolver<ContactAggregate, ContactAggregate>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdateContactCommand>();
                    var command = (UpdateContactCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);

                })
            };
            _ = schema.Mutation.AddField(updateContactField);

            var deleteContactField = new EventStreamFieldType
            {
                Name = "deleteContact",
                Type = typeof(BooleanGraphType),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputDeleteContactType>>(_commandName),
                Resolver = new AsyncFieldResolver<ContactAggregate, bool>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<DeleteContactCommand>();
                    var command = (DeleteContactCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command, CustomerModule.Core.ModuleConstants.Security.Permissions.Delete);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(deleteContactField);

            var updatePersonalDataField = new EventStreamFieldType
            {
                Name = "updatePersonalData",
                Type = GraphTypeExtenstionHelper.GetActualType<IdentityResultType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdatePersonalDataType>>(_commandName),
                Resolver = new AsyncFieldResolver<object, IdentityResult>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdatePersonalDataCommand>();
                    var command = (UpdatePersonalDataCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(updatePersonalDataField);

            var sendVerifyEmailField = new EventStreamFieldType
            {
                Name = "sendVerifyEmail",
                Type = typeof(BooleanGraphType),
                Arguments = QueryArgumentsHelper.GetQueryArguments<InputSendVerifyEmailType>(_commandName),
                Resolver = new AsyncFieldResolver<object, bool>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<SendVerifyEmailCommand>();
                    var command = (SendVerifyEmailCommand)context.GetArgument(type, _commandName);

                    var isAuthenticated = ((GraphQLUserContext)context.UserContext).User?.Identity?.IsAuthenticated ?? false;
                    if (isAuthenticated)
                    {
                        command.Email = await GetUserEmailAsync(context.GetCurrentUserId());
                    }

                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(sendVerifyEmailField);

            // Security API fields

            #region user query

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                            {
                                user(id: "1eb2fa8ac6574541afdb525833dadb46"){
                                userName isAdministrator roles { name } userType memberId storeId
                                }
                            }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            _ = schema.Query.AddField(new FieldType
            {
                Name = "user",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "userName" },
                    new QueryArgument<StringGraphType> { Name = "email" },
                    new QueryArgument<StringGraphType> { Name = "loginProvider" },
                    new QueryArgument<StringGraphType> { Name = "providerKey" }),
                Type = GraphTypeExtenstionHelper.GetActualType<UserType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var user = await _mediator.Send(new GetUserQuery(
                        id: context.GetArgument<string>("id"),
                        userName: context.GetArgument<string>("userName"),
                        email: context.GetArgument<string>("email"),
                        loginProvider: context.GetArgument<string>("loginProvider"),
                        providerKey: context.GetArgument<string>("providerKey")));

                    await CheckAuthAsync(context.GetCurrentUserId(), user);

                    return user;
                })
            });

            #region role query

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         {
                          getRole(roleName: "Use api"){
                           permissions
                          }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            _ = schema.Query.AddField(new FieldType
            {
                Name = "role",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "roleName" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<RoleType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetRoleQuery(context.GetArgument<string>("roleName")));

                    return result;
                })
            });

            #region create user

#pragma warning disable S125 // Sections of code should not be commented out
            /*
            mutation ($command: InputCreateUserType!){
                createUser(command: $command){ succeeded errors { code }}
            }
            Query variables:
            {
                "command": {
                "createdBy": "eXp1", "email": "eXp1@mail.com", "password":"eXp1@mail.com", "userName": "eXp1@mail.com", "userType": "Customer"
                }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            var createUserField = new EventStreamFieldType
            {
                Name = "createUser",
                Type = GraphTypeExtenstionHelper.GetActualType<IdentityResultType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputCreateUserType>>(_commandName),
                Resolver = new AsyncFieldResolver<object, IdentityResult>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<CreateUserCommand>();
                    var command = (CreateUserCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(createUserField);

            #region update user

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         mutation ($command: InputUpdateUserType!){
                          updateUser(command: $command){ succeeded errors { description } }
                        }
                        Query variables:
                        {
                         "command":{
                          "isAdministrator": false,
                          "userType": "Customer",
                          "roles": [],
                          "id": "b5d28a83-c296-4212-b89e-046fca3866be",
                          "userName": "_loGIN999",
                          "email": "_loGIN999@gmail.com"
                            }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            var updateUserField = new EventStreamFieldType
            {
                Name = "updateUser",
                Type = GraphTypeExtenstionHelper.GetActualType<IdentityResultType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdateUserType>>(_commandName),
                Resolver = new AsyncFieldResolver<object, IdentityResult>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdateUserCommand>();
                    var command = (UpdateUserCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(updateUserField);

            #region delete user

#pragma warning disable S125 // Sections of code should not be commented out
            /*
             mutation ($command: InputDeleteUserType!){
              deleteUser(command: $command){ succeeded errors { description } }
            }
            Query variables:
            {
              "command": {
                "userNames": ["admin",  "eXp1@mail.com"]
              }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            var deleteUsersField = new EventStreamFieldType
            {
                Name = "deleteUsers",
                Type = GraphTypeExtenstionHelper.GetActualType<IdentityResultType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputDeleteUserType>>(_commandName),
                Resolver = new AsyncFieldResolver<object, IdentityResult>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<DeleteUserCommand>();
                    var command = (DeleteUserCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command, PlatformConstants.Security.Permissions.SecurityDelete);
                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(deleteUsersField);

            #region update role query

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         mutation ($command: InputUpdateRoleType!){
                          updateRole(command: $command){ succeeded errors { description } }
                        }
                        Query variables:
                        {
                         "command":{
                         "id": "graphtest",  "name": "graphtest", "permissions": [
                            { "name": "order:read", "assignedScopes": [{"scope": "{{userId}}", "type": "OnlyOrderResponsibleScope" }] }
                          ]
                         }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            var updateRoleField = new EventStreamFieldType
            {
                Name = "updateRole",
                Type = GraphTypeExtenstionHelper.GetActualType<IdentityResultType>(),
                Arguments = QueryArgumentsHelper.GetComplexTypeQueryArguments<NonNullGraphType<InputUpdateRoleType>>(_commandName),
                Resolver = new AsyncFieldResolver<object, IdentityResult>(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<UpdateRoleCommand>();
                    var command = (UpdateRoleCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command, PlatformConstants.Security.Permissions.SecurityUpdate);

                    return await _mediator.Send(command);
                })
            };
            _ = schema.Mutation.AddField(updateRoleField);
        }


        private async Task CheckAuthAsync(string userId, object resource, params string[] permissions)
        {
            var signInManager = _signInManagerFactory();

            var user = await signInManager.UserManager.FindByIdAsync(userId) ?? new ApplicationUser
            {
                Id = userId,
                UserName = Core.AnonymousUser.UserName,
            };

            var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);

            if (!permissions.IsNullOrEmpty())
            {
                foreach (var permission in permissions ?? Array.Empty<string>())
                {
                    var permissionAuthorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, null, new PermissionAuthorizationRequirement(permission));
                    if (!permissionAuthorizationResult.Succeeded)
                    {
                        throw new ExecutionError($"User doesn't have the required permission '{permission}'.");
                    }
                }
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, resource, new ProfileAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new ExecutionError($"Access denied");
            }
        }

        private async Task<string> GetUserEmailAsync(string userId)
        {
            var signInManager = _signInManagerFactory();

            var user = await signInManager.UserManager.FindByIdAsync(userId);

            return user?.Email;
        }
    }
}
