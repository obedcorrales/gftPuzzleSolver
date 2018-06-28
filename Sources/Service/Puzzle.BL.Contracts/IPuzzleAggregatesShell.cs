using Puzzle.BL.Contracts.Markov;

namespace Puzzle.BL.Contracts
{
    public interface IPuzzleAggregatesShell
    {
        IReplacementRules ReplacementRules { get; }
        ICyphers Cyphers { get; }
    }
}
