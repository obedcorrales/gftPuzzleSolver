using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using dF.Commons.Models.BL;

namespace dF.Commons.Services.BL.Contracts
{
    public interface IBusinessAggregateReadOnly<T> where T : class
    {
        #region Resource Properties
        string ResourceName { get; }
        string PluralResourceName { get; }

        List<ResourceLink> ResourceMap { get; }
        #endregion

        Task<ResponseContext<IList<T>>> GetAllAsync<TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where);
        Task<ResponseContext<IList<K>>> GetAllAsync<K, TKey>(Expression<Func<T, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<T, bool>>[] where) where K : class, T;

        Task<ResponseContext<T>> GetByIdAsync(params Expression<Func<T, bool>>[] keySelectors);
        Task<ResponseContext<K>> GetByIdAsync<K>(params Expression<Func<T, bool>>[] keySelectors) where K : class, T;
    }
}
