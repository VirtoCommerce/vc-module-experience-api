using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EnsurePropertyMetadataLoadedMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IPropertyService _propertyService;

        public EnsurePropertyMetadataLoadedMiddleware(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.Query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            return RunInternal(parameter, next);
        }

        private async Task RunInternal(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            var responseGroup = EnumUtility.SafeParse(parameter.Query.GetResponseGroup(), ExpProductResponseGroup.None);
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadPropertyMetadata))
            {
                var productProperties = parameter.Results
                    .Where(x => x.IndexedProduct is not null && x.IndexedProduct.Properties is not null)
                    .SelectMany(x => x.IndexedProduct.Properties)
                    .Where(property => property?.Id is not null)
                    .ToArray();
                var propertyIds = productProperties.Select(x => x.Id).Distinct().ToArray();

                if (propertyIds.Any())
                {
                    var properties = (await _propertyService.GetByIdsAsync(propertyIds)).ToDictionary(x => x.Id);

                    foreach (var property in productProperties)
                    {
                        if (!properties.TryGetValue(property.Id, out var loadedProperty))
                        {
                            continue;
                        }

                        var isInherited = property.IsInherited;
                        property.TryInheritFrom(loadedProperty);
                        property.IsInherited = isInherited;
                    }
                }

            }

            await next(parameter);
        }
    }
}
