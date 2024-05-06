using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Validators
{
    public class ItemQtyAdjustmentValidatorTests : XPurchaseMoqHelper
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task ValidateItemQtyAdjustment_NewQuantityLessOrEqualZero_Invalid(int newQuantity)
        {
            // Arrange
            var item = _fixture.Create<ItemQtyAdjustment>();
            item.NewQuantity = newQuantity;

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "NewQuantity" && x.ErrorCode.Contains("GreaterThanValidator"));
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_LineItemIdIsNull_Invalid()
        {
            // Arrange
            var item = _fixture.Create<ItemQtyAdjustment>();
            item.LineItemId = null;

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "LineItemId" && x.ErrorCode.Contains("NotNullValidator"));
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_CartProductIsNull_Invalid()
        {
            // Arrange
            var item = _fixture.Create<ItemQtyAdjustment>();
            item.CartProduct = null;

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "CartProduct" && x.ErrorCode.Contains("NotNullValidator"));
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_LineItemNotFound_Invalid()
        {
            // Arrange
            var item = _fixture.Create<ItemQtyAdjustment>();
            item.LineItem = null;
            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item, options => options.IncludeRuleSets("strict"));
            var expected = CartErrorDescriber.LineItemWithGivenIdNotFound(new LineItem
            {
                Id = item.LineItemId
            });

            // Assert
            result.Errors.Should().Contain(x =>
                x.ErrorMessage == expected.ErrorMessage &&
                x.ErrorCode == expected.ErrorCode);
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_LineItemIsReadOnly_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            var lineItem = _fixture
                .Build<LineItem>()
                .With(x => x.IsReadOnly, true)
                .Create();

            aggregate.Cart.Items = new List<LineItem> { lineItem };

            var item = _fixture
                .Build<ItemQtyAdjustment>()
                .With(x => x.LineItemId, lineItem.Id)
                .With(x => x.LineItem, lineItem)
                .Create();

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item, options => options.IncludeRuleSets("strict"));
            var expected = CartErrorDescriber.LineItemIsReadOnly(lineItem);

            // Assert
            result.Errors.Should().Contain(x =>
                x.ErrorMessage == expected.ErrorMessage &&
                x.ErrorCode == expected.ErrorCode);
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_ProductIsUnavailable_Invalid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            var lineItem = _fixture.Create<LineItem>();

            aggregate.Cart.Items = new List<LineItem> { lineItem };

            var item = _fixture
                .Build<ItemQtyAdjustment>()
                .With(x => x.LineItemId, lineItem.Id)
                .Create();

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item, options => options.IncludeRuleSets("strict"));
            var expected = CartErrorDescriber.ProductQtyInsufficientError(
                item.CartProduct,
                item.NewQuantity,
                item.CartProduct.AvailableQuantity);

            // Assert
            result.Errors.Should().Contain(x =>
                x.ErrorMessage == expected.ErrorMessage &&
                x.ErrorCode == expected.ErrorCode);
        }

        [Fact]
        public async Task ValidateItemQtyAdjustment_ProductAvailable_Valid()
        {
            // Arrange
            var aggregate = GetValidCartAggregate();

            var lineItem = _fixture.Create<LineItem>();
            lineItem.IsGift = false;

            aggregate.Cart.Items = new List<LineItem> { lineItem };

            var item = _fixture
                .Build<ItemQtyAdjustment>()
                .With(x => x.LineItemId, lineItem.Id)
                .With(x => x.NewQuantity, lineItem.Quantity) // Request max quantity of line item
                .Create();

            var validator = new ItemQtyAdjustmentValidator();

            // Act
            var result = await validator.ValidateAsync(item);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
