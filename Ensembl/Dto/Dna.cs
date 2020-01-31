using System.Data;
using System.Linq;
using Dapper;

namespace Ensembl.Dto
{
    public class Dna
    {
        public int SeqRegionId { get; private set; }
        public string Sequence { get; private set; }

        public Dna(int id, string seq)
        {
            SeqRegionId = id;
            Sequence = seq;
        }

        public static Dna Get(int id, IDbConnection conn)
        {
            var sql = @"select sequence from dna where seq_region_id=@Id";

            return conn.Query<dynamic>(sql, new { Id = id })
                .Select(r => new Dna(id, r.sequence))
                .FirstOrDefault();
        }
    }
}