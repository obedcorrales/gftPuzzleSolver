using Puzzle.BL.Contracts;
using Puzzle.BL.Contracts.Context;
using Puzzle.BL.Contracts.Markov;
using Puzzle.BL.Markov;
using Puzzle.Data.Contracts;

namespace Puzzle.BL
{
    public class PuzzleAggregatesShell : IPuzzleAggregatesShell
    {
        IUow _uow;
        IPuzzleContext _context;

        #region Users
        IReplacementRules _replacementRules = null;
        ICyphers _cyphers = null;

        public IReplacementRules ReplacementRules => _replacementRules ?? (_replacementRules = new ReplacementRules(_uow, _context, true));
        public ICyphers Cyphers => _cyphers ?? (_cyphers = new Cyphers(_uow, _context, true));
        #endregion

        public PuzzleAggregatesShell(IUow uow, IPuzzleContext context)
        {
            _uow = uow;
            _context = context;
        }
    }
}
