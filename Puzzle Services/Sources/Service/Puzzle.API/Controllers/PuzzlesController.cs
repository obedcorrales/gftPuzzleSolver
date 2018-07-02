using System.Threading.Tasks;
using System.Web.Http;

namespace Puzzle.API.Controllers
{
    [RoutePrefix("PuzzleServices/Puzzles")]
    public class PuzzlesController : BaseApiController
    {
        #region CRUD
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetMatrix()
        {
            return Ok(await Puzzle.Puzzles.GetMatrix());
        }

        [HttpGet]
        [Route("solve")]
        public async Task<IHttpActionResult> SolvePuzzle()
        {
            return Ok(await Puzzle.Puzzles.SolvePuzzle());
        }
        #endregion
    }
}
