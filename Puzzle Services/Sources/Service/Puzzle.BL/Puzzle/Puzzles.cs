using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dF.Commons.Models.BL;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;

using Puzzle.BL.Contracts.Context;
using Puzzle.BL.Contracts.Markov;
using Puzzle.BL.Contracts.Puzzle;
using Puzzle.BL.Markov;
using Puzzle.Data.Contracts;
using Puzzle.Domain;

namespace Puzzle.BL.Puzzle
{
    public class Puzzles : IPuzzles
    {
        protected IUow _uow = null;
        protected IPuzzleContext _context = null;

        #region BL Dependencies
        ICyphers _cyphers = null;
        IPuzzleWords _puzzleWords = null;

        protected ICyphers Cyphers => _cyphers ?? (_cyphers = new Cyphers(_uow, _context, true));
        protected IPuzzleWords PuzzleWords => _puzzleWords ?? (_puzzleWords = new PuzzleWords(_uow, _context, true));
        #endregion

        public Puzzles(IUow uow, IPuzzleContext context)
        {
            _uow = uow;
            _context = context;
        }

        // TODO: Make Functional
        public async Task<ResponseContext<IList<PuzzleMatrix>>> GetMatrix()
        {
            var cypherData = await Cyphers.GetAllAsync(c => c.Id, 0, null);

            var solvedCyphers = new List<string>();

            foreach (var cypher in cypherData.Result)
            {
                var markov = await Cyphers.SolveMarkov(cypher.Id);
                if (markov.IsSuccess)
                    solvedCyphers.Add(markov.Result);
            }

            IList<PuzzleMatrix> matrix = new List<PuzzleMatrix>();
            var j = 0;

            foreach (var markov in solvedCyphers)
            {
                j++;
                var i = 0;

                foreach (var character in markov.ToCharArray())
                {
                    i++;
                    matrix.Add(new PuzzleMatrix() { Row = j, Column = i, Character = character });
                }
            }

            return ResponseContext.Ok(matrix);
        }

        // TODO: Make Functional
        public async Task<ResponseContext<IList<PuzzleSolution>>> SolvePuzzle()
        {
            var matrix = await GetMatrix();
            var wordData = await PuzzleWords.GetAllAsync(c => c.Id, 0, null);

            IList<PuzzleSolution> solutions = new List<PuzzleSolution>();

            foreach (var word in wordData.Result)
            {
                var solution = FindWord(word, matrix.Result);
                if (solution.IsSuccess)
                    solutions.Add(solution);
                else
                    solutions.Add(new PuzzleSolution() { Word = word.Word, Breakdown = new List<PuzzleMatrix>() });
            }

            return ResponseContext.Ok(solutions);
        }

        #region Puzzle Solver
        Result<PuzzleSolution> FindWord(PuzzleWord word, IList<PuzzleMatrix> matrix)
        {
            var characters = word.Word.ToCharArray();
            var character = characters.FirstOrDefault();

            foreach (var cell in matrix)
            {
                if (cell.Character == character)
                {
                    var solutionMatrix = new List<PuzzleMatrix>();
                    solutionMatrix.Add(cell);

                    var solutionTrace = TraceWord(characters.Skip(1).ToArray(), cell, matrix, solutionMatrix);

                    if (solutionTrace.Count == characters.Length)
                        return Result.Ok(new PuzzleSolution() { Word = word.Word, Breakdown = solutionTrace });
                }
            }

            return Result.Fail<PuzzleSolution>("Word Not Found", ErrorType.EntityNotFound);
        }

        IList<PuzzleMatrix> TraceWord(char[] characters, PuzzleMatrix currentCell, IList<PuzzleMatrix> wholeMatrix, IList<PuzzleMatrix> solutionMatrix)
        {
            var character = characters.FirstOrDefault();
            var originalSolutionMatrix = new List<PuzzleMatrix>(solutionMatrix);
            var surroundings = FindSurroundings(currentCell, wholeMatrix, originalSolutionMatrix);

            foreach (var surrounding in surroundings)
            {
                if (surrounding.Character == character)
                {
                    originalSolutionMatrix.Add(surrounding);

                    if (characters.Length > 1)
                    {
                        var newSolutionMatrix = TraceWord(characters.Skip(1).ToArray(), surrounding, wholeMatrix, originalSolutionMatrix);

                        if (newSolutionMatrix.Count == solutionMatrix.Count + characters.Length)
                            return newSolutionMatrix;
                        else
                            originalSolutionMatrix.Remove(surrounding);
                    }
                    else
                    {
                        solutionMatrix.Add(surrounding);
                        break;
                    }
                }
            }

            return solutionMatrix;
        }

        IList<PuzzleMatrix> FindSurroundings(PuzzleMatrix point, IList<PuzzleMatrix> wholeMatrix, IList<PuzzleMatrix> solutionMatrix)
        {
            IList<PuzzleMatrix> surroundings = new List<PuzzleMatrix>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i != 0 || j != 0) && solutionMatrix.Count(s => s.Column == point.Column + i && s.Row == point.Row + j) == 0)
                    {
                        var surrounding = wholeMatrix.FirstOrDefault(m => m.Column == point.Column + i && m.Row == point.Row + j);

                        if (surrounding != null)
                            surroundings.Add(surrounding);
                    }
                }
            }

            return surroundings;
        }
        #endregion
    }
}
