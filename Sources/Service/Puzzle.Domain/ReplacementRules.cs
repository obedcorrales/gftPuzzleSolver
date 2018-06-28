using System.Collections.Generic;

namespace Puzzle.Domain
{
    public partial class ReplacementRule
    {
        public virtual int Id { get; set; }
        public virtual string Source { get; set; }
        public virtual string Replacement { get; set; }

        // Relations
        public virtual ICollection<CypherScheme> Schemes { get; set; }
    }
}
