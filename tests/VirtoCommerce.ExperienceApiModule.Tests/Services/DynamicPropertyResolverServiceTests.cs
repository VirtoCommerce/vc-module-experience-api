using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers.Stubs;
using VirtoCommerce.Platform.Core.DynamicProperties;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Services
{
    public class DynamicPropertyResolverServiceTests
    {
        [Theory]
        [MemberData(nameof(GetPropertyData))]
        public async Task LoadDynamicPropertyValuesTest(IHasDynamicProperties entity, List<DynamicPropertyObjectValue> expectedResults)
        {
            // Arrange
            var dynamicPropertySearchServiceMock = new Mock<IDynamicPropertySearchService>();
            dynamicPropertySearchServiceMock
                .Setup(x => x.SearchDynamicPropertiesAsync(It.Is<DynamicPropertySearchCriteria>(x => x.ObjectType == entity.ObjectType)))
                .Returns(Task.FromResult(new DynamicPropertySearchResult { Results = Properties }));

            // Act
            var target = new DynamicPropertyResolverService(dynamicPropertySearchServiceMock.Object);
            var result = await target.LoadDynamicPropertyValues(entity, "en-US");

            // Assert
            foreach (var expected in expectedResults)
            {
                result.Should().ContainSingle(x => x.PropertyName == expected.PropertyName);
            }
        }

        private static DynamicProperty Property1 = new DynamicProperty
        {
            Id = "PropertyId_1",
            Name = "PropertyName_1",
        };

        private static DynamicProperty Property2 = new DynamicProperty
        {
            Id = "PropertyId_2",
            Name = "PropertyName_2",
        };

        // properties meta data
        private static List<DynamicProperty> Properties = new List<DynamicProperty> { Property1, Property2 };

        public static IEnumerable<object[]> GetPropertyData { get; } = new List<object[]>
        {
            // should return empty properties with metadata for an entity with no dynamic property values
            new object[]
            {
                // entity
                new StubDynamicPropertiesOwner(),
                // results
                new List<DynamicPropertyObjectValue>
                {
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property1.Id,
                        PropertyName = Property1.Name
                    },
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property2.Id,
                        PropertyName = Property2.Name
                    },
                },
            },

            // should compare properties by id
            new object[]
            {
                // entity
                new StubDynamicPropertiesOwner(new List<DynamicObjectProperty>
                {
                    new DynamicObjectProperty
                    {
                        Values = new List<DynamicPropertyObjectValue>
                        {
                            new DynamicPropertyObjectValue
                            {
                                PropertyId = Property1.Id
                            }
                        }
                    },
                    new DynamicObjectProperty
                    {
                        Values = new List<DynamicPropertyObjectValue>
                        {
                            new DynamicPropertyObjectValue
                            {
                                PropertyId = Property2.Id
                            }
                        }
                    },
                }),
                // results
                new List<DynamicPropertyObjectValue>
                {
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property1.Id,
                        PropertyName = Property1.Name
                    },
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property2.Id,
                        PropertyName = Property2.Name
                    },
                },
            },

            // should compare properties by name
            new object[]
            {
                // entity
                new StubDynamicPropertiesOwner(new List<DynamicObjectProperty>
                {
                    new DynamicObjectProperty
                    {
                        Name = Property1.Name,
                        Values = new List<DynamicPropertyObjectValue>
                        {
                            new DynamicPropertyObjectValue
                            {
                                PropertyId = Guid.NewGuid().ToString(),
                                PropertyName = Property1.Name
                            }
                        }
                    },
                    new DynamicObjectProperty
                    {
                        Name = Property2.Name,
                        Values = new List<DynamicPropertyObjectValue>
                        {
                            new DynamicPropertyObjectValue
                            {
                                PropertyId = Guid.NewGuid().ToString(),
                                PropertyName = Property2.Name
                            }
                        }
                    },
                }),
                // results
                new List<DynamicPropertyObjectValue>
                {
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property1.Id,
                        PropertyName = Property1.Name
                    },
                    new DynamicPropertyObjectValue
                    {
                        PropertyId = Property2.Id,
                        PropertyName = Property2.Name
                    },
                },
            },
        };
    }
}
