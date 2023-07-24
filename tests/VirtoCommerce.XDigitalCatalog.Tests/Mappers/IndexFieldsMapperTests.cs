using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Mappers
{
    public class IndexFieldsMapperTests
    {
        [Theory]
        [InlineData("minVariationPrice", "__minvariationprice")]
        [InlineData("items.minVariationPrice", "__minvariationprice")]
        [InlineData("items.minVariationPrice.actual.amount", "__minvariationprice")]
        public void Test(string includeField, string mappedField)
        {
            // arrange
            var includeFields = new List<string> { includeField };

            // act
            var result = IndexFieldsMapper.MapToIndexIncludes(includeFields).ToArray();

            // assert
            result.Should().Contain(mappedField);
        }
    }
}
