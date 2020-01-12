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
            PeterHan.PLib.Codex.PCodex.RegisterCreatures();
            PeterHan.PLib.Codex.PCodex.RegisterPlants();
        }
    }
}
