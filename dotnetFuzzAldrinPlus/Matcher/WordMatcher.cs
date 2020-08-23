using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using dotnetFuzzAldrinPlus.Extensions;
using dotnetFuzzAldrinPlus.Scorers;

namespace dotnetFuzzAldrinPlus.Matcher
{
    public class WordMatcher
    {
        public static int[] Match(string subject, string query, StringScorerOptions options)
        {

            if (!(options.AllowErrors || Scorer.IsMatch(subject, options.PreparedQuery.CoreLw, options.PreparedQuery.CoreUp)))
            {
                return new int[] { };
            }

            var string_lw = subject.ToLower();
            var matches = ComputeMatch(subject, string_lw, options.PreparedQuery);

            if (matches.Length == 0) return matches;

            if (subject.IndexOf(options.PathSeparator) > -1)
            {
                var baseMatches = BasenameMatch(subject, string_lw, options.PreparedQuery, options.PathSeparator);
                matches = MergeMatches(matches, baseMatches);
            }

            return matches;
        }

        public static HighlightedResult Wrap(string subject, string query, StringScorerOptions options)
        {
            var highlight = new int[subject.Length];

            if (subject == query)
            {
                int j = -1;
                while (++j < highlight.Length) highlight[j] = j;
                return new HighlightedResult(subject,highlight);
            }

            var matchPositions = Match(subject, query, options);

            return new HighlightedResult(subject, matchPositions);
        }

        private static int[] MergeMatches(int[] a, int[] b)
        {
            var m = a.Length;
            var n = b.Length;

            if (n == 0) return a;
            if (m == 0) return b;

            var i = -1;
            var j = 0;
            var bj = b[j];

            var outArray = new List<int>();

            while (++i < m)
            {
                var ai = a[i];
                while (bj <= ai && ++j < n)
                {
                    if (bj <ai) outArray.Add(bj);
                    bj = b[j];
                }
                outArray.Add(ai);
            }

            while (j < n) outArray.Add(b[j++]);

            return outArray.ToArray();
        }

        private static int[] BasenameMatch(string subject, string subject_lw, StringScorerQuery preparedQuery, string pathSeparator)
        {
            var end = subject.Length - 1;
            while (subject[end] == Convert.ToChar(pathSeparator)) end--;

            var basePos = subject.LastIndexOf(pathSeparator, end);

            if (basePos == -1) return new int[0];

            var depth = preparedQuery.Depth;

            while (depth-- > 0)
            {
                basePos = subject.LastIndexOf(pathSeparator, basePos - 1);
                if (basePos == -1) return new int[0];
            }

            basePos++;
            end++;

            return ComputeMatch(subject.Slice(basePos, end), subject_lw.Slice(basePos, end), preparedQuery, basePos);
        }

        private static int[] ComputeMatch(string subject, string subject_lw, StringScorerQuery preparedQuery, int offset = 0)
        {
            var query = preparedQuery.Query;
            var query_lw = preparedQuery.QueryLw;
            var m = subject.Length;
            var n = query.Length;
            var acro_score = AcronymResult.ScoreAcronyms(subject, subject_lw, query, query_lw).Score;

            var score_row = new double[n];
            var csc_row = new double[n];

            var trace = new double[m * n];
            var pos = -1;
            var j = -1;

            while (++j < n)
            {
                score_row[j] = 0;
                csc_row[j] = 0;
            }

            var i = -1;
            Moves? move = null;

            while (++i < m)
            {
                var score = 0.0;
                var score_up = 0.0;
                var csc_diag = 0.0;
                var si_lw = subject_lw[i];

                j = -1;

                while (++j < n)
                {
                    var csc_score = 0.0;
                    var align = 0.0;
                    var score_diag = score_up;

                    if (query_lw[j] == si_lw)
                    {
                        var start = ScorerUtil.IsWordStart(i, subject, subject_lw);
                        csc_score = csc_diag > 0 ? csc_diag : Scorer.ScoreConsecutives(subject, subject_lw, query, query_lw, i, j, start);
                        align = score_diag + Scorer.ScoreCharacter(i, j, start, acro_score, csc_score);
                    }

                    score_up = score_row[j];
                    csc_diag = csc_row[j];

                    if (score > score_up)
                    {
                        move = Moves.LEFT;
                    }
                    else
                    {
                        score = score_up;
                        move = Moves.UP;
                    }

                    if (align > score)
                    {
                        score = align;
                        move = Moves.DIAGONAL;
                    }
                    else
                    {
                        csc_score = 0;
                    }

                    score_row[j] = score;
                    csc_row[j] = csc_score;
                    trace[++pos] = score > 0 ? (int) move.Value : (int) Moves.STOP;
                }
            }

            i = m - 1;
            j = n - 1;
            pos = i * n + j;

            var backtrack = true;
            var matches = new List<int>();

            while (backtrack && i >= 0 && j >= 0)
            {
                switch ((Moves) trace[pos])
                {
                    case Moves.UP:
                        i--;
                        pos -= n;
                        break;
                    case Moves.LEFT:
                        j--;
                        pos--;
                        break;
                    case Moves.DIAGONAL:
                        matches.Add(i+offset);
                        j--;
                        i--;
                        pos -= n + 1;
                        break;
                    default:
                        backtrack = false;
                        break;
                }
            }

            matches.Reverse();

            return matches.ToArray();

        }
    }
}
