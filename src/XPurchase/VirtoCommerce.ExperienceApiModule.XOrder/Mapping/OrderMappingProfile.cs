using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<IList<IFilter>, CustomerOrderSearchCriteria>()
                .ConvertUsing((terms, criteria, context) =>
                {
                    foreach (var term in terms.OfType<TermFilter>())
                    {
                        term.MapTo(criteria);
                    }
                    return criteria;
                });
        }
    }
}
