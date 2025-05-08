using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductImages.Commands;
using RookieEcommerce.Domain.Entities;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.ProductImages.Commands
{
    public class ProductImageCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductImageCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productImageRepositoryMock = new Mock<IProductImageRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task CreateProductImageCommand_ValidData_ShouldCreateImage()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new CreateProductImageCommand
            {
                ProductId = productId,
                ImageUrl = "https://waxshop.vn/wp-content/uploads/2024/01/reuzel-matte-clay-1.jpg",
            };

            var product = Product.Create(
                "Reuzel Matte Clay",
                "Sáp tạo kiểu tóc matte clay từ Reuzel",
                249000,
                Guid.NewGuid(),
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new CreateProductImageCommandHandler(
                _unitOfWorkMock.Object,
                _productImageRepositoryMock.Object,
                _productRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(command.ImageUrl, result.ImageUrl);

            _productImageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProductImage>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductImageCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new CreateProductImageCommand
            {
                ProductId = productId,
                ImageUrl = "https://waxshop.vn/wp-content/uploads/2024/01/reuzel-matte-clay-1.jpg",
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new CreateProductImageCommandHandler(
                _unitOfWorkMock.Object,
                _productImageRepositoryMock.Object,
                _productRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProductImageCommand_ValidData_ShouldUpdateImage()
        {
            // Arrange
            var imageId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new UpdateProductImageCommand
            {
                Id = imageId,
                AltText = "https://waxshop.vn/wp-content/uploads/2024/01/reuzel-matte-clay-2.jpg",
                SortOrder = 2,
                IsPrimary = false
            };

            var productImage = ProductImage.Create(
                productId,
                "https://waxshop.vn/wp-content/uploads/2024/01/reuzel-matte-clay-1.jpg",
                "alt text"
            );

            _productImageRepositoryMock.Setup(x => x.GetByIdAsync(imageId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productImage);

            var handler = new UpdateProductImageCommandHandler(
                _unitOfWorkMock.Object,
                _productImageRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert

            _productImageRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductImage>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductImageCommand_ValidData_ShouldRemoveImage()
        {
            // Arrange
            var imageId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new DeleteProductImageCommand
            {
                Id = imageId
            };

            var productImage = ProductImage.Create(
                productId,
                "https://waxshop.vn/wp-content/uploads/2024/01/reuzel-matte-clay-1.jpg",
                "alt text"
            );

            _productImageRepositoryMock.Setup(x => x.GetByIdAsync(imageId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productImage);

            var handler = new DeleteProductImageCommandHandler(
                _unitOfWorkMock.Object,
                _productImageRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productImageRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<ProductImage>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 