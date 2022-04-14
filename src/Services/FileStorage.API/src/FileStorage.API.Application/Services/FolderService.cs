﻿using System.Threading.Tasks;
using AutoMapper;
using FileStorage.API.Application.Abstractions;
using FileStorage.API.Application.Database.Repositories;
using FileStorage.API.Application.Features.GetFolderByKey;
using FileStorage.API.Domain.Entities;
using FileStorage.API.Domain.ValueObjects;
using Library.Shared.Exceptions;
using Library.Shared.Logging;
using SimpleFileSystem.Abstractions;

namespace FileStorage.API.Application.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileSystemConfiguration _fileSystemConfiguration;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public FolderService(IFolderRepository folderRepository,
            IFileSystemConfiguration fileSystemConfiguration,
            IMapper mapper,
            ILogger logger)
        {
            _folderRepository = folderRepository;
            _fileSystemConfiguration = fileSystemConfiguration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Folder> GetFolderByKeyAsync(GetFolderByKeyQuery query)
        {
            var folderKey = new FolderKey(query.FolderKey);

            var folderPersistenceModel = await _folderRepository.GetFolderByKeyAsync(folderKey)
                                         ?? throw new EntityNotFoundException($"Folder with key: '{folderKey.Value}' not found in the database");

            var folder = _mapper.Map<Folder>(folderPersistenceModel);

            folder.SetUrlForAllFiles(_fileSystemConfiguration.BaseUrl);

            _logger.Info($"Folder #{folder.FolderId} with key: '{folder.Key}' found in the database. It contains {folder.Files.Count} files");

            return folder;
        }
    }
}