using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using GraphQL.Types;
using Moq;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XDigitalCatalog.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Schemas
{
    public class CatalogDiscountTypeTests : XDigitalCatalogMoqHelper
    {
        private readonly CatalogDiscountType _catalogDiscountType;
        private readonly Mock<IPromotionSearchService> _promotionSearchServiceMock;
        private readonly List<Promotion> _promotions;

        public CatalogDiscountTypeTests()
        {
            _promotions = new List<Promotion>
            {
                new Promotion()
                {
                    Id = _fixture.Create<string>()
                }
            };

            _promotionSearchServiceMock = new Mock<IPromotionSearchService>();
            _promotionSearchServiceMock
                .Setup(x => x.SearchPromotionsAsync(It.IsAny<PromotionSearchCriteria>()))
                .ReturnsAsync(new PromotionSearchResult { Results = _promotions });

            _catalogDiscountType = new CatalogDiscountType(_promotionSearchServiceMock.Object);
        }

        [Fact]
        public void CatalogDiscountType_ShouldHavePropperFieldAmount()
        {
            // Assert
            _catalogDiscountType.Fields.Should().HaveCount(6);
        }

        [Fact]
        public void CatalogDiscountType_Promotion_ShouldResolve()
        {
            // Arrange
            var resolveContext = new ResolveFieldContext()
            {
                Source = GetDiscount()
            };

            // Act
            var result = _catalogDiscountType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("Promotion")).Resolver.Resolve(resolveContext);
            result = (result as Task<object>).Result;

            // Assert
            result.Should().BeOfType<Promotion>();
            ((Promotion)result).Should().BeEquivalentTo(_promotions.FirstOrDefault());
        }
    }
}
