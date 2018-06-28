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

namespace Puzzle.BL
{
    public class Cyphers : BusinessAggregate<Cypher>, ICyphers
    {
        #region Parameters
        public new static string ResourceName => CypherResources.ResourceName;
        public new static string PluralResourceName => CypherResources.PluralResourceName;

        static Lazy<List<ResourceLink>> _resourceMap = new Lazy<List<ResourceLink>>(() => ResourceLinkBuilder.BuildResourceMap<Cyphers>());
        public new static List<ResourceLink> ResourceMap => _resourceMap.Value;

        // Security
        static Func<ClaimsPrincipal, Actions, Result> _canAccessUsers = (principal, action) => principal.CheckResourceAccess(action, new string[] { ResourceName });
        #endregion

        public Cyphers(IUow uow, IPuzzleContext context, bool commitInmediately = true, params Expression<Func<Cypher, bool>>[] parentKeys) :
            base(new BusinessAggregateParams<Cypher>(uow, uow.Cyphers, context, _canAccessUsers, ResourceMap, ResourceName, PluralResourceName, parentKeys),
                commitInmediately)
        { }

        public Cyphers(Result result) : base(result, ResourceName) { }

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
