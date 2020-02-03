using System;
using System.Collections.Generic;
using System.Linq;
using Ensembl.Dto;
using MySql.Data.MySqlClient;

namespace Ensembl
{
    internal static class Cache
    {
        public static IDictionary<int, CoordSystem> CoordSystems { get; private set; }

        public static CoordSystem TopLevelCoordSystem => CoordSystems.Values.Where(cs => cs.Attributes.Contains("sequence_level")).First();

        private static IDictionary<int, Dna> sequences;

        private static IDictionary<string, Chromosome> chromosomes;

        static Cache()
        {
            using var conn = new MySqlConnection(EnsemblConfig.ConnectionString);

            CoordSystems = CoordSystem.GetAll(conn).ToDictionary(cs => cs.Id);

            sequences = new Dictionary<int, Dna>();

            chromosomes = new Dictionary<string, Chromosome>();
        }

        public static Chromosome GetChromosome(string name)
        {
            if (!chromosomes.ContainsKey(name))
            {
                using var conn = new MySqlConnection(EnsemblConfig.ConnectionString);
                chromosomes[name] = Chromosome.Get(name, conn);
            }

            return chromosomes[name];
        }

        public static ReadOnlyMemory<char> GetDnaSequence(int id, int start = 1, int end = Int32.MaxValue)
        {
            if (!sequences.ContainsKey(id))
            {
                using var conn = new MySqlConnection(EnsemblConfig.ConnectionString);
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
    }
}