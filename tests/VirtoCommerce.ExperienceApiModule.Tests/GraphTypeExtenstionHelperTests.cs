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
            var targetType = GraphTypeExtenstionHelper.GetActualComplexType<FooType>();

            // Assert
            targetType.Name.Should().Be(nameof(FooTypeExtended));
        }

        [Fact]
        public void GetComplexTypeTwoLevels_HasOverriddenType_OverriddenTypeReturned()
        {
            // Arrange
            AbstractTypeFactory<IGraphType>.OverrideType<FooType, FooTypeExtended>();

            // Act
            var targetType = GraphTypeExtenstionHelper.GetActualComplexType<FooComplex<FooType>>();

            // Assert
            typeof(FooComplex<FooType>).GenericTypeArguments.Should().OnlyContain(x => x.Name.EqualsInvariant(nameof(FooType)));
            targetType.GenericTypeArguments.Should().OnlyContain(x => x.Name.EqualsInvariant(nameof(FooTypeExtended)));
        }

        [Fact]
        public void GetComplexTypeThreeLevels_HasOverriddenType_OverriddenTypeReturned()
        {
            // Arrange
            AbstractTypeFactory<IGraphType>.OverrideType<FooType, FooTypeExtended>();

            // Act
            var targetType = GraphTypeExtenstionHelper.GetActualComplexType<FooComplex2<FooComplex<FooType>>>();

            // Assert
            typeof(FooComplex2<FooComplex<FooType>>).GenericTypeArguments[0].GenericTypeArguments.Should().OnlyContain(x => x.Name.EqualsInvariant(nameof(FooType)));
            targetType.GenericTypeArguments[0].GenericTypeArguments.Should().OnlyContain(x => x.Name.EqualsInvariant(nameof(FooTypeExtended)));
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

        public class FooComplex<T> : FooComplex where T : GraphType
        {
        }

        public class FooComplex2 : GraphType
        {
        }

        public class FooComplex2<T> : FooComplex where T : GraphType
        {
        }
    }
}
