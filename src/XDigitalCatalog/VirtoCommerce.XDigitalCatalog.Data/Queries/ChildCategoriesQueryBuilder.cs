using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Core.Queries;
using VirtoCommerce.XDigitalCatalog.Core.Types;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Data.Queries;

public class ChildCategoriesQueryBuilder : CatalogQueryBuilder<ChildCategoriesQuery, ChildCategoriesQueryResponse, ChildCategoriesQueryResponseType>
{
    private const int _batchSize = 100;
    private readonly ICategoryService _categoryService;

    protected override string Name => "ChildCategories";

    public ChildCategoriesQueryBuilder(
        IMediator mediator,
        IAuthorizationService authorizationService,
        IStoreService storeService,
        ICurrencyService currencyService,
        ICategoryService categoryService)
        : base(mediator, authorizationService, storeService, currencyService)
    {
        _categoryService = categoryService;
    }


    protected override async Task AfterMediatorSend(IResolveFieldContext<object> context, ChildCategoriesQuery request, ChildCategoriesQueryResponse response)
    {
        await base.AfterMediatorSend(context, request, response);

        var categoryIds = new HashSet<string>();
        var root = new ExpCategory { ChildCategories = response.ChildCategories };

        foreach (var category in root.Traverse(x => x.ChildCategories).Where(x => x.Key != null))
        {
            categoryIds.Add(category.Key);
        }

        if (categoryIds.Any())
        {
            var responseGroup = GetCategoryResponseGroup(context, request, response);
            var categoriesByIds = new Dictionary<string, Category>();

            foreach (var idsBatch in categoryIds.Paginate(_batchSize))
            {
                var categories = await _categoryService.GetAsync(idsBatch, responseGroup);
                categoriesByIds.AddRange(categories.ToDictionary(x => x.Id));
            }

            foreach (var category in root.Traverse(x => x.ChildCategories))
            {
                category.Category ??= categoriesByIds.GetValueSafe(category.Key);
            }
        }
    }

    protected virtual string GetCategoryResponseGroup(IResolveFieldContext<object> context, ChildCategoriesQuery request, ChildCategoriesQueryResponse response)
    {
        var searchCategoryQuery = context.GetCatalogQuery<SearchCategoryQuery>();
        searchCategoryQuery.IncludeFields = request.IncludeFields;

        return searchCategoryQuery.GetCategoryResponseGroup();
    }
}
