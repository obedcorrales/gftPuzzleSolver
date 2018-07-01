using dF.Commons.Services.Data.InMemory;
using Puzzle.Domain;

namespace Puzzle.Models.InMemory
{
    public class Puzzle_Context : DbContext
    {
        static bool initialized = false;
        static DbSet<Cypher> _cyphers = null;
        static DbSet<CypherScheme> _cypherSchemes = null;
        static DbSet<ReplacementRule> _replacementRules = null;

        public Puzzle_Context()
        {
            if (!initialized)
            {
                base.Add(_cyphers = DbSet<Cypher>
                    .withKey(k => k.Id)
                    .andSeed(DataInitializer.Cyphers_SeedData()));

                base.Add(_cypherSchemes = DbSet<CypherScheme>
                    .withKey(k => k.CypherId)
                    .andKey(k => k.OrderId)
                    .andSeed(DataInitializer.CypherSchemes_SeedData()));

                base.Add(_replacementRules = DbSet<ReplacementRule>
                    .withKey(k => k.Id)
                    .andSeed(DataInitializer.ReplacementRules_SeedData()));

                initialized = true;
            }
            else
            {
                base.Add(_cyphers);
                base.Add(_cypherSchemes);
                base.Add(_replacementRules);
            }
        }

        public virtual DbSet<Cypher> Cyphers => _cyphers;
        public virtual DbSet<CypherScheme> CypherSchemes => _cypherSchemes;
        public virtual DbSet<ReplacementRule> ReplacementRules => _replacementRules;
    }
}
