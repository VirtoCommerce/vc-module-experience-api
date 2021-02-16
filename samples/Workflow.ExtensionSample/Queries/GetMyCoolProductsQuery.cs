using System.Collections.Generic;
using MediatR;
using WorkflowExtension.Model;

namespace WorkflowExtension.Queries
{
    public class GeMyCoolProductsQuery : IRequest<GetMyCoolProductsResponse>
    {
        public IEnumerable<string> Ids { get; set; }
    }
}
