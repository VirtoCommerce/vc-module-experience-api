using GraphQL.Language.AST;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Schemas.ScalarTypes;
using Xunit;

namespace VirtoCommerce.XDigitalCatalog.Tests.Schemas.ScalarTypes
{
    public class PropertyValueGraphTypeTests
    {
        private static readonly GeoPoint GeoPoint = new(54.7249865, 20.5428642);

        private readonly PropertyValueGraphType _propertyValueGraphType = new();

        [Fact]
        public void ParseLiteral_GeoPoint_Parsed()
        {
            // Arrange
            var geoPoint = GeoPoint.ToString();

            // Act
            var result = _propertyValueGraphType.ParseLiteral(new StringValue(geoPoint));

            // Assert
            Assert.Equal(geoPoint, result);
            Assert.Equal(typeof(string), result?.GetType());
        }

        [Fact]
        public void ParseValue_GeoPoint_Parsed()
        {
            // Arrange
            var geoPoint = GeoPoint.ToString();

            // Act
            var result = _propertyValueGraphType.ParseValue(geoPoint);

            // Assert
            Assert.Equal(geoPoint, result);
            Assert.Equal(typeof(string), result?.GetType());
        }
    }
}
