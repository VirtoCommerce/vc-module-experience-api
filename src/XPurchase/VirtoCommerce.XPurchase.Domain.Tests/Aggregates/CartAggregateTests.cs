using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Aggregates
{
    public class CartAggregateTests : XPurchaseMoqHelper
    {
        private readonly CartAggregate aggregate;

        public CartAggregateTests()
        {
            aggregate = new CartAggregate(
                _marketingPromoEvaluatorMock.Object,
                _shoppingCartTotalsCalculatorMock.Object,
                _taxProviderSearchServiceMock.Object,
                _mapperMock.Object);

            var cart = GetCart();
            var member = GetMember();
            var store = GetStore();
            var currency = GetCurrency();

            aggregate.GrabCartAsync(cart, store, member, currency).GetAwaiter().GetResult();
        }

        #region UpdateCartComment

        [Fact]
        public async Task UpdateCartComment_ShouldSaveCommentToAggregate()
        {
            // Arrange
            var comment = _fixture.Create<string>();

            // Act
            await aggregate.UpdateCartComment(comment);

            // Assert
            aggregate.Cart.Should().NotBeNull();
            aggregate.Cart.Comment.Should().Be(comment);
        }

        #endregion UpdateCartComment

        #region AddItemAsync

        [Fact]
        public void AddItemAsync_ShouldThrowArgumentNullException_IfNewCartItemIsNull()
        {
            // Arrange
            NewCartItem newCartItem = null;

            // Act
            Action action = () => aggregate.AddItemAsync(newCartItem).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>("NewCartItem is null");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void AddItemAsync_ShouldThrow_IfQuantityLessOrEqualZero(int quantity)
        {
            // Arrange
            var productId = _fixture.Create<string>();
            var newCartItem = new NewCartItem(productId, quantity);

            var shoppingCart = _fixture.Create<ShoppingCart>();
            shoppingCart.Items = Enumerable.Empty<LineItem>().ToList();

            _cartProductServiceMock
                .Setup(x => x.GetCartProductsByIdsAsync(It.IsAny<CartAggregate>(), new[] { productId }))
                .ReturnsAsync(new List<CartProduct>() { new CartProduct(new CatalogProduct()) });

            // Act
            Action action = () => aggregate.AddItemAsync(newCartItem).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ValidationException>($"Quantity is {quantity}");
        }

        #endregion AddItemAsync

        #region ChangeItemPriceAsync

        [Fact]
        public async Task ChangeItemPriceAsync_LineItemNotFound_ShouldNotChangeItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            await cartAggregate.ChangeItemPriceAsync(new PriceAdjustment(
                _fixture.Create<string>(),
                _fixture.Create<decimal>()
            ));

            // Assert
            cartAggregate.Cart.Items.Should().Contain(x => x.ListPrice == lineItem.ListPrice && x.SalePrice == lineItem.SalePrice);
        }

        [Fact]
        public async Task ChangeItemPriceAsync_LineItemFound_ShouldChangeItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };
            var newPrice = _fixture.Create<decimal>();

            // Act
            await cartAggregate.ChangeItemPriceAsync(new PriceAdjustment(lineItem.Id, newPrice));

            // Assert
            cartAggregate.Cart.Items.Should().Contain(x => x.ListPrice == newPrice && x.SalePrice == newPrice);
        }

        #endregion ChangeItemPriceAsync

        #region ChangeItemQuantityAsync

        [Fact]
        public void ChangeItemQuantityAsync_LineItemNotFound_ShouldThrowValidationException()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            Action act = () => cartAggregate.ChangeItemQuantityAsync(new ItemQtyAdjustment(
                _fixture.Create<string>(),
                5,
                _fixture.Create<CartProduct>())).GetAwaiter().GetResult();

            // Assert
            act.Should().ThrowExactly<ValidationException>();
        }

        #endregion ChangeItemQuantityAsync

        #region ChangeItemCommentAsync

        [Fact]
        public async Task ChangeItemCommentAsync_LineItemNotFound_ShouldNotChangeItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            await cartAggregate.ChangeItemCommentAsync(new NewItemComment(
                _fixture.Create<string>(),
                _fixture.Create<string>()
            ));

            // Assert
            cartAggregate.Cart.Items.Should().Contain(x => x.Note == lineItem.Note && x.Id == lineItem.Id);
        }

        [Fact]
        public async Task ChangeItemCommentAsync_LineItemFound_ShouldChangeItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };
            var newComment = _fixture.Create<string>();

            // Act
            await cartAggregate.ChangeItemCommentAsync(new NewItemComment(lineItem.Id, newComment));

            // Assert
            cartAggregate.Cart.Items.Should().Contain(x => x.Note == newComment && x.Id == lineItem.Id);
        }

        #endregion ChangeItemCommentAsync

        #region RemoveItemAsync

        [Fact]
        public async Task RemoveItemAsync_LineItemNotFound_ShouldNotRemoveItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            await cartAggregate.RemoveItemAsync(_fixture.Create<string>());

            // Assert
            cartAggregate.Cart.Items.Should().Contain(x => x.Id == lineItem.Id);
        }

        [Fact]
        public async Task RemoveItemAsync_LineItemFound_ShouldRemoveItem()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            await cartAggregate.RemoveItemAsync(lineItem.Id);

            // Assert
            cartAggregate.Cart.Items.Should().NotContain(x => x.Id == lineItem.Id);
        }

        #endregion RemoveItemAsync

        #region AddCouponAsync

        [Fact]
        public async Task AddCouponAsync_CouponNotFound_ShouldAddCoupon()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var coupon = _fixture.Create<string>();
            cartAggregate.Cart.Coupons = new List<string> { coupon };
            var newCoupon = _fixture.Create<string>();

            // Act
            await cartAggregate.AddCouponAsync(newCoupon);

            // Assert
            cartAggregate.Cart.Coupons.Should().Contain(newCoupon);
            cartAggregate.Cart.Coupons.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddCouponAsync_CouponFound_ShouldContaintOnlyOneCouponWIthSameCode()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var coupon = _fixture.Create<string>();
            cartAggregate.Cart.Coupons = new List<string> { coupon };

            // Act
            await cartAggregate.AddCouponAsync(coupon);

            // Assert
            cartAggregate.Cart.Coupons.Should().ContainSingle(coupon);
        }

        #endregion AddCouponAsync

        #region RemoveCouponAsync

        [Fact]
        public async Task RemoveCouponAsync_CouponFound_ShouldRemoveCoupon()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var firstCoupon = _fixture.Create<string>();
            var secondCoupon = _fixture.Create<string>();

            cartAggregate.Cart.Coupons = new List<string>
            {
                firstCoupon,
                secondCoupon,
            };

            // Act
            await cartAggregate.RemoveCouponAsync(firstCoupon);

            // Assert
            cartAggregate.Cart.Coupons.Should().ContainSingle(secondCoupon);
        }

        [Fact]
        public async Task RemoveCouponAsync_CouponNotFound_ShouldPass()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var firstCoupon = _fixture.Create<string>();
            var notExistingCoupon = _fixture.Create<string>();

            cartAggregate.Cart.Coupons = new List<string> { firstCoupon };

            // Act
            await cartAggregate.RemoveCouponAsync(notExistingCoupon);

            // Assert
            cartAggregate.Cart.Coupons.Should().ContainSingle(firstCoupon);
        }

        [Fact]
        public async Task RemoveCouponAsync_Null_ShouldRemoveAllCoupons()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var firstCoupon = _fixture.Create<string>();
            var secondCoupon = _fixture.Create<string>();

            cartAggregate.Cart.Coupons = new List<string>
            {
                firstCoupon,
                secondCoupon,
            };

            // Act
            await cartAggregate.RemoveCouponAsync(null);

            // Assert
            cartAggregate.Cart.Coupons.Should().BeEmpty();
        }

        #endregion RemoveCouponAsync

        #region ClearAsync

        [Fact]
        public async Task ClearAsync_ShouldClearCartItems()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();
            var lineItem = _fixture.Create<LineItem>();
            cartAggregate.Cart.Items = new List<LineItem> { lineItem };

            // Act
            await cartAggregate.ClearAsync();

            // Assert
            cartAggregate.Cart.Items.Should().HaveCount(0);
        }

        #endregion ClearAsync

        #region AddShipmentAsync

        // TODO: Write tests

        #endregion AddShipmentAsync

        #region RemoveShipmentAsync

        // TODO: Write tests

        #endregion RemoveShipmentAsync

        #region AddPaymentAsync

        // TODO: Write tests

        #endregion AddPaymentAsync

        #region MergeWithCartAsync

        // TODO: Write tests

        #endregion MergeWithCartAsync

        #region ValidateAsync

        // TODO: Write tests

        #endregion ValidateAsync

        #region ValidateCouponAsync

        // TODO: Write tests

        #endregion ValidateCouponAsync

        #region EvaluatePromotionsAsync(PromotionEvaluationContext evalContext)

        // TODO: Write tests

        #endregion EvaluatePromotionsAsync(PromotionEvaluationContext evalContext)

        #region RecalculateAsync

        // TODO: Write tests

        #endregion RecalculateAsync
    }
}
