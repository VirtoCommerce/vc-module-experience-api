using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
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
                        var propertyInfo = criteria.GetType().GetProperty(term.FieldName.ToPascalCase());
                        propertyInfo.SetValue(criteria, term.Values.FirstOrDefault().ChangeType(propertyInfo.PropertyType), null);
                    }

                    return criteria;
                });
        }
    }
}
