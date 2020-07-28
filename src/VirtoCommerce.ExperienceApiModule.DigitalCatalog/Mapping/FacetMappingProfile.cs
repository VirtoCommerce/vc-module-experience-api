using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AggregationRequest, FacetResult>().IncludeAllDerived();

            CreateMap<TermAggregationRequest, TermFacetResult>().ConvertUsing((request, facet, context) =>
            {
                var aggregations = context.Items["aggregations"] as IList<AggregationResponse>;
                var aggregation = aggregations.FirstOrDefault(x => x.Id.EqualsInvariant(request.Id) || x.Id.EqualsInvariant(request.FieldName));
                if (aggregation != null)
                {
                    return new TermFacetResult
                    {
                        Name = request.Id ?? request.FieldName,
                        Terms = aggregation.Values.Select(x => new FacetTerm { Count = x.Count, Term = x.Id }).ToList()
                    };
                }
                return null;
            });
            CreateMap<RangeAggregationRequest, RangeFacetResult>().ConvertUsing((request, facet, context) =>
            {
                var aggregations = context.Items["aggregations"] as IList<AggregationResponse>;
                var aggregation = aggregations.FirstOrDefault(x => x.Id.EqualsInvariant(request.Id) || x.Id.EqualsInvariant(request.FieldName));
                var result = new RangeFacetResult
                {
                    Name = request.Id ?? request.FieldName
                };
                foreach (var aggrValue in aggregation.Values)
                {
                    var aggrRequestValue = request.Values.FirstOrDefault(x => ((x.Lower ?? "*") + "-" + (x.Upper ?? "*")).EqualsInvariant(aggrValue.Id));
                    if (aggrRequestValue != null)
                    {
                        result.Ranges.Add(new FacetRange
                        {
                            Count = aggrValue.Count,
                            From = Convert.ToInt64(aggrRequestValue.Lower ?? "0"),
                            IncludeFrom = aggrRequestValue.IncludeLower,
                            FromStr = aggrRequestValue.Lower ?? string.Empty,
                            To = Convert.ToInt64(aggrRequestValue.Upper ?? "0"),
                            IncludeTo = aggrRequestValue.IncludeUpper,
                            ToStr = aggrRequestValue.Upper ?? string.Empty
                        });
                    }
                }
                return result;
            });
        }
    }
}
