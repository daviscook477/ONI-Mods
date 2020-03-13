using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;

namespace Loggers
{
    public class LoggersPatches
    {

        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch("Load")]
        [HarmonyPatch(new[] { typeof(string) })]
        public static class SaveLoader_Load_Patch
        {
            public static void Postfix(string filename)
            {
                Console.WriteLine("Consumed load event");
                //TeleStorageData.Load(filename);
            }
        }

        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch("Save")]
        [HarmonyPatch(new[] { typeof(string), typeof(bool), typeof(bool) })]
        public static class SaveLoader_Save_Patch
        {
            public static void Postfix(string filename)
            {
                Console.WriteLine("Consumed save event");
                //TeleStorageData.Save(filename);
            }
        }

    }
}
