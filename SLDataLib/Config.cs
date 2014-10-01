using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SLDataLib
{
    public static class Config
    {
        private static string _connectionString = string.Empty;
        public static readonly string configFileName = "SCRConfig.xml";

        public static string dbConnectionString_SchoolLogic
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = LoadConnectionString();
                }

                return _connectionString;
            }
        }


        private static string LoadConnectionString()
        {
            string returnMe = string.Empty;

            if (!string.IsNullOrEmpty(configFileName))
            {
                if (ConfigFileExists())
                {
                    XElement main = XElement.Load(configFileName);
                    var linqResults = from item in main.Descendants("Database")
                        select new
                        {
                            connectionString = item.Element("ConnectionString").Value
                        };

                    foreach (var result in linqResults)
                    {
                        returnMe = result.connectionString;
                    }
                }
                else
                {
                    CreateNewConfigFile();
                }
            }
            return returnMe;
        }

        public static bool ConfigFileExists()
        {
            if (File.Exists(configFileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CreateNewConfigFile()
        {
            string[] configFileLines =
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<Settings>",
                "  <Database>",
                "    <ConnectionString>data source=HOSTNAME;initial catalog=DATABASE;user id=USERNAME;password=PASSWORD;Trusted_Connection=false</ConnectionString>",
                "  </Database>",
                "</Settings>"
            };

            System.IO.File.WriteAllLines(configFileName, configFileLines);
        }

    }
}
