using System;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Data
{
    // TODO: to data project
    public class PropertySearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly PropertySearchCriteria _searchCriteria;


        public PropertySearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public PropertySearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<PropertySearchCriteria>.TryCreateInstance();
        }

        public virtual PropertySearchCriteria Build()
        {
            return _searchCriteria.Clone() as PropertySearchCriteria;
        }

        public PropertySearchCriteriaBuilder ParseFilters(string filterPhrase)
        {
            if (filterPhrase == null)
            {
                return this;
            }
            if (_phraseParser == null)
            {
                throw new OperationCanceledException("phrase parser must be set");
            }

            var parseResult = _phraseParser.Parse(filterPhrase);
            _mapper.Map(parseResult.Filters, _searchCriteria);

            return this;
        }

        public PropertySearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public PropertySearchCriteriaBuilder WithCatalogId(string catalogId)
        {
            _searchCriteria.CatalogId = catalogId;
            return this;
        }
    }
}
