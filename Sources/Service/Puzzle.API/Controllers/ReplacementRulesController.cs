using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
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
            #region ToBeDeleted
            //var pathToReplacementRules = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\Databases\\JSON.DB\\base.json");

            //StreamReader stream = new StreamReader(pathToReplacementRules);
            //string data = stream.ReadToEnd();
            //Console.WriteLine(data);

            //IList<ReplacementRule> jsonReplacementRules = JsonConvert.DeserializeObject<List<ReplacementRule>>(data);

            //IList<ReplacementRule> ReplacementRules = new List<ReplacementRule>();
            //int id = 0;

            //foreach (var rule in jsonReplacementRules)
            //{
            //    rule.Id = id++;
            //    Console.WriteLine(rule);
            //    ReplacementRules.Add(rule);
            //}
            #endregion

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
