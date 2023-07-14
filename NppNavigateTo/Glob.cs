using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NavigateTo.Plugin.Namespace
{
    public enum Ternary
    {
        FALSE,
        TRUE,
        UNDECIDED,
    }

    public class GlobFunction
    {
        public Func<string, bool> Matcher { get; }
        public bool Or { get; }
        public bool Negated { get; }

        public GlobFunction(Func<string, bool> matcher, bool or, bool negated)
        {
            this.Matcher = matcher;
            this.Or = or;
            this.Negated = negated;
        }

        public Ternary IsMatch(string inp, Ternary previousResult)
        {
            if (Or && (previousResult == Ternary.TRUE))
                return Ternary.TRUE;
            if (!Or && (previousResult == Ternary.FALSE))
                return Ternary.FALSE;
            bool result = Matcher(inp);
            return (result ^ Negated) ? Ternary.TRUE : Ternary.FALSE;
        }
    }

    /// <summary>
    /// parser that parses glob syntax in space-separated globs<br></br>
    /// 1. Default behavior is to match ALL space-separated globs
    ///     (e.g. "foo bar txt" matches "foobar.txt" and "bartxt.foo")<br></br>
    /// 2. Also use glob syntax:<br></br>
    ///     * "*" matches any number (incl. zero) of characters except "\\"<br></br>
    ///     * "**" matches any number (incl. zero) of characters including "\\"<br></br>
    ///     * "[chars]" matches all characters inside the square brackets (same as in Perl-style regex, also includes character classes like [a-z], [0-9]). Note: can match ' ', which is normally a glob separator<br></br>
    ///     * "[!chars]" matches NONE of the characters inside the square brackets (and also doesn't match "\\") (same as Perl-style "[^chars]")<br></br>
    ///     * "foo.{abc,def,hij}" matches "foo.abc", "foo.def", or "foo.hij". Essentially "{x,y,z}" is equivalent to "(?:x|y|z)" in Perl-style regex<br></br>
    ///     * "?" matches any one character except "\\"<br></br>
    /// 3. "foo | bar" matches foo OR bar ("|" implements logical OR)<br></br>
    /// 4. "!foo" matches anything that DOES NOT CONTAIN "foo"<br></br>
    /// 5. "foo | &lt;baz bar&gt;" matches foo OR (bar AND baz) (that is, "&lt;" and "&gt;" act as grouping parentheses)
    /// </summary>
    public class Glob
    {
        public int ii;
        public string ErrorMsg;
        public int ErrorPos;
        public List<string> globs;

        public Glob()
        {
            globs = new List<string>();
            Reset();
        }

        public void Reset()
        {
            globs.Clear();
            ErrorMsg = null;
            ErrorPos = -1;
            ii = 0;
        }

        public char Peek(string inp)
        {
            return (ii < inp.Length - 1)
                ? inp[ii + 1]
                : '\x00';
        }

        public Regex Glob2Regex(string inp)
        {
            var sb = new StringBuilder();
            int start = ii;
            bool is_char_class = false;
            bool uses_metacharacters = false;
            bool is_alternation = false;
            while (ii < inp.Length)
            {
                char c = inp[ii];
                char next_c;
                switch (c)
                {
                case '/': sb.Append("\\\\"); break;
                case '\\':
                    next_c = Peek(inp);
                    if (next_c == 'x' || next_c == 'u' // \xNN, \uNNNN unicode escapes
                        || (next_c == ']' && is_char_class))
                        sb.Append('\\'); 
                    else
                        sb.Append("\\\\");
                    break;
                case '*':
                    uses_metacharacters = true;
                    if (is_char_class)
                    {
                        sb.Append("\\*"); // "[*]" matches literal * character
                        break;
                    }
                    else if (sb.Length == 0)
                        break; // since globs are only anchored at the end,
                               // leading * in globs should not influence the matching behavior.
                               // For example, the globs "*foo.txt" and "foo.txt" should match the same things.
                    next_c = Peek(inp);
                    if (next_c == '*')
                    {
                        ii++;
                        // "**" means recursive search; ignores path delimiters
                        sb.Append(".*");
                    }
                    else
                    {
                        // "*" means anything but a path delimiter
                        sb.Append(@"[^\\]*");
                    }
                    break;
                case '[':
                    uses_metacharacters = true;
                    sb.Append('[');
                    next_c = Peek(inp);
                    if (!is_char_class && next_c == '!')
                    {
                        sb.Append("^\\\\"); // [! begins a negated char class, but also need to exclude path sep
                        ii++;
                    }
                    is_char_class = true;
                    break;
                case ']':
                    is_char_class = false;
                    sb.Append(']');
                    break; // TODO: consider allowing nested [] inside char class
                case '{':
                    if (is_char_class)
                        sb.Append("\\{");
                    else
                    {
                        uses_metacharacters = true;
                        is_alternation = true; // e.g. *.{cpp,h,c} matches *.cpp or *.h or *.c
                        sb.Append("(?:");
                    }
                    break;
                case '}':
                    if (is_char_class)
                        sb.Append("\\}");
                    else
                    {
                        sb.Append(")");
                        is_alternation = false;
                    }
                    break;
                case ',':
                    if (is_alternation)
                        sb.Append('|'); // e.g. *.{cpp,h,c} matches *.cpp or *.h or *.c
                    else
                        sb.Append(',');
                    break;
                case '.': case '$': case '(': case ')': case '^':
                    // these chars have no special meaning in glob syntax, but they're regex metacharacters
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '?':
                    if (is_char_class)
                        sb.Append('?');
                    else
                    {
                        uses_metacharacters = true;
                        sb.Append(@"[^\\]"); // '?' is any single char
                    }
                    break;
                case '|': case '<': case '>': case ';': case '\t':
                    goto endOfLoop; // these characters are never allowed in Windows paths
                case ' ': case '!':
                    if (is_char_class)
                        sb.Append(c);
                    else
                        goto endOfLoop; // allow ' ' and '!' inside char classes, but otherwise these are special chars in NavigateTo
                    break;
                default:
                    sb.Append(c);
                    break;
                }
                ii++;
            }
            endOfLoop:
            if (uses_metacharacters) // anything without any chars in "*?[]{}" will just be treated as a normal string
                sb.Append('$'); // globs are anchored at the end; that is "*foo" does not match "foo/bar.txt" but "*foo.tx?" does
            string pat = sb.ToString();
            try
            {
                var regex = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                globs.Add(pat);
                return regex;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ErrorPos = start;
                return null;
            }
        }

        public Func<string, bool> Parse(string inp, bool fromBeginning = true)
        {
            if (fromBeginning)
                Reset();
            bool or = false;
            bool negated = false;
            var globFuncs = new List<GlobFunction>();
            while (ii < inp.Length)
            {
                char c = inp[ii];
                if (c == ' ' || c == '\t' || c == ';') { }
                else if (c == '|')
                    or = true;
                else if (c == '!')
                    negated = !negated;
                else if (c == '<')
                {
                    ii++;
                    var subFunc = Parse(inp, false);
                    globFuncs.Add(new GlobFunction(subFunc, or, negated));
                    negated = false;
                    or = false;
                }
                else if (c == '>')
                    break;
                else
                {
                    var globRegex = Glob2Regex(inp);
                    if (globRegex == null)
                        continue; // ignore errors, try to parse everything else
                    globFuncs.Add(new GlobFunction(globRegex.IsMatch, or, negated));
                    ii--;
                    negated = false;
                    or = false;
                }
                ii++;
            }
            ii++;
            if (globFuncs.All(gf => !gf.Or))
                // return a more efficient function that short-circuits
                // if none of the GlobFunctions use logical or
                return (string x) => globFuncs.All(gf => gf.Matcher(x) ^ gf.Negated);
            bool finalFunc(string x)
            {
                Ternary result = Ternary.UNDECIDED;
                foreach (GlobFunction globFunc in globFuncs)
                {
                    result = globFunc.IsMatch(x, result);
                }
                return result != Ternary.FALSE;
            }
            return finalFunc;
        }
    }
}
