using GraphQL.Server;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XCMS.Core;

namespace VirtoCommerce.XCMS.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXCMS(this IServiceCollection services, IGraphQLBuilder graphQlbuilder)
        {
            graphQlbuilder.AddSchema(typeof(XCMSCoreAssemblyMarker), typeof(XCMSDataAssemblyMarker));

            return services;
        }
    }
}
