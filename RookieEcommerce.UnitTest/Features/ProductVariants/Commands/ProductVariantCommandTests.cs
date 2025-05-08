using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductVariants.Commands;
using RookieEcommerce.Domain.Entities;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.ProductVariants.Commands
{
    public class ProductVariantCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IProductVariantRepository> _productVariantRepositoryMock;

        public ProductVariantCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _productVariantRepositoryMock = new Mock<IProductVariantRepository>();
        }

        [Fact]
        public async Task CreateVariantCommand_ValidData_ShouldCreateVariant()
        {
            // Arrange
            var productId = Guid.NewGuid();
            
            var command = new CreateVariantCommand
            {
                ProductId = productId,
                VariantType = "Kích thước",
                Name = "Hộp 113g",
                StockQuantity = 50,
                Price = 479000
            };

            // Setup existing product
            var existingProduct = Product.Create(
                "Heavy Hold Pomade - Lockhart's Pomade", 
                "Pomade giữ nếp mạnh từ Lockhart's", 
                479000, 
                Guid.NewGuid(), 
                "Độ bóng cao, giữ nếp mạnh"
            );
            existingProduct.Sku = "HE-HO-PO";
            
            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _productVariantRepositoryMock.Setup(x => x.AnyAsync(
                    It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new CreateVariantCommandHandler(
                _unitOfWorkMock.Object, 
                _productRepositoryMock.Object, 
                _productVariantRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Price, result.Price);
            Assert.Equal(command.StockQuantity, result.StockQuantity);
            Assert.Equal(command.VariantType, result.VariantType);
            Assert.StartsWith(existingProduct.Sku + "-", result.Sku);
            
            _productVariantRepositoryMock.Verify(x => x.AddAsync(
                It.Is<ProductVariant>(v => 
                    v.Name == command.Name && 
                    v.Price == command.Price && 
                    v.StockQuantity == command.StockQuantity
                ), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            
            _productRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<Product>(p => p.TotalQuantity == existingProduct.TotalQuantity + command.StockQuantity), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateVariantCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            
            var command = new CreateVariantCommand
            {
                ProductId = productId,
                VariantType = "Kích thước",
                Name = "Hộp 30g",
                StockQuantity = 20,
                Price = 249000
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new CreateVariantCommandHandler(
                _unitOfWorkMock.Object, 
                _productRepositoryMock.Object, 
                _productVariantRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateVariantCommand_DuplicateName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            
            var command = new CreateVariantCommand
            {
                ProductId = productId,
                VariantType = "Kích thước",
                Name = "Hộp 113g",
                StockQuantity = 50,
                Price = 479000
            };

            var existingProduct = Product.Create(
                "Reuzel Fiber Pomade", 
                "Pomade fiber từ Reuzel", 
                249000, 
                Guid.NewGuid(), 
                "Độ bám tốt, mùi thơm cola, dễ gội sạch"
            );
            
            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _productVariantRepositoryMock.Setup(x => x.AnyAsync(
                    It.IsAny<Expression<Func<ProductVariant, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new CreateVariantCommandHandler(
                _unitOfWorkMock.Object, 
                _productRepositoryMock.Object, 
                _productVariantRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateVariantCommand_ValidData_ShouldUpdateVariant()
        {
            // Arrange
            var variantId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var existingVariant = ProductVariant.Create(
                productId, 
                "Hộp 35g", 
                249000, 
                30, 
                "Kích thước"
            );

            var command = new UpdateVariantCommand
            {
                Id = variantId,
                Name = "Hộp 35g Travel Size",
                StockQuantity = 40,
                Price = 229000
            };

            var existingProduct = Product.Create(
                "Reuzel Matte Clay", 
                "Sáp tạo kiểu tóc matte clay từ Reuzel", 
                249000, 
                Guid.NewGuid(), 
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );
            existingProduct.Update(null, null, null, null, 30, null);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(
                    variantId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingVariant);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var handler = new UpdateVariantCommandHandler(
                _unitOfWorkMock.Object, 
                _productVariantRepositoryMock.Object, 
                _productRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productVariantRepositoryMock.Verify(x => x.GetByIdAsync(variantId, null, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()), Times.Once);
            
            _productVariantRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<ProductVariant>(v => 
                    v.Name == command.Name && 
                    v.Price == command.Price && 
                    v.StockQuantity == command.StockQuantity), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            
            _productRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<Product>(p => p.TotalQuantity == existingProduct.TotalQuantity - existingVariant.StockQuantity + command.StockQuantity), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteVariantCommand_ValidData_ShouldDeleteVariant()
        {
            // Arrange
            var variantId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var existingVariant = ProductVariant.Create(
                productId, 
                "Hộp 113g", 
                479000, 
                20, 
                "Kích thước"
            );

            var command = new DeleteVariantCommand
            {
                Id = variantId
            };

            var existingProduct = Product.Create(
                "Goon Grease Heavy Hold - Lockhart's Pomade", 
                "Pomade giữ nếp mạnh từ Lockhart's", 
                479000, 
                Guid.NewGuid(), 
                "Độ bóng cao, giữ nếp cực mạnh"
            );
            existingProduct.Update(null, null, null, null, 20, null);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(
                    variantId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingVariant);

            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var handler = new DeleteVariantCommandHandler(
                _unitOfWorkMock.Object, 
                _productVariantRepositoryMock.Object, 
                _productRepositoryMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _productVariantRepositoryMock.Verify(x => x.GetByIdAsync(variantId, null, It.IsAny<CancellationToken>()), Times.Once);
            _productRepositoryMock.Verify(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()), Times.Once);
            
            _productVariantRepositoryMock.Verify(x => x.DeleteAsync(existingVariant, It.IsAny<CancellationToken>()), Times.Once);
            
            _productRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<Product>(p => p.TotalQuantity == existingProduct.TotalQuantity - existingVariant.StockQuantity), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 