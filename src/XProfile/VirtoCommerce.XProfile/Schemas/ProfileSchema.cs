using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.ExperienceApiModule.XProfile.Authorization;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
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
        private readonly IMemberAggregateFactory _factory;
        private readonly IMemberService _memberService;

        public ProfileSchema(
            IMediator mediator,
            IAuthorizationService authorizationService,
            Func<SignInManager<ApplicationUser>> signInManagerFactory,
            IMemberAggregateFactory factory,
            IMemberService memberService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
            _signInManagerFactory = signInManagerFactory;
            _factory = factory;
            _memberService = memberService;
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
                    var query = new GetOrganizationByIdQuery(context.GetArgument<string>("id"));
                    var organizationAggregate = await _mediator.Send(query);
                    await CheckAuthAsync(context.GetCurrentUserId(), organizationAggregate);
                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("organizationAggregate", organizationAggregate);

                    return organizationAggregate;
                })
            });

            var organizationsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<OrganizationType, object>()
                .Name("organizations")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            organizationsConnectionBuilder.ResolveAsync(async context =>
            {
                context.CopyArgumentsToUserContext();

                var query = context.GetSearchMembersQuery<SearchOrganizationsQuery>();
                var response = await _mediator.Send(query);

                return new PagedConnection<OrganizationAggregate>(response.Results.Select(x => _factory.Create<OrganizationAggregate>(x)), query.Skip, query.Take, response.TotalCount);
            });

            schema.Query.AddField(organizationsConnectionBuilder.FieldType);

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
                    //store contactAggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("contactAggregate", contactAggregate);

                    return contactAggregate;
                })
            });

            var contactsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ContactType, object>()
                .Name("contacts")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            contactsConnectionBuilder.ResolveAsync(async context =>
            {
                context.CopyArgumentsToUserContext();

                var query = context.GetSearchMembersQuery<SearchContactsQuery>();
                var response = await _mediator.Send(query);

                return new PagedConnection<ContactAggregate>(response.Results.Select(x => _factory.Create<ContactAggregate>(x)), query.Skip, query.Take, response.TotalCount);
            });

            schema.Query.AddField(contactsConnectionBuilder.FieldType);

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

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     requestPasswordReset(loginOrEmail: "user@email")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            _ = schema.Query.AddField(new FieldType
            {
                Name = "requestPasswordReset",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "loginOrEmail" },
                    new QueryArgument<StringGraphType> { Name = "urlSuffix" }),
                Type = GraphTypeExtenstionHelper.GetActualType<BooleanGraphType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new RequestPasswordResetQuery
                    {
                        LoginOrEmail = context.GetArgument<string>("loginOrEmail"),
                        UrlSuffix = context.GetArgument<string>("urlSuffix"),
                    });

                    return result;
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
                Type = GraphTypeExtenstionHelper.GetActualType<CustomIdentityResultType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new PasswordValidationQuery
                    {
                        Password = context.GetArgument<string>("password"),
                    });

                    return result;
                })
            });

            #region updateAddressMutation

            /// sample code for updating addresses:
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                        mutation updateMemberAddresses($command: UpdateMemberAddressesCommand!){
                          updateMemberAddresses(command: $command)
                          {
                            memberType
                            addresses { key city countryCode countryName email firstName  lastName line1 line2 middleName name phone postalCode regionId regionName zip }
                          }
                        }
                        query variables:
                        {
                            "command": {
                              "memberId": "any-member-id",
                              "addresses": [{"addressType": "Shipping", "name": "string", "countryCode": "string", "countryName": "string", "city": "string", "postalCode": "string", "line1": "string", "regionId": "string", "regionName": "string", "firstName": "string", "lastName": "string", "phone": "string", "email": "string", "regionId": "string"
                                }]
                            }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, MemberAggregateRootBase>(GraphTypeExtenstionHelper.GetActualType<MemberType>())
                            .Name("updateMemberAddresses")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateMemberAddressType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateMemberAddressesCommand>();
                                var command = (UpdateMemberAddressesCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command);
                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(GraphTypeExtenstionHelper.GetActualType<OrganizationType>())
                            .Name("updateOrganization")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateOrganizationType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateOrganizationCommand>();
                                var command = (UpdateOrganizationCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command, CustomerModule.Core.ModuleConstants.Security.Permissions.Update);
                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(GraphTypeExtenstionHelper.GetActualType<OrganizationType>())
                            .Name("createOrganization")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateOrganizationType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<CreateOrganizationCommand>();
                                var command = (CreateOrganizationCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command);
                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(GraphTypeExtenstionHelper.GetActualType<ContactType>())
                            .Name("createContact")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateContactType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<CreateContactCommand>();
                                var command = (CreateContactCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command);

                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(GraphTypeExtenstionHelper.GetActualType<ContactType>())
                            .Name("updateContact")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateContactType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<UpdateContactCommand>();
                                var command = (UpdateContactCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command);
                                return await _mediator.Send(command);

                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, bool>(typeof(BooleanGraphType))
                            .Name("deleteContact")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputDeleteContactType>>(), _commandName)
                            .ResolveAsync(async context =>
                            {
                                var type = GenericTypeHelper.GetActualType<DeleteContactCommand>();
                                var command = (DeleteContactCommand)context.GetArgument(type, _commandName);
                                await CheckAuthAsync(context.GetCurrentUserId(), command, CustomerModule.Core.ModuleConstants.Security.Permissions.Delete);
                                return await _mediator.Send(command);
                            })
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(GraphTypeExtenstionHelper.GetActualType<IdentityResultType>())
                          .Name("updatePersonalData")
                          .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdatePersonalDataType>>(), _commandName)
                          .ResolveAsync(async context =>
                          {
                              var type = GenericTypeHelper.GetActualType<UpdatePersonalDataCommand>();
                              var command = (UpdatePersonalDataCommand)context.GetArgument(type, _commandName);
                              await CheckAuthAsync(context.GetCurrentUserId(), command);
                              return await _mediator.Send(command);
                          })
                          .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IMemberAggregateRoot>(GraphTypeExtenstionHelper.GetActualType<MemberType>())
                        .Name("updateMemberDynamicProperties")
                        .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateMemberDynamicPropertiesType>>(), _commandName)
                        .ResolveAsync(async context =>
                        {
                            var type = GenericTypeHelper.GetActualType<UpdateMemberDynamicPropertiesCommand>();
                            var command = (UpdateMemberDynamicPropertiesCommand)context.GetArgument(type, _commandName);
                            await CheckAuthAsync(context.GetCurrentUserId(), command, CustomerModule.Core.ModuleConstants.Security.Permissions.Update);

                            return await _mediator.Send(command);
                        })
                        .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                        .Name("sendVerifyEmail")
                        .Argument(GraphTypeExtenstionHelper.GetActualType<InputSendVerifyEmailType>(), _commandName)
                        .ResolveAsync(async context =>
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
                        .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResultResponse>(GraphTypeExtenstionHelper.GetActualType<CustomIdentityResultType>())
                .Name("resetPasswordByToken")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<InputResetPasswordByTokenType>(), _commandName)
                .ResolveAsync(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<ResetPasswordByTokenCommand>();
                    var command = (ResetPasswordByTokenCommand)context.GetArgument(type, _commandName);

                    return await _mediator.Send(command);
                })
                .FieldType);

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

            #region invite user

