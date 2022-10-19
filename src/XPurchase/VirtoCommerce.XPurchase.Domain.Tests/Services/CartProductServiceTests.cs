using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartProductServiceTests
    {
        private readonly Mock<IItemService> _productService;
        private readonly Mock<IInventorySearchService> _inventorySearchService;
        private readonly Mock<IPricingEvaluatorService> _pricingEvaluatorService;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<LoadUserToEvalContextService> _loadUserToEvalContextService;
        private readonly CartProductServiceFake _service;

        public CartProductServiceTests()
        {
            _productService = new Mock<IItemService>();
            _inventorySearchService = new Mock<IInventorySearchService>();
            _pricingEvaluatorService = new Mock<IPricingEvaluatorService>();
            _mapper = new Mock<IMapper>();
            _loadUserToEvalContextService = new Mock<LoadUserToEvalContextService>();
            _service = new CartProductServiceFake(_productService.Object, _inventorySearchService.Object, _pricingEvaluatorService.Object, _mapper.Object, _loadUserToEvalContextService.Object);
        }

        [Fact]
        public async Task GetProductsByIdsAsync_NumberProducts_ReturnsProducts()
        {
            // Arrange
            var productId1 = Guid.NewGuid().ToString();
            var productId2 = Guid.NewGuid().ToString();
            var ids = new[] { productId1, productId2 };

            var testCatalogProduct = new List<CatalogProduct>()
            {
                new CatalogProduct() { Id = productId1 },
                new CatalogProduct() { Id = productId2 }
            };

            _productService.Setup(x => x.GetByIdsAsync(ids, It.IsAny<string>(), null)).ReturnsAsync(testCatalogProduct.ToArray());

            //Act
            var result = await _service.GetProductsByIdsFakeAsync(ids);

            //Assert
            result.Should().HaveCount(2);
        }
    }
}
