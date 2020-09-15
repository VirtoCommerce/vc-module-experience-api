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
        private ShoppingCartSearchCriteria searchCriteria { get; set; }

        public CartSearchCriteriaBuilder(ISearchPhraseParser phraseParser, IMapper mapper) : this()
        {
            _phraseParser = phraseParser;
            _mapper = mapper;
        }

        public CartSearchCriteriaBuilder()
        {
            searchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();
        }

        public virtual ShoppingCartSearchCriteria Build()
        {
            return searchCriteria;
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
            _mapper.Map(parseResult.Filters, searchCriteria);

            return this;
        }

        public CartSearchCriteriaBuilder WithLanguage(string language)
        {
            searchCriteria.LanguageCode = language;
            return this;

        }
        public CartSearchCriteriaBuilder WithStore(string storeId)
        {
            searchCriteria.StoreId = storeId;
            return this;
        }

        public CartSearchCriteriaBuilder WithType(string type)
        {
            searchCriteria.Type = type;
            return this;
        }

        public CartSearchCriteriaBuilder WithCurrency(string currency)
        {
            searchCriteria.Currency = currency;
            return this;
        }

        public CartSearchCriteriaBuilder WithCustomerId(string customerId)
        {
            searchCriteria.CustomerId = customerId;
            return this;
        }

        public CartSearchCriteriaBuilder WithPaging(int skip, int take)
        {
            searchCriteria.Skip = skip;
            searchCriteria.Take = take;
            return this;
        }

        public CartSearchCriteriaBuilder WithSorting(string sort)
        {
            searchCriteria.Sort = sort;

            return this;
        }

        public CartSearchCriteriaBuilder AddResponseGroup(CustomerOrderResponseGroup responseGroup)
        {
            searchCriteria.ResponseGroup = responseGroup.ToString();

            return this;
        }
    }
}
