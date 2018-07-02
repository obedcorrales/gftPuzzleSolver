namespace Puzzle.Domain
{
    public partial class PuzzleMatrix
    {
        public PuzzleMatrix()
        {
        }

        public virtual int Column { get; set; }
        public virtual int Row { get; set; }
        public char Character { get; set; }
    }
}
