using System;
using AutoMapper;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderSearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly CustomerOrderSearchCriteria _searchCriteria;

        public CustomerOrderSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public CustomerOrderSearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
        }

        public virtual CustomerOrderSearchCriteria Build()
        {
            return _searchCriteria.Clone() as CustomerOrderSearchCriteria;
        }

        public CustomerOrderSearchCriteriaBuilder ParseFilters(string filterPhrase)
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

        public CustomerOrderSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            _searchCriteria.CustomerId = customerId ?? _searchCriteria.CustomerId;
            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithSorting(string sort)
        {
            _searchCriteria.Sort = sort ?? _searchCriteria.Sort;

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder AddResponseGroup(CustomerOrderResponseGroup responseGroup)
        {
            _searchCriteria.ResponseGroup = responseGroup.ToString();

            return this;
        }
    }
}
