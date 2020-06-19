using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;

        public ProfileSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
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
        }

        public static async Task<IDictionary<string, Profile>> LoadProfileAsync(IMediator mediator, IEnumerable<string> ids, IEnumerable<string> includeFields)
        {
            var response = await mediator.Send(new LoadProfileRequest { Id = ids.FirstOrDefault(), IncludeFields = includeFields });
            return response.Results;
        }
    }
}
