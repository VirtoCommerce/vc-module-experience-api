using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Schemas;
using VirtoCommerce.XDigitalCatalog.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Shemas
{
    public class ProductTypeTests : XDigitalCatalogMoqHelper
    {
        private readonly ProductType _productType;

        public ProductTypeTests()
        {
            _productType = new ProductType(_mediatorMock.Object, _dataLoaderContextAccessorMock.Object);
        }

        [Fact]
        public void ProductType_ShouldHavePropperFieldAmount()
        {
            // Assert
            _productType.Fields.Should().HaveCount(25);
        }

        #region Properties

        [Fact]
        public void ProductType_Properties_ShouldResolve()
        {
            // Arrange
            var propValues = _fixture
                .Build<PropertyValue>()
                .With(x => x.LanguageCode, CULTURE_NAME)
                .With(x => x.Property, default(Property))
                .CreateMany()
                .ToList();

            var product = new ExpProduct
            {
                IndexedProduct = new CatalogProduct
                {
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Values = propValues
                        }
                    }
                }
            };
            var resolveContext = new ResolveFieldContext()
            {
                Source = product,
                UserContext = new Dictionary<string, object>
                {
                    { "cultureName", CULTURE_NAME }
                }
            };

            // Act
            var result = _productType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("properties")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<List<Property>>();
            ((List<Property>)result).Count.Should().Be(propValues.Count);
        }

        [Fact]
        public void ProductType_Properties_ShouldFilterPropertiesByCultureName()
        {
            // Arrange
            var propValues = _fixture
                .Build<PropertyValue>()
                .With(x => x.LanguageCode, CULTURE_NAME)
                .With(x => x.Property, default(Property))
                .With(x => x.Alias, "i_grouped")
                .CreateMany()
                .ToList();

            propValues.First().LanguageCode = "de-De";

            var product = new ExpProduct
            {
                IndexedProduct = new CatalogProduct
                {
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Values = propValues
                        }
                    }
                }
            };
            var resolveContext = new ResolveFieldContext()
            {
                Source = product,
                UserContext = new Dictionary<string, object>
                {
                    { "cultureName", CULTURE_NAME }
                }
            };

            // Act
            var result = _productType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("properties")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<List<Property>>();
            ((List<Property>)result).Count.Should().Be(1);
        }

        [Fact]
        public void ProductType_Properties_SelectedLanguageNotFound_ShouldReturnFlatList()
        {
            // Arrange
            var propValue = _fixture
                .Build<PropertyValue>()
                .With(x => x.LanguageCode, "de-De")
                .With(x => x.Property, default(Property))
                .Create();

            var product = new ExpProduct
            {
                IndexedProduct = new CatalogProduct
                {
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Values = new List<PropertyValue>
                            {
                                propValue
                            }
                        }
                    }
                }
            };
            var resolveContext = new ResolveFieldContext()
            {
                Source = product,
                UserContext = new Dictionary<string, object>
                {
                    { "cultureName", CULTURE_NAME }
                }
            };

            // Act
            var result = _productType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("properties")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<List<Property>>();
            ((List<Property>)result).Count.Should().Be(1);
            ((List<Property>)result).First().Values.First().Should().BeEquivalentTo(propValue);
        }

        [Fact]
        public void ProductType_Properties_NoLocalization_ShouldGetDefaultValue()
        {
            // Arrange
            var alias = "i_grouped";
            var propValues = _fixture
                .Build<PropertyValue>()
                .With(x => x.LanguageCode, "de-De")
                .With(x => x.Property, default(Property))
                .With(x => x.Alias, alias)
                .CreateMany()
                .ToList();

            var product = new ExpProduct
            {
                IndexedProduct = new CatalogProduct
                {
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Values = propValues
                        }
                    }
                }
            };
            var resolveContext = new ResolveFieldContext()
            {
                Source = product,
                UserContext = new Dictionary<string, object>
                {
                    { "cultureName", CULTURE_NAME }
                }
            };

            // Act
            var result = _productType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("properties")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<List<Property>>();
            ((List<Property>)result).Count.Should().Be(1);
            ((List<Property>)result).Any(p => p.Values.Any(pv => pv.Value.ToString().EqualsInvariant(alias))).Should().BeTrue();
        }

        #endregion Properties
    }
}
