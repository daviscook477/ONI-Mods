using System;
using System.IO;
using Newtonsoft.Json;

namespace Loggers
{
    public static class FileManager
    {
        public static T LoadFile<T>(string executingAssemblyPath, string FileName = "File.json")
        {
            var directory = Path.GetDirectoryName(executingAssemblyPath);

            if (directory == null)
            {
                DebugUtil.LogErrorArgs((object)string.Format("Could not read save data from provided executing assembly path: {0}. Using default values instead.", executingAssemblyPath));
                return default;
            }

            var filePath = Path.Combine(directory, FileName);
            Console.WriteLine("Attempt load from " + filePath);

            T File;
            try
            {
                using (var r = new StreamReader(filePath))
                {
                    var json = r.ReadToEnd();
                    File = JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception)
            {
                DebugUtil.LogArgs((object)string.Format("Could not read save data from File file: {0}. Using default values instead.", filePath));
                return default;
            }

            return File;
        }

        public static void SaveFile<T>(string executingAssemblyPath, T data, string FileName = "File.json")
        {
            var directory = Path.GetDirectoryName(executingAssemblyPath);

            if (directory == null)
            {
                return;
            }

            var filePath = Path.Combine(directory, FileName);
            Console.WriteLine("Attempt save to " + filePath);
            try
            {
                using (var w = new StreamWriter(filePath))
                {
                    w.Write(JsonConvert.SerializeObject(data));
                }
            }
            catch (Exception)
            {
                DebugUtil.LogErrorArgs((object)string.Format("Could not save data to File file: {0}", filePath));
            }
        }
    }
}
