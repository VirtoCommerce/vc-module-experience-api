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
                        object value = term.Values;
                        if (value != null)
                        {
                            if (propertyInfo.PropertyType.IsArray)
                            {
                                var elementType = propertyInfo.PropertyType.GetElementType();
                                var actualValues = Array.CreateInstance(elementType, term.Values.Count);
                                for (var i = 0; i < term.Values.Count; i++)
                                {
                                    actualValues.SetValue(term.Values[i].ChangeType(elementType), i);
                                }
                                value = actualValues;
                            }
                            else
                            {
                                value = term.Values.FirstOrDefault().ChangeType(propertyInfo.PropertyType);
                            }
                        }
                        propertyInfo.SetValue(criteria, value, null);
                    }

                    return criteria;
                });
        }
    }
}
