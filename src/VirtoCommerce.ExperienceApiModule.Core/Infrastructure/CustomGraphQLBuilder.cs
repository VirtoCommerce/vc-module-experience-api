using GraphQL.Server;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// Custom implementation of GraphQLBuilder to call GraphQL.Server extentions methods
    /// </summary>
    public sealed class CustomGraphQLBuilder : IGraphQLBuilder
    {
        public IServiceCollection Services { get; }

        public CustomGraphQLBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
