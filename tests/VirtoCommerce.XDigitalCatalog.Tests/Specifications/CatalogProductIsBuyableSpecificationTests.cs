using System.Collections.Generic;
using FluentAssertions;
using VirtoCommerce.XDigitalCatalog.Specifications;
using VirtoCommerce.XDigitalCatalog.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Specifications
{
    public class CatalogProductIsBuyableSpecificationTests : XDigitalCatalogMoqHelper
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void IsSatisfiedByTest(bool isActive, bool isBuyable, bool hasPrices, bool isSatisfied)
        {
            // Arrange
            var target = new CatalogProductIsBuyableSpecification();

            var product = GetExpProduct(new ExpProductOptions
            {
                IsActive = isActive,
                IsBuyable = isBuyable,
                HasPrices = hasPrices
            });

            // Act
            var result = target.IsSatisfiedBy(product);

            // Assert
            result.Should().Be(isSatisfied);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                //is active, is buyable, has price, result true
                new object[] { true, true, true, true},
                //is active, is buyable, has no price, result false
                new object[] { true, true, false, false },
            };
    }
}
