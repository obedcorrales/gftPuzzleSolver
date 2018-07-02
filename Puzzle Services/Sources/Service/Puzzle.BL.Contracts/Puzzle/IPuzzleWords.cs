using dF.Commons.Services.BL.Contracts;

using Puzzle.Domain;

namespace Puzzle.BL.Contracts.Puzzle
{
    public static class PuzzleWordsResources
    {
        public const string ResourceName = "PuzzleWord";
        public const string PluralResourceName = "PuzzleWords";
    }

    public interface IPuzzleWords : IBusinessAggregate<PuzzleWord>
    {
    }
}
