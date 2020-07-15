using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GraphQL;
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
                        var property = criteria.GetType().GetProperty(term.FieldName.ToPascalCase());
                        if (property.PropertyType.IsArray)
                        {
                            property.SetValue(criteria, term.Values);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(criteria, term.Values.FirstOrDefault());
                        }
                        else if (property.PropertyType == typeof(bool) && bool.TryParse(term.Values.FirstOrDefault(), out var boolValue))
                        {
                            property.SetValue(criteria, boolValue);
                        }
                        else if (property.PropertyType == typeof(DateTime?))
                        {
                            var dateValue = term.Values.FirstOrDefault();
                            property.SetValue(criteria, string.IsNullOrEmpty(dateValue) ? (DateTime?)null : DateTime.Parse(dateValue));
                        }
                    }

                    return criteria;
                });
        }
    }
}
