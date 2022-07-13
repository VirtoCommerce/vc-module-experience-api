using System.Collections.Generic;
using System.Linq;
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
    public class CartLineItemValidatorTests : XPurchaseMoqHelper
    {
        public CartLineItemValidatorTests()
        {
            _context.AllCartProducts = new List<CartProduct>()
            {
                _fixture.Create<CartProduct>()
            };
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_Valid()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            // Act
            var validator = new CartLineItemValidator();
            var result = await validator.ValidateAsync(new LineItemValidationContext
            {
                AllCartProducts = _context.AllCartProducts,
                LineItem = item
            }, options => options.IncludeRuleSets("strict"));

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Product_null")]
        [InlineData("Product_IsActive_false")]
        [InlineData("Product_IsBuyable_false")]
        public async Task ValidateCartLineItem_RuleSetStrict_UnavailableError(string scenario)
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var cartProduct = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = cartProduct.Id;

            switch (scenario)
            {
                case "Product_null":
                    cartProduct.Id = _fixture.Create<string>();
                    break;

                case "Product_IsActive_false":
                    cartProduct.Product.IsActive = false;
                    break;

                case "Product_IsBuyable_false":
                    cartProduct.Product.IsBuyable = false;
                    break;
            }

            // Act
            var validator = new CartLineItemValidator();
            var result = await validator.ValidateAsync(new LineItemValidationContext
            {
                LineItem = item,
                AllCartProducts = _context.AllCartProducts
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(x => x.ErrorCode == "CART_PRODUCT_UNAVAILABLE");
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_QuantityError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.Quantity = InStockQuantity * 2;

            // Act
            var validator = new CartLineItemValidator();
            var result = await validator.ValidateAsync(new LineItemValidationContext
            {
                LineItem = item,
                AllCartProducts = _context.AllCartProducts
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_ProductMinQuantityError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.Quantity = 1;
            lineItem.Product.MinQuantity = 5;

            // Act
            var validator = new CartLineItemValidator();
            var result = await validator.ValidateAsync(new LineItemValidationContext
            {
                LineItem = item,
                AllCartProducts = _context.AllCartProducts
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorCode == "PRODUCT_MIN_QTY");
        }

        [Fact]
        public async Task ValidateCartLineItem_RuleSetStrict_ProductMaxQuantityError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.Quantity = 10;
            lineItem.Product.MaxQuantity = 5;


            // Act
            var validator = new CartLineItemValidator();
            var result = await validator.ValidateAsync(new LineItemValidationContext
            {
                LineItem = item,
                AllCartProducts = _context.AllCartProducts
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorCode == "PRODUCT_MAX_QTY");
        }


        [Fact]
        public async Task ValidateCartLineItem_PriceChanged_PriceError()
        {
            // Arrange
            var item = _fixture.Create<LineItem>();
            var lineItem = _context.AllCartProducts.FirstOrDefault();
            item.ProductId = lineItem.Id;

            item.SalePrice /= 2m;

            // Act
            var validator = new CartLineItemPriceChangedValidator();
            var result = await validator.ValidateAsync(new CartLineItemPriceChangedValidationContext
            {
                LineItem = item,
                CartProducts = _context.AllCartProducts.ToDictionary(x => x.Id)
            });

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().Contain(x => x.ErrorCode == "PRODUCT_PRICE_CHANGED");
        }
    }
}
