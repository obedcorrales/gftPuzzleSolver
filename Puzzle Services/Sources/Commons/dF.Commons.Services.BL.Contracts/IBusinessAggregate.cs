using dF.Commons.Models.BL;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace dF.Commons.Services.BL.Contracts
{
    public interface IBusinessAggregate<T> : IBusinessAggregateReadOnly<T> where T : class
    {
        Task<ResponseContext<T>> AddAsync(T entity);

        Task<ResponseContext<T>> UpdateAsync(T entity, params Expression<Func<T, bool>>[] keySelectors);

        Task<ResponseContext> DeleteAsync(params Expression<Func<T, bool>>[] keySelectors);
    }
}
