using Puzzle.BL.Contracts.Markov;
using Puzzle.BL.Contracts.Puzzle;

namespace Puzzle.BL.Contracts
{
    public interface IPuzzleAggregatesShell
    {
        IReplacementRules ReplacementRules { get; }
        ICyphers Cyphers { get; }
        IPuzzleWords PuzzleWords { get; }
        IPuzzles Puzzles { get; }
    }
}
