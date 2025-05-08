using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Products.Commands;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.ProductDtos;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.Products.Commands
{
    public class ProductCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepositoryMock = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task CreateProductCommand_ValidData_ShouldCreateProduct()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new CreateProductCommand
            {
                Name = "Reuzel Matte Clay",
                Description = "Sáp tạo kiểu tóc matte clay từ Reuzel",
                Price = 249000,
                CategoryId = categoryId,
                Details = "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            };

            _productRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _productRepositoryMock.Setup(x => x.ListAllAsync(It.IsAny<Expression<Func<Product, bool>>>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var handler = new CreateProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Price, result.Price);
            _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateProductCommand_DuplicateName_ShouldThrowArgumentException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new CreateProductCommand
            {
                Name = "Firsthand Water Based Pomade",
                Description = "Pomade gốc nước FirstHand",
                Price = 459000,
                CategoryId = categoryId,
                Details = "Độ bóng vừa phải, giữ nếp cả ngày"
            };

            _productRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new CreateProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateProductCommand_ValidData_ShouldUpdateProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var existingProduct = Product.Create(
                "Powermade", 
                "Pomade từ By Vilain", 
                480000, 
                categoryId, 
                "Độ bóng cao, hương thơm dễ chịu"
            );
            
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Powermade - By Vilain",
                Description = "Pomade cao cấp từ By Vilain",
                Price = 399000,
                Details = "Độ bóng cao, hương thơm dễ chịu, giữ nếp cả ngày",
                TotalQuantity = 50,
                TotalSell = 25
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var handler = new UpdateProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Product>(p => 
                p.Name == command.Name && 
                p.Description == command.Description && 
                p.Price == command.Price), 
                It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductCommand_NonExistentProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Original Matte - Tough & Tumble",
                Description = "Sáp vuốt tóc matte từ Tough & Tumble",
                Price = 399000,
                Details = "Độ giữ nếp cao, không bóng, dễ tạo kiểu"
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new UpdateProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteProductCommand_ValidData_ShouldDeleteProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var existingProduct = Product.Create(
                "Goon Grease Heavy Hold", 
                "Pomade giữ nếp mạnh từ Lockhart's", 
                479000, 
                categoryId, 
                "Độ bóng cao, giữ nếp cực mạnh"
            );
            
            var command = new DeleteProductCommand
            {
                Id = productId
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var handler = new DeleteProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(x => x.DeleteAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductCommand_NonExistentProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new DeleteProductCommand
            {
                Id = productId
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new DeleteProductCommandHandler(_unitOfWorkMock.Object, _productRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
} 