using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;

using dF.Commons.Models.BL;
using dF.Commons.Models.Globals;
using dF.Commons.Security.Constants;
using dF.Commons.Services.BL;
using dF.Commons.Services.BL.Helpers;

using Puzzle.BL.Contracts.Context;
using Puzzle.BL.Contracts.Puzzle;
using Puzzle.Data.Contracts;
using Puzzle.Domain;

namespace Puzzle.BL.Puzzle
{
    public class PuzzleWords : BusinessAggregate<PuzzleWord>, IPuzzleWords
    {
        #region Parameters
        public new static string ResourceName => PuzzleWordsResources.ResourceName;
        public new static string PluralResourceName => PuzzleWordsResources.PluralResourceName;

        static Lazy<List<ResourceLink>> _resourceMap = new Lazy<List<ResourceLink>>(() => ResourceLinkBuilder.BuildResourceMap<PuzzleWords>());
        public new static List<ResourceLink> ResourceMap => _resourceMap.Value;

        // Security
        static Func<ClaimsPrincipal, Actions, Result> _canAccessPuzzleWords = (principal, action) => Result.Ok();
        //static Func<ClaimsPrincipal, Actions, Result> _canAccessReplacementRules = (principal, action) => principal.CheckResourceAccess(action, new string[] { ResourceName });
        #endregion

        public PuzzleWords(IUow uow, IPuzzleContext context, bool commitInmediately = true, params Expression<Func<PuzzleWord, bool>>[] parentKeys) :
            base(new BusinessAggregateParams<PuzzleWord>(uow, uow.PuzzleWords, context, _canAccessPuzzleWords, ResourceMap, ResourceName, PluralResourceName, parentKeys),
                commitInmediately)
        { }

        public PuzzleWords(Result result) : base(result, ResourceName) { }
    }
}
