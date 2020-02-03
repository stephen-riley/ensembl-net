using Dapper.FluentMap.Mapping;

namespace Ensembl.Dto.Mappers
{
    internal class CoordSystemMapper : EntityMap<CoordSystem>
    {
        internal CoordSystemMapper()
        {
            Map(u => u.Id).ToColumn("coord_system_id");
            Map(u => u.SpeciesId).ToColumn("species_id");
            Map(u => u.Name).ToColumn("name");
            Map(u => u.Version).ToColumn("version");
            Map(u => u.Rank).ToColumn("rank");
            Map(u => u.AttributesString).ToColumn("attrib");
        }
    }
}