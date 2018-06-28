using System.Security.Claims;

using dF.Commons.Services.BL.Contracts.Context;

namespace dF.Commons.Services.BL
{
    class BusinessAggregateContext : IBusinessAggregateContext
    {
        public bool IsHATEOASRequest { get; private set; }

        public ClaimsPrincipal Principal { get; private set; }

        public BusinessAggregateContext(ClaimsPrincipal securityPrincipal, bool isHATEOASRequest = false)
        {
            Principal = securityPrincipal;
            IsHATEOASRequest = isHATEOASRequest;
        }
    }
}
