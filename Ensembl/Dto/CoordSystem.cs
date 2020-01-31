using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Ensembl.Dto
{
    public class CoordSystem : IEquatable<CoordSystem>
    {
        public int Id { get; set; }

        public int SpeciesId { get; set; }

        public string Name { get; set; }

        public string? Version { get; set; }

        public int Rank { get; set; }

        public ISet<string> Attributes { get; private set; }

        public CoordSystem(int id, int speciesId, string name, string? version, int rank, string attributes)
        {
            Id = id;
            SpeciesId = speciesId;
            Name = name;
            Version = version;
            Rank = rank;
            Attributes = new HashSet<string>(attributes.Split(","));
        }

        public override string ToString()
        {
            return $"{Id}:{SpeciesId}:{Name}:{(String.IsNullOrEmpty(Version) ? "<null>" : Version)}:{Rank}:{string.Join(",", Attributes)}";
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            if (!(other is CoordSystem))
            {
                return false;
            }
            return Equals((CoordSystem)other);
        }

        public bool Equals(CoordSystem other)
        {
            return other is CoordSystem
                ? ToString() == other.ToString()
                : false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(CoordSystem lhs, CoordSystem rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CoordSystem lhs, CoordSystem rhs)
        {
            return !lhs.Equals(rhs);
        }

        internal static IEnumerable<CoordSystem> GetAll(IDbConnection conn)
        {
            var sql = @"select * from coord_system";

            return conn.Query<dynamic>(sql)
                .Select(r =>
                {
                    var dict = (IDictionary<string, object?>)r;
                    //Console.WriteLine($"r.coord_system_id:{r.coord_system_id.GetType()} ,r.species_id:{r.species_id.GetType()} ,r.name:{r.name.GetType()} ,r.version:{r.version.GetType()} ,r.rank:{r.rank.GetType()} ,r.attrib:{r.attrib.GetType()} ,");
                    return new CoordSystem
                    (
                        (int)r.coord_system_id,
                        (int)r.species_id,
                        (string)r.name,
                        (string?)r.version,
                        (int)r.rank,
                        (string)r.attrib
                    );
                });
        }
    }
}