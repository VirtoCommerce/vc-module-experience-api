using System;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class FacetMappingProfile : Profile
    {
        public FacetMappingProfile()
        {
            CreateMap<Aggregation, FacetResult>().IncludeAllDerived();

            CreateMap<Aggregation, FacetResult>().ConvertUsing((request, facet, context) =>
            {
                context.Items.TryGetValue("cultureName", out var cultureNameObj);
                var cultureName = cultureNameObj as string;
                FacetResult result = request.AggregationType switch
                {
                    "attr" => new TermFacetResult
                    {
                        Terms = request.Items.Select(x => new FacetTerm
                        {
                            Count = x.Count,
                            IsSelected = x.IsApplied,
                            Term = x.Value.ToString(),

                            Label = x.Labels?.FirstOrDefault(x => x.Language == cultureName)?.Label ?? x.Value.ToString(),
                        })
                            .ToArray(),
                        Name = request.Field
                    },
                    "pricerange" => new RangeFacetResult
                    {
                        Ranges = request.Items.Select(x => new FacetRange
                        {
                            Count = x.Count,
                            From = Convert.ToInt64(x.RequestedLowerBound),
                            IncludeFrom = !string.IsNullOrEmpty(x.RequestedLowerBound),
                            FromStr = x.RequestedLowerBound,
                            To = Convert.ToInt64(x.RequestedUpperBound),
                            IncludeTo = !string.IsNullOrEmpty(x.RequestedUpperBound),
                            ToStr = x.RequestedUpperBound,
                            IsSelected = x.IsApplied,
                            Label = x.Value.ToString(),
                        })
                            .ToArray(),
                        Name = request.Field,
                    },
                    _ => null
                };
                if (result != null)
                    result.Label = request.Labels?.FirstOrDefault(x => x.Language == cultureName)?.Label ?? result.Name;
                return result;
            });
        }
    }
}
