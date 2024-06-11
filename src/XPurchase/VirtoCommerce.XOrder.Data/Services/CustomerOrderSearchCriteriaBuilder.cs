using System;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XOrder.Data.Services
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

        public CustomerOrderSearchCriteriaBuilder ParseFacets(string facetPhrase)
        {
            if (facetPhrase == null)
            {
                return this;
            }

            if (_phraseParser == null)
            {
                throw new OperationCanceledException("phrase parser must be set");
            }

            _searchCriteria.Facet = facetPhrase;

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                // customerId should be added before custom Keyword to prevent overriding
                _searchCriteria.Keyword = $"customerId:\"{customerId}\" {_searchCriteria.Keyword}";
            }

            return this;
        }

        public CustomerOrderSearchCriteriaBuilder WithOrganizationId(string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId))
            {
                // organizationId should be added before custom Keyword to prevent overriding
                _searchCriteria.Keyword = $"organizationId:\"{organizationId}\" {_searchCriteria.Keyword}";
            }

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

        public CustomerOrderSearchCriteriaBuilder WithCultureName(string cultureName)
        {
            _searchCriteria.LanguageCode = cultureName;
            return this;
        }
    }
}
