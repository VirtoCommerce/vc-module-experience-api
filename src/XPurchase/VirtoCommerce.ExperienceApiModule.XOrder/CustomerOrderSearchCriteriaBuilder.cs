using System;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderSearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly CustomerOrderIndexedSearchCriteria _searchCriteria;

        public CustomerOrderSearchCriteriaBuilder(ISearchPhraseParser phraseParser) : this()
        {
            _phraseParser = phraseParser;
        }

        public CustomerOrderSearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<CustomerOrderIndexedSearchCriteria>.TryCreateInstance();
        }

        public virtual CustomerOrderIndexedSearchCriteria Build()
        {
            return _searchCriteria.Clone() as CustomerOrderIndexedSearchCriteria;
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

            _searchCriteria.Keyword = filterPhrase;

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            _searchCriteria.Keyword += !string.IsNullOrEmpty(customerId) ? $" customerId:\"{customerId}\"" : string.Empty;
            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithOrganizationId(string organizationId)
        {
            _searchCriteria.Keyword += !string.IsNullOrEmpty(organizationId) ? $" organizationId:\"{organizationId}\"" : string.Empty;
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
