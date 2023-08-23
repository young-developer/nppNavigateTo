/*
A test runner for all of this package.
*/
using System.Threading.Tasks;
using NppPluginNET;

namespace NavigateTo.Tests
{
    public class TestRunner
    {
        public static void RunAll()
        {
            MiscUtils.notepad.FileNew();
            MiscUtils.AddLine($"Test results for NavigateTo v{MiscUtils.AssemblyVersionString()} on Notepad++ {MiscUtils.nppVersionStr}");

            MiscUtils.AddLine(@"=========================
Testing Glob syntax
=========================
");
            GlobTester.Test();

            MiscUtils.AddLine(@"=========================
Testing Glob syntax with cached top directories
=========================
");
            GlobTester.TestCachedTopDirectory();
        }
    }
}
