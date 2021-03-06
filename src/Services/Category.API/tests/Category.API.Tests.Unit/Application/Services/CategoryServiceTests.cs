using System;
using System.Threading.Tasks;
using AutoMapper;
using Category.API.Application.Database.PersistenceModels;
using Category.API.Application.Database.Repositories;
using Category.API.Application.Features.AddCategory;
using Category.API.Application.Features.DeleteCategory;
using Category.API.Application.Services;
using Category.API.Tests.Unit.Utilities.Models;
using FluentAssertions;
using Library.Shared.Exceptions;
using Library.Shared.Logging;
using Moq;
using NUnit.Framework;

namespace Category.API.Tests.Unit.Application.Services
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;

        private readonly static string _categoryId = Guid.NewGuid().ToString();
        private const string CategoryName = "NAME";

        private AddCategoryCommand _addCategoryCommand;
        private DeleteCategoryCommand _deleteCategoryCommand;

        private CategoryService _categoryService;

        [SetUp]
        public void SetUp()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger>();

            _addCategoryCommand = new AddCategoryCommand { Name = CategoryName };
            _deleteCategoryCommand = new DeleteCategoryCommand { CategoryId = _categoryId };

            _categoryService = new CategoryService(_categoryRepository.Object,
                _mapper.Object,
                _logger.Object);
        }

        #region AddCategoryAsync

        [Test]
        public async Task AddCategoryAsync_WhenCategoryAlreadyExistsInDatabase_ThrowDuplicateExistsException()
        {
            //Arrange
            _categoryRepository.Setup(x => x.AnyCategoryExistAsync(CategoryName))
                .ReturnsAsync(true);

            //Act
            Func<Task> act = () => _categoryService.AddCategoryAsync(_addCategoryCommand);

            //Assert
            await act.Should().ThrowAsync<DuplicateExistsException>();
        }

        [Test]
        public async Task AddCategoryAsync_WhenInsertingCategoryToDatabaseFailed_ThrowDatabaseOperationException()
        {
            //Arrange
            _categoryRepository.Setup(x => x.AnyCategoryExistAsync(CategoryName))
                .ReturnsAsync(false);
            _categoryRepository.Setup(x => x.InsertCategoryAsync(CategoryName))
                .ReturnsAsync(() => null);

            //Act
            Func<Task> act = () => _categoryService.AddCategoryAsync(_addCategoryCommand);

            //Assert
            await act.Should().ThrowAsync<DatabaseOperationException>();
        }

        [Test]
        public async Task AddCategoryAsync_WhenCategoryInsertedToDatabaseSuccessfully_ReturnAddedCategory()
        {
            //Arrange
            var now = DateTime.Now;

            var categoryPersistenceModel = new CategoryPersistenceModel
            {
                CategoryId = Guid.NewGuid().ToString(),
                Name = CategoryName,
                CreatedOn = now
            };

            var expectedCategory = new StubCategory(categoryPersistenceModel.CategoryId,
                categoryPersistenceModel.Name,
                categoryPersistenceModel.CreatedOn);

            _categoryRepository.Setup(x => x.AnyCategoryExistAsync(CategoryName))
                .ReturnsAsync(false);
            _categoryRepository.Setup(x => x.InsertCategoryAsync(CategoryName))
                .ReturnsAsync(categoryPersistenceModel);

            _mapper.Setup(x => x.Map<CategoryPersistenceModel, API.Domain.Entities.Category>(categoryPersistenceModel))
                .Returns(expectedCategory);

            //Act
            var result = await _categoryService.AddCategoryAsync(_addCategoryCommand);

            //Assert
            result.Should().BeEquivalentTo(expectedCategory);
        }

        #endregion

        #region DeleteCategoryAsync

        [Test]
        public async Task DeleteCategoryAsync_WhenCategoryNotFoundInDatabase_ThrowEntityNotFoundException()
        {
            //Arrange

            _categoryRepository.Setup(x => x.DeleteCategoryAsync(_categoryId))
                .ReturnsAsync(false);

            //Act
            Func<Task> act = () => _categoryService.DeleteCategoryAsync(_deleteCategoryCommand);

            //Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Test]
        public async Task DeleteCategoryAsync_WhenOtherExceptionThanEntityNotFoundExceptionOccurredDuringDeletingCategoryFromDatabase_ThrowDatabaseOperationException()
        {
            //Arrange
            _categoryRepository.Setup(x => x.DeleteCategoryAsync(_categoryId))
                .Throws<Exception>();

            //Act
            Func<Task> act = () => _categoryService.DeleteCategoryAsync(_deleteCategoryCommand);

            //Assert
            await act.Should().ThrowAsync<DatabaseOperationException>();
        }

        [Test]
        public async Task DeleteCategoryAsync_WhenCategoryDeletedFromDatabaseSuccessfully_ReturnDeletedCategoryId()
        {
            //Arrange
            _categoryRepository.Setup(x => x.DeleteCategoryAsync(_categoryId))
                .ReturnsAsync(true);

            //Act
            var result = await _categoryService.DeleteCategoryAsync(_deleteCategoryCommand);

            //Assert
            result.Should().Be(_categoryId);
        }

        #endregion
    }
}