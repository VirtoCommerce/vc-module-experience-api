using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Handlers
{
    public class LoadProductsRequestHandler : IRequestHandler<LoadProductRequest, LoadProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
 
        public LoadProductsRequestHandler(ISearchProvider searchProvider, IMapper mapper)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductRequest request, CancellationToken cancellationToken)
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
