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
        static DbSet<PuzzleWord> _puzzleWords = null;

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

                base.Add(_puzzleWords = DbSet<PuzzleWord>
                    .withKey(k => k.Id)
                    .andSeed(DataInitializer.PuzzleWords_SeedData()));

                initialized = true;
            }
            else
            {
                base.Add(_cyphers);
                base.Add(_cypherSchemes);
                base.Add(_replacementRules);
                base.Add(_puzzleWords);
            }
        }

        public virtual DbSet<Cypher> Cyphers => _cyphers;
        public virtual DbSet<CypherScheme> CypherSchemes => _cypherSchemes;
        public virtual DbSet<ReplacementRule> ReplacementRules => _replacementRules;
        public virtual DbSet<PuzzleWord> PuzzleWords => _puzzleWords;
    }
}
