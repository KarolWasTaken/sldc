using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Newtonsoft.Json;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc
{
    public static class Helper
    {
        public static Settings settings = new Settings();
       
        public static IConfigurationRoot Config
        {
            get
            {
                // Load the existing JSON file when needed. Ensures up-to-date JSON is always used.
                return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            }
        }
        public static void UpdateSettings()
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText("appsettings.json", json);
            Config.Reload();
        }
        public static Settings ReturnSettings()
        {
            // read json file
            string json = File.ReadAllText("appsettings.json");
            // Deserialize JSON into Settings object
            Settings settings = JsonConvert.DeserializeObject<Settings>(json);
            Helper.settings = settings;
            return settings;
        }
    }

}

