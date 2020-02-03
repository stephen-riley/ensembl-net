using System;
using Dapper.FluentMap;
using Ensembl.Dto.Mappers;

namespace Ensembl
{
    public static class EnsemblInitializer
    {
        private static bool initialized = false;

        public static void Init()
        {
            if (!initialized)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new AssemblyMapper());
                    config.AddMap(new CoordSystemMapper());
                    config.AddMap(new DnaMapper());
                });

                initialized = true;
            }
        }
    }
}