using IniFileParser.Model;

namespace Ensembl.Config
{
    public class DbConfig
    {
        public string Host { get; set; }

        public string User { get; set; }

        public DbConfig(IniData iniData)
        {
            Host = iniData["Database"]["server"];
            User = iniData["Database"]["user"];
        }
    }
}