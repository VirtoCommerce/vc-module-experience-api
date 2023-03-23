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

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadPropertyMetadata))
            {
                var propertyIds = parameter.Results
                    .SelectMany(x => x.IndexedProduct.Properties)
                    .Where(property => property.Id is not null)
                    .Select(property => property.Id)
                    .Distinct()
                    .ToList();

                if (propertyIds.Any())
                {
                    var properties = await _propertyService.GetByIdsAsync(propertyIds);
                    var propertiesDictionary = properties.ToDictionary(x => x.Id);

                    foreach (var productResult in parameter.Results)
                    {
                        foreach (var property in productResult.IndexedProduct.Properties.Where(x => x.Id is not null))
                        {
                            if (propertiesDictionary.TryGetValue(property.Id, out var loadedProperty))
                            {
                                property.Attributes = loadedProperty.Attributes;
                            }
                        }
                    }
                }

            }

            await next(parameter);
        }
    }
}
