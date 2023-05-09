using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQueryHandler : IQueryHandler<ChildCategoriesQuery, ChildCategoriesQueryResponse>
{
    private readonly ICategoryTreeService _categoryTreeService;

    private readonly IMapper _mapper;
    private readonly ISearchProvider _searchProvider;
    private readonly IStoreCurrencyResolver _storeCurrencyResolver;
    private readonly ICrudService<Store> _storeService;
    private readonly IGenericPipelineLauncher _pipeline;
    private readonly IAggregationConverter _aggregationConverter;
    private readonly ISearchPhraseParser _phraseParser;
    private readonly IMediator _mediator;

    public ChildCategoriesQueryHandler(
        ICategoryTreeService categoryTreeService,

        ISearchProvider searchProvider,
        IMapper mapper,
        IStoreCurrencyResolver storeCurrencyResolver,
        ICrudService<Store> storeService,
        IGenericPipelineLauncher pipeline,
        IAggregationConverter aggregationConverter,
        ISearchPhraseParser phraseParser,
        IMediator mediator)
    {
        _categoryTreeService = categoryTreeService;

        _searchProvider = searchProvider;
        _mapper = mapper;
        _storeCurrencyResolver = storeCurrencyResolver;
        _storeService = storeService;
        _pipeline = pipeline;
        _aggregationConverter = aggregationConverter;
        _phraseParser = phraseParser;
        _mediator = mediator;
    }

    public virtual async Task<ChildCategoriesQueryResponse> Handle(ChildCategoriesQuery request, CancellationToken cancellationToken)
    {
        var result = AbstractTypeFactory<ChildCategoriesQueryResponse>.TryCreateInstance();

        if (request.Store is null)
        {
            return result;
        }

        var level = request.MaxLevel;
        var root = new ExpCategory { Key = request.CategoryId };
        var parents = new List<ExpCategory> { root };

        while (level > 0)
        {
            var parentIds = parents.Select(x => x.Key).ToList();
            var parentNodes = await _categoryTreeService.GetNodesWithChildren(request.Store.Catalog, parentIds, request.OnlyActive);

            foreach (var parent in parents)
            {
                var parentNode = parentNodes.FirstOrDefault(x => x.Id == parent.Key);
                parent.ChildCategories = parentNode?.ChildIds.Select(id => new ExpCategory { Key = id }).ToList() ?? new List<ExpCategory>();
            }

            parents = parents.SelectMany(x => x.ChildCategories).ToList();
            level--;
        }

        result.ChildCategories = root.ChildCategories ?? new List<ExpCategory>();

        // try resolve products via facets
        if (!string.IsNullOrEmpty(request.ProductFilter))
        {
            var outlineIds = await GetProductOutlineIds(request);
            if (outlineIds.Any())
            {
                FilterChildCategories(result.ChildCategories, outlineIds);
            }
            else
            {
                result.ChildCategories = new List<ExpCategory>();
            }
        }

        return result;
    }

    private void FilterChildCategories(IList<ExpCategory> categories, HashSet<string> outlines)
    {
        if (categories.IsNullOrEmpty())
        {
            return;
        }

        foreach (var category in categories.ToList())
        {
            if (!outlines.TryGetValue(category.Key, out var _))
            {
                categories.Remove(category);
            }
            else
            {
                FilterChildCategories(category.ChildCategories, outlines);
            }
        }
    }

    private async Task<HashSet<string>> GetProductOutlineIds(ChildCategoriesQuery childCategoriesQuery)
    {
        var result = new HashSet<string>();

        var productsRequest = new SearchProductQuery()
        {
            StoreId = childCategoriesQuery?.StoreId,
            CultureName = childCategoriesQuery?.CultureName,
            CurrencyCode = childCategoriesQuery?.CurrencyCode,
            UserId = childCategoriesQuery?.UserId ?? AnonymousUser.UserName,
            Filter = childCategoriesQuery.ProductFilter,
            Take = 0,
            Facet = "__outline",
            IncludeFields = new List<string>
            {
                "term_facets.name",
                "term_facets.label",
                "term_facets.terms.label",
                "term_facets.terms.term",
                "term_facets.terms.count"
            },
        };

        var productsResult = await _mediator.Send(productsRequest);

        var facetNames = new[] { "__outline" };
        var facets = productsResult.Facets.OfType<TermFacetResult>().Where(x => facetNames.Contains(x.Name));
        foreach (var facet in facets)
        {
            foreach (var term in facet.Terms)
            {
                var terms = term.Term.Split('/', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
                result.AddRange(terms);
            }
        }

        return result;
    }
}
