using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using PetsStoreClient;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Contracts;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Data.UseCases.OnTheFly
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
                var notLoadedProductIds = loadProductRequest.Ids.Except(loadProductResponse.Products.Select(x => x.Id));
                if (notLoadedProductIds.Any())
                {
                    var petProducts = new List<CatalogProduct>();
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
            else if(request is SearchProductRequest searchProductRequest && response is SearchProductResponse searchProductResponse)
            {
                var searchProductResult = searchProductResponse.Result;

                var totalCount = searchProductResult.TotalCount;
                var skip = Math.Min(totalCount, searchProductRequest.Criteria.Skip);
                var take = Math.Min(searchProductRequest.Criteria.Take, Math.Max(0, totalCount - searchProductRequest.Criteria.Skip));

                var searchPetsQuery = new SearchPetsQuery
                {
                    Keyword = searchProductRequest.Criteria.Keyword,
                    Skip = searchProductRequest.Criteria.Skip,
                    Take = searchProductRequest.Criteria.Take
                };

                searchPetsQuery.Skip -= skip;
                searchPetsQuery.Take -= take;

                var petsResult = await _petsSearchService.SearchPetsAsync(searchPetsQuery);
                searchProductResponse.Result.Results.AddRange(petsResult.Pets.Select(PetToProduct));
                searchProductResponse.Result.TotalCount += petsResult.TotalCount;

            }
        }


        private static  CatalogProduct PetToProduct(Pet pet)
        {
            var petProduct = AbstractTypeFactory<CatalogProduct>.TryCreateInstance() as CatalogProduct2;
            petProduct.Id = pet.Id.ToString();
            petProduct.Name = pet.Name;
            petProduct.ProductType = "Pet";
            petProduct.DataSource = "PetStore";
            return petProduct;
        }
    }

}
