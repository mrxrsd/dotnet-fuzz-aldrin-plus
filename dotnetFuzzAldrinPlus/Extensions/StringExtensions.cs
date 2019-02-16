using System.ComponentModel;

namespace dotnetFuzzAldrinPlus.Extensions
{
    public static class StringExtensions
    {
        public static string Slice(this string value, int start, int end)
        {
            var from2 = start;
            var to2 = end;
            var result = string.Empty;
            int i;

            if (from2 < 0) from2 = value.Length + start;
            if (to2 < 0) to2 = value.Length + end;

            if ((to2 > from2) && (to2 > 0) && (from2 < value.Length))
            {
                if (from2 < 0) from2 = 0;
                if (to2 > value.Length) to2 = value.Length;

                for (i = from2; i < to2; i++) result += value[i];
            }

            return result;
        }
    }
}