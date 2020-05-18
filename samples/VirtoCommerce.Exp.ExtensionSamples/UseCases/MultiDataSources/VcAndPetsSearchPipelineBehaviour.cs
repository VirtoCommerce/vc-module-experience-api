using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using PetsStoreClient;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class VcAndPetsSearchPipelineBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly IPetsSearchService _petsSearchService;
        public VcAndPetsSearchPipelineBehaviour(IPetsSearchService petsSearchService)
        {
            _petsSearchService = petsSearchService;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (request is LoadProductRequest loadProductRequest && response is LoadProductResponse loadProductResponse)
            {
                var notLoadedProductIds = loadProductRequest.Ids.Except(loadProductResponse.Products.Select(x => x.Id)).Where(x => long.TryParse(x, out var _));
                if (notLoadedProductIds.Any())
                {
                    var petProducts = new List<ExpProduct>();
                    foreach (var id in notLoadedProductIds)
                    {
                        var pet = await _petsSearchService.LoadByIdAsync(long.Parse(id));
                        if (pet != null)
                        {
                            petProducts.Add(PetToProduct(pet));
                        }
                    }
                    loadProductResponse.Products = loadProductResponse.Products.Concat(petProducts).ToArray();
                }

            }
            else if (request is SearchProductRequest searchProductRequest && response is SearchProductResponse searchProductResponse)
            {
                var totalCount = searchProductResponse.TotalCount;
                var skip = Math.Min(totalCount, searchProductRequest.Skip);
                var take = Math.Min(searchProductRequest.Take, Math.Max(0, totalCount - searchProductRequest.Skip));

                var searchPetsQuery = new SearchPetsQuery
                {
                    Skip = searchProductRequest.Skip,
                    Take = searchProductRequest.Take
                };

                searchPetsQuery.Skip -= skip;
                searchPetsQuery.Take -= take;

                var petsResult = await _petsSearchService.SearchPetsAsync(searchPetsQuery);
                searchProductResponse.Results.AddRange(petsResult.Pets.Select(PetToProduct));
                searchProductResponse.TotalCount += petsResult.TotalCount;
            }
        }


        private static ExpProduct PetToProduct(Pet pet)
        {
            var petProduct = AbstractTypeFactory<ExpProduct>.TryCreateInstance();
            petProduct.CatalogProduct = new CatalogProduct
            {
                Id = pet.Id.ToString(),
                Name = pet.Name,
                ProductType = "Pet",
                OuterId = "PetStore"
            };
            return petProduct;
        }
    }

}
