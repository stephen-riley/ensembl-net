using System.Data;
using System.Linq;
using Dapper;

namespace Ensembl.Dto
{
    public class Dna
    {
        public int SeqRegionId { get; private set; }
        public string Sequence { get; private set; }

        // for Dapper
        internal Dna()
        {
            Sequence = string.Empty;
        }

        public Dna(int id, string seq)
        {
            SeqRegionId = id;
            Sequence = seq;
        }

        public static Dna Get(int id, IDbConnection conn)
        {
            var sql = @"select * from dna where seq_region_id=@Id";

            return conn.Query<Dna>(sql, new { Id = id }).FirstOrDefault();
        }
    }
}