using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQueryHandler :
        IQueryHandler<SearchCategoryQuery, SearchCategoryResponse>,
        IQueryHandler<LoadCategoryQuery, LoadCategoryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _phraseParser;
        private readonly ICrudService<Store> _storeService;
        private readonly IGenericPipelineLauncher _pipeline;
        private readonly IMediator _mediator;

        public SearchCategoryQueryHandler(
            ISearchProvider searchProvider,
            IMapper mapper,
            ISearchPhraseParser phraseParser,
            IStoreService storeService,
            IGenericPipelineLauncher pipeline,
            IMediator mediator)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _phraseParser = phraseParser;
            _storeService = (ICrudService<Store>)storeService;
            _pipeline = pipeline;
            _mediator = mediator;
        }

        public virtual async Task<SearchCategoryResponse> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
        {
            var essentialTerms = new List<string>();
            var store = await GetStore(request);

            essentialTerms.Add($"__outline:{store.Catalog}");

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

            if (request.GetLoadChildCategories())
            {
                var childCategoriesQuery = new ChildCategoriesQuery
                {
                    OnlyActive = true,
                    Store = store,
                    StoreId = request.StoreId,
                    UserId = request.UserId,
                    CultureName = request.CultureName,
                    CurrencyCode = request.CurrencyCode,
                };

                var childCategoriesSearchQuery = new SearchCategoryQuery
                {
                    Store = store,
                    StoreId = request.StoreId,
                    UserId = request.UserId,
                    CultureName = request.CultureName,
                    CurrencyCode = request.CurrencyCode,
                };

                var regex = new Regex("^childCategories\\.");

                foreach (var expCategory in result.Results)
                {
                    if (expCategory.ChildCategories != null)
                    {
                        continue;
                    }

                    childCategoriesQuery.CategoryId = expCategory.Id;
                    childCategoriesQuery.MaxLevel = expCategory.Level;

                    var response = await _mediator.Send(childCategoriesQuery, cancellationToken);
                    var categoryIds = response.ChildCategories.Select(x => x.Key).ToArray();

                    if (categoryIds.IsNullOrEmpty())
                    {
                        continue;
                    }

                    childCategoriesSearchQuery.ObjectIds = categoryIds;
                    childCategoriesSearchQuery.Take = categoryIds.Length;
                    childCategoriesSearchQuery.IncludeFields = request.IncludeFields.Where(x => x.StartsWith("childCategories.")).Select(x => regex.Replace(x, string.Empty, 1)).ToList();

                    var childCategories = await Handle(childCategoriesSearchQuery, cancellationToken);

                    expCategory.ChildCategories = childCategories.Results.ToList();
                }
            }

            return result;
        }

        public virtual async Task<LoadCategoryResponse> Handle(LoadCategoryQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = _mapper.Map<SearchCategoryQuery>(request);
            searchRequest.Store = await GetStore(request);

            var result = await Handle(searchRequest, cancellationToken);

            return new LoadCategoryResponse(result.Results);
        }

        private async Task<Store> GetStore<T>(CatalogQueryBase<T> request)
        {
            var store = request.Store;

            if (store is null && !string.IsNullOrWhiteSpace(request.StoreId))
            {
                store = await _storeService.GetByIdAsync(request.StoreId);
                if (store == null)
                {
                    throw new ArgumentException($"Store with Id: {request.StoreId} is absent");
                }
            }

            return store;
        }
    }
}
