using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQueryHandler :
        IQueryHandler<SearchCategoryQuery, SearchCategoryResponse>
        , IQueryHandler<LoadCategoryQuery, LoadCategoryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IStoreService _storeService;
        private readonly IGenericPipelineLauncher _pipeline;

        public SearchCategoryQueryHandler(
            ISearchProvider searchProvider
            , IMapper mapper
            , ISearchPhraseParser phraseParser
            , IStoreService storeService
            , IGenericPipelineLauncher pipeline)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _phraseParser = phraseParser;
            _storeService = storeService;
            _pipeline = pipeline;
        }

        public virtual async Task<SearchCategoryResponse> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
        {
            var essentialTerms = new List<string>();
            Store store = null;

            if (!string.IsNullOrWhiteSpace(request.StoreId))
            {
                store = await _storeService.GetByIdAsync(request.StoreId);
                if (store == null)
                {
                    throw new ArgumentException($"Store with Id: {request.StoreId} is absent");
                }

                essentialTerms.Add($"__outline:{store.Catalog}");
            }

            var searchRequest = new IndexSearchRequestBuilder()
                                          .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                                          .ParseFilters(_phraseParser, request.Filter)
                                          .WithSearchPhrase(request.Query)
                                          .WithPaging(request.Skip, request.Take)
                                          .AddObjectIds(request.ObjectIds)
                                          .AddSorting(request.Sort)
                                          //Limit search result with store catalog
                                          .AddTerms(essentialTerms)
                                          .WithIncludeFields(IndexFieldsMapper.MapToIndexIncludes(request.IncludeFields).ToArray())
                                          .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Category, searchRequest);

            var categories = searchResult.Documents?.Select(x => _mapper.Map<ExpCategory>(x, options =>
            {
                options.Items["store"] = store;
                options.Items["cultureName"] = request.CultureName;
            })).ToList() ?? new List<ExpCategory>();

            var result = new SearchCategoryResponse
            {
                Query = request,
                Results = categories,
                Store = store,
                TotalCount = (int)searchResult.TotalCount
            };

            await _pipeline.Execute(result);

            return result;
        }

        public virtual async Task<LoadCategoryResponse> Handle(LoadCategoryQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = _mapper.Map<SearchCategoryQuery>(request);

            var result = await Handle(searchRequest, cancellationToken);

            return new LoadCategoryResponse(result.Results);
        }
    }
}
