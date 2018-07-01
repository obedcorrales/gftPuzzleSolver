using Puzzle.BL;
using Puzzle.BL.Contracts;
using Puzzle.BL.Contracts.Context;
using Puzzle.Data.InMemory;

namespace Puzzle.IoC
{
    public class PuzzlesShell : PuzzleAggregatesShell, IPuzzleAggregatesShell
    {
        public PuzzlesShell(IPuzzleContext context) :
            base(new UOW(), context)
        { }
    }
}
