using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Middlewares;
using VirtoCommerce.XDigitalCatalog.Queries;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Middlewares
{
    public class EvalProductsTaxMiddlewareTest
    {
        [Fact]
        public void EvalProductsTaxMiddleware_TaxNotCalculatedWithoutResponseGroup_Success()
        {
            // Arrange
            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
            var genericPipelineLauncher = new Mock<IGenericPipelineLauncher>();

            var evalProductsTaxMiddleware = new EvalProductsTaxMiddleware(mapper.Object, taxProviderSearchService.Object, genericPipelineLauncher.Object);

            var response = new SearchProductResponse()
            {
                TotalCount = 1,
                Results = new List<ExpProduct>() {new ExpProduct() },
                Query = new SearchProductQuery() {CurrencyCode = "USD"}
            };

            //Act
            evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            // Assert
            taxProviderSearchService.Verify(x=>x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Never);
        }

        [Fact]
        public void EvalProductsTaxMiddleware_TaxNotCalculatedWithoutTaxProvider_Success()
        {
            // Arrange
            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
            taxProviderSearchService.Setup(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()))
                .ReturnsAsync(() => new TaxProviderSearchResult()
                {
                    TotalCount = 0,
                    Results = new List<TaxProvider>() 
                });
            var genericPipelineLauncher = new Mock<IGenericPipelineLauncher>();

            var evalProductsTaxMiddleware = new EvalProductsTaxMiddleware(mapper.Object, taxProviderSearchService.Object, genericPipelineLauncher.Object);

            var response = new SearchProductResponse()
            {
                TotalCount = 1,
                Results = new List<ExpProduct>() { new ExpProduct() },
                Query = new SearchProductQuery()
                {
                    CurrencyCode = "USD",
                    IncludeFields = new List<string>() {"price" }  //ResponseGroup.LoadPrices
                },
                
            };

            //Act
            Action action = () => evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            // Assert
            action.Should().NotThrow();
            taxProviderSearchService.Verify(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Once);
        }

        [Fact]
        public void EvalProductsTaxMiddleware_TaxRatesCalculated_Success()
        {
            // Arrange
            var taxProvider = new Mock<TaxProvider>();

            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
            taxProviderSearchService.Setup(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()))
                .ReturnsAsync(() => new TaxProviderSearchResult()
                {
                    TotalCount = 1,
                    Results = new List<TaxProvider>() {taxProvider.Object}
                });
            var genericPipelineLauncher = new Mock<IGenericPipelineLauncher>();

            var evalProductsTaxMiddleware = new EvalProductsTaxMiddleware(mapper.Object, taxProviderSearchService.Object, genericPipelineLauncher.Object);

            var response = new SearchProductResponse()
            {
                TotalCount = 1,
                Results = new List<ExpProduct>()
                {
                    new ExpProduct()
                },
                Query = new SearchProductQuery()
                {
                    CurrencyCode = "USD",
                    IncludeFields = new List<string>() { "price" }  //ResponseGroup.LoadPrices
                },

            };

            //Act
            Action action = () => evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            // Assert
            action.Should().NotThrow();
            taxProviderSearchService.Verify(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Once);
            taxProvider.Verify(x=>x.CalculateRates(It.IsAny<TaxEvaluationContext>()), Times.Once);
        }

        [Fact]
        public void EvalProductsTaxMiddleware_TaxesApply_Success()
        {
            // Arrange
            var currency = new Currency(Language.InvariantLanguage, "USD");

            var productPrice = new Mock<ProductPrice>(currency).Object;
            productPrice.TaxPercentRate = 0;
            productPrice.ListPrice = new Money(100m, currency);
            productPrice.DiscountAmount = new Money(0m, currency);
            productPrice.ProductId = "someId";

            var taxProvider = new Mock<TaxProvider>();
            taxProvider.Setup(x => x.CalculateRates(It.IsAny<TaxEvaluationContext>()))
                .Returns(() => new List<TaxRate>() {new TaxRate() {Currency = "USD", Rate = 50, Line = new TaxLine() {Id = "someId", Quantity = 0}}});

            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
            taxProviderSearchService.Setup(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()))
                .ReturnsAsync(() => new TaxProviderSearchResult()
                {
                    TotalCount = 0,
                    Results = new List<TaxProvider>() { taxProvider.Object }
                });
            var genericPipelineLauncher = new Mock<IGenericPipelineLauncher>();

            var evalProductsTaxMiddleware = new EvalProductsTaxMiddleware(mapper.Object, taxProviderSearchService.Object, genericPipelineLauncher.Object);

            var response = new SearchProductResponse()
            {
                TotalCount = 1,
                Results = new List<ExpProduct>()  {
                    new ExpProduct()
                    {
                       
                        AllPrices = new List<ProductPrice>()
                        {
                            productPrice
                        }

                    }
                },
                Query = new SearchProductQuery() {CurrencyCode = "USD", IncludeFields = new List<string>() { "price" }},
            };

            //Act
            Action action = () => evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            // Assert
            action.Should().NotThrow();
            taxProviderSearchService.Verify(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Once);
            taxProvider.Verify(x => x.CalculateRates(It.IsAny<TaxEvaluationContext>()), Times.Once);
            productPrice.TaxPercentRate.Should().Be(0.5m);
        }
    }
}
