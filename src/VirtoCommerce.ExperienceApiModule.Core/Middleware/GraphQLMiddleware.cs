using System.Linq;
using System.Threading.Tasks;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server.Transports.AspNetCore.Common;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;

namespace VirtoCommerce.ExperienceApiModule.Core.Middleware
{
    public class GraphQLMiddleware<TSchema> : GraphQLHttpMiddleware<TSchema>
        where TSchema : ISchema
    {
        public GraphQLMiddleware(
            RequestDelegate next,
            PathString path,
            IGraphQLRequestDeserializer requestDeserializer)
            : base(next, path, requestDeserializer)
        {
        }

        protected override Task RequestExecutedAsync(in GraphQLRequestExecutionResult requestExecutionResult)
        {
            var authError = requestExecutionResult.Result?.Errors?.FirstOrDefault(x => x is AuthorizationError);
            if (authError != null)
            {
                throw authError;
            }

            return base.RequestExecutedAsync(requestExecutionResult);
        }
    }
}
