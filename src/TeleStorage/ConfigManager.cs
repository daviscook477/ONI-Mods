using System;
using System.IO;
using Newtonsoft.Json;

namespace TeleStorage
{
    public static class ConfigManager
    {
        public static T LoadConfig<T>(string executingAssemblyPath, string configFileName = "Config.json")
        {
            var directory = Path.GetDirectoryName(executingAssemblyPath);

            if (directory == null)
            {
                DebugUtil.LogErrorArgs((object)string.Format("Could not read save data from provided executing assembly path: {0}. Using default values instead.", executingAssemblyPath));
                return default;
            }

            var configPath = Path.Combine(directory, configFileName);
            Console.WriteLine("Attempt load from " + configPath);

            T config;
            try
            {
                using (var r = new StreamReader(configPath))
                {
                    var json = r.ReadToEnd();
                    config = JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception)
            {
                DebugUtil.LogArgs((object)string.Format("Could not read save data from config file: {0}. Using default values instead.", configPath));
                return default;
            }

            return config;
        }

        public static void SaveConfig<T>(string executingAssemblyPath, T data, string configFileName = "Config.json")
        {
            var directory = Path.GetDirectoryName(executingAssemblyPath);

            if (directory == null)
            {
                return;
            }

            var configPath = Path.Combine(directory, configFileName);
            Console.WriteLine("Attempt save to " + configPath);
            try
            {
                using (var w = new StreamWriter(configPath))
                {
                    w.Write(JsonConvert.SerializeObject(data));
                }
            }
            catch (Exception)
            {
                DebugUtil.LogErrorArgs((object)string.Format("Could not save data to config file: {0}", configPath));
            }
        }
    }
}
