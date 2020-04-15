using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Index
{

    public class ProductIndexBuilder : IIndexDocumentBuilder
    {
        private readonly IItemService _itemService;
        public ProductIndexBuilder(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var result = new List<IndexDocument>();
            var products = await _itemService.GetByIdsAsync(documentIds.ToArray(), null);
            foreach (var product in products)
            {
                var document = new IndexDocument(product.Id);

                document.Add(new IndexDocumentField("product_src", product) { IsRetrievable = false, IsFilterable = false, IsCollection = false });

                result.Add(document);

            }
            return await Task.FromResult(result);
        }
    }
}
