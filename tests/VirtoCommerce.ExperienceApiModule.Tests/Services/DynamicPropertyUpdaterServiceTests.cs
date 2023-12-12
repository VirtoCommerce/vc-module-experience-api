using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Services
{
    public class DynamicPropertyUpdaterServiceTests
    {
        private static readonly DateTime DateTimeUtc = new(2022, 2, 3, 1, 2, 3, DateTimeKind.Utc);
        private static readonly string DateTimeIso8601UtcString = DateTimeUtc.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", DateTimeFormatInfo.InvariantInfo);

        public static readonly IEnumerable<object[]> ValuesMatchingValueType = new List<object[]>
        {
            //             Type                                Value         Expected value
            new object[] { DynamicPropertyValueType.ShortText, string.Empty, string.Empty },
            new object[] { DynamicPropertyValueType.ShortText, "test",       "test"       },
            new object[] { DynamicPropertyValueType.LongText,  "test",       "test"       },
            new object[] { DynamicPropertyValueType.Integer,   0,            0            },
            new object[] { DynamicPropertyValueType.Decimal,   0,            0m           },
            new object[] { DynamicPropertyValueType.Decimal,   0m,           0m           },
            new object[] { DynamicPropertyValueType.Boolean,   true,         true         },
            new object[] { DynamicPropertyValueType.DateTime,  DateTimeUtc,  DateTimeUtc  },
            // ISO 8601 string is a valid value for short/long text.
            // It will be converted to DateTime by scalar type and should be converted back as workaround 
            new object[] { DynamicPropertyValueType.ShortText, DateTimeUtc,  DateTimeIso8601UtcString },
            new object[] { DynamicPropertyValueType.LongText,  DateTimeUtc,  DateTimeIso8601UtcString },
        };

        public static readonly IEnumerable<object[]> ValuesNotMatchingValueType = new List<object[]>
        {
            //             Type                                Value         Expected exception
            new object[] { DynamicPropertyValueType.Integer,   "test",                   typeof(InvalidOperationException) },
            new object[] { DynamicPropertyValueType.Integer,   0m,                       typeof(InvalidOperationException) },
            new object[] { DynamicPropertyValueType.Decimal,   "test",                   typeof(InvalidOperationException) },
            new object[] { DynamicPropertyValueType.Boolean,   "test",                   typeof(InvalidOperationException) },
            new object[] { DynamicPropertyValueType.DateTime,  "test",                   typeof(FormatException) },
            new object[] { DynamicPropertyValueType.DateTime,  DateTimeUtc.ToString(),   typeof(FormatException) },
        };

        [MemberData(nameof(ValuesMatchingValueType))]
        [Theory]
        public void UpdateDynamicPropertyValues_ValueMatchingValueType_Parsed(DynamicPropertyValueType dynamicPropertyValueType, object value, object expectedValue)
        {
            // Arrange
            var dynamicProperties = GetDynamicProperties(dynamicPropertyValueType);
            var testObject = GetTestObject(dynamicProperties);
            var values = GetDynamicPropertyValues(dynamicPropertyValueType, value);

            var dynamicPropertiesUpdaterService = GetDynamicPropertyUpdaterService(dynamicProperties);

            // Act
            dynamicPropertiesUpdaterService.UpdateDynamicPropertyValues(testObject, values);

            // Assert
            Assert.Equal(expectedValue, testObject.DynamicProperties.First().Values.First().Value);
        }

        [MemberData(nameof(ValuesNotMatchingValueType))]
        [Theory]
        public async Task UpdateDynamicPropertyValues_ValueNotMatchingValueType_ThrowsException(DynamicPropertyValueType dynamicPropertyValueType, object value, Type expectedException)
        {
            // Arrange
            var dynamicProperties = GetDynamicProperties(dynamicPropertyValueType);
            var testObject = GetTestObject(dynamicProperties);
            var values = GetDynamicPropertyValues(dynamicPropertyValueType, value);

            var dynamicPropertiesUpdaterService = GetDynamicPropertyUpdaterService(dynamicProperties);

            // Act
            var action = () => dynamicPropertiesUpdaterService.UpdateDynamicPropertyValues(testObject, values);

            // Assert
            await Assert.ThrowsAsync(expectedException, action);
        }

        private static string GetDynamicPropertyName(DynamicPropertyValueType dynamicPropertyValueType)
        {
            return dynamicPropertyValueType.ToString();
        }

        private static ICollection<DynamicObjectProperty> GetDynamicProperties(DynamicPropertyValueType dynamicPropertyValueType)
        {
            var dynamicPropertyName = GetDynamicPropertyName(dynamicPropertyValueType);

            var dynamicProperties = new List<DynamicObjectProperty>
            {
                new() { Name = dynamicPropertyName, ValueType = dynamicPropertyValueType }
            };

            return dynamicProperties;
        }

        private static IHasDynamicProperties GetTestObject(ICollection<DynamicObjectProperty> dynamicProperties)
        {
            return new TestHasDynamicProperties { DynamicProperties = dynamicProperties };
        }

        private static IList<DynamicPropertyValue> GetDynamicPropertyValues(DynamicPropertyValueType dynamicPropertyValueType, object value)
        {
            var dynamicPropertyName = GetDynamicPropertyName(dynamicPropertyValueType);

            return new List<DynamicPropertyValue> { new() { Name = dynamicPropertyName, Value = value } };
        }

        private static IDynamicPropertyMetaDataResolver GetDynamicPropertyMetaDataResolver(IEnumerable<DynamicObjectProperty> dynamicProperties)
        {
            Func<string, bool> matchName = propertyName => dynamicProperties.Any(dynamicProperty => dynamicProperty.Name == propertyName);
            Func<string, DynamicObjectProperty> getByName = propertyName => dynamicProperties.FirstOrDefault(dynamicProperty => dynamicProperty.Name == propertyName);

            var mock = new Mock<IDynamicPropertyMetaDataResolver>();

            mock.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.Is<string>(propertyName => matchName(propertyName))))
                .ReturnsAsync<string, string, IDynamicPropertyMetaDataResolver, DynamicProperty>((_, propertyName) => getByName(propertyName));

            mock.Setup(x => x.GetByNameAsync(It.IsAny<string>(), It.Is<string>(propertyName => !matchName(propertyName))))
                .ReturnsAsync(() => null);

            return mock.Object;
        }

        private static IDynamicPropertyUpdaterService GetDynamicPropertyUpdaterService(IEnumerable<DynamicObjectProperty> dynamicProperties)
        {
            return new DynamicPropertyUpdaterService(GetDynamicPropertyMetaDataResolver(dynamicProperties));
        }

        private class TestHasDynamicProperties : IHasDynamicProperties
        {
            public string Id { get; set; }

            public string ObjectType { get; } = nameof(TestHasDynamicProperties);

            public ICollection<DynamicObjectProperty> DynamicProperties { get; set; } = new List<DynamicObjectProperty>();
        }
    }
}
