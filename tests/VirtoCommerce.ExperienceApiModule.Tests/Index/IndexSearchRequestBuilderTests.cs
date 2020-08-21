using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Index
{
    public class IndexSearchRequestBuilderTests : MoqHelper
    {
        private readonly Mock<ISearchPhraseParser> _phraseParserMock;
        private readonly Mock<IAggregationConverter> _aggregationConverterMock;

        private readonly IndexSearchRequestBuilder _builder;

        public IndexSearchRequestBuilderTests()
        {
            _phraseParserMock = new Mock<ISearchPhraseParser>();
            _aggregationConverterMock = new Mock<IAggregationConverter>();

            _builder = new IndexSearchRequestBuilder();
        }

        #region FromQuery(IGetDocumentsByIdsQuery)

        [Fact]
        public void IndexSearchRequestBuilder_WithIncludeFields_IncludeFieldsSaved()
        {
            // Arrange
            var includeFields = _fixture.CreateMany<string>().ToArray();

            // Act
            var result = _builder.WithIncludeFields(includeFields).Build();

            // Assert
            result.IncludeFields.Should().BeEquivalentTo(includeFields.ToArray());
        }

        [Fact]
        public void IndexSearchRequestBuilder_AddObjectIds_ShouldSaveTakeAndAddIdsFilter()
        {
            // Arrange
            var objectIds = _fixture.CreateMany<string>().ToArray();

            // Act
            var result = _builder.AddObjectIds(objectIds).Build();

            // Assert
            result.Take.Should().Be(objectIds.Count());
            result.Filter.As<AndFilter>().ChildFilters.Should().ContainEquivalentOf(new IdsFilter { Values = objectIds });
        }

        #endregion FromQuery(IGetDocumentsByIdsQuery)

        #region FromQuery(ISearchDocumentsQuery)

        [Fact]
        public void IndexSearchRequestBuilder_Search_PropertiesMapsCorrectly()
        {
            // Arrange
            var fuzzy = _fixture.Create<bool>();
            var fuzzyLevel = _fixture.Create<int>();
            var skip = _fixture.Create<int>();
            var take = _fixture.Create<int>();
            var searchKeywords = _fixture.Create<string>();
            var includeFields = _fixture.CreateMany<string>().ToArray();

            // Act
            var result = _builder
                .WithFuzzy(fuzzy, fuzzyLevel)
                .WithIncludeFields(includeFields)
                .WithPaging(skip, take)
                .WithSearchPhrase(searchKeywords)
                .Build();

            // Assert
            result.IsFuzzySearch.Should().Be(fuzzy);
            result.Fuzziness.Should().Be(fuzzyLevel);
            result.Skip.Should().Be(skip);
            result.Take.Should().Be(take);
            result.SearchKeywords.Should().Be(searchKeywords);
            result.IncludeFields.Should().BeEquivalentTo(includeFields.ToList());
        }

        [Theory]
        [InlineData(20)]
        [InlineData(30)]
        public void IndexSearchRequestBuilder_Search_ObjectIdsPassed_ShoulddAddIdsFilter_TakeNoBindToObjectIdsCount(int take)
        {
            // Arrange
            var objectIds = _fixture.CreateMany<string>().ToArray();

            // Act
            var result = _builder.AddObjectIds(objectIds).WithPaging(0, take).Build();

            // Assert
            result.Take.Should().Be(take);
            result.Filter.As<AndFilter>().ChildFilters.Should().ContainEquivalentOf(new IdsFilter { Values = objectIds });
        }

        #endregion FromQuery(ISearchDocumentsQuery)

        #region ParseFilters

        [Theory]
        [InlineData("rangeFilter")]
        [InlineData("geoDistanceFilter")]
        [InlineData("idsFilter")]
        [InlineData("wildCardTermFilter")]
        public void ParseFilters_Filters_MapsCorrectly(string filterName)
        {
            // Arrange
            var searchPhraseParseResult = new SearchPhraseParseResult
            {
                Filters = new List<IFilter>
                {
                    base.GetFilterByName(filterName)
                }
            };

            _phraseParserMock
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(searchPhraseParseResult);

            // Act
            var result = _builder
                .ParseFilters(_phraseParserMock.Object, _fixture.Create<string>())
                .Build();

            // Assert
            var filters = result.Filter.As<AndFilter>().ChildFilters;
            base.AssertFiltersContainFilterByTypeName(filters, filterName);
        }

        [Fact]
        public void ParseFilters_TermFilterWithoutWildCards_MapsCorrectly()
        {
            // Arrange
            var termFilter = _fixture.Create<TermFilter>();
            var searchPhraseParseResult = new SearchPhraseParseResult
            {
                Filters = new List<IFilter>
                {
                    termFilter
                }
            };

            _phraseParserMock
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(searchPhraseParseResult);

            // Act
            var result = _builder
                .ParseFilters(_phraseParserMock.Object, _fixture.Create<string>())
                .Build();

            // Assert
            var filters = result.Filter.As<AndFilter>().ChildFilters;
            filters.Should().ContainEquivalentOf(termFilter);
        }

        [Theory]
        [InlineData("?")]
        [InlineData("*")]
        public void ParseFilters_TermFilterWildCards_MapsCorrectly(string wildcard)
        {
            // Arrange
            var termFilter = _fixture.Create<TermFilter>();
            termFilter.Values.Add(wildcard);
            var searchPhraseParseResult = new SearchPhraseParseResult
            {
                Filters = new List<IFilter>
                {
                    termFilter
                },
            };

            _phraseParserMock
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(searchPhraseParseResult);

            // Act
            var result = _builder
                .ParseFilters(_phraseParserMock.Object, _fixture.Create<string>())
                .Build();

            // Assert
            var filters = result.Filter.As<AndFilter>().ChildFilters;

            filters.Should().ContainEquivalentOf(new OrFilter
            {
                ChildFilters = new List<IFilter>
                {
                    new WildCardTermFilter
                    {
                        FieldName = termFilter.FieldName,
                        Value = wildcard
                    },
                    new TermFilter
                    {
                        Values = termFilter.Values.Where(x => !x.Contains(wildcard)).ToList(),
                        FieldName = termFilter.FieldName
                    }
                }
            });
        }

        #endregion ParseFilters

        #region ParseFacets

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseFacets_FacetPhraseIsNull_ShouldSkipParsing(string facetPhrase)
        {
            // Arrange
            var expected = _fixture.Create<SearchRequest>();

            // Act
            var actual = _builder.ParseFacets(_phraseParserMock.Object, facetPhrase).Build();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseFacets_FacetPhraseIsNull_AggregationsAreEmpty(string facetPhrase)
        {
            // Arrange
            var aggregations = new List<AggregationRequest>
            {
                new AggregationRequest
                {
                    FieldName = _fixture.Create<string>(),
                    Id = _fixture.Create<string>()
                }
            };

            _aggregationConverterMock
                .Setup(x => x.GetAggregationRequestsAsync(It.IsAny<ProductIndexedSearchCriteria>(), It.IsAny<FiltersContainer>()))
                .ReturnsAsync(aggregations);

            // Act
            var actual = _builder.ParseFacets(_phraseParserMock.Object, facetPhrase).Build();

            // Assert
            actual.Aggregations.Should().BeEmpty();
        }

        [Fact]
        public void ParseFacets_FacetPhraseNotNull_MappingCorrect()
        {
            // Arrange
            var idsFilter = _fixture.Create<IdsFilter>();
            var rangeFilter = _fixture.Create<RangeFilter>();
            var termFilter = _fixture.Create<TermFilter>();
            var searchPhrase = _fixture.Create<string>();
            var keywords = _fixture.CreateMany<string>();
            var searchPhraseParseResult = new SearchPhraseParseResult
            {
                Filters = new List<IFilter>
                {
                    idsFilter,
                    rangeFilter,
                    termFilter
                },
                Keyword = string.Join(" ", keywords)
            };

            _phraseParserMock
                .Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(searchPhraseParseResult);

            // Act
            var actual = _builder
                .ParseFacets(_phraseParserMock.Object, searchPhrase)
                .Build();

            // Assert
            actual.Aggregations.Should().ContainEquivalentOf(new RangeAggregationRequest
            {
                Id = rangeFilter.As<IFilter>().Stringify(),
                FieldName = rangeFilter.FieldName,
                Values = rangeFilter.Values.Select(x => new RangeAggregationRequestValue
                {
                    Id = x.Stringify(),
                    Lower = x.Lower,
                    Upper = x.Upper,
                    IncludeLower = x.IncludeLower,
                    IncludeUpper = x.IncludeUpper
                }).ToList()
            }, config => config.Excluding(x => x.Filter));

            foreach (var fieldName in keywords)
            {
                var filter = new TermFilter
                {
                    FieldName = fieldName,
                    Values = new List<string>()
                };

                actual.Aggregations.Should().ContainEquivalentOf(new TermAggregationRequest
                {
                    Id = filter.As<IFilter>().Stringify(),
                    FieldName = filter.FieldName,
                    Filter = filter
                });
            }

            actual.Aggregations.Should().ContainEquivalentOf(new TermAggregationRequest
            {
                Id = termFilter.As<IFilter>().Stringify(),
                FieldName = termFilter.FieldName,
                Filter = termFilter
            });

            actual.Aggregations.Count.Should().Be(
                searchPhraseParseResult.Filters.OfType<RangeFilter>().Count()
                + searchPhraseParseResult.Filters.OfType<TermFilter>().Count());
        }

        #endregion ParseFacets

        #region Build

        [Fact]
        public void BuildRequestFromDefault_ReturnDefaultRequest()
        {
            // Arrange
            var expected = _fixture.Create<SearchRequest>();

            // Act
            var actual = _builder.Build();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        //[Fact]
        //public void Build_ShouldCopyNonNamedFiltersToAggregations()
        //{
        //    // Arrange
        //    var idsFilter = _fixture.Create<IdsFilter>();
        //    var searchPhraseParseResult = new SearchPhraseParseResult
        //    {
        //        Filters = new List<IFilter>
        //        {
        //            idsFilter
        //        }
        //    };

        //    _phraseParserMock
        //        .Setup(x => x.Parse(It.IsAny<string>()))
        //        .Returns(searchPhraseParseResult);

        //    var aggregation = new AggregationRequest
        //    {
        //        FieldName = _fixture.Create<string>(),
        //        Id = _fixture.Create<string>()
        //    };

        //    var aggregations = new List<AggregationRequest>
        //    {
        //        aggregation
        //    };

        //    _aggregationConverterMock
        //        .Setup(x => x.GetAggregationRequestsAsync(It.IsAny<ProductIndexedSearchCriteria>(), It.IsAny<FiltersContainer>()))
        //        .ReturnsAsync(aggregations);

        //    // Act
        //    var actual = _builder
        //        .ParseFilters(_phraseParserMock.Object, idsFilter.Stringify())
        //        .ParseFacets(_phraseParserMock.Object, null)
        //        .Build();

        //    // Assert
        //    var filters = actual.Filter.As<AndFilter>().ChildFilters;
        //    filters.Should().ContainEquivalentOf(idsFilter);
        //    actual.Aggregations.Should().BeEquivalentTo(aggregations);
        //    aggregation.Filter.Should().NotBeNull();
        //}

        //[Fact]
        //public void Build_ShouldNotCopySameNamedFiltersToAggregations()
        //{
        //    // Arrange
        //    var rangeFilter = new RangeFilter
        //    {
        //        FieldName = "price",
        //        Values = new List<RangeFilterValue>
        //        {
        //            new RangeFilterValue
        //            {
        //                IncludeLower = false,
        //                IncludeUpper = false,
        //                Lower = string.Empty,
        //                Upper = string.Empty
        //            }
        //        }
        //    };
        //    var searchPhraseParseResult = new SearchPhraseParseResult
        //    {
        //        Filters = new List<IFilter>
        //        {
        //            rangeFilter,
        //        }
        //    };

        //    _phraseParserMock
        //        .Setup(x => x.Parse(It.IsAny<string>()))
        //        .Returns(searchPhraseParseResult);

        //    var aggregation = new AggregationRequest
        //    {
        //        FieldName = _fixture.Create<string>(),
        //        Id = _fixture.Create<string>(),
        //        Filter = new RangeFilter
        //        {
        //            FieldName = "price_usd"
        //        }
        //    };

        //    var aggregations = new List<AggregationRequest>
        //    {
        //        aggregation
        //    };

        //    _aggregationConverterMock
        //        .Setup(x => x.GetAggregationRequestsAsync(It.IsAny<ProductIndexedSearchCriteria>(), It.IsAny<FiltersContainer>()))
        //        .ReturnsAsync(aggregations);

        //    // Act
        //    var actual = _builder
        //        .ParseFilters(_phraseParserMock.Object, _fixture.Create<string>())
        //        .ParseFacets(_phraseParserMock.Object, null)
        //        .Build();

        //    // Assert
        //    var filters = actual.Aggregations.Single().Filter.As<AndFilter>().ChildFilters;

        //    var resultRangeFilter = filters.OfType<AndFilter>().FirstOrDefault()?.ChildFilters.FirstOrDefault() as RangeFilter;
        //    resultRangeFilter.Should().BeNull();
        //    actual.Aggregations.Should().BeEquivalentTo(aggregations);
        //    aggregation.Filter.Should().NotBeNull();
        //}

        #endregion Build
    }
}
