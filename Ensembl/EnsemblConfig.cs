using System;
using System.IO;
using IniFileParser;
using IniFileParser.Model;
using static System.Environment;

namespace Ensembl
{
    public class EnsemblConfig
    {
        private static EnsemblConfig? singletonInstance;

        private static EnsemblConfig GetInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new EnsemblConfig();
            }
            return singletonInstance;
        }

        private string ConnStr = "Server=useastdb.ensembl.org;User ID=anonymous;Database=homo_sapiens_core_99_38";

        public static string ConnectionString => GetInstance().ConnStr;

        public EnsemblConfig()
        {
            var etcData = ReadConfigFile("/etc/ensembl.conf");
            var homeData = ReadConfigFile($"{GetFolderPath(SpecialFolder.UserProfile)}/.ensembl.conf");

            if (etcData != null && homeData != null)
            {
                etcData.Merge(homeData);
                ConnStr = etcData["Database"]["connection_string"];
            }
            else if (etcData != null)
            {
                ConnStr = etcData["Database"]["connection_string"];
            }
            else if (homeData != null)
            {
                ConnStr = homeData["Database"]["connection_string"];
            }

            EnsemblInitializer.Init();
        }

        private static IniData? ReadConfigFile(string path)
        {
            if (File.Exists(path))
            {
                var parser = new IniStringParser();
                var data = parser.Parse(File.ReadAllText(path));
                return data;
            }
            else
            {
                return null;
            }
        }

    }
}