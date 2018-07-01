using dF.Commons.Services.Data.Contracts;
using dF.Commons.Services.Data.InMemory;

using Puzzle.Data.Contracts;
using Puzzle.Domain;
using Puzzle.Models.InMemory;

namespace Puzzle.Data.InMemory
{
    public class UOW : BaseUOW, IUow
    {
        public UOW() : base(new Puzzle_Context()) { }

        IRepository<Cypher> _cyphers = null;
        IRepository<CypherScheme> _cypherSchemes = null;
        IRepository<ReplacementRule> _replacementRules = null;

        public IRepository<Cypher> Cyphers => _cyphers ?? (_cyphers = new Repository<Cypher>(dbContext));
        public IRepository<CypherScheme> CypherSchemes => _cypherSchemes ?? (_cypherSchemes = new Repository<CypherScheme>(dbContext));
        public IRepository<ReplacementRule> ReplacementRules => _replacementRules ?? (_replacementRules = new Repository<ReplacementRule>(dbContext));
    }
}
