using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductRatings.Commands;
using RookieEcommerce.Domain.Entities;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.ProductRatings.Commands
{
    public class ProductRatingCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductRatingRepository> _productRatingRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public ProductRatingCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRatingRepositoryMock = new Mock<IProductRatingRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task CreateProductRatingCommand_ValidData_ShouldCreateRating()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new CreateProductRatingCommand
            {
                ProductId = productId,
                CustomerId = customerId,
                RatingValue = 5,
                Comment = "Sản phẩm rất tốt, độ bám cao, mùi thơm dễ chịu. Đóng gói cẩn thận, giao hàng nhanh."

            };

            var product = Product.Create(
                "Reuzel Matte Clay",
                "Sáp tạo kiểu tóc matte clay từ Reuzel",
                249000,
                Guid.NewGuid(),
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );

            var customer = Customer.Create(
                "Dan",
                "Luong",
                "customer@example.com",
                "0123456789"
            );

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var handler = new CreateProductRatingCommandHandler(
                _unitOfWorkMock.Object,
                _productRatingRepositoryMock.Object,
                _productRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(5, result.RatingValue);
            Assert.Equal(command.Comment, result.Comment);

            _productRatingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProductRating>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductRatingCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new CreateProductRatingCommand
            {
                ProductId = productId,
                CustomerId = customerId,
                RatingValue = 5,
                Comment = "Sản phẩm rất tốt"
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new CreateProductRatingCommandHandler(
                _unitOfWorkMock.Object,
                _productRatingRepositoryMock.Object,
                _productRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateProductRatingCommand_CustomerNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new CreateProductRatingCommand
            {
                ProductId = productId,
                CustomerId = customerId,
                RatingValue = 5,
                Comment = "Sản phẩm rất tốt"
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

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            var handler = new CreateProductRatingCommandHandler(
                _unitOfWorkMock.Object,
                _productRatingRepositoryMock.Object,
                _productRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProductRatingCommand_ValidData_ShouldUpdateRating()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new UpdateProductRatingCommand
            {
                Id = ratingId,
                RatingValue = 4,
                Comment = "Sản phẩm tốt, nhưng giá hơi cao"
            };

            var productRating = ProductRating.Create(
                productId,
                customerId.ToString(),
                5,
                "Sản phẩm rất tốt"
            );

            _productRatingRepositoryMock.Setup(x => x.GetByIdAsync(ratingId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productRating);

            var handler = new UpdateProductRatingCommandHandler(
                _unitOfWorkMock.Object,
                _productRatingRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert

            _productRatingRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductRating>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductRatingCommand_ValidData_ShouldRemoveRating()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var command = new DeleteProductRatingCommand
            {
                Id = ratingId
            };

            var productRating = ProductRating.Create(
                productId,
                customerId.ToString(),
                5,
                "Sản phẩm rất tốt"
            );

            _productRatingRepositoryMock.Setup(x => x.GetByIdAsync(ratingId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(productRating);

            var handler = new DeleteProductRatingCommandHandler(
                _unitOfWorkMock.Object,
                _productRatingRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productRatingRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<ProductRating>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 