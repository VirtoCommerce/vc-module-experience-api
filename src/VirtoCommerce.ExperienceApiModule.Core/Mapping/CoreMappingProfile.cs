using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XPurchase.Mapping
{
    public class CoreMappingProfile : Profile
    {
        public CoreMappingProfile()
        {
            CreateMap<IList<IFilter>, DynamicPropertySearchCriteria>()
              .ConvertUsing((terms, criteria, context) =>
              {
                  foreach (var term in terms.OfType<TermFilter>())
                  {
                      term.MapTo(criteria);
                  }

                  return criteria;
              });

            CreateMap<IList<IFilter>, DynamicPropertyDictionaryItemSearchCriteria>()
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
