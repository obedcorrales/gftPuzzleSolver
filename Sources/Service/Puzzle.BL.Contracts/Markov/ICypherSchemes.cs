using dF.Commons.Services.BL.Contracts;

using Puzzle.Domain;

namespace Puzzle.BL.Contracts.Markov
{
    public static class CypherSchemeResources
    {
        public const string ResourceName = "CypherScheme";
        public const string PluralResourceName = "CypherSchemes";
    }

    public interface ICypherSchemes : IBusinessAggregate<CypherScheme>
    {
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
