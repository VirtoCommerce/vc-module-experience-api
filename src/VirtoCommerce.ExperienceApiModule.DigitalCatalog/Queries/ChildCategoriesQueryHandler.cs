using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQueryHandler : IQueryHandler<ChildCategoriesQuery, ChildCategoriesQueryResponse>
{
    private readonly IStoreService _storeService;
    private readonly ICategoryTreeService _categoryTreeService;

    public ChildCategoriesQueryHandler(IStoreService storeService, ICategoryTreeService categoryTreeService)
    {
        _storeService = storeService;
        _categoryTreeService = categoryTreeService;
    }

    public async Task<ChildCategoriesQueryResponse> Handle(ChildCategoriesQuery request, CancellationToken cancellationToken)
    {
        var store = await _storeService.GetByIdAsync(request.StoreId);

        if (store is null)
        {
            return new ChildCategoriesQueryResponse();
        }

        var level = request.MaxLevel;
        var root = new ExpCategory { Key = request.CategoryId };
        var parents = new List<ExpCategory> { root };

        while (level > 0)
        {
            var parentIds = parents.Select(x => x.Key).ToList();
            var parentNodes = await _categoryTreeService.GetNodesWithChildren(store.Catalog, parentIds, request.OnlyActive);

            foreach (var parent in parents)
            {
                var parentNode = parentNodes.FirstOrDefault(x => x.Id == parent.Key);
                parent.ChildCategories = parentNode?.ChildIds.Select(id => new ExpCategory { Key = id }).ToArray() ?? Array.Empty<ExpCategory>();
            }

            parents = parents.SelectMany(x => x.ChildCategories).ToList();
            level--;
        }

        return new ChildCategoriesQueryResponse
        {
            ChildCategories = root.ChildCategories ?? Array.Empty<ExpCategory>(),
        };
    }
}
