using System.Threading.Tasks;
using System.Web.Http;

using Puzzle.Domain;

namespace Puzzle.API.Controllers
{
    [RoutePrefix("PuzzleServices/PuzzleWords")]
    public class PuzzleWordsController : BaseApiController
    {
        #region CRUD
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll(int page = 1, int pageSize = 50, bool descendingOrder = false)
        {
            return Ok(await Puzzle.PuzzleWords.GetAllAsync(c => c.Id, page, pageSize, descendingOrder));
        }

        [HttpGet]
        [Route("{wordId:int}")]
        public async Task<IHttpActionResult> GetById(int wordId)
        {
            return Ok(await Puzzle.PuzzleWords.GetByIdAsync(c => c.Id == wordId));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Add(PuzzleWord word)
        {
            return Ok(await Puzzle.PuzzleWords.AddAsync(word));
        }

        [HttpPut]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> Update(PuzzleWord word)
        {
            return Ok(await Puzzle.PuzzleWords.UpdateAsync(word, c => c.Id == word.Id));
        }

        [HttpDelete]
        [Route("{wordId:int}")]
        public async Task<IHttpActionResult> Remove(int wordId)
        {
            return Ok(await Puzzle.PuzzleWords.DeleteAsync(c => c.Id == wordId));
        }
        #endregion
    }
}
