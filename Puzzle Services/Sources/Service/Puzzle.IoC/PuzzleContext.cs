using System.Security.Claims;

using Puzzle.BL.Contracts.Context;

namespace Puzzle.IoC
{
    public class PuzzleContext : IPuzzleContext
    {
        public bool IsHATEOASRequest { get; private set; }

        public ClaimsPrincipal Principal { get; private set; }

        public PuzzleContext(ClaimsPrincipal securityPrincipal, bool isHATEOASRequest = false)
        {
            Principal = securityPrincipal;
            IsHATEOASRequest = isHATEOASRequest;
        }
    }
}
