using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.XDigitalCatalog;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductsQueryHandler : IQueryHandler<LoadProductQuery, LoadProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;

        public LoadProductsQueryHandler(ISearchProvider searchProvider, IMapper mapper)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductQuery request, CancellationToken cancellationToken)
        {
            var result = new LoadProductResponse();
            var searchRequest = new SearchRequestBuilder()
                                            .WithPaging(0, request.Ids.Count())
                                            .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                                            .WithIncludeFields(request.IncludeFields.Where(x => x.StartsWith("prices.")).Concat(new[] { "id" }).Select(x => "__prices." + x.TrimStart("prices.")).ToArray())
                                            .AddObjectIds(request.Ids)
                                            .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            result.Products = searchResult.Documents.Select(x => _mapper.Map<ExpProduct>(x)).ToList();

            return result;
        }
    }
}
