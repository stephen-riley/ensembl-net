using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Ensembl.Dto
{
    public class Chromosome
    {
        public IEnumerable<Assembly> Assemblies { get; private set; }

        public string Id { get; private set; }

        public int Length => Assemblies.Last().AssemblyEnd;

        private string SpeciesDbName;

        public Chromosome(string speciesDbName, string id, IEnumerable<Assembly> assemblies)
        {
            Id = id;
            Assemblies = assemblies;
            SpeciesDbName = speciesDbName;
        }

        internal static Chromosome Get(string speciesDbName, string name, IDbConnection conn)
        {
            var asmSql = @"
                select a.*
                from assembly a, seq_region r
                where a.cmp_seq_region_id=r.seq_region_id
                and asm_seq_region_id=
                    ( select seq_region_id from seq_region r where r.name=@Name and r.coord_system_id=@ChrCsId )
                and coord_system_id=@SeqCsId
                order by asm_start
            ";

            var seqCsId = SpeciesCache.ByName(speciesDbName).TopLevelCoordSystem.Id;
            var chrCsId = SpeciesCache.ByName(speciesDbName).CoordSystems.Values.Where(cs => cs.Name == "chromosome" && cs.Attributes.Contains("default_version")).First().Id;

            var dbAssemblies = conn.Query<dynamic>(asmSql, new { SeqCsId = seqCsId, ChrCsId = chrCsId, Name = name })
                .Select(r => new Assembly(
                    (int)r.asm_seq_region_id,
                    (int)r.asm_start,
                    (int)r.asm_end,
                    (int)r.cmp_seq_region_id,
                    (int)r.cmp_start,
                    (int)r.cmp_end,
                    (int)r.ori
                ));

            var assemblies = new List<Assembly>();
            var start = 1;

            foreach (var asm in dbAssemblies)
            {
                if (asm.AssemblyStart != start)
                {
                    assemblies.Add(new NAssembly(start, asm.AssemblyStart - 1, 0, asm.AssemblyStart - start));
                }
                assemblies.Add(asm);
                start = asm.AssemblyEnd + 1;
            }

            return new Chromosome(speciesDbName, name, assemblies);
        }
    }
}