using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NppPluginNET
{
    public static class SearchUtils
    {
        public static List<FileModel> FuzzySearchFilePath(
            string filter,
            List<FileModel> fileList,
            int tolerance)
        {
            List<FileModel> foundFiles =
            (
                from s in fileList
                let lcs = s.FilePath.ToLower().LongestCommonSubsequence(filter.ToLower()).Length
                where lcs >= filter.Length - tolerance
                orderby lcs
                select s
            ).ToList();

            return foundFiles;
        }

        public static List<FileModel> FuzzySearchFileName(
            string filter,
            List<FileModel> fileList,
            int tolerance)
        {
            List<FileModel> foundFiles =
            (
                from s in fileList
                let lcs = s.FileName.ToLower().LongestCommonSubsequence(filter.ToLower()).Length
                where lcs >= filter.Length - tolerance
                orderby lcs
                select s
            ).ToList();

            return foundFiles;
        }

        public static List<FileModel> FuzzySearch(string filter, List<FileModel> fileList, int tolerance)
        {
            List<FileModel> foundFiles =
            (
                from s in fileList
                let nameLCS = s.FileName.ToLower().LongestCommonSubsequence(filter.ToLower()).Length
                let pathLCS = s.FilePath.ToLower().LongestCommonSubsequence(filter.ToLower()).Length
                let subsequenceTolerated = nameLCS >= filter.Length - tolerance ||
                                           pathLCS >= filter.Length - tolerance
                where subsequenceTolerated
                orderby nameLCS, pathLCS
                select s
            ).ToList();
            return foundFiles;
        }


        // Implementation from https://en.wikipedia.org/wiki/Longest_common_subsequence_problem
        private static string LongestCommonSubsequence(this string source, string target)
        {
            int[,] C = LcsLength(source, target);

            return Backtrack(C, source.ToCharArray(), target.ToCharArray(), source.Length, target.Length);
        }

        private static int[,] LcsLength(string a, string b)
        {
            int m = a.Length;
            int n = b.Length;
            int[,] C = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
                C[i, 0] = 0;
            for (int j = 0; j <= n; j++)
                C[0, j] = 0;
            for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
            {
                if (a[i - 1] == b[j - 1])
                    C[i, j] = C[i - 1, j - 1] + 1;
                else
                    C[i, j] = Math.Max(C[i, j - 1], C[i - 1, j]);
            }

            return C;
        }

        private static string Backtrack(int[,] C, char[] aStr, char[] bStr, int x, int y)
        {
            if (x == 0 | y == 0)
                return "";
            if (aStr[x - 1] == bStr[y - 1]) // x-1, y-1
                return Backtrack(C, aStr, bStr, x - 1, y - 1) + aStr[x - 1]; // x-1
            if (C[x, y - 1] > C[x - 1, y])
                return Backtrack(C, aStr, bStr, x, y - 1);
            return Backtrack(C, aStr, bStr, x - 1, y);
        }

        public static int[] AllIndexesOf(this string str, string substr, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(str) ||
                string.IsNullOrWhiteSpace(substr))
            {
                throw new ArgumentException("String or substring is not specified.");
            }

            var indexes = new List<int>();
            int index = 0;

            while ((index = str.IndexOf(substr, index,
                       ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
            {
                indexes.Add(index++);
            }

            return indexes.ToArray();
        }

        #region IEnumerable_Extensions
        /// <summary>
        /// Iterates through the enumerable itbl,
        /// checking at each member to see if it is time to perform a more expensive check.<br></br>
        /// When the predicate checkIf returns true, a check is performed (e.g., a messagebox asks the user if they want to keep going)
        /// and a new checkIf and check are taken from the list of checks.<br></br>
        /// If the check returns true, stop iterating through itbl immediately.<br></br>
        /// If the check returns false, move to the next checkIf and check in the sequence of checks
        /// and keep iterating through itbl.<br></br>
        /// EXAMPLE:<br></br>
        /// * we have a sequence of numbers. There are two (checkIf, check) tuples:<br></br>
        ///     - checkIf 1 asks if a number is greater than 100. check 1 asks if the sum of a list that's being generated is greater than 100 thousand.<br></br>
        ///     - checkIf 2 asks if a number is greater than 1000. check 2 asks if the sum of the running list is greater than 1 million.<br></br>
        /// * we run this on a list that has a total sum over 1 million.
        /// The first check happens when the sum is 99 thousand, and the counting continues.
        /// The second check happens when the sum is over 1 million and the counting is halted prematurely.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itbl"></param>
        /// <param name="checks"></param>
        /// <returns></returns>
        public static IEnumerable<T> CheckWhen<T>(this IEnumerable<T> itbl, IEnumerable<(Func<T, bool> checkIf, Func<bool> check)> checks)
        {
            IEnumerator<(Func<T, bool>, Func<bool>)> checkIterator = checks.GetEnumerator();
            Func<bool> check = null;
            Func<T, bool> checkIf = null;
            bool noMoreChecks = false;
            try
            {
                (checkIf, check) = checkIterator.Current;
            }
            catch
            {
                noMoreChecks = true;
            }
            foreach (T t in itbl)
            {
                if (!noMoreChecks && checkIf(t))
                {
                    if (check())
                    {
                        yield break;
                    }
                    (checkIf, check) = checkIterator.MoveNext()
                        ? checkIterator.Current
                        : (null, null);
                    if (check == null)
                        noMoreChecks = true;
                }
                yield return t;
            }
        }

        /// <summary>
        /// Iterate through elements t in itbl until checkIf returns true.<br></br>
        /// When checkIf(t) is true, call check().<br></br>
        /// If check() returns true, terminate iteration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itbl"></param>
        /// <param name="checkIf">condition that must be true to trigger the first check</param>
        /// <param name="check">the first check that determines whether to stop iteration</param>
        /// <returns></returns>
        public static IEnumerable<T> CheckWhen<T>(this IEnumerable<T> itbl, Func<T, bool> checkIf, Func<bool> check)
        {
            bool alreadyChecked = false;
            foreach (T t in itbl)
            {
                if (!alreadyChecked && checkIf(t))
                {
                    if (check())
                    {
                        yield break;
                    }
                    alreadyChecked = true;
                }
                yield return t;
            }
        }
    }
    #endregion
}