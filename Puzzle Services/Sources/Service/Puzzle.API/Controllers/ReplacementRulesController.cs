using System.Threading.Tasks;
using System.Web.Http;

using Puzzle.Domain;

namespace Puzzle.API.Controllers
{
    [RoutePrefix("PuzzleServices/ReplacementRules")]
    public class ReplacementRulesController : BaseApiController
    {
        #region CRUD
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll(int page = 1, int pageSize = 50, bool descendingOrder = false)
        {
            return Ok(await Puzzle.ReplacementRules.GetAllAsync(c => c.Id, page, pageSize, descendingOrder));
        }

        [HttpGet]
        [Route("{ruleId:int}")]
        public async Task<IHttpActionResult> GetById(int ruleId)
        {
            return Ok(await Puzzle.ReplacementRules.GetByIdAsync(c => c.Id == ruleId));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Add(ReplacementRule rule)
        {
            return Ok(await Puzzle.ReplacementRules.AddAsync(rule));
        }

        [HttpPut]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> Update(ReplacementRule rule)
        {
            return Ok(await Puzzle.ReplacementRules.UpdateAsync(rule, c => c.Id == rule.Id));
        }

        [HttpDelete]
        [Route("{ruleId:int}")]
        public async Task<IHttpActionResult> Remove(int ruleId)
        {
            return Ok(await Puzzle.ReplacementRules.DeleteAsync(c => c.Id == ruleId));
        }
        #endregion
    }
}
