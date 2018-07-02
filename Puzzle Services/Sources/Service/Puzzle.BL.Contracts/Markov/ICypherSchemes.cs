using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Constants;
using dF.Commons.Services.BL.Contracts;

using Puzzle.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.BL.Contracts.Markov
{
    public static class CypherSchemeResources
    {
        public const string ResourceName = "CypherScheme";
        public const string PluralResourceName = "CypherSchemes";
    }

    public interface ICypherSchemes : IBusinessAggregate<CypherScheme>
    {
        Task<ResponseContext<IList<CypherScheme>>> GetAllReplacementRules<TKey>(Expression<Func<CypherScheme, TKey>> orderBy, int page = 1, int? pageSize = Paging.pageSize, bool descendingOrder = false, params Expression<Func<CypherScheme, bool>>[] where);
        ICypherSchemesChildren WithId(int? cypherId, int? replacementRuleId);
    }

    public interface ICypherSchemesChildren
    {
        int CypherId { get; }
        int ReplacementRuleId { get; }

        ICyphers Cyphers { get; }
        IReplacementRules ReplacementRules { get; }
    }
}
