using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Champagne
{
    public class CodexPatches
    {
        public static void OnLoad()
        {
            PeterHan.PLib.PUtil.InitLibrary();
            PeterHan.PLib.Datafiles.PCodex.RegisterPlants();
        }
    }
}
