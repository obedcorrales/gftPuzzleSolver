namespace Puzzle.Models.InMemory.Models
{
    internal class RawCypherScheme
    {
        public virtual int Rule { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsTermination { get; set; }
    }
}
