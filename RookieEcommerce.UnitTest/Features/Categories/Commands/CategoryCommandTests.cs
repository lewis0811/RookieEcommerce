using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Categories.Commands;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.UnitTests.Features.Categories.Commands
{
    public class CategoryCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

        public CategoryCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
        }

        [Fact]
        public async Task CreateCategoryCommand_ValidData_ShouldCreateCategory()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = "Reuzel Fiber Pomade - Reuzel",
                Description = "Fiber-based pomade for texture and hold",
                ParentCategoryId = null
            };

            _categoryRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .Callback<Category, CancellationToken>((predicate, token) =>
                {
                    var category = Category.Create(command.Name, command.Description, command.ParentCategoryId);
                    _categoryRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(category);
                })
                .ReturnsAsync(false);

            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);

            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCategoryCommand_DuplicateName_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateCategoryCommand
            {
                Name = "Reuzel Fiber Pomade - Reuzel",
                Description = "Fiber-based pomade for texture and hold",
                ParentCategoryId = null
            };

            _categoryRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateCategoryCommand_ValidData_ShouldUpdateCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = Category.Create("Reuzel Fiber Pomade - Reuzel", "Fiber-based pomade for texture and hold", null);
            var command = new UpdateCategoryCommand
            {
                Id = categoryId,
                Name = "Reuzel Fiber Pomade - Reuzel 2",
                Description = "Fiber-based pomade for texture and hold"
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            var handler = new UpdateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryCommand_NonExistentCategory_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new UpdateCategoryCommand
            {
                Id = categoryId,
                Name = "Reuzel Fiber Pomade - Reuzel 2",
                Description = "Fiber-based pomade for texture and hold"
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new UpdateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteCategoryCommand_ValidData_ShouldDeleteCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = Category.Create("Reuzel Fiber Pomade - Reuzel", "Fiber-based pomade for texture and hold", null);

            var command = new DeleteCategoryCommand
            {
                Id = categoryId
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            var handler = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _categoryRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryCommand_NonExistentCategory_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var command = new DeleteCategoryCommand
            {
                Id = categoryId
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateCategoryCommand_WithValidParentCategoryId_ShouldCreateCategory()
        {
            // Arrange
            var parentCategoryId = Guid.NewGuid();
            var parentCategory = Category.Create("Parent Category", "Parent Description", null);

            var command = new CreateCategoryCommand
            {
                Name = "Child Category",
                Description = "Child Description",
                ParentCategoryId = parentCategoryId
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(parentCategoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(parentCategory);

            _categoryRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.ParentCategoryId, result.ParentCategoryId);
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCategoryCommand_WithInvalidParentCategoryId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var parentCategoryId = Guid.NewGuid();

            var command = new CreateCategoryCommand
            {
                Name = "Child Category",
                Description = "Child Description",
                ParentCategoryId = parentCategoryId
            };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(parentCategoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object, _categoryRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}