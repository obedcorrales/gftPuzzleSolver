using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dF.Commons.Models.Globals;

namespace dF.Commons.Services.Data.Contracts
{
    public interface IRepositoryCachedReference<T> where T : class
    {
        DateTime ExpirationTime { get; }
        bool IsValid { get; }

        Result<IEnumerable<T>> GetCache();
        Task<Result<IEnumerable<T>>> GetCacheAsync();
    }
}
