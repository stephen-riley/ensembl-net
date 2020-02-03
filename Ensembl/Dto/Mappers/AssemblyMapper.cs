using Dapper.FluentMap.Mapping;

namespace Ensembl.Dto.Mappers
{
    internal class AssemblyMapper : EntityMap<Assembly>
    {
        internal AssemblyMapper()
        {
            Map(u => u.AssemblySeqRegionId).ToColumn("asm_seq_region_id");
            Map(u => u.AssemblyStart).ToColumn("asm_start");
            Map(u => u.AssemblyEnd).ToColumn("asm_end");
            Map(u => u.ComponentSeqRegionId).ToColumn("cmp_seq_region_id");
            Map(u => u.ComponentStart).ToColumn("cmp_start");
            Map(u => u.ComponentEnd).ToColumn("cmp_end");
            Map(u => u.Orientation).ToColumn("ori");
        }
    }
}