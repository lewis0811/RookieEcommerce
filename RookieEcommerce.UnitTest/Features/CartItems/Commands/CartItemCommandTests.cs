using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.CartItems.Commands;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.UnitTest.Features.CartItems.Commands
{
    public class CartItemCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IProductVariantRepository> _productVariantRepositoryMock;

        public CartItemCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _productVariantRepositoryMock = new Mock<IProductVariantRepository>();
        }

        [Fact]
        public async Task AddCartItemCommand_ValidData_ShouldAddItemToCart()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();

            var command = new CreateCartItemCommand
            {
                CartId = cartId,
                ProductId = productId,
                ProductVariantId = variantId,
                Quantity = 2
            };

            var cart = Cart.Create(Guid.NewGuid().ToString());
            var product = Product.Create(
                "Reuzel Matte Clay",
                "Sáp tạo kiểu tóc matte clay từ Reuzel",
                249000,
                Guid.NewGuid(),
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );

            var variant = ProductVariant.Create(
                productId,
                "Hộp 113g",
                0,
                5,
                "Kích thước"
            );

            _cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(variantId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(variant);

            var handler = new CreateCartItemCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productVariantRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.Id);
            Assert.Equal(2, result.Quantity);

            _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCartItemCommand_CartNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();

            var command = new CreateCartItemCommand
            {
                CartId = cartId,
                ProductId = productId,
                ProductVariantId = variantId,
                Quantity = 1
            };

            _cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            var handler = new CreateCartItemCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productVariantRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCartItemCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();

            var command = new CreateCartItemCommand
            {
                CartId = cartId,
                ProductId = productId,
                ProductVariantId = variantId,
                Quantity = 1
            };

            var cart = Cart.Create(Guid.NewGuid().ToString());

            _cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new CreateCartItemCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productVariantRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateCartItemCommand_ValidData_ShouldUpdateQuantity()
        {
            // Arrange
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var command = new UpdateCartItemCommand
            {
                CartId = cartId,
                ItemId = cartItemId,
                Quantity = 3
            };

            var cart = Cart.Create(Guid.NewGuid().ToString());


            _cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var handler = new UpdateCartItemCommandHandler(
                _cartRepositoryMock.Object, 
                _unitOfWorkMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItemCommand_ValidData_ShouldRemoveItem()
        {
            // Arrange
            var cartItemId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();

            var command = new DeleteCartItemCommand
            {
                CartId = cartId,
                ItemId = cartItemId
            };

            var cart = Cart.Create(Guid.NewGuid().ToString());



            _cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var handler = new DeleteCardItemCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _cartRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 