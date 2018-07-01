using System.Collections.Generic;

namespace Puzzle.Domain
{
    public partial class CypherScheme
    {
        public CypherScheme()
        {
        }

        public virtual int CypherId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual int ReplacementRuleId { get; set; }
        public virtual bool IsTermination { get; set; }

        // Relations
        public virtual Cypher Cypher { get; set; }
        public virtual ReplacementRule ReplacementRule { get; set; }
    }
}
