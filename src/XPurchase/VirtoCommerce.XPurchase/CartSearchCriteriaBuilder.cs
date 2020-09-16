using System;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XPurchase
{
    public class CartSearchCriteriaBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IMapper _mapper;
        private readonly ShoppingCartSearchCriteria _searchCriteria;

        public CartSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public CartSearchCriteriaBuilder()
        {
            _searchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();
        }

        public virtual ShoppingCartSearchCriteria Build()
        {
            return _searchCriteria.Clone() as ShoppingCartSearchCriteria;
        }

        public CartSearchCriteriaBuilder ParseFilters(string filterPhrase)
        {
            if (string.IsNullOrEmpty(filterPhrase))
            {
                return this;
            }
            if (_phraseParser == null)
            {
                throw new OperationCanceledException("phrase parser must be initialized");
            }

            var parseResult = _phraseParser.Parse(filterPhrase);
            _mapper.Map(parseResult.Filters, _searchCriteria);

            return this;
        }

        public CartSearchCriteriaBuilder WithLanguage(string language)
        {
            _searchCriteria.LanguageCode = language ?? _searchCriteria.LanguageCode;
            return this;

        }
        public CartSearchCriteriaBuilder WithStore(string storeId)
        {
            _searchCriteria.StoreId = storeId ?? _searchCriteria.StoreId;
            return this;
        }

        public CartSearchCriteriaBuilder WithType(string type)
        {
            _searchCriteria.Type = type ?? _searchCriteria.Type;
            return this;
        }

        public CartSearchCriteriaBuilder WithCurrency(string currency)
        {
            _searchCriteria.Currency = currency ?? _searchCriteria.Currency;
            return this;
        }

        public CartSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            _searchCriteria.CustomerId = customerId ?? _searchCriteria.CustomerId;
            return this;
        }

        public CartSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            _searchCriteria.Skip = skip;
            _searchCriteria.Take = take;
            return this;
        }

        public CartSearchCriteriaBuilder WithSorting(string sort)
        {
            _searchCriteria.Sort = sort ?? _searchCriteria.Sort;

            return this;
        }

        public CartSearchCriteriaBuilder AddResponseGroup(CustomerOrderResponseGroup responseGroup)
        {
            _searchCriteria.ResponseGroup = responseGroup.ToString();

            return this;
        }
    }
}
