using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;

using dF.Commons.Models.BL;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Security.Constants;
using dF.Commons.Security.Helpers;
using dF.Commons.Services.BL;
using dF.Commons.Services.BL.Helpers;

using Puzzle.BL.Contracts.Context;
using Puzzle.BL.Contracts.Markov;
using Puzzle.Data.Contracts;
using Puzzle.Domain;

namespace Puzzle.BL.Markov
{
    public class ReplacementRules : BusinessAggregate<ReplacementRule>, IReplacementRules
    {
        #region Parameters
        public new static string ResourceName => CypherResources.ResourceName;
        public new static string PluralResourceName => CypherResources.PluralResourceName;

        static Lazy<List<ResourceLink>> _resourceMap = new Lazy<List<ResourceLink>>(() => ResourceLinkBuilder.BuildResourceMap<ReplacementRules>());
        public new static List<ResourceLink> ResourceMap => _resourceMap.Value;

        // Security
        static Func<ClaimsPrincipal, Actions, Result> _canAccessReplacementRules = (principal, action) => Result.Ok();
        //static Func<ClaimsPrincipal, Actions, Result> _canAccessReplacementRules = (principal, action) => principal.CheckResourceAccess(action, new string[] { ResourceName });
        #endregion

        public ReplacementRules(IUow uow, IPuzzleContext context, bool commitInmediately = true, params Expression<Func<ReplacementRule, bool>>[] parentKeys) :
            base(new BusinessAggregateParams<ReplacementRule>(uow, uow.ReplacementRules, context, _canAccessReplacementRules, ResourceMap, ResourceName, PluralResourceName, parentKeys),
                commitInmediately)
        { }

        public ReplacementRules(Result result) : base(result, ResourceName) { }

        #region Children
        IReplacementRulesChildren _children = null;

        public IReplacementRulesChildren WithId(int replacementRuleId)
        {
            return this.Then(() => OnGetByIdSecurityClearance())
                .WithResultDo(r =>
                {
                    if (r.IsSuccess)
                        return _children?.ReplacementRuleId == replacementRuleId ? _children : (_children = new ReplacementRulesChildren(AggregateParams, replacementRuleId));
                    else
                        return new ReplacementRulesChildren((Result)r);
                });
        }
        #endregion
    }

    internal class ReplacementRulesChildren : Result, IReplacementRulesChildren
    {
        protected IUow _uow = null;
        protected IPuzzleContext _context = null;
        public int ReplacementRuleId { get; }

        public ReplacementRulesChildren(BusinessAggregateParams<ReplacementRule> parentParams, int replacementRuleId)
        {
            _uow = (IUow)parentParams.UOW;
            _context = (IPuzzleContext)parentParams.Context;
            ReplacementRuleId = replacementRuleId;
        }

        public ReplacementRulesChildren(Result result) : base(result) { }

        ICypherSchemes _schemas = null;

        public ICypherSchemes Schemes => _schemas ?? (_schemas = IsSuccess ? new CypherSchemes(_uow, _context, true, s => s.ReplacementRuleId == ReplacementRuleId) : new CypherSchemes(this));
    }
}
