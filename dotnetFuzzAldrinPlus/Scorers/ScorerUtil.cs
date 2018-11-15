using System.Net;
using System.Runtime.InteropServices.ComTypes;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public static class ScorerUtil
    {
        public static bool IsSeparator(char c)
        {
            return c == ' ' || c == '.' || c == '-' || c == '_' || c == '/' || c == '\\';
        }

        public static bool IsWordStart(int pos, string subject, string subjectLw)
        {
            if (pos == 0) return true;

            var currS = subject[pos];
            var prevS = subject[pos - 1];

            return IsSeparator(prevS) || ((currS != subjectLw[pos] && prevS == subjectLw[pos - 1]));
        }

        public static bool IsWordEnd(int pos, string subject, string subjectLw, int len)
        {
            if (pos == len - 1) return true;

            var currS = subject[pos];
            var nextS = subject[pos + 1];

            return IsSeparator(nextS) || ((currS == subjectLw[pos] && nextS != subjectLw[pos + 1]));
        }

        public static int CountDir(string path, int end, char pathSeparator)
        {
            if (end < 1) return 0;

            var count = 0;
            var i = -1;

            while (++i < end && path[i] == pathSeparator) continue;

            while (++i < end)
            {
                if (path[i] == pathSeparator)
                {
                    count++;
                    while (++i < end && path[i] == pathSeparator) continue;
                }
            }

            return count;
        }

        public static string GetExtension(string subject)
        {
            var pos = subject?.LastIndexOf(".") ?? -1;
            if (pos < 0) return string.Empty;

            return subject?.Substring(pos + 1);
        }
        
    }
}