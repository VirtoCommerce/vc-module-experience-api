using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Tests.Helpers;
using Xunit;
using PropertyType = VirtoCommerce.XDigitalCatalog.Schemas.PropertyType;

namespace VirtoCommerce.XDigitalCatalog.Tests.Shemas
{
    public class PropertyTypeTests : XDigitalCatalogMoqHelper
    {
        private readonly PropertyType _propertyType;

        public PropertyTypeTests()
        {
            _propertyType = new PropertyType();
        }

        [Fact]
        public void PropertyType_ShouldHavePropperFieldAmount()
        {
            // Assert
            _propertyType.Fields.Should().HaveCount(9);
        }

        [Fact]
        public void PropertyType_Properties_ShouldFilterPropertiesByCultureName()
        {
            // Arrange
            var label = _fixture.Create<string>();

            var product = new Property
            {
                Name = _fixture.Create<string>(),
                DisplayNames = new List<PropertyDisplayName>
                {
                    new PropertyDisplayName
                    {
                        LanguageCode = CULTURE_NAME,
                        Name = label
                    },
                    new PropertyDisplayName
                    {
                        LanguageCode = "de-De",
                        Name = _fixture.Create<string>()
                    },
                }
            };

            var resolveContext = new ResolveFieldContext
            {
                Source = product,
                UserContext = new Dictionary<string, object>
                {
                    { "cultureName", CULTURE_NAME }
                }
            };

            // Act
            var result = _propertyType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("label")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Be(label);
        }

        [Fact]
        public void PropertyType_Properties_CultureNameNotPassed_ShouldReturnSourceName()
        {
            // Arrange
            var label = _fixture.Create<string>();

            var product = new Property
            {
                Name = label,
                DisplayNames = new List<PropertyDisplayName>
                {
                    new PropertyDisplayName
                    {
                        LanguageCode = CULTURE_NAME,
                        Name = _fixture.Create<string>()
                    },
                }
            };

            var resolveContext = new ResolveFieldContext
            {
                Source = product,
                UserContext = new Dictionary<string, object>()
            };

            // Act
            var result = _propertyType.Fields.FirstOrDefault(x => x.Name.EqualsInvariant("label")).Resolver.Resolve(resolveContext);

            // Assert
            result.Should().BeOfType<string>();
            ((string)result).Should().Be(label);
        }
    }
}
