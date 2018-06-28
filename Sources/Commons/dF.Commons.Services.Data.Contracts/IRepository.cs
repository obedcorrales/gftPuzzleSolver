using System.Threading.Tasks;

using dF.Commons.Models.Globals;

namespace dF.Commons.Services.Data.Contracts
{
    public interface IRepository<T> : IRepositoryReadOnly<T> where T : class
    {
        #region Add
        Result Add(T entity);
        Task<Result> AddAsync(T entity);
        #endregion

        #region Update
        Result Update(T entity);
        Task<Result> UpdateAsync(T entity);
        #endregion

        #region Delete
        Result Delete(T entity);
        Task<Result> DeleteAsync(T entity);
        //Result Delete(params Expression<Func<T, bool>>[] keySelectors);
        //Task<Result> DeleteAsync(params Expression<Func<T, bool>>[] keySelectors);
        #endregion
    }
}
