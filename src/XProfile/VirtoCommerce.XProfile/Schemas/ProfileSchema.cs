using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
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
            var customerField = new FieldType
            {
                Name = "customer",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "user id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ProfileType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var loader = _dataLoader.Context.GetOrAddBatchLoader<string, Profile>("profileLoader", (id) => LoadProfileAsync(_mediator, id, context.SubFields.Values.GetAllNodesPaths()));
                    return await loader.LoadAsync(context.GetArgument<string>("id"));
                })
            };
            schema.Query.AddField(customerField);

            schema.Mutation.AddField(FieldBuilder.Create<UserUpdateInfo, bool>(typeof(BooleanGraphType))
                            .Name("updateAccount")
                            .Argument<NonNullGraphType<UserUpdateInfoInputType>>("userUpdateInfo")
                            .ResolveAsync(async context =>
                            {
                                await _memberService.UpdateContactAsync(context.GetArgument<UserUpdateInfo>("userUpdateInfo"));
                                return true;
                            }).FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<IList<string>, string>(typeof(StringGraphType))
                            .Name("testSimple")
                            .Argument<NonNullGraphType<StringGraphType>>("customerId")
                            .Resolve(context =>
                            {
                                return context.GetArgument<string>("customerId");
                            }).FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<IList<string>, string>(typeof(StringGraphType))
                            .Name("testList")
                            .Argument<ListGraphType<StringGraphType>>("items")
                            .Resolve(context =>
                            {
                                var items = context.GetArgument<IList<string>>("items");
                                return items.FirstOrDefault();
                            }).FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<IList<Address>, Contact>(GraphTypeExtenstionHelper.GetActualType<ContactType>())
                            .Name("updateAddresses")
                            .Argument<NonNullGraphType<StringGraphType>>("customerId")
                            .Argument<ListGraphType<AddressInputType>>("addresses")
                            .ResolveAsync(async context =>
                            {
                                return await _memberService.UpdateContactAddressesAsync(
                                             context.GetArgument<string>("customerId"),
                                             context.GetArgument<IList<Address>>("addresses"));
                            }).FieldType);
        }

        public static async Task<IDictionary<string, Profile>> LoadProfileAsync(IMediator mediator, IEnumerable<string> ids, IEnumerable<string> includeFields)
        {
            var response = await mediator.Send(new LoadProfileRequest { Id = ids.FirstOrDefault(), IncludeFields = includeFields });
            return response.Results;
        }
    }
}
