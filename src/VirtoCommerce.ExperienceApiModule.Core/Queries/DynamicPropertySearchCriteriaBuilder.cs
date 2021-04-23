using System;
using AutoMapper;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class DynamicPropertySearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly DynamicPropertySearchCriteria _searchCriteria;

        public DynamicPropertySearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public DynamicPropertySearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<DynamicPropertySearchCriteria>.TryCreateInstance();
        }

        public virtual DynamicPropertySearchCriteria Build()
        {
            return _searchCriteria.Clone() as DynamicPropertySearchCriteria;
        }

        public DynamicPropertySearchCriteriaBuilder ParseFilters(string filterPhrase)
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

        public DynamicPropertySearchCriteriaBuilder WithLanguage(string language)
        {
            _searchCriteria.LanguageCode = language ?? _searchCriteria.LanguageCode;
            return this;

        }

        public DynamicPropertySearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public DynamicPropertySearchCriteriaBuilder WithSorting(string sort)
        {
            _searchCriteria.Sort = sort ?? _searchCriteria.Sort;

            return this;
        }

    }
}
