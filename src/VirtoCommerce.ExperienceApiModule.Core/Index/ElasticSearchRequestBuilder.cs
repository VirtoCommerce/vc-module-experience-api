using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public class ElasticSearchRequestBuilder : IRequestBuilder
    {
        private readonly ISearchPhraseParser _phraseParser;
        private readonly IAggregationConverter _aggregationConverter;

        private SearchRequest SearchRequest { get; set; }

        public ElasticSearchRequestBuilder(
            ISearchPhraseParser phraseParser
            , IAggregationConverter aggregationConverter
            )
        {
            _phraseParser = phraseParser;
            _aggregationConverter = aggregationConverter;

            SearchRequest = new SearchRequest
            {
                Filter = new AndFilter()
                {
                    ChildFilters = new List<IFilter>(),
                },
                SearchFields = new List<string> { "__content" },
                Sorting = new List<SortingField> { new SortingField("__sort") },
                Skip = 0,
                Take = 20,
                Aggregations = new List<AggregationRequest>(),
                IncludeFields = new List<string>(),
            };
        }

        public virtual IRequestBuilder FromQuery(ISearchDocumentsQuery query)
        {
            SearchRequest.IsFuzzySearch = query.Fuzzy;
            SearchRequest.Fuzziness = query.FuzzyLevel;
            SearchRequest.Skip = query.Skip;
            SearchRequest.Take = query.Take;
            SearchRequest.SearchKeywords = query.Query;
            SearchRequest.IncludeFields = query.IncludeFields.ToList();

            ParseFilters(query.Filter, query.CurrencyCode, query.StoreId);

            AddSorting(query.Sort);

            if (!query.ObjectIds.IsNullOrEmpty())
            {
                AddFiltersToSearchRequest(new IFilter[] { new IdsFilter { Values = query.ObjectIds } });
            }

            return this;
        }

        public virtual IRequestBuilder FromQuery(IGetDocumentsByIdsQuery query)
        {
            if (!query.ObjectIds.IsNullOrEmpty())
            {
                AddFiltersToSearchRequest(new IFilter[] { new IdsFilter { Values = query.ObjectIds } });
                SearchRequest.Take = query.ObjectIds.Count();
            }

            SearchRequest.IncludeFields = query.IncludeFields.ToList();

            return this;
        }

        public virtual SearchRequest Build()
        {
            //Apply multi-select facet search policy by default

            foreach (var aggr in SearchRequest.Aggregations)
            {
                var aggregationFilterFieldName = (aggr.Filter as INamedFilter)?.FieldName;

                var clonedFilter = SearchRequest.Filter.Clone() as AndFilter;

                clonedFilter.ChildFilters = clonedFilter
                    .ChildFilters
                    .Where(x =>
                    {
                        var result = true;
                    
                        if (x is INamedFilter namedFilter)
                        {
                            result = !(aggregationFilterFieldName?.StartsWith(namedFilter.FieldName) ?? false);
                        }
                    
                        return result;
                    })
                    .ToList();
            
                aggr.Filter = aggr.Filter == null ? clonedFilter : aggr.Filter.And(clonedFilter);
            }
            return SearchRequest;
        }

        public IRequestBuilder AddTerms(IEnumerable<string> terms)
        {
            if (terms != null)
            {
                const string commaEscapeString = "%x2C";

                var nameValueDelimeter = new[] { ':' };
                var valuesDelimeter = new[] { ',' };

                var termsFields = terms.Select(item => item.Split(nameValueDelimeter, 2))
                        .Where(item => item.Length == 2)
                        .Select(item => new TermFilter
                        {
                            FieldName = item[0],
                            Values = item[1].Split(valuesDelimeter, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x?.Replace(commaEscapeString, ","))
                                .ToArray()
                        }).ToArray<IFilter>();

                AddFiltersToSearchRequest(termsFields);
            }

            return this;
        }

        private void AddFiltersToSearchRequest(IFilter[] filters)
        {
            ((AndFilter)SearchRequest.Filter).ChildFilters.AddRange(filters);
        }

        private void ParseFilters(string filterPhrase, string currencyCode, string storeId)
        {
            var filters = new List<IFilter>();

            if (filterPhrase == null || currencyCode == null || storeId == null)
            {
                return;
            }
            const string spaceEscapeString = "%x20";
            filterPhrase = filterPhrase.Replace(spaceEscapeString, " ");

            var parseResult = _phraseParser.Parse(filterPhrase);

            foreach (var filter in parseResult.Filters)
            {
                FilterSyntaxMapper.MapFilterAdditionalSyntax(filter);
                if (filter is TermFilter termFilter)
                {
                    termFilter.FieldName = termFilter.FieldName?.Replace(spaceEscapeString, " ");
                    termFilter.Values = termFilter.Values?.Select(x => x?.Replace(spaceEscapeString, " ")).ToList();

                    var wildcardValues = termFilter.Values.Where(x => new[] { "?", "*" }.Any(x.Contains));

                    if (wildcardValues.Any())
                    {
                        var orFilter = new OrFilter
                        {
                            ChildFilters = new List<IFilter>()
                        };

                        var wildcardTermFilters = wildcardValues.Select(x => new WildCardTermFilter
                        {
                            FieldName = termFilter.FieldName,
                            Value = x
                        }).ToList();

                        orFilter.ChildFilters.AddRange(wildcardTermFilters);

                        termFilter.Values = termFilter.Values.Except(wildcardValues).ToList();

                        if (termFilter.Values.Any())
                        {
                            orFilter.ChildFilters.Add(termFilter);
                        }

                        filters.Add(orFilter);
                    }
                    else
                    {
                        filters.Add(termFilter);
                    }
                }
                else
                {
                    filters.Add(filter);
                }
            }

            AddFiltersToSearchRequest(filters.ToArray());
        }

        public IRequestBuilder ParseFacets(string facetPhrase, string storeId = null, string currency = null)
        {
            if (string.IsNullOrWhiteSpace(facetPhrase))
            {
                if (!string.IsNullOrEmpty(storeId) && !string.IsNullOrEmpty(currency))
                {
                    // TODO: Add izolation from store
                    // Maybe we need to implement ProductSearchRequestBuilder.BuildRequestAsync to fill FilterContainer correctly?
                    SearchRequest.Aggregations = _aggregationConverter.GetAggregationRequestsAsync(new ProductIndexedSearchCriteria
                    {
                        StoreId = storeId,
                        Currency = currency,
                    }, new FiltersContainer()).GetAwaiter().GetResult();
                }
            
                return this;
            }
            
            //TODO: Support aliases for Facet expressions e.g price.usd[TO 200) as price_below_200
            //TODO: Need to create a new  Antlr file with g4-lexer rules and generate parser especially for facets expression that will return proper AggregationRequests objects
            var parseResult = _phraseParser.Parse(facetPhrase);
            
            //Term facets
            if (!string.IsNullOrEmpty(parseResult.Keyword))
            {
                var termFacetExpressions = parseResult.Keyword.Split(" ");
                parseResult.Filters.AddRange(termFacetExpressions.Select(x => new TermFilter
                {
                    FieldName = x,
                    Values = new List<string>()
                }));
            }
            
            SearchRequest.Aggregations = parseResult.Filters
                .Select<IFilter, AggregationRequest>(filter =>
                {
                    FilterSyntaxMapper.MapFilterAdditionalSyntax(filter);
            
                    return filter switch
                    {
                        RangeFilter rangeFilter => new RangeAggregationRequest
                        {
                            Id = filter.Stringify(),
                            FieldName = rangeFilter.FieldName,
                            Values = rangeFilter.Values.Select(x => new RangeAggregationRequestValue
                            {
                                Id = x.Stringify(),
                                Lower = x.Lower,
                                Upper = x.Upper,
                                IncludeLower = x.IncludeLower,
                                IncludeUpper = x.IncludeUpper
                            }).ToList()
                        },
                        TermFilter termFilter => new TermAggregationRequest
                        {
                            FieldName = termFilter.FieldName,
                            Id = filter.Stringify(),
                            Filter = termFilter
                        },
                        _ => null,
                    };
                })
                .Where(x => x != null)
                .ToList();
            
            return this;
        }

        protected virtual void AddSorting(string sort)
        {
            //TODO: How to sort by scoring relevance???
            //TODO: Alias replacement for sort fields as well as for filter and facet expressions
            var sortFields = new List<SortingField>();
            foreach (var sortInfo in SortInfo.Parse(sort))
            {
                var sortingField = new SortingField();
                if (sortInfo is GeoSortInfo geoSortInfo)
                {
                    sortingField = new GeoDistanceSortingField
                    {
                        Location = geoSortInfo.GeoPoint
                    };
                }
                sortingField.FieldName = sortInfo.SortColumn.ToLowerInvariant();
                sortingField.IsDescending = sortInfo.SortDirection == SortDirection.Descending;

                switch (sortingField.FieldName)
                {
                    case "name":
                    case "title":
                        sortFields.Add(new SortingField("name", sortingField.IsDescending));
                        break;

                    default:
                        sortFields.Add(sortingField);
                        break;
                }
            }

            if (sortFields.Any())
            {
                SearchRequest.Sorting = sortFields;
            }
        }
    }
}
