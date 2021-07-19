using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<SearchPropertiesQuery, PropertySearchCriteria>();
            CreateMap<SearchPropertyDictionaryItemQuery, PropertyDictionaryItemSearchCriteria>();
        }
    }
}
