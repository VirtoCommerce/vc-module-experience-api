using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
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

            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
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
                    CurrencyCode = "USD"
                }
            };

            evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            taxProviderSearchService.Verify(x=>x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Never);
        }

        [Fact]
        public void EvalProductsTaxMiddleware_TaxNotCalculatedWithoutTaxProvider_Success()
        {
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
                Results = new List<ExpProduct>()
                {
                    new ExpProduct()
                },
                Query = new SearchProductQuery()
                {
                    CurrencyCode = "USD",
                    IncludeFields = new List<string>() {"price" }  //ResponseGroup.LoadPrices
                },
                
            };

            Action action = () => evalProductsTaxMiddleware.Run(response, resp => Task.CompletedTask);

            action.Should().NotThrow();
            taxProviderSearchService.Verify(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Once);
        }

        [Fact]
        public void EvalProductsTaxMiddleware_TaxRatesCalculated_Success()
        {

            var taxProvider = new Mock<TaxProvider>();

            var mapper = new Mock<IMapper>();
            var taxProviderSearchService = new Mock<ITaxProviderSearchService>();
            taxProviderSearchService.Setup(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()))
                .ReturnsAsync(() => new TaxProviderSearchResult()
                {
                    TotalCount = 0,
                    Results = new List<TaxProvider>() {taxProvider.Object}
                });
            var genericPipelineLauncher = new Mock<IGenericPipelineLauncher>();

            var q = new EvalProductsTaxMiddleware(mapper.Object, taxProviderSearchService.Object, genericPipelineLauncher.Object);

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

            Action action = () => q.Run(response, resp => Task.CompletedTask);

            action.Should().NotThrow();
            taxProviderSearchService.Verify(x => x.SearchTaxProvidersAsync(It.IsAny<TaxProviderSearchCriteria>()), Times.Once);
            taxProvider.Verify(x=>x.CalculateRates(It.IsAny<TaxEvaluationContext>()), Times.Once);
        }

    }
}
