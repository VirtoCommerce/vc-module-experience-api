using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Index
{
    public class SearchRequestBuilder
    {
        private ISearchPhraseParser _phraseParser;

        private SearchRequest SearchRequest { get; set; }
        public SearchRequestBuilder(ISearchPhraseParser phraseParser)
            :this()
        {
            _phraseParser = phraseParser;
        }
        public SearchRequestBuilder()
        {
            SearchRequest = new SearchRequest
            {
                Filter = new AndFilter()
                {
                    ChildFilters = new List<IFilter>(),
                },
                SearchFields = new List<string> { "__content" },
                Sorting = new List<SortingField> { new SortingField("__sort") },
                Skip = 0,
                Take = 20

            };
        }

        public virtual SearchRequest Build()
        {
            return SearchRequest;
        }

        public SearchRequestBuilder WithFuzzy(bool fuzzy)
        {
            SearchRequest.IsFuzzySearch = fuzzy;
            return this;
        }

        public SearchRequestBuilder WithPaging(int skip, int take)
        {
            SearchRequest.Skip = skip;
            SearchRequest.Take = take;
            return this;
        }

        public SearchRequestBuilder WithSearchPhrase(string searchPhrase)
        {
            SearchRequest.SearchKeywords = searchPhrase;
            return this;
        }

        public SearchRequestBuilder WithPhraseParser(ISearchPhraseParser phraseParser)
        {
            _phraseParser = phraseParser;
            return this;
        }

        public SearchRequestBuilder WithIncludeFields(params string[] includeFields)
        {
            if(SearchRequest.IncludeFields == null)
            {
                SearchRequest.IncludeFields = new List<string>() { };
            }
            SearchRequest.IncludeFields.AddRange(includeFields);
            return this;
        }

        public SearchRequestBuilder AddTerms(IEnumerable<string> terms)
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
                        }).ToArray();
                ((AndFilter)SearchRequest.Filter).ChildFilters.AddRange(termsFields);
            }
            return this;
        }

        public SearchRequestBuilder ParseFilters(string filterPhrase)
        {
            if (filterPhrase != null)
            {
                if (_phraseParser == null)
                {
                    throw new OperationCanceledException("phrase parser must be initialized");
                }
                var parseResult = _phraseParser.Parse(filterPhrase);
                var filters = new List<IFilter>();
                foreach (var filter in parseResult.Filters)
                {
                    FilterSyntaxMapper.MapFilterSyntax(filter);
                    if (filter is TermFilter termFilter)
                    {
                        var wildcardValues = termFilter.Values.Where(x => new[] { "?", "*" }.Any(x.Contains));
                        if (wildcardValues.Any())
                        {
                            filters.AddRange(wildcardValues.Select(x => new WildCardTermFilter { FieldName = termFilter.FieldName, Value = x }));
                            termFilter.Values = termFilter.Values.Except(wildcardValues).ToList();
                        }
                        if (termFilter.Values.Any())
                        {
                            filters.Add(termFilter);
                        }
                    }
                    else
                    {
                        filters.Add(filter);
                    }
                }
                ((AndFilter)SearchRequest.Filter).ChildFilters.AddRange(filters);
            }
            return this;
        }

        public SearchRequestBuilder AddObjectIds(IEnumerable<string> ids)
        {
            ((AndFilter)SearchRequest.Filter).ChildFilters.Add(new IdsFilter { Values = ids.ToArray() });
            return this;
        }

        public SearchRequestBuilder AddSorting(string sort)
        {
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

            return this;
        }



    }
}
