using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using dF.Commons.Exceptions.Resources;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Services.Data.Contracts;

namespace dF.Commons.Services.Data.InMemory
{
    public class RepositoryReadOnly<TEntity> : IRepositoryReadOnly<TEntity> where TEntity : class
    {
        protected virtual DbContext _dbContext { get; private set; }
        protected virtual DbSet<TEntity> _dbSet { get; set; } = null;

        public RepositoryReadOnly(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _dbSet = _dbContext.Set<TEntity>();
        }

        #region GetAll
        public virtual IQueryableRepository<TEntity> GetQueryable()
        {
            return new QueryableRepository<TEntity>(_dbSet.AsQueryable());
        }

        // All
        public virtual Result<IQueryable<TEntity>> GetAll()
        {
            try
            {
                if (_dbSet.Count > 0)
                    return Result.Ok(_dbSet.AsQueryable());
                else
                    return Result.Fail<IQueryable<TEntity>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, typeof(TEntity).Name), ErrorType.EntityNotFound);
            }
            catch (Exception e)
            {
                return Result.Fail<IQueryable<TEntity>>(e);
            }
        }

        public virtual Task<Result<IQueryable<TEntity>>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        // All with Where
        public virtual Result<IQueryable<TEntity>> GetAll(IQueryable<TEntity> dbSet, params Expression<Func<TEntity, bool>>[] where)
        {
            try
            {
                foreach (var keySelector in where)
                    dbSet = dbSet.Where(keySelector);

                var payload = dbSet.ToList();

                if (payload.Count > 0)
                    return Result.Ok(payload.AsQueryable());
                else
                    return Result.Fail<IQueryable<TEntity>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, typeof(TEntity).Name), ErrorType.EntityNotFound);
            }
            catch (Exception e)
            {
                return Result.Fail<IQueryable<TEntity>>(e);
            }
        }

        public virtual Result<IQueryable<TEntity>> GetAll(params Expression<Func<TEntity, bool>>[] where)
        {
            return GetAll(_dbSet.AsQueryable(), where);
        }

        public virtual Task<Result<IQueryable<TEntity>>> GetAllAsync(IQueryable<TEntity> dbSet, params Expression<Func<TEntity, bool>>[] where)
        {
            return Task.FromResult(GetAll(where));
        }

        public virtual Task<Result<IQueryable<TEntity>>> GetAllAsync(params Expression<Func<TEntity, bool>>[] where)
        {
            return GetAllAsync(_dbSet.AsQueryable(), where);
        }

        // All with Where and Paging
        public virtual Result<IQueryable<TEntity>> GetAll<TKey>(IQueryable<TEntity> dbSet, Expression<Func<TEntity, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<TEntity, bool>>[] where)
        {
            try
            {
                foreach (var keySelector in where)
                    dbSet = dbSet.Where(keySelector);

                if (descendingOrder)
                    dbSet = dbSet.OrderByDescending(orderBy);
                else
                    dbSet = dbSet.OrderBy(orderBy);

                if (page != 0 & !pageSize.HasValue)
                    pageSize = 50;

                if (pageSize.HasValue)
                    dbSet = dbSet.Skip(pageSize.Value * (page - 1))
                                 .Take(pageSize.Value);

                var payload = dbSet.ToList();

                if (payload.Count > 0)
                    return Result.Ok(payload.AsQueryable());
                else
                    return Result.Fail<IQueryable<TEntity>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, typeof(TEntity).Name), ErrorType.EntityNotFound);
            }
            catch (Exception e)
            {
                return Result.Fail<IQueryable<TEntity>>(e);
            }
        }

        public virtual Result<IQueryable<TEntity>> GetAll<TKey>(Expression<Func<TEntity, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<TEntity, bool>>[] where)
        {
            return GetAll(_dbSet.AsQueryable(), orderBy, page, pageSize, descendingOrder, where);
        }

        public virtual Task<Result<IQueryable<TEntity>>> GetAllAsync<TKey>(IQueryable<TEntity> dbSet, Expression<Func<TEntity, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<TEntity, bool>>[] where)
        {
            return Task.FromResult(this.GetAll<TKey>(dbSet, orderBy, page, pageSize, descendingOrder, where));
        }

        public virtual Task<Result<IQueryable<TEntity>>> GetAllAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<TEntity, bool>>[] where)
        {
            return Task.FromResult(this.GetAll<TKey>(orderBy, page, pageSize, descendingOrder, where));
        }
        #endregion

        #region GetById
        public virtual Result<TEntity> GetById(params Expression<Func<TEntity, bool>>[] keySelectors)
        {
            try
            {
                IQueryable<TEntity> entities = _dbSet.AsQueryable();

                foreach (var keySelector in keySelectors)
                {
                    entities = entities.Where(keySelector);
                }

                var entity = entities.SingleOrDefault();

                if (entity == null)
                    return Result.Fail<TEntity>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordFoundByProvidedID, typeof(TEntity).Name), ErrorType.EntityNotFound);

                return Result.Ok(entity);
            }
            catch (Exception e)
            {
                return Result.Fail<TEntity>(e);
            }
        }

        public virtual Task<Result<TEntity>> GetByIdAsync(params Expression<Func<TEntity, bool>>[] keySelectors)
        {
            return Task.FromResult(this.GetById(keySelectors));
        }
        #endregion

        #region Count
        public Result<int> Count(params Expression<Func<TEntity, bool>>[] where)
        {
            try
            {
                IQueryable<TEntity> entities = _dbSet.AsQueryable();

                foreach (var keySelector in where)
                    entities = entities.Where(keySelector);

                return Result.Ok(entities.Count());
            }
            catch (Exception e)
            {
                return Result.Fail<int>(e);
            }
        }

        public Task<Result<int>> CountAsync(params Expression<Func<TEntity, bool>>[] where)
        {
            return Task.FromResult(Count(where));
        }
        #endregion

        #region Max
        public Result<TResult> Max<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, bool>>[] where)
        {
            try
            {
                IQueryable<TEntity> entities = _dbSet.AsQueryable();

                foreach (var keySelector in where)
                    entities = entities.Where(keySelector);

                return Result.Ok(entities.Max(selector));
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException)
                    return Result.Ok(default(TResult));

                return Result.Fail<TResult>(e);
            }
        }

        public Task<Result<TResult>> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, bool>>[] where)
        {
            return Task.FromResult(Max(selector, where));
        }
        #endregion
    }

    public class QueryableRepository<T> : IQueryableRepository<T> where T : class
    {
        IQueryable<T> _dbSet;

        public QueryableRepository(IQueryable<T> dbSet)
        {
            _dbSet = dbSet;
        }

        public IQueryableRepository<T> Where(params Expression<Func<T, bool>>[] where)
        {
            foreach (var condition in where)
                _dbSet = _dbSet.Where(condition);

            return this;
        }

        public IQueryableRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> property) where TProperty : class
        {
            // TODO: Implement Include
            //_dbSet = _dbSet.Include(element);

            return this;
        }

        #region Resolvers
        public Result<IQueryable<TResult>> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            var payload = _dbSet.Select(selector).ToList();

            if (payload.Count > 0)
                return Result.Ok(payload.AsQueryable());
            else
                return Result.Fail<IQueryable<TResult>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, typeof(T).Name), ErrorType.EntityNotFound);
        }

        public Task<Result<IQueryable<TResult>>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
        {
            return Task.FromResult(Select(selector));
        }

        public Result<IQueryable<T>> GetAll()
        {
            var payload = _dbSet.ToList();

            if (payload.Count > 0)
                return Result.Ok(payload.AsQueryable());
            else
                return Result.Fail<IQueryable<T>>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordsFound, typeof(T).Name), ErrorType.EntityNotFound);
        }

        public Task<Result<IQueryable<T>>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }
        #endregion
    }
}
