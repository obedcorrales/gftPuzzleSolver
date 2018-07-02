using System.Threading.Tasks;

using dF.Commons.Models.BL;
using dF.Commons.Services.BL.Contracts;

using Puzzle.Domain;

namespace Puzzle.BL.Contracts.Markov
{
    public static class CypherResources
    {
        public const string ResourceName = "Cypher";
        public const string PluralResourceName = "Cyphers";
    }

    public interface ICyphers : IBusinessAggregate<Cypher>
    {
        Task<ResponseContext<string>> SolveMarkov(int cypherId);
        ICyphersChildren WithId(int cypherId);
    }

    public interface ICyphersChildren
    {
        int CypherId { get; }

        ICypherSchemes Schemes { get; }
    }
}
