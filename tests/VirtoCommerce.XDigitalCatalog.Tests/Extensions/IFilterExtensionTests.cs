using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Extensions
{
    public class IFilterExtensionTests
    {
        [Theory]
        [InlineData("equalString", "equalString", true)]
        [InlineData("1", 1, true)]
        [InlineData(null, null, true)]
        [InlineData("differentString1", "differentString2", false)]
        [InlineData("1", 2, false)]
        [InlineData("1", null, false)]
        [InlineData(null, "1", false)]
        public void FillIsAppliedForItems_TermFilter_IsAppliedSetProperly(string termValue, object aggregationItemValue, bool expectedIsApplied)
        {
            // Arrange
            var filter = new TermFilter()
            {
                Values = new string[] { termValue }
            };
            var aggregationItem = new AggregationItem()
            {
                Value = aggregationItemValue
            };

            // Act
            filter.FillIsAppliedForItems(new[] { aggregationItem });

            // Assert
            Assert.Equal(expectedIsApplied, aggregationItem.IsApplied);
        }

        [Theory]
        [InlineData("anyLower", "anyUpper", "anyLower", "anyUpper", true)]
        [InlineData("anyLower", null, "anyLower", null, true)]
        [InlineData(null, "anyUpper", null, "anyUpper", true)]
        [InlineData(null, null, null, null, true)]
        [InlineData("anyLower", "anyUpper", "anyLower", "differentUpper", false)]
        [InlineData("anyLower", "anyUpper", "differentLower", "anyUpper", false)]
        [InlineData("anyLower", null, "differentLower", null, false)]
        [InlineData("notNullLower", "anyUpper", null, "anyUpper", false)]
        [InlineData(null, "anyUpper", "notNullLower", "anyUpper", false)]
        [InlineData("anyLower", null, "anyLower", "notNullUpper", false)]
        public void FillIsAppliedForItems_RangeFilter_IsAppliedSetProperly(string filterLower, string filterUpper, string aggregationItemLowerBound, string aggregationItemUpperBound, bool expectedIsApplied)
        {
            // Arrange
            var filter = new RangeFilter()
            {
                Values = new[]
                {
                    new RangeFilterValue()
                    {
                        Lower = filterLower,
                        Upper = filterUpper
                    }
                }
            };
            var aggregationItem = new AggregationItem()
            {
                RequestedLowerBound = aggregationItemLowerBound,
                RequestedUpperBound = aggregationItemUpperBound
            };

            // Act
            filter.FillIsAppliedForItems(new[] { aggregationItem });

            // Assert
            Assert.Equal(expectedIsApplied, aggregationItem.IsApplied);
        }

        [Theory]
        [InlineData("any Name", "any Name", "TermFilter")]
        [InlineData("price_USD", "price", "RangeFilter")]
        [InlineData("anyName_2384765_236745236", "anyName_2384765_236745236", "TermFilter")]
        [InlineData("__outline", "__outline", "TermFilter")]
        public void GetFieldName_NamedFilter_ParsedCorrectly(string fieldName, string expectedName, string filterType)
        {
            // Arrage
            IFilter filter = null;
            if (filterType == "RangeFilter")
            {
                filter = new RangeFilter { FieldName = fieldName };
            }
            else if (filterType == "TermFilter")
            {
                filter = new TermFilter { FieldName = fieldName };
            }

            // Act
            var actualFieldName = filter.GetFieldName();

            // Assert
            Assert.Equal(expectedName, actualFieldName);
        }

        [Fact]
        public void SetAppliedAggregations_ComplexExample_IsAppliedSetProperly()
        {
            // Arrange
            var termfilterName = "TermFilter";
            var termFilter = new TermFilter()
            {
                FieldName = termfilterName,
                Values = new string[] { "1", "2" },

            };
            var rangeFilterName = "RangeFilter";
            var rangeFilter = new RangeFilter()
            {
                FieldName = rangeFilterName,
                Values = new[]
                {
                    new RangeFilterValue()
                {
                    Lower = "3",
                    Upper = "5"

                } },

            };

            var searchRequest = new SearchRequest()
            {
                Filter = new AndFilter()
                {
                    ChildFilters = new List<IFilter>() { termFilter, rangeFilter },
                }
            };

            var aggregations = new[]
            {
                new Aggregation()
                {
                    Field = termfilterName,
                    Items = new[]
                    {
                        new AggregationItem() { Value = "1" },
                        new AggregationItem() { Value = "2" },
                        new AggregationItem() { Value = "3" },

                    }
                },
                new Aggregation()
                {
                    Field = "nonExistentAggr",
                    Items = new[]
                    {
                        new AggregationItem() { Value = "1" },

                    }
                },
               new Aggregation()
                {
                    Field = rangeFilterName,
                    Items = new[]
                    {
                        new AggregationItem() { RequestedUpperBound = "5", RequestedLowerBound = "3" },
                        new AggregationItem() { RequestedUpperBound = "5", RequestedLowerBound = "1" },
                    }
                },
            };


            // Act
            searchRequest.SetAppliedAggregations(aggregations);

            // Assert
            Assert.True(aggregations.First(x => x.Field == termfilterName).Items.First(x => x.Value.ToString() == "1").IsApplied);
            Assert.True(aggregations.First(x => x.Field == termfilterName).Items.First(x => x.Value.ToString() == "2").IsApplied);
            Assert.True(aggregations.First(x => x.Field == rangeFilterName).Items.First(x => x.RequestedUpperBound == "5" && x.RequestedLowerBound == "3").IsApplied);

            Assert.False(aggregations.First(x => x.Field == termfilterName).Items.First(x => x.Value.ToString() == "3").IsApplied);
            Assert.False(aggregations.First(x => x.Field == "nonExistentAggr").Items.First(x => x.Value.ToString() == "1").IsApplied);
            Assert.False(aggregations.First(x => x.Field == rangeFilterName).Items.First(x => x.RequestedUpperBound == "5" && x.RequestedLowerBound == "1").IsApplied);
        }
    }
}
