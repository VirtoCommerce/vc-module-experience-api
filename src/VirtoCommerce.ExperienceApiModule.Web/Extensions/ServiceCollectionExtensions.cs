using GraphQL;
using GraphQL.Caching;
using GraphQL.Execution;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Enable document (query body string) caching
        /// https://web.archive.org/web/20210927012540/https://graphql-dotnet.github.io/docs/guides/document-caching/
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDocumentCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<MemoryDocumentCacheOptions>().Bind(configuration.GetSection("VirtoCommerce:GraphQL:DocumentCache")).ValidateDataAnnotations();
            services.AddSingleton<IDocumentBuilder, GraphQLDocumentBuilder>();
            services.AddSingleton<IDocumentValidator, DocumentValidator>();
            services.AddSingleton<IComplexityAnalyzer, ComplexityAnalyzer>();
            services.AddSingleton<IDocumentCache, MemoryDocumentCache>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        }
    }
}
