using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Core.Queries;

namespace VirtoCommerce.XDigitalCatalog.Data.Mapping
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<SearchPropertiesQuery, PropertySearchCriteria>();
            CreateMap<SearchPropertyDictionaryItemQuery, PropertyDictionaryItemSearchCriteria>();
            CreateMap<IList<IFilter>, PropertySearchCriteria>()
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
