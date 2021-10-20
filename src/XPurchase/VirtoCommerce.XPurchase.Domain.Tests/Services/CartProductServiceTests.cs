using Moq;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;
using AutoMapper;
using Xunit;
using VirtoCommerce.CatalogModule.Core.Model;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartProductServiceTests
    {
        private readonly Mock<IItemService> _productService;
        private readonly Mock<IInventorySearchService> _inventorySearchService;
        private readonly Mock<IPricingService> _pricingService;
        private readonly Mock<IMapper> _mapper;

        public CartProductServiceTests()
        {
            _productService = new Mock<IItemService>();
            _inventorySearchService = new Mock<IInventorySearchService>();
            _pricingService = new Mock<IPricingService>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetProductsByIdsAsync_NumberProducts_ReturnsProducts()
        {
            // Arrange
            var productId1 = Guid.NewGuid().ToString();
            var productId2 = Guid.NewGuid().ToString();
            var ids = new[] { productId1, productId2 };

            var TestCatalogProduct = new List<CatalogProduct>()
            {
                new CatalogProduct() { Id = productId1 },
                new CatalogProduct() { Id = productId2 }
            };

            var _service = new CartProductServiceFake(_productService.Object, _inventorySearchService.Object, _pricingService.Object, _mapper.Object);
            _productService.Setup(x => x.GetByIdsAsync(ids, It.IsAny<string>(), null)).ReturnsAsync(TestCatalogProduct.ToArray());

            //Act
            var result = await _service.GetProductsByIdsFakeAsync(ids);

            //Assert
            result.Should().HaveCount(2);
        }
    }
}
