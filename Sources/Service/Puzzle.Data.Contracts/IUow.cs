using dF.Commons.Services.Data.Contracts;

using Puzzle.Domain;

namespace Puzzle.Data.Contracts
{
    public interface IUow : IBaseUOW
    {
        IRepository<Cypher> Cyphers { get; }
        IRepository<CypherScheme> CypherSchemes { get; }
        IRepository<ReplacementRule> ReplacementRules { get; }

        IRepository<PuzzleWord> PuzzleWords { get; }
    }
}
