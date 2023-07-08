using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NavigateTo.Plugin.Namespace;
using NppPluginNET;

namespace NavigateTo.Tests
{
    public class GlobTester
    {
        public static void Test()
        {
            int ii = 0;
            int failed = 0;
            var testcases = new (string query, (string input, bool desiredResult)[])[]
            {
                ("foo bar", new[]
                {
                    ("foobar.txt", true),
                    ("bar.foo", true),
                    ("bar.txt", false),
                    ("foo.txt", false),
                }),
                ("foo bar txt", new[]
                {
                    ("foobar.txt", true),
                    ("bar.foo", false),
                    ("bar.txt", false),
                    ("foo.txt", false),
                }),
                ("foo;bar;*.txt", new[]
                {
                    ("foobar.txt", true),
                    ("bar.foo", false),
                    ("bar.txt", false),
                    ("foo.txt", false),
                }),
                ("foo | bar", new[]
                {
                    ("foobar.txt", true),
                    ("bar.txt", true),
                    ("foo.txt", true),
                    ("ghi.txt", false),
                }),
                ("!bar", new[]
                {
                    ("foo.txt", true),
                    ("bar.txt", false),
                }),
                ("foo !bar", new[]
                {
                    ("foobar.txt", false),
                    ("foo.txt", true),
                    ("ghi.txt", false),
                }),
                ("foo | < !bar baz", new[]
                {
                    ("foo.bar", true),
                    ("bar.baz", false),
                    ("baz.txt", true),
                    ("foo.baz", true),
                }),
                ("foo**[!c-p]u\\u0434ck*.{tx?,cpp} !fgh | <ba\\x72 baz", new[]
                {
                    ("foo\\boo\\quдcked.txt", true),
                    ("foo\\boo\\quдcked.cpp", true),
                    ("foozuдck.txb", true),
                    ("bar.baz", true),
                    ("baz.bar", true),
                    ("fgh\\bar.baz", true),
                    ("foo\\boo\\duдcked.txt", false),
                    ("foo\\boo\\uдcked.txt", false),
                    ("foozuдck.xml", false),
                    ("foo.baz", false),
                    ("foo\\boo\\quдcked.txto", false),
                    ("foo\\fgh\\quдcked.txt", false),
                    ("foo\\fgh\\quдcked.cpp", false),
                    ("foo\\boo\\quдcked\\bad.txt", false),
                }),
                ("foo[ !]*.*", new[]
                {
                    ("foo!man.txt", true),
                    ("foo man.txt", true),
                    ("footman.txt", false),
                    ("C:\\reoreno\\84303\\foo .eorn", true),
                    ("foo!.zzy", true),
                }),
                ("foo[! ].*", new[]
                {
                    ("foot.txt", true),
                    ("foo .txt", false),
                    ("boot.txt", false),
                    ("foo\\.txt", false),
                }),
                ("**!bar", new[]
                {
                    ("", true),
                    ("reore", true),
                    ("$#@.~~~", true),
                    ("bar.txt", false),
                }),
                ("foo|bar!baz", new[]
                {
                    ("bar.txt", true),
                    ("foo.txt", true),
                    ("foo.baz", false),
                    ("bar.baz", false),
                }),
                ("^.()[[ ]]$.*", new[]
                {
                    ("^.()[]$.xml", true),
                    ("^.() ]$.xml", true),
                    ("^.()]]$.xml", false),
                    ("foo.xml", false),
                }),
                ("*[*].{h,c[px][px]}", new[]
                {
                    ("foo*.h", true),
                    ("bar*.cpp", true),
                    ("baz*.cpx", true),
                    ("foo\\qux*.cxp", true),
                    ("roeu\\843*.cxx", true),
                    ("roeu\\843*.c\\x", false),
                    ("foo\\qux.cxp", false),
                    ("foo.h", false),
                    ("roeu\\843*.cxxe", false),
                    ("roeu\\843*.xxx", false),
                    ("843.cxx", false),
                }),
                ("{foo,bar}.{txt,md}", new[]
                {
                    ("foo.txt", true),
                    ("foo.md", true),
                    ("bar.txt", true),
                    ("bar.md", true),
                    ("bar.baz", false),
                }),
                ("foo{ baz bar[", new[] // test ignoring of globs with syntactically invalid globs
                {
                    ("baz.txt", true),
                    ("foo.txt", false),
                }),
                ("baz *.foo{1,2} bar{", new[]
                {
                    ("baz.foo1", true),
                    ("c:\\bazel.foo2", true),
                    ("baz\\fjie.foo1", true),
                    ("baz\\fjie\\foo3", false),
                    ("baz\\fjie\\foo1", false),
                    ("bazfjie.foo3", false),
                    ("baz\\fjie\\foo12", false),
                }),
                ("\\x20\\x21a\\u0434.txt", new[] // " !aд", check support for \xNN, \uNNNN
                {
                    (" !aд.txt", true),
                    ("foo\\ !aд.txt", true),
                    ("!aд.txt", false),
                    ("aд.txt", false),
                    ("д.md", false),
                }),
            };
            var glob = new Glob();
            foreach ((string query, var examples) in testcases)
            {
                Func<string, bool> resultFunc;
                try
                {
                    resultFunc = glob.Parse(query);
                }
                catch (Exception ex)
                {
                    ii++;
                    MiscUtils.AddLine($"While parsing query \"{query}\", got exception\r\n{ex}");
                    failed++;
                    continue;
                }
                foreach ((string filename, bool desiredResult) in examples)
                {
                    ii++;
                    bool result;
                    try
                    {
                        result = resultFunc(filename);
                    }
                    catch (Exception ex)
                    {
                        MiscUtils.AddLine($"While executing query \"{query}\" on filename \"{filename}\", got exception\r\n{ex}");
                        failed++;
                        continue;
                    }
                    if (result != desiredResult)
                    {
                        MiscUtils.AddLine($"Running query \"{query}\" on filename \"{filename}\", EXPECTED {desiredResult}, GOT {result}");
                        failed++;
                        continue;
                    }
                }
            }
            MiscUtils.AddLine($"Ran {ii} tests and failed {failed}");
        }
    }
}
