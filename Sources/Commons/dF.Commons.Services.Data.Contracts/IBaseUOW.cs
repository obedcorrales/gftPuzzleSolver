using System.Threading.Tasks;

using dF.Commons.Models.Globals;

namespace dF.Commons.Services.Data.Contracts
{
    public interface IBaseUOW
    {
        Result<int> Commit();
        Task<Result<int>> CommitAsync();

        Result<int> CommitWithinTransaction();
        Task<Result<int>> CommitWithinTransactionAsync();
    }
}
