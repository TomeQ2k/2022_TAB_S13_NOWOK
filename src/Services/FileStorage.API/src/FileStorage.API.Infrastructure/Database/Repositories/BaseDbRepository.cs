using System;
using System.Linq;
using FileStorage.API.Application.Database.Attributes;
using Library.Database;
using Library.Database.Abstractions;
using MongoDB.Driver;

namespace FileStorage.API.Infrastructure.Database.Repositories
{
    public abstract class BaseDbRepository<TModel> : IDbRepository<TModel>
        where TModel : BasePersistenceModel
    {
        protected readonly IMongoCollection<TModel> _collection;

        protected BaseDbRepository(FileStorageDbContext dbContext)
            => _collection = dbContext.Database.GetCollection<TModel>(GetCollectionName(typeof(TModel)));

        private protected string GetCollectionName(Type documentType)
            => ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
    }
}