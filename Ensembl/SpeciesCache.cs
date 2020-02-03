using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Ensembl.Config;
using Ensembl.Dto;
using MySql.Data.MySqlClient;

namespace Ensembl
{
    internal class SpeciesCache
    {
        private static IDictionary<string, SpeciesCache> Caches { get; set; } = new Dictionary<string, SpeciesCache>();

        public string SpeciesDbName { get; private set; }

        public IDictionary<int, CoordSystem> CoordSystems { get; private set; }

        public CoordSystem TopLevelCoordSystem => CoordSystems.Values.Where(cs => cs.Attributes.Contains("sequence_level")).First();

        private IDictionary<int, Dna> sequences;

        private IDictionary<string, Chromosome> chromosomes;

        static SpeciesCache()
        {
        }

        public string ConnectionString => string.Format(EnsemblConfig.ConnectionString, SpeciesDbName);

        public SpeciesCache(string speciesDbName)
        {
            SpeciesDbName = speciesDbName;
            sequences = new Dictionary<int, Dna>();
            chromosomes = new Dictionary<string, Chromosome>();

            using var conn = new MySqlConnection(ConnectionString);
            CoordSystems = CoordSystem.GetAll(conn).ToDictionary(cs => cs.Id);
        }

        public static SpeciesCache ByName(string speciesDbName)
        {
            if (!Caches.ContainsKey(speciesDbName))
            {
                Caches[speciesDbName] = new SpeciesCache(speciesDbName);
            }
            return Caches[speciesDbName];
        }

        public Chromosome GetChromosome(string name)
        {
            if (!chromosomes.ContainsKey(name))
            {
                using var conn = new MySqlConnection(ConnectionString);
                chromosomes[name] = Chromosome.Get(SpeciesDbName, name, conn);
            }

            return chromosomes[name];
        }

        public ReadOnlyMemory<char> GetDnaSequence(int id, int start = 1, int end = Int32.MaxValue)
        {
            if (!sequences.ContainsKey(id))
            {
                using var conn = new MySqlConnection(ConnectionString);
                sequences[id] = Dna.Get(id, conn);
            }

            if (start == 1 && end == Int32.MaxValue)
            {
                return sequences[id].Sequence.AsMemory();
            }
            else
            {
                if (end == Int32.MaxValue)
                {
                    end = sequences[id].Sequence.Length + 1;
                }

                return sequences[id].Sequence.AsMemory(start - 1, end - start + 1);
            }
        }

        public static IEnumerable<string> GetInstalledEnsemblDatabases()
        {
            var sql = "show databases";

            using var conn = new MySqlConnection(EnsemblConfig.ConnectionString);
            var dbs = conn.Query<string>(sql);

            var regex = new Regex(@"_\d+_\d+$");
            var valid = dbs.Where(name => regex.IsMatch(name));
            return valid;
        }
    }
}