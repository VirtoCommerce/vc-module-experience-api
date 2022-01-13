using System.Collections.Generic;
using FluentAssertions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Specifications;
using VirtoCommerce.XDigitalCatalog.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Specifications
{
    public class CatalogProductIsAvailableSpecificationTests : XDigitalCatalogMoqHelper
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void IsSatisfiedByTest(bool hasInventory, bool allowBackorder, bool allowPreorder, bool isSatisfied)
        {
            // Arrange
            AbstractTypeFactory<CatalogProductIsBuyableSpecification>.RegisterType<CatalogProductIsBuyableSpecification>();

            var product = GetExpProduct(new ExpProductOptions
            {
                HasInventory = hasInventory,
                AllowBackorder = allowBackorder,
                AllowPreorder = allowPreorder,
            });

            var target = new CatalogProductIsAvailableSpecification();

            // Act
            var result = target.IsSatisfiedBy(product);

            // Assert
            result.Should().Be(isSatisfied);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                //has inventory, result true
                new object[] { true, true, true, true},
                //has no inventory, result false
                new object[] { false, false, false, false },
            };
    }
}
