//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoFixture;
//using FluentAssertions;
//using VirtoCommerce.XPurchase.Domain.Services;
//using VirtoCommerce.XPurchase.Models.Catalog;
//using VirtoCommerce.XPurchase.Models.Common;
//using Xunit;

//namespace VirtoCommerce.ExpirienceApiModule.XPurchase.Domain.Tests.Services
//{
//    public class CatalogServiceTests
//    {
//        private readonly Fixture _fixture;

//        private readonly CatalogService service = new CatalogService();

//        public CatalogServiceTests()
//        {
//            _fixture = new Fixture();
//            _fixture.Register(() => new Language("en-US"));
//            _fixture.Register(() => new Currency(_fixture.Create<Language>(), "USD"));
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
//            products.Should().BeEquivalentTo(new List<Product>()
//            {
//                new Product(currency, language)
//                {
//                    Id = ids?[0] ?? "777",
//                    Name = "Hi! I Mocked!",
//                },
//                new Product(currency, language)
//                {
//                    Id = ids?[1] ?? "888",
//                    Name = "Hi! I Mocked!",
//                }
//            });
//        }
//    }
//}
