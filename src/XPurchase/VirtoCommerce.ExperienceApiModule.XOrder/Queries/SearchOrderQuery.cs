using System;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQuery : IQuery<SearchOrderResponse>
    {
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }

        //public bool WithPrototypes { get; set; }
        //public bool OnlyRecurring { get; set; }
        //public string SubscriptionId { get; set; }
        //public string[] SubscriptionIds { get; set; }
        //public string OperationId { get; set; }
        //public string CustomerId { get; set; }
        //public string[] CustomerIds { get; set; }

        //public string[] Ids { get; set; }
        //public string EmployeeId { get; set; }
        //public string[] StoreIds { get; set; }
        //public string Status { get; set; }
        //public string[] Statuses { get; set; }
        //public string Number { get; set; }
        //public string[] Numbers { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
    }
}
