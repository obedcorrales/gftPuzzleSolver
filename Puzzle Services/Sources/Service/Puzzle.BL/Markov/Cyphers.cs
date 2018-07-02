using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

using dF.Commons.Models.BL;
using dF.Commons.Models.BL.Extensions;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Security.Constants;
//using dF.Commons.Security.Helpers;
using dF.Commons.Services.BL;
using dF.Commons.Services.BL.Helpers;

using Puzzle.BL.Contracts.Context;
using Puzzle.BL.Contracts.Markov;
using Puzzle.Data.Contracts;
using Puzzle.Domain;

namespace Puzzle.BL.Markov
{
    public class Cyphers : BusinessAggregate<Cypher>, ICyphers
    {
        #region Parameters
        public new static string ResourceName => CypherResources.ResourceName;
        public new static string PluralResourceName => CypherResources.PluralResourceName;

        static Lazy<List<ResourceLink>> _resourceMap = new Lazy<List<ResourceLink>>(() => ResourceLinkBuilder.BuildResourceMap<Cyphers>());
        public new static List<ResourceLink> ResourceMap => _resourceMap.Value;

        // Security
        static Func<ClaimsPrincipal, Actions, Result> _canAccessCyphers = (principal, action) => Result.Ok();
        //static Func<ClaimsPrincipal, Actions, Result> _canAccessCyphers = (principal, action) => principal.CheckResourceAccess(action, new string[] { ResourceName });
        #endregion

        public Cyphers(IUow uow, IPuzzleContext context, bool commitInmediately = true, params Expression<Func<Cypher, bool>>[] parentKeys) :
            base(new BusinessAggregateParams<Cypher>(uow, uow.Cyphers, context, _canAccessCyphers, ResourceMap, ResourceName, PluralResourceName, parentKeys),
                commitInmediately)
        { }

        public Cyphers(Result result) : base(result, ResourceName) { }

        #region Markov Solver
        public async Task<ResponseContext<string>> SolveMarkov(int cypherId)
        {
            string cypher = "";

            return await GetByIdAsync(c => c.Id == cypherId).MapAsync(cypherData =>
            {
                return ((WithId(cypherId).Schemes.GetAllReplacementRules(s => s.OrderId, 0, null).MapAsync(schemeData =>
                {
                    cypher = cypherData.CypherText;
                    var schemeLength = schemeData.Count;
                    var solved = false;

                    do
                    {
                        var i = 0;
                        var original = cypher;

                        foreach (var scheme in schemeData)
                        {
                            i++;
                            cypher = cypher.Replace(scheme.ReplacementRule.Source, scheme.ReplacementRule.Replacement);

                            if (cypher != original)
                            {
                                if (scheme.IsTermination)
                                    solved = true;

                                break;
                            }
                            else if (i == schemeLength)
                            {
                                solved = true;
                                break;
                            }
                        }
                    } while (!solved);

                    return cypher;
                })));
            })
            .ReturnAsync(() => ResponseContext.Ok(cypher));

            /// ****************************************************************************
            /// 
            /// The Non-Functional way has the characteristic that should
            ///     any part of the process error out, the message would
            ///     NOT boil up all the way to the UI.
            ///     
            /// The Functional way, on the other hand, allows for the processes
            ///     to continue their normal course down their happy paths,
            ///     but catches all failures on any of the unhappy paths
            ///     and bubbles them up to the UI effortlessly.
            /// 
            /// As an exercise, comment the code above and uncomment the code bellow,
            ///     then try to make a request for Cypher #15 (which does not exist).
            ///     Check the outputs.
            /// 
            /// ****************************************************************************
            #region Non-Functional Way
            //var cypherData = await GetByIdAsync(c => c.Id == cypherId);
            //var schemeData = await WithId(cypherId).Schemes.GetAllReplacementRules(s => s.OrderId, 0, null);

            //var cypher = cypherData.Result.CypherText;
            //var schemeLength = schemeData.RecordCount;
            //var solved = false;

            //do
            //{
            //    var i = 0;
            //    var original = cypher;

            //    foreach (var scheme in schemeData.Result)
            //    {
            //        i++;
            //        cypher = cypher.Replace(scheme.ReplacementRule.Source, scheme.ReplacementRule.Replacement);

            //        if (cypher != original)
            //        {
            //            if (scheme.IsTermination)
            //                solved = true;

            //            break;
            //        }
            //        else if (i == schemeLength)
            //        {
            //            solved = true;
            //            break;
            //        }
            //    }
            //} while (!solved);

            //return ResponseContext.Ok(cypher);
            #endregion
        }
        #endregion

        #region Children
        ICyphersChildren _children = null;

        public ICyphersChildren WithId(int cypherId)
        {
            return this.Then(() => OnGetByIdSecurityClearance())
                .WithResultDo(r =>
                {
                    if (r.IsSuccess)
                        return _children?.CypherId == cypherId ? _children : (_children = new CyphersChildren(AggregateParams, cypherId));
                    else
                        return new CyphersChildren((Result)r);
                });
        }
        #endregion
    }

    internal class CyphersChildren : Result, ICyphersChildren
{
        protected IUow _uow = null;
        protected IPuzzleContext _context = null;
        public int CypherId { get; }

        public CyphersChildren(BusinessAggregateParams<Cypher> parentParams, int cypherId)
        {
            _uow = (IUow)parentParams.UOW;
            _context = (IPuzzleContext)parentParams.Context;
            CypherId = cypherId;
        }

        public CyphersChildren(Result result) : base(result) { }

        ICypherSchemes _schemas = null;

        public ICypherSchemes Schemes => _schemas ?? (_schemas = IsSuccess ? new CypherSchemes(_uow, _context, true, s => s.CypherId == CypherId) : new CypherSchemes(this));
    }
}
