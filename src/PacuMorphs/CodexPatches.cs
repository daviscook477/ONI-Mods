using PeterHan.PLib;
using PeterHan.PLib.Datafiles;

namespace Codex
{
    public class CodexPatches
    {
        public static void OnLoad()
        {
            PUtil.InitLibrary();
            PCodex.RegisterCreatures();
        }
    }
}
