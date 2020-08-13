using System;
using AutoMapper;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderSearchCriteriaBuilder
    {
        private ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private CustomerOrderSearchCriteria searchCriteria { get; set; }

        public CustomerOrderSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public CustomerOrderSearchCriteriaBuilder()
        {
            searchCriteria = new CustomerOrderSearchCriteria();
        }

        public virtual CustomerOrderSearchCriteria Build()
        {
            return searchCriteria;
        }

        public CustomerOrderSearchCriteriaBuilder ParseFilters(string filterPhrase)
        {
            if (filterPhrase == null)
            {
                return this;
            }
            if (_phraseParser == null)
            {
                throw new OperationCanceledException("phrase parser must be initialized");
            }

            var parseResult = _phraseParser.Parse(filterPhrase);
            _mapper.Map(parseResult.Filters, searchCriteria);

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder AddCustomerId(string customerId)
        {
            searchCriteria.EmployeeId = customerId;
            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            searchCriteria.Skip = skip;
            searchCriteria.Take = take;
            return this;
        }

        public CustomerOrderSearchCriteriaBuilder AddSorting(string sort)
        {
            searchCriteria.Sort = sort;

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder AddResponseGroup(CustomerOrderResponseGroup responseGroup)
        {
            searchCriteria.ResponseGroup = responseGroup.ToString();

            return this;
        }
    }
}
