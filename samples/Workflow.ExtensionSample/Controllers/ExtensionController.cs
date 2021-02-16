using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WorkflowExtension.Model;
using WorkflowExtension.Queries;

namespace WorkflowExtension
{
    [Route("api")]
    public class ExtensionServiceController : Controller
    {
        [HttpPost]
        [Route("load-product-inventories")]
        public ActionResult<GetMyCoolProductsResponse> LoadProductInventories([FromBody] GeMyCoolProductsQuery query)
        {
            var result = query.Ids.Select(x => new MyCoolProduct { Id = x, InStockQty = 10 }).ToArray();
            return Json(new GetMyCoolProductsResponse { Products = result });
        }      
    }
}
