using System;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

using Puzzle.BL.Contracts;
using Puzzle.IoC;

namespace Puzzle.API.Controllers
{
    public class BaseApiController : ApiController
    {
        private Lazy<IPuzzleAggregatesShell> _puzzleServices;

        protected IPuzzleAggregatesShell Puzzle { get { return _puzzleServices.Value; } }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            _puzzleServices = new Lazy<IPuzzleAggregatesShell>(() =>
                new PuzzlesShell(
                        new PuzzleContext(
                                controllerContext.RequestContext.Principal as ClaimsPrincipal,
                                false
                )));

            base.Initialize(controllerContext);
        }
    }
}
