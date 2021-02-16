using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowExtension.Model;

namespace WorkflowExtension.Queries
{
    public class GetMyCoolProductsResponse
    {
        public IList<MyCoolProduct> Products { get; set; }
    }
}
