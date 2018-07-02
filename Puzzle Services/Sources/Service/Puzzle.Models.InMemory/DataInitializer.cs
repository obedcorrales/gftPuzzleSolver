using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puzzle.Domain;
using Puzzle.Models.InMemory.Models;

namespace Puzzle.Models.InMemory
{
    public class DataInitializer
    {
        public static IList<Cypher> Cyphers_SeedData()
        {
            var pathToCypher = Path.Combine(Directory.GetCurrentDirectory(), DBSources.RelativePathToCypher);

            StreamReader stream = new StreamReader(pathToCypher);
            string data = stream.ReadToEnd();

            IList<string> cypherStrings = JsonConvert.DeserializeObject<List<string>>(data);

            IList<Cypher> Cyphers = new List<Cypher>();
            int id = 0;

            foreach (var cypher in cypherStrings)
                Cyphers.Add(new Cypher { Id = id++, CypherText = cypher });

            return Cyphers;
        }

        public static IList<CypherScheme> CypherSchemes_SeedData()
        {
            var pathToCypherSchemes = Path.Combine(Directory.GetCurrentDirectory(), DBSources.RelativePathToCypherScheme);

            StreamReader stream = new StreamReader(pathToCypherSchemes);
            string data = stream.ReadToEnd();
            var jsonCypherSchemesArray = JArray.Parse(data);

            IList <CypherScheme> CypherSchemes = new List<CypherScheme>();

            int id = 0;
            
            foreach (var cypher in jsonCypherSchemesArray)
            {
                foreach (var rawScheme in cypher)
                {
                    var scheme = rawScheme.ToObject<RawCypherScheme>();

                    CypherSchemes.Add(
                        new CypherScheme {
                            CypherId = id,
                            ReplacementRuleId = scheme.Rule,
                            OrderId = scheme.Order,
                            IsTermination = scheme.IsTermination
                        });
                }
                id++;
            }

            return CypherSchemes;
        }

        public static IList<ReplacementRule> ReplacementRules_SeedData()
        {
            var pathToReplacementRules = Path.Combine(Directory.GetCurrentDirectory(), DBSources.RelativePathToReplacementRules);

            StreamReader stream = new StreamReader(pathToReplacementRules);
            string data = stream.ReadToEnd();
            //Console.WriteLine(data);

            IList<ReplacementRule> ReplacementRules = JsonConvert.DeserializeObject<List<ReplacementRule>>(data);

            int id = 0;

            foreach (var rule in ReplacementRules)
                rule.Id = id++;

            return ReplacementRules;
        }

        public static IList<PuzzleWord> PuzzleWords_SeedData()
        {
            var pathToPuzzleWords = Path.Combine(Directory.GetCurrentDirectory(), DBSources.RelativePathToPuzzleWords);

            StreamReader stream = new StreamReader(pathToPuzzleWords);
            string data = stream.ReadToEnd();

            IList<string> wordsStrings = JsonConvert.DeserializeObject<List<string>>(data);

            IList<PuzzleWord> PuzzleWords = new List<PuzzleWord>();
            int id = 0;

            foreach (var word in wordsStrings)
                PuzzleWords.Add(new PuzzleWord { Id = id++, Word = word });

            return PuzzleWords;
        }
    }
}
