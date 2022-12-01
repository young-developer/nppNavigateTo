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
    }
}