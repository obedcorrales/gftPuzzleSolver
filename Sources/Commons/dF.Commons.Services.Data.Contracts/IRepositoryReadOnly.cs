using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using dF.Commons.Models.Globals;

namespace dF.Commons.Services.Data.Contracts
{
    public interface IRepositoryReadOnly<T> where T : class
    {
        #region GetAll
        // All Builder
        IQueryableRepository<T> GetQueryable();

        // All
        Result<IQueryable<T>> GetAll();
        Task<Result<IQueryable<T>>> GetAllAsync();

        // All with Where
        Result<IQueryable<T>> GetAll(IQueryable<T> dbSet, params Expression<Func<T, bool>>[] where);
        Result<IQueryable<T>> GetAll(params Expression<Func<T, bool>>[] where);
        Task<Result<IQueryable<T>>> GetAllAsync(IQueryable<T> dbSet, params Expression<Func<T, bool>>[] where);
        Task<Result<IQueryable<T>>> GetAllAsync(params Expression<Func<T, bool>>[] where);

        // All with Where and Paging
        Result<IQueryable<T>> GetAll<TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where);
        Result<IQueryable<T>> GetAll<TKey>(IQueryable<T> dbSet, Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where);
        Task<Result<IQueryable<T>>> GetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where);
        Task<Result<IQueryable<T>>> GetAllAsync<TKey>(IQueryable<T> dbSet, Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where);
        #endregion

        #region GetById
        Result<T> GetById(params Expression<Func<T, bool>>[] keySelectors);
        Task<Result<T>> GetByIdAsync(params Expression<Func<T, bool>>[] keySelectors);
        #endregion

        #region Count
        Result<int> Count(params Expression<Func<T, bool>>[] where);
        Task<Result<int>> CountAsync(params Expression<Func<T, bool>>[] where);
        #endregion

        #region Max
        Result<TResult> Max<TResult>(Expression<Func<T, TResult>> selector, params Expression<Func<T, bool>>[] where);
        Task<Result<TResult>> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, params Expression<Func<T, bool>>[] where);
        #endregion
    }

    public interface IQueryableRepository<T> where T : class
    {
        IQueryableRepository<T> Where(params Expression<Func<T, bool>>[] where);

        IQueryableRepository<T> Include<TProperty>(Expression<Func<T, TProperty>> property) where TProperty : class;

        Result<IQueryable<TResult>> Select<TResult>(Expression<Func<T, TResult>> selector);
        Task<Result<IQueryable<TResult>>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
        Result<IQueryable<T>> GetAll();
        Task<Result<IQueryable<T>>> GetAllAsync();
    }
}
