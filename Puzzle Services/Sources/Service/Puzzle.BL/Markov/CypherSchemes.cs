using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CypherSchemes : BusinessAggregate<CypherScheme>, ICypherSchemes
    {
        #region Parameters
        public new static string ResourceName => CypherSchemeResources.ResourceName;
        public new static string PluralResourceName => CypherSchemeResources.PluralResourceName;

        static Lazy<List<ResourceLink>> _resourceMap = new Lazy<List<ResourceLink>>(() => ResourceLinkBuilder.BuildResourceMap<CypherSchemes>());
        public new static List<ResourceLink> ResourceMap => _resourceMap.Value;

        // Security
        static Func<ClaimsPrincipal, Actions, Result> _canAccessCypherSchemes = (principal, action) => Result.Ok();
        //static Func<ClaimsPrincipal, Actions, Result> _canAccessCypherSchemes = (principal, action) => principal.CheckResourceAccess(action, new string[] { ResourceName });
        #endregion

        public CypherSchemes(IUow uow, IPuzzleContext context, bool commitInmediately = true, params Expression<Func<CypherScheme, bool>>[] parentKeys) :
            base(new BusinessAggregateParams<CypherScheme>(uow, uow.CypherSchemes, context, _canAccessCypherSchemes, ResourceMap, ResourceName, PluralResourceName, parentKeys),
                commitInmediately)
        { }

        public CypherSchemes(Result result) : base(result, ResourceName) { }

        #region Children
        ICypherSchemesChildren _children = null;

        public ICypherSchemesChildren WithId(int? cypherId, int? replacementRuleId)
        {
            return this.Then(() => OnGetByIdSecurityClearance())
                .WithResultDo(r =>
                {
                    if (r.IsSuccess)
                        return _children?.CypherId == cypherId ? _children : (_children = new CypherSchemesChildren(AggregateParams, cypherId, null));
                    else
                        return new CypherSchemesChildren((Result)r);
                });
        }

        private async Task<Result<IList<CypherScheme>>> _getAllReplacementRules<TKey>(Expression<Func<CypherScheme, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<CypherScheme, bool>>[] where)
        {
            var schemes = await GetAllCoreAsync(orderBy, page, pageSize, descendingOrder, where);

            IList<int> ruleIds = new List<int>();
            foreach (var scheme in schemes.Value)
                ruleIds.Add(scheme.ReplacementRuleId);

            var rules = await ((IUow)AggregateParams.UOW).ReplacementRules.GetQueryable().Where(r => ruleIds.Contains(r.Id)).GetAllAsync();

            foreach (var scheme in schemes.Value)
            {
                var rule = rules.Value.First(r => r.Id == scheme.ReplacementRuleId);
                if (rule != null)
                    scheme.ReplacementRule = rule;
            }

            return schemes;
        }

        public async Task<ResponseContext<IList<CypherScheme>>> GetAllReplacementRules<TKey>(Expression<Func<CypherScheme, TKey>> orderBy, int page = 1, int? pageSize = 50, bool descendingOrder = false, params Expression<Func<CypherScheme, bool>>[] where)
        {
            var schemes = await _getAllReplacementRules(orderBy, page, pageSize, descendingOrder, where);

            return schemes.ToResponseContext()
                    .Then(response => response.withRecordCount(response.Result.Count))
                    .ThenIf(AggregateParams.Context.IsHATEOASRequest, response => GetAllHATEOAS(response, page, pageSize.Value));
        }
        #endregion
    }

    internal class CypherSchemesChildren : Result, ICypherSchemesChildren
    {
        protected IUow _uow = null;
        protected IPuzzleContext _context = null;
        public int CypherId { get; }
        public int ReplacementRuleId { get; }

        private Lazy<Result<int>> _internalUserId = null;
        private Result<int> InternalUserId => _internalUserId.Value;

        public CypherSchemesChildren(BusinessAggregateParams<CypherScheme> parentParams, int? cypherId, int? replacementRuleId)
        {
            _uow = (IUow)parentParams.UOW;
            _context = (IPuzzleContext)parentParams.Context;
            CypherId = cypherId.Value;
            ReplacementRuleId = replacementRuleId.Value;
        }

        public CypherSchemesChildren(Result result) : base(result) { }

        ICyphers _cyphers = null;
        IReplacementRules _replacementRules = null;

        public ICyphers Cyphers => _cyphers ?? (_cyphers = IsSuccess ? new Cyphers(_uow, _context, true, c => c.Id == CypherId) : new Cyphers(this));
        public IReplacementRules ReplacementRules => _replacementRules ?? (_replacementRules = IsSuccess ? new ReplacementRules(_uow, _context, true, r => r.Id == ReplacementRuleId) : new ReplacementRules(this));
    }
}
