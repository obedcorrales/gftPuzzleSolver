using System.Threading.Tasks;
using System.Web.Http;
using Puzzle.Domain;

namespace Puzzle.API.Controllers
{
    [RoutePrefix("PuzzleServices/Cyphers")]
    public class CyphersController : BaseApiController
    {
        #region CRUD
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll(int page = 1, int pageSize = 50, bool descendingOrder = false)
        {
            return Ok(await Puzzle.Cyphers.GetAllAsync(c => c.Id, page, pageSize, descendingOrder));
        }

        [HttpGet]
        [Route("{cypherId:int}")]
        public async Task<IHttpActionResult> GetById(int cypherId)
        {
            return Ok(await Puzzle.Cyphers.GetByIdAsync(c => c.Id == cypherId));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Add(Cypher cypher)
        {
            return Ok(await Puzzle.Cyphers.AddAsync(cypher));
        }

        [HttpPut]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> Update(Cypher cypher)
        {
            return Ok(await Puzzle.Cyphers.UpdateAsync(cypher, c => c.Id == cypher.Id));
        }

        [HttpDelete]
        [Route("{cypherId:int}")]
        public async Task<IHttpActionResult> Remove(int cypherId)
        {
            return Ok(await Puzzle.Cyphers.DeleteAsync(c => c.Id == cypherId));
        }
        #endregion

        #region Cypher Scheme CRUD
        [HttpGet]
        [Route("{cypherId:int}/scheme")]
        public async Task<IHttpActionResult> SchemeGetAll(int cypherId, int page = 1, int pageSize = 50, bool descendingOrder = false)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.GetAllAsync(s => s.OrderId, page, pageSize, descendingOrder));
        }

        [HttpGet]
        [Route("{cypherId:int}/scheme/rules")]
        public async Task<IHttpActionResult> SchemeGetAllRules(int cypherId, int page = 1, int pageSize = 50, bool descendingOrder = false)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.GetAllReplacementRules(s => s.OrderId, page, pageSize, descendingOrder));
        }

        [HttpGet]
        [Route("{cypherId:int}/scheme/{orderId:int}")]
        public async Task<IHttpActionResult> SchemeGetById(int cypherId, int orderId)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.GetByIdAsync(s => s.OrderId == orderId));
        }

        [HttpPost]
        [Route("{cypherId:int}/scheme")]
        public async Task<IHttpActionResult> SchemeAdd(int cypherId, CypherScheme scheme)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.AddAsync(scheme));
        }

        [HttpPut]
        [HttpPatch]
        [Route("{cypherId:int}/scheme")]
        public async Task<IHttpActionResult> SchemeUpdate(int cypherId, CypherScheme scheme)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.UpdateAsync(scheme, s => s.OrderId == scheme.OrderId));
        }

        [HttpDelete]
        [Route("{cypherId:int}/scheme/{orderId:int}")]
        public async Task<IHttpActionResult> SchemeRemove(int cypherId, int orderId)
        {
            return Ok(await Puzzle.Cyphers.WithId(cypherId).Schemes.DeleteAsync(s => s.OrderId == orderId));
        }
        #endregion

        #region Solve Markov Cypher
        [HttpGet]
        [Route("{cypherId:int}/solve")]
        public async Task<IHttpActionResult> SolveMarkov(int cypherId)
        {
            return Ok(await Puzzle.Cyphers.SolveMarkov(cypherId));
        }
        #endregion
    }
}
