using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;


namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class MoveWishListItemCommandHandlerTests : XPurchaseMoqHelper
    {
        [Fact]
        public async Task Handle_LineItemNotFound_DestinationItemsEmpty()
        {
            // Arrange
            var sourceAggregare = GetValidCartAggregate();
            var destinationAggregate = GetValidCartAggregate();

            var sourceListId = sourceAggregare.Cart.Id;
            var destinaitonListId = destinationAggregate.Cart.Id;

            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            cartAggregateRepositoryMock
                .Setup(x => x.GetCartByIdAsync(It.Is<string>(x => x == sourceListId), It.IsAny<string>()))
                .ReturnsAsync(sourceAggregare);
            cartAggregateRepositoryMock
                .Setup(x => x.GetCartByIdAsync(It.Is<string>(x => x == destinaitonListId), It.IsAny<string>()))
                .ReturnsAsync(destinationAggregate);

            var handler = new MoveWishListItemCommandHandler(cartAggregateRepositoryMock.Object);

            var request = new MoveWishlistItemCommand(sourceListId, destinaitonListId, _fixture.Create<string>());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Cart.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_LineItemFound_ItemCopied()
        {
            // Arrange
            var sourceAggregare = GetValidCartAggregate();

            var destinationAggregate = GetValidCartAggregate();
            destinationAggregate.ValidationRuleSet = new [] { "default" };

            var sourceListId = sourceAggregare.Cart.Id;
            var destinaitonListId = destinationAggregate.Cart.Id;

            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            cartAggregateRepositoryMock
                .Setup(x => x.GetCartByIdAsync(It.Is<string>(x => x == sourceListId), It.IsAny<string>()))
                .ReturnsAsync(sourceAggregare);
            cartAggregateRepositoryMock
                .Setup(x => x.GetCartByIdAsync(It.Is<string>(x => x == destinaitonListId), It.IsAny<string>()))
                .ReturnsAsync(destinationAggregate);

            var lineItem = _fixture.Create<LineItem>();
            sourceAggregare.Cart.Items = new List<LineItem> { lineItem };

            destinationAggregate.Cart.Items = new List<LineItem>();

            var productId = lineItem.ProductId;
            _cartProductServiceMock
                .Setup(x => x.GetCartProductsByIdsAsync(It.IsAny<CartAggregate>(), new[] { productId }))
                .ReturnsAsync(new List<CartProduct> { new CartProduct(new CatalogProduct { Id = productId }) });

            _mapperMock.Setup(m => m.Map<LineItem>(It.IsAny<object>())).Returns<object>((x) =>
            {
                return x is CartProduct cartProduct
                    ? new LineItem { ProductId = cartProduct.Id }
                    : null;
            });

            var request = new MoveWishlistItemCommand(sourceListId, destinaitonListId, lineItem.Id);
            var handler = new MoveWishListItemCommandHandler(cartAggregateRepositoryMock.Object);

            // Act
            destinationAggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            sourceAggregare.Cart.Items.Should().BeEmpty();
            destinationAggregate.Cart.Items.Should().ContainSingle(x => x.ProductId == productId);
        }
    }
}
