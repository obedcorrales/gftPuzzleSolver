using System.Security.Claims;

namespace dF.Commons.Services.BL.Contracts.Context
{
    public interface ISecurityContext
    {
        ClaimsPrincipal Principal { get; }
    }
}
