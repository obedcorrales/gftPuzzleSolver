using System.Collections.Generic;

namespace Puzzle.Domain
{
    public partial class PuzzleSolution
    {
        public PuzzleSolution()
        {
        }

        public virtual string Word { get; set; }
        public virtual IList<PuzzleMatrix> Breakdown { get; set; }
    }
}
