using System;
using AutoMapper;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.SearchModule.Core.Services;

// todo: delete?
namespace VirtoCommerce.ExperienceApiModule.Data.Services
{
    public class DynamicPropertyDictionaryItemSearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly DynamicPropertyDictionaryItemSearchCriteria _searchCriteria;

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<DynamicPropertyDictionaryItemSearchCriteria>.TryCreateInstance();
        }

        public virtual DynamicPropertyDictionaryItemSearchCriteria Build()
        {
            return _searchCriteria.Clone() as DynamicPropertyDictionaryItemSearchCriteria;
        }

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder ParseFilters(string filterPhrase)
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

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder WithPropertyId(string propertyId)
        {
            _searchCriteria.PropertyId = propertyId ?? _searchCriteria.PropertyId;
            return this;
        }

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder WithLanguage(string language)
        {
            _searchCriteria.LanguageCode = language ?? _searchCriteria.LanguageCode;
            return this;

        }

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public DynamicPropertyDictionaryItemSearchCriteriaBuilder WithSorting(string sort)
        {
            _searchCriteria.Sort = sort ?? _searchCriteria.Sort;
            return this;
        }

    }
}
