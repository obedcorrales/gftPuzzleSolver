using dF.Commons.Services.BL.Contracts;

using Puzzle.Domain;

namespace Puzzle.BL.Contracts.Markov
{
    public static class ReplacementRuleResources
    {
        public const string ResourceName = "ReplacementRule";
        public const string PluralResourceName = "ReplacementRules";
    }

    public interface IReplacementRules : IBusinessAggregate<ReplacementRule>
    {
        IReplacementRulesChildren WithId(int replacementRuleId);
    }

    public interface IReplacementRulesChildren
    {
        int ReplacementRuleId { get; }

        ICypherSchemes Schemes { get; }
    }
}
