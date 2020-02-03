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
        public string AttributesString { get; set; }

        public ISet<string> Attributes => new HashSet<string>(AttributesString.Split(","));

        // for Dapper
        internal CoordSystem()
        {
            Name = string.Empty;
            AttributesString = string.Empty;
        }

        public CoordSystem(int id, int speciesId, string name, string? version, int rank, string attributes)
        {
            Id = id;
            SpeciesId = speciesId;
            Name = name;
            Version = version;
            Rank = rank;
            AttributesString = attributes;
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

            var res = conn.Query<CoordSystem>(sql);
            return res;
        }
    }
}