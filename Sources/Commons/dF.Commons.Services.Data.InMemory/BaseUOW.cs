using dF.Commons.Models.Globals;
using dF.Commons.Services.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace dF.Commons.Services.Data.InMemory
{
    public class BaseUOW : IBaseUOW
    {
        private DbContext _dbContext;

        protected DbContext dbContext { get { return _dbContext; } }

        public BaseUOW(DbContext context)
        {
            _dbContext = context;
        }

        public virtual Result<int> Commit()
        {
            return Result.Ok(1);
        }

        public virtual Task<Result<int>> CommitAsync()
        {
            return Task.FromResult(Result.Ok(1));
        }

        public Result<int> CommitWithinTransaction()
        {
            return Commit();
        }

        public Task<Result<int>> CommitWithinTransactionAsync()
        {
            return CommitAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                    _dbContext.Dispose();
            }
        }
    }
}
