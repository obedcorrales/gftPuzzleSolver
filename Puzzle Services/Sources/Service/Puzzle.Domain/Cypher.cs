using System.Collections.Generic;

namespace Puzzle.Domain
{
    public partial class Cypher
    {
        public Cypher()
        {
        }

        public virtual int Id { get; set; }
        public string CypherText { get; set; }

        // Relations
        public virtual ICollection<CypherScheme> Schemes { get; set; }
    }
}
