//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoFixture;
//using FluentAssertions;
//using Moq;
//using VirtoCommerce.CatalogModule.Core.Services;
//using VirtoCommerce.XPurchase.Domain.Services;
//using VirtoCommerce.XPurchase.Models.Catalog;
//using VirtoCommerce.XPurchase.Models.Common;
//using Xunit;

//namespace VirtoCommerce.ExpirienceApiModule.XPurchase.Domain.Tests.Services
//{
//    public class ProductsRepositoryTests
//    {
//        private readonly Fixture _fixture;

//        private readonly Mock<IItemService> _itemsService;

//        private readonly ProductsRepository service;

//        public ProductsRepositoryTests()
//        {
//            _fixture = new Fixture();
//            _fixture.Register(() => new Language("en-US"));
//            _fixture.Register(() => new Currency(_fixture.Create<Language>(), "USD"));

//            _itemsService = new Mock<IItemService>();
//            _itemsService
//                .Setup(x => x.GetByIdsAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<CatalogModule.Core.Model.CatalogProduct>().ToArray()); // todo add full object and correct GetProductsAsync_ShouldReturnMockedProducts test

//            service = new ProductsRepository(_itemsService.Object);
//        }

//        [Fact]
//        public async Task GetProductsAsync_ShouldReturnMockedProducts()
//        {
//            // Arrange
//            var ids = _fixture.CreateMany<string>(3).ToArray();
//            var currency = _fixture.Create<Currency>();
//            var language = _fixture.Create<Language>();

//            // Act
//            var products = await service.GetProductsAsync(ids, currency, language);

//            // Assert
//            products.Should().BeEquivalentTo(Enumerable.Empty<Product>().ToList());
//        }

//        [Fact]
//        public async Task GetProductsAsync_ShouldReturnEmptyResultIfIdsIsNull()
//        {
//            // Arrange
//            var currency = _fixture.Create<Currency>();
//            var language = _fixture.Create<Language>();

//            // Act
//            var products = await service.GetProductsAsync(null, currency, language);

//            // Assert
//            products.Should().BeEquivalentTo(Enumerable.Empty<Product>().ToList());
//        }
//    }
//}