#pragma warning disable S125 // Sections of code should not be commented out
            /*
            mutation ($command: InputInviteUserType!){
                inviteUser(command: $command){ succeeded errors { code }}
            }
            Query variables:
            {
                "command": {
                    "storeId": "my-store",
                    "organizationId": "my-org",
                    "urlSuffix": "/invite",
                    "email": "example@example.org",
                    "message": "Message"
                }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResultResponse>(GraphTypeExtenstionHelper.GetActualType<CustomIdentityResultType>())
                .Name("inviteUser")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputInviteUserType>>(), _commandName)
                .ResolveAsync(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<InviteUserCommand>();
                    var command = (InviteUserCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
                .FieldType);

            #region register by invitation

#pragma warning disable S125 // Sections of code should not be commented out
            /*
            mutation ($command: InputRegisterByInvitationType!){
                registerByInvitation(command: $command){ succeeded errors { code }}
            }
            Query variables:
            {
                "command": {
                    "userId": "my-user",
                    "token": "large-unique-token",
                    "firstName": "John",
                    "lastName": "Smith",
                    "userName": "johnsmith",
                    "password": "password1!"
                }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResultResponse>(GraphTypeExtenstionHelper.GetActualType<CustomIdentityResultType>())
                .Name("registerByInvitation")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputRegisterByInvitationType>>(), _commandName)
                .ResolveAsync(async context =>
                {
                    var type = GenericTypeHelper.GetActualType<RegisterByInvitationCommand>();
                    var command = (RegisterByInvitationCommand)context.GetArgument(type, _commandName);
                    await CheckAuthAsync(context.GetCurrentUserId(), command);
                    return await _mediator.Send(command);
                })
                .FieldType);

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
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(GraphTypeExtenstionHelper.GetActualType<IdentityResultType>())
                        .Name("createUser")
                        .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputCreateUserType>>(), _commandName)
                        .ResolveAsync(async context =>
                        {
                            var type = GenericTypeHelper.GetActualType<CreateUserCommand>();
                            var command = (CreateUserCommand)context.GetArgument(type, _commandName);
                            await CheckAuthAsync(context.GetCurrentUserId(), command);
                            return await _mediator.Send(command);
                        })
                        .FieldType);

            #region update user

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         mutation ($command: InputUpdateUserType!){
                          updateUser(command: $command){ succeeded errors { description } }
                        }
                        Query variables:
                        {
                         "command":{
                          "securityStamp": ...,
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
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(GraphTypeExtenstionHelper.GetActualType<IdentityResultType>())
                        .Name("updateUser")
                        .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateUserType>>(), _commandName)
                        .ResolveAsync(async context =>
                        {
                            var type = GenericTypeHelper.GetActualType<UpdateUserCommand>();
                            var command = (UpdateUserCommand)context.GetArgument(type, _commandName);
                            await CheckAuthAsync(context.GetCurrentUserId(), command);
                            return await _mediator.Send(command);
                        })
                        .FieldType);

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
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(GraphTypeExtenstionHelper.GetActualType<IdentityResultType>())
                        .Name("deleteUsers")
                        .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputDeleteUserType>>(), _commandName)
                        .ResolveAsync(async context =>
                        {
                            var type = GenericTypeHelper.GetActualType<DeleteUserCommand>();
                            var command = (DeleteUserCommand)context.GetArgument(type, _commandName);
                            await CheckAuthAsync(context.GetCurrentUserId(), command, PlatformConstants.Security.Permissions.SecurityDelete);
                            return await _mediator.Send(command);
                        })
                        .FieldType);

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
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(GraphTypeExtenstionHelper.GetActualType<IdentityResultType>())
                     .Name("updateRole")
                     .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputUpdateRoleType>>(), _commandName)
                     .ResolveAsync(async context =>
                     {
                         var type = GenericTypeHelper.GetActualType<UpdateRoleCommand>();
                         var command = (UpdateRoleCommand)context.GetArgument(type, _commandName);
                         await CheckAuthAsync(context.GetCurrentUserId(), command, PlatformConstants.Security.Permissions.SecurityUpdate);

                         return await _mediator.Send(command);
                     })
                     .FieldType);
        }

        // PT-1654: Fix Authentication
        private async Task CheckAuthAsync(string userId, object resource, params string[] permissions)
        {
            var signInManager = _signInManagerFactory();

            var user = await signInManager.UserManager.FindByIdAsync(userId) ?? new ApplicationUser
            {
                Id = userId,
                UserName = Core.AnonymousUser.UserName,
            };

            var userPrincipal = await signInManager.CreateUserPrincipalAsync(user);

            if (!await CanExecuteWithoutPermissionAsync(user, resource) && !permissions.IsNullOrEmpty())
            {
                foreach (var permission in permissions)
                {
                    var permissionAuthorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, null, new PermissionAuthorizationRequirement(permission));
                    if (!permissionAuthorizationResult.Succeeded)
                    {
                        throw new AuthorizationError($"User doesn't have the required permission '{permission}'.");
                    }
                }
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(userPrincipal, resource, new ProfileAuthorizationRequirement());

            if (!authorizationResult.Succeeded)
            {
                throw new AuthorizationError($"Access denied");
            }
        }

        private async Task<bool> CanExecuteWithoutPermissionAsync(ApplicationUser user, object resource)
        {
            var result = false;

            if (resource is UpdateMemberDynamicPropertiesCommand updateMemberDynamicPropertiesCommand)
            {
                result = updateMemberDynamicPropertiesCommand.MemberId == user.MemberId;
            }
            else if (resource is UpdateOrganizationCommand updateOrganizationCommand && !string.IsNullOrEmpty(user.MemberId))
            {
                var member = await _memberService.GetByIdAsync(user.MemberId) as Contact;
                result = member?.Organizations.Any(x => x.EqualsInvariant(updateOrganizationCommand.Id)) ?? false;
            }

            return result;
        }

        private async Task<string> GetUserEmailAsync(string userId)
        {
            var signInManager = _signInManagerFactory();

            var user = await signInManager.UserManager.FindByIdAsync(userId);

            return user?.Email;
        }
    }
}
