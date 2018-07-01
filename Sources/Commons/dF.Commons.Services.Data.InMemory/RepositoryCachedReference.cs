using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using dF.Commons.Exceptions.Resources;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Services.Data.Contracts;

namespace dF.Commons.Services.Data.InMemory
{
    public class RepositoryCachedReference<T> : IRepositoryCachedReference<T> where T : class
    {
        protected virtual DbContext _dbContext { get; private set; }
        protected virtual DbSet<T> _dbSet { get; set; } = null;

        public RepositoryCachedReference(DbContext dbContext, int cacheMinutes)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _dbSet = _dbContext.Set<T>();
            _cacheMinutes = cacheMinutes;
        }

        private static Dictionary<string, CacheEntity> _cacheDictionary = new Dictionary<string, CacheEntity>();
        static object _locker = new object();

        private int _cacheMinutes;
        private string _entityName = typeof(T).Name;

        public DateTime ExpirationTime => _cacheDictionary[_entityName].ExpirationTime;
        public bool IsValid => _cacheDictionary[_entityName].IsValid;

        public virtual Result<IEnumerable<T>> GetCache()
        {
            try
            {
                if (!_cacheDictionary.TryGetValue(_entityName, out var cachedEntity))
                {
                    if (_dbSet.Count > 0)
                    {
                        cachedEntity = new CacheEntity(_dbSet.AsEnumerable(), _cacheMinutes);

                        bool acquiredLock = false;
                        try
                        {
                            Monitor.TryEnter(_locker, 500, ref acquiredLock);
                            if (acquiredLock)
                                _cacheDictionary.Add(_entityName, cachedEntity);
                        }
                        finally
                        {
                            if (acquiredLock)
                                Monitor.Exit(_locker);
                        }
                    }
                    else
                        return Result.Fail<IEnumerable<T>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, _entityName), ErrorType.EntityNotFound);
                }
                else if (!cachedEntity.IsValid)
                {
                    if (_dbSet.Count > 0)
                    {
                        bool acquiredLock = false;
                        try
                        {
                            Monitor.TryEnter(_locker, 500, ref acquiredLock);
                            if (acquiredLock)
                                cachedEntity.RenewCache(_dbSet.AsEnumerable(), _cacheMinutes);
                        }
                        finally
                        {
                            if (acquiredLock)
                                Monitor.Exit(_locker);
                        }
                    }
                    else
                        return Result.Fail<IEnumerable<T>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, _entityName), ErrorType.EntityNotFound);
                }

                return Result.Ok(cachedEntity.CachedResults).ExpiresOn(cachedEntity.ExpirationTime);
            }
            catch (Exception e)
            {
                return Result.Fail<IEnumerable<T>>(e);
            }
        }

        public virtual Task<Result<IEnumerable<T>>> GetCacheAsync()
        {
            return Task.FromResult(GetCache());
        }

        private struct CacheEntity
        {
            public int CacheMinutes { get; private set; }
            public DateTime ExpirationTime { get; private set; }
            public bool IsValid => DateTime.UtcNow < ExpirationTime;
            public IEnumerable<T> CachedResults { get; set; }

            public CacheEntity(IEnumerable<T> cachedResults, int cacheMinutes = 480)
            {
                CachedResults = cachedResults;
                CacheMinutes = cacheMinutes;
                ExpirationTime = DateTime.UtcNow.AddMinutes(cacheMinutes);
            }

            public void RenewCache(IEnumerable<T> cachedResults, int cacheMinutes = 480)
            {
                CachedResults = cachedResults;
                CacheMinutes = cacheMinutes;
                ExpirationTime = DateTime.UtcNow.AddMinutes(cacheMinutes);
            }
        }
    }
}
