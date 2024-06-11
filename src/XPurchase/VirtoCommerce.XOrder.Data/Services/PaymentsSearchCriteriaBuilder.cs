using System;
using AutoMapper;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XOrder.Data.Services
{
    public class PaymentsSearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly PaymentSearchCriteria _searchCriteria;

        public PaymentsSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public PaymentsSearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<PaymentSearchCriteria>.TryCreateInstance();
        }

        public virtual PaymentSearchCriteria Build()
        {
            return _searchCriteria.Clone() as PaymentSearchCriteria;
        }

        public PaymentsSearchCriteriaBuilder ParseFilters(string filterPhrase)
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
            _searchCriteria.Keyword = parseResult.Keyword;
            return this;
        }

        public PaymentsSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            _searchCriteria.CustomerId = customerId ?? _searchCriteria.CustomerId;
            return this;
        }

        public PaymentsSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public PaymentsSearchCriteriaBuilder WithSorting(string sort)
        {
            _searchCriteria.Sort = sort ?? _searchCriteria.Sort;

            return this;
        }

    }
}
