using System;
using System.Threading.Tasks;
using AccountDefinition.API.Application.Database.PersistenceModels;
using AccountDefinition.API.Application.Database.Repositories;
using AccountDefinition.API.Application.Features.AddAccountProvider;
using AccountDefinition.API.Application.Features.DeleteAccountProviderById;
using AccountDefinition.API.Application.Services;
using AccountDefinition.API.Domain.Entities;
using AccountDefinition.API.Tests.Unit.Utilities.Models;
using AutoMapper;
using FluentAssertions;
using Library.Shared.Exceptions;
using Library.Shared.Logging;
using Library.Shared.Models.AccountDefinition.Dtos;
using Moq;
using Npgsql;
using NUnit.Framework;

namespace AccountDefinition.API.Tests.Unit.Application.Services
{
    [TestFixture]
    public class AccountProviderServiceTests
    {
        private const string Provider = " ProVider ";
        private const string ExpectedProvider = "PROVIDER";
        private const long ProviderId = 1;

        private readonly static DateTime _createdOn = DateTime.UtcNow;

        private Mock<IAccountProviderRepository> _accountProviderRepository;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;

        private StubAccountProvider _accountProvider;
        private AccountProviderPersistenceModel _accountProviderPersistenceModel;

        private AddAccountProviderCommand _addAccountProviderCommand;
        private DeleteAccountProviderByIdCommand _deleteAccountProviderByIdCommand;

        private AccountProviderService _accountProviderService;

        [SetUp]
        public void SetUp()
        {
            _accountProvider = new StubAccountProvider(ExpectedProvider, _createdOn);
            _accountProviderPersistenceModel = new AccountProviderPersistenceModel {Provider = ExpectedProvider, CreatedOn = _createdOn};

            _accountProviderRepository = new Mock<IAccountProviderRepository>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger>();

            _addAccountProviderCommand = new AddAccountProviderCommand {Provider = Provider};
            _deleteAccountProviderByIdCommand = new DeleteAccountProviderByIdCommand {AccountProviderId = ProviderId};

            _accountProviderService = new AccountProviderService(_accountProviderRepository.Object,
                _mapper.Object,
                _logger.Object);
        }

        #region AddAccountProviderAsync

        [Test]
        public async Task AddAccountProviderAsync_WhenAccountProviderInsertedToDatabase_ReturnAccountProviderDto()
        {
            //Arrange
            var expectedResult = new AccountProviderDto {Provider = ExpectedProvider, CreatedOn = _createdOn};

            _accountProviderRepository.Setup(x => x.InsertAccountProviderAsync(It.IsAny<AccountProvider>()))
                .ReturnsAsync(_accountProviderPersistenceModel);

            _mapper.Setup(x => x.Map<AccountProviderPersistenceModel, AccountProvider>(_accountProviderPersistenceModel))
                .Returns(_accountProvider);
            _mapper.Setup(x => x.Map<AccountProviderDto>(_accountProvider))
                .Returns(new AccountProviderDto {Provider = _accountProvider.Provider, CreatedOn = _accountProvider.CreatedOn});

            //Act
            var result = await _accountProviderService.AddAccountProviderAsync(_addAccountProviderCommand);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task AddAccountProviderAsync_WhenAccountProviderInsertingToDatabaseFailed_ThrowDatabaseOperationException()
        {
            //Arrange
            _accountProviderRepository.Setup(x => x.InsertAccountProviderAsync(It.IsAny<AccountProvider>()))
                .ReturnsAsync(() => null);

            //Act
            Func<Task> act = () => _accountProviderService.AddAccountProviderAsync(_addAccountProviderCommand);

            //Assert
            await act.Should().ThrowAsync<DatabaseOperationException>();
        }

        [Test]
        public async Task AddAccountProviderAsync_WhenAccountProviderAlreadyExistsInDatabase_ThrowDuplicateAlreadyExistsException()
        {
            //Arrange
            const string UniqueConstraintViolationExceptionCode = "23505";

            _accountProviderRepository.Setup(x => x.InsertAccountProviderAsync(It.IsAny<AccountProvider>()))
                .Throws(() => new PostgresException("", "", "", UniqueConstraintViolationExceptionCode));

            //Act
            Func<Task> act = () => _accountProviderService.AddAccountProviderAsync(_addAccountProviderCommand);

            //Assert
            await act.Should().ThrowAsync<DuplicateExistsException>();
        }

        #endregion

        #region DeleteAccountProviderAsync

        [Test]
        public async Task DeleteAccountProviderAsync_WhenAccountProviderExistsInDatabase_ReturnDeletedAccountProviderId()
        {
            //Arrange
            _accountProviderRepository.Setup(x => x.DeleteAccountProviderByIdAsync(ProviderId))
                .ReturnsAsync(ProviderId);

            //Act
            var result = await _accountProviderService.DeleteAccountProviderByIdAsync(_deleteAccountProviderByIdCommand);

            //Assert
            result.Should().Be(ProviderId);
        }

        [Test]
        public async Task DeleteAccountProviderAsync_WhenAccountProviderDoesNotExistInDatabase_ThrowEntityNotFoundException()
        {
            //Arrange
            _accountProviderRepository.Setup(x => x.DeleteAccountProviderByIdAsync(ProviderId))
                .ReturnsAsync(default(long));

            //Act
            Func<Task> act = () => _accountProviderService.DeleteAccountProviderByIdAsync(_deleteAccountProviderByIdCommand);

            //Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        #endregion
    }
}