using System;
using System.IO;
using IniFileParser;
using IniFileParser.Model;
using static System.Environment;

namespace Ensembl.Config
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

        public static string ConnectionString { get; private set; }

        public static string ShortConnectionString { get; private set; }

        static EnsemblConfig()
        {
            var etcData = ReadConfigFile("/etc/ensembl.conf");
            var homeData = ReadConfigFile($"{GetFolderPath(SpecialFolder.UserProfile)}/.ensembl.conf");

            var config = default(IniData);

            if (etcData != null && homeData != null)
            {
                etcData.Merge(homeData);
                config = etcData;
            }
            else if (etcData != null)
            {
                config = etcData;
            }
            else if (homeData != null)
            {
                config = homeData;
            }
            else
            {
                throw new Exception("No ensembl configuration present");
            }

            var dbConfig = new DbConfig(config);
            ShortConnectionString = string.Format("Server={0};User ID={1}", dbConfig.Host, dbConfig.User);
            ConnectionString = $"{ShortConnectionString};Database={{0}}";

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