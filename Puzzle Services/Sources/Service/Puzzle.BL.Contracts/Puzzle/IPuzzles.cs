using System.Collections.Generic;
using System.Threading.Tasks;

using dF.Commons.Models.BL;

using Puzzle.Domain;

namespace Puzzle.BL.Contracts.Puzzle
{
    public static class PuzzleResources
    {
        public const string ResourceName = "Puzzle";
        public const string PluralResourceName = "Puzzles";
    }

    public interface IPuzzles
    {
        Task<ResponseContext<IList<PuzzleMatrix>>> GetMatrix();
        Task<ResponseContext<IList<PuzzleSolution>>> SolvePuzzle();
    }
}
