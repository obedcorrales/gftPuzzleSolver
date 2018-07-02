using System;
using System.Threading.Tasks;

using dF.Commons.Models.Globals;
using dF.Commons.Services.Data.Contracts;

namespace dF.Commons.Services.Data.InMemory
{
    public class Repository<TEntity> : RepositoryReadOnly<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public Repository(DbContext dbContext) : base(dbContext) { }

        #region Add
        public virtual Result Add(TEntity entity)
        {
            return _dbSet.Add(entity);
        }

        public virtual Task<Result> AddAsync(TEntity entity)
        {
            return Task.FromResult(Add(entity));
        }
        #endregion

        #region Update
        public Result Update(TEntity entity)
        {
            try
            {
                return _dbSet.Update(entity);
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }

        public Task<Result> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }
        #endregion

        #region Delete
        public virtual Result Delete(TEntity entity)
        {
            try
            {
                return _dbSet.Delete(entity);
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }

        public virtual Task<Result> DeleteAsync(TEntity entity)
        {
            return Task.FromResult(Delete(entity));
        }
        #endregion
    }
}
