using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WorkflowExtension.Model;
using WorkflowExtension.Queries;

namespace WorkflowExtension
{
    [Route("api")]
    public class ServiceController : Controller
    {
        private readonly IMediator _mediator;
        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("products")]
        public async Task<ActionResult<MyCoolProduct[]>> GetProducts()
        {
            var response = await _mediator.Send(new GeMyCoolProductsQuery { Ids = new string[] { "1", "2", "3" } });
            return Json(response);
        }

    }
}
