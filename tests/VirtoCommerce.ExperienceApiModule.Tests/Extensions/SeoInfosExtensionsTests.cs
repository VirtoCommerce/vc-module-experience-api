using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Extensions
{
    public class SeoInfosExtensionsTests
    {
        [Fact]
        public void GetBestMatchingSeoInfo_WithValidParameters_ReturnsSeoInfo()
        {
            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product2" },
                new SeoInfo { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product2" }
            };
            var storeId = "Store1";
            var cultureName = "en-US";
            var slug = "product1";

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, cultureName, slug);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Store1", result.StoreId);
            Assert.Equal("en-US", result.LanguageCode);
            Assert.Equal("product1", result.SemanticUrl);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithNullParameters_ReturnsNull()
        {
            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product2" },
                new SeoInfo { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product2" }
            };
            string storeId = null;
            string cultureName = null;
            string slug = null;

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, cultureName, slug);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithNonExistLang_ReturnsDefaultStoreLang()
        {
            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new SeoInfo { StoreId = "Store1", LanguageCode = null, SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store1", LanguageCode = "fr-FR", SemanticUrl = "product1" },
            };
            var storeId = "Store1";
            var cultureName = "de-DE";
            var slug = "product1";
            var defaultStoreLang = "en-US";

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, cultureName, defaultStoreLang, slug);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.LanguageCode, defaultStoreLang);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithNonExistLangAndStoreLang_ReturnsEmptyLang()
        {
            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new SeoInfo { StoreId = "Store1", LanguageCode = "fr-FR", SemanticUrl = "product1" },
                new SeoInfo { StoreId = "Store1", LanguageCode = null, SemanticUrl = "product1" },
            };
            var storeId = "Store1";
            var cultureName = "de-DE";
            var slug = "product1";
            var defaultStoreLang = "en-US";

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, cultureName, defaultStoreLang, slug);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.LanguageCode);
        }
    }
}
