using FluentAssertions;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests
{
    public class GraphTypeExtenstionHelperTests
    {
        [Fact]
        public void GetActualType_HasOverriddenType_OverriddenTypeReturned()
        {
            // Arrange
            AbstractTypeFactory<IGraphType>.OverrideType<FooType, FooTypeExtended>();

            // Act
            var targetType = GraphTypeExtenstionHelper.GetActualType<FooType>();

            // Assert
            targetType.Name.Should().Be(nameof(FooTypeExtended));
        }

        [Fact]
        public void GetComplexType_HasOverriddenType_OverriddenTypeReturned()
        {
            // Arrange
            AbstractTypeFactory<IGraphType>.OverrideType<FooType, FooTypeExtended>();

            // Act
            var targetType = GraphTypeExtenstionHelper.GetComplexType<FooCompltex<FooType>>();

            // Assert
            targetType.GenericTypeArguments.Should().OnlyContain(x => x.Name.EqualsInvariant(nameof(FooTypeExtended)));
        }

        public class FooType : GraphType
        {
        }

        public class FooTypeExtended : FooType
        {
        }

        public class FooComplex : GraphType
        {
        }

        public class FooCompltex<T> : FooComplex where T : GraphType
        {
        }
    }
}
