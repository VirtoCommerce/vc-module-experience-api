using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Tests.Index
{
    public class SearchRequestBuilderTests
    {
        private readonly ElasticSearchRequestBuilder builder;
        private readonly Mock<ISearchPhraseParser> phraseParserMock;
        private readonly Mock<IAggregationConverter> aggregationConverterMock;

        public SearchRequestBuilderTests()
        {
            phraseParserMock = new Mock<ISearchPhraseParser>();
            //phraseParserMock
            //    .Setup(x => x.Parse(It.IsAny<string>()))
            //    .Returns(null);

            //builder = new ElasticSearchRequestBuilder(phraseParserMock.Object, aggregationConverterMock.Object);
        }
    }
}
