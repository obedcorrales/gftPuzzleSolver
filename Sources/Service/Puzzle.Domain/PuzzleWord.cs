namespace Puzzle.Domain
{
    public partial class PuzzleWord
    {
        public PuzzleWord()
        {
        }

        public virtual int Id { get; set; }
        public string Word { get; set; }
    }
}
