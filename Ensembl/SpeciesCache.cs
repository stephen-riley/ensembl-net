using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Dapper;
using Ensembl.Config;
using Ensembl.Dto;
using MySql.Data.MySqlClient;

[assembly: InternalsVisibleTo("Ensembl.Tests")]

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

        internal IEnumerable<string> GetChromosomeList()
        {
            var sql = @"
                select name 
                from seq_region 
                where coord_system_id=@ChrCsId
                and length(name) <= 3
                order by name";

            var chrCsId = SpeciesCache.ByName(SpeciesDbName).CoordSystems.Values.Where(cs => cs.Name == "chromosome" && cs.Attributes.Contains("default_version")).First().Id;

            using var conn = new MySqlConnection(ConnectionString);
            var list = conn.Query<string>(sql, new { ChrCsId = chrCsId });
            return list;
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

            using var conn = new MySqlConnection(EnsemblConfig.ShortConnectionString);
            var dbs = conn.Query<string>(sql);

            // WARNING: currently only supports "core" species databases
            var regex = new Regex(@"core_\d+_\d+$");
            var valid = dbs.Where(name => regex.IsMatch(name));
            return valid;
        }

        public static string? GetDbNameForCommonName(string name)
        {
            var sql = @"
                use ensembl_metadata_99;
                select gdb.dbname
                from genome_database gdb, organism o, genome g
                where o.organism_id = g.organism_id
                and gdb.type='core'
                and g.genome_id = gdb.genome_id
                and ( o.display_name=@Name or o.name=@Name or o.scientific_name=@Name or o.url_name=@Name )
                order by gdb.dbname desc
                limit 1;";

            try
            {
                // We try this first because if the name is already a db name, the above query is *very*
                // expensive just to return null.
                var isDbName = TryCommonNameAsSpeciesDbName(name);
                if (isDbName)
                {
                    return name;
                }

                using var conn = new MySqlConnection(EnsemblConfig.ShortConnectionString);
                var dbname = conn.Query<string>(sql, new { Name = name }).FirstOrDefault();
                return dbname;
            }
            catch (MySqlException e)
            {
                if (!e.Message.StartsWith("Unknown database"))
                {
                    throw e;
                }
                return null;
            }
        }

        internal static bool TryCommonNameAsSpeciesDbName(string name)
        {
            try
            {
                using var conn = new MySqlConnection(EnsemblConfig.ShortConnectionString);
                conn.Query<int>($"use {name}");
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }

        }
    }
}