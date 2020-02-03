using Dapper.FluentMap.Mapping;

namespace Ensembl.Dto.Mappers
{
    internal class DnaMapper : EntityMap<Dna>
    {
        internal DnaMapper()
        {
            Map(u => u.SeqRegionId).ToColumn("seq_region_id");
            Map(u => u.Sequence).ToColumn("sequence");
        }
    }
}