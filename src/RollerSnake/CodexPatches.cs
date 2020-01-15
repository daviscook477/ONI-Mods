using System;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Klei;

namespace Codex
{
    public class CodexPatches
    {
        public static void OnLoad()
        {
            PeterHan.PLib.PUtil.InitLibrary();
            PeterHan.PLib.Datafiles.PCodex.RegisterCreatures();
            PeterHan.PLib.Datafiles.PCodex.RegisterPlants();
        }
    }
}
