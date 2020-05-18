using System.Linq;
using System.Threading.Tasks;
using PetsStoreClient;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    //public class PetsProductSearchService : IProductSearchService
    //{
    //    private readonly IPetsSearchService _petsSearchService;
    //    public PetsProductSearchService(IPetsSearchService petsSearchService)
    //    {
    //        _petsSearchService = petsSearchService;
    //    }
        
    //    public async Task<ProductSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
    //    {
    //        var petsQuery = new SearchPetsQuery
    //        {
    //            Keyword = criteria.Keyword,
    //            Skip = criteria.Skip,
    //            Take = criteria.Take
    //        };
    //        var searchResult = await _petsSearchService.SearchPetsAsync(petsQuery);
    //        var result = new ProductSearchResult
    //        {
    //            Results = searchResult.Pets.Select(PetToProduct).ToList(),
    //            TotalCount = searchResult.TotalCount
    //        };
    //        return result;
    //    }

    //    private static ExpProduct PetToProduct(Pet pet)
    //    {
    //        var petProduct = AbstractTypeFactory<ExpProduct>.TryCreateInstance();
    //        petProduct.CatalogProduct = new CatalogProduct
    //        {
    //            Id = pet.Id.ToString(),
    //            Name = pet.Name,
    //            ProductType = "Pet",
    //            OuterId = "PetStore"
    //        };
    //        return petProduct;
    //    }
    //}
}
