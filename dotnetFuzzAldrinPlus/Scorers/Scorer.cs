using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public class Scorer : IScorerEngine
    {
        public const double CONST_WM = 150;
        public const double CONST_POS_BONUS = 20;
        public const double CONST_TAU_SIZE = 150;
        public const double CONST_MISS_COEFF = 0.75;


        public double Score(string subject, string query, StringScorerOptions options)
        {
            if (!(options.AllErrows || IsMatch(subject, options.PreparedQuery.CoreLw, options.PreparedQuery.CoreUp))) return 0;

            var stringLw = subject.ToLower();
            var score = ComputeScore(subject, stringLw, options.PreparedQuery);

            return Math.Ceiling(score);
        }

        public static double ComputeScore(string subject, string subjectLw, StringScorerQuery preparedQuery)
        {
            var query   = preparedQuery.Query;
            var queryLw = preparedQuery.QueryLw;

            var score = 0.0;
            var m = subject.Length;
            var n = query.Length;

            var acro = AcronymResult.ScoreAcronyms(subject, subjectLw, query, queryLw);
            var acroScore = acro.Score;

            if (acro.Count == n) return Scorer.ScoreExact(n, m, acroScore, acro.Pos);

            var pos = subjectLw.IndexOf(queryLw);

            if (pos > -1) return ScoreExactMatch(subject, subjectLw, query, queryLw, pos, n, m);

            var scoreRow = new double[n];
            var cscRow = new double[n];
            var sz = Scorer.ScoreSize(n, m);

            var missBudget = Math.Ceiling(CONST_MISS_COEFF * n) + 5;
            var missLeft = missBudget;
            var cscShouldRebuild = true;

            var j = -1;

            while (++j < n)
            {
                scoreRow[j] = 0;
                cscRow[j] = 0;
            }

            var i = -1;

            while (++i < m)
            {
                var siLW = subjectLw[i];

                if (!preparedQuery.CharCodes.Contains(siLW))
                {
                    if (cscShouldRebuild)
                    {
                        j = -1;
                        while (++j < n)
                        {
                            cscRow[j] = 0;
                        }

                        cscShouldRebuild = false;
                    }
                    continue;

                }

                score = 0.0;
                var scoreDiag = 0.0;
                var cscDiag = 0.0;
                var recordMiss = true;
                cscShouldRebuild = true;

                j = -1;
                while (++j < n)
                {
                    var scoreUp = scoreRow[j];
                    if (scoreUp > score) score = scoreUp;

                    var cscScore = 0.0;

                    if (queryLw[j] == siLW)
                    {
                        var start = ScorerUtil.IsWordStart(i, subject, subjectLw);

                        if (cscDiag > 0)
                        {
                            cscScore = cscDiag;
                        }
                        else
                        {
                            cscScore = Scorer.ScoreConsecutives(subject, subjectLw, query, queryLw, i, j, start);
                        }

                        var align = scoreDiag + Scorer.ScoreCharacter(i, j, start, acroScore, cscScore);

                        if (align > score)
                        {
                            score = align;
                            missLeft = missBudget;
                        }
                        else
                        {
                            if (recordMiss && --missLeft <= 0)
                            {
                                return Math.Max(score, scoreRow[n - 1]) * sz;
                            }
                            else
                            {
                                recordMiss = false;
                            }
                        }
                    }

                    scoreDiag = scoreUp;
                    cscDiag = cscRow[j];
                    cscRow[j] = cscScore;
                    scoreRow[j] = score;
                }
            }

            score = scoreRow[n - 1];
            
            return score * sz;
        }

        public static bool IsMatch(string subject, string queryLw, string queryUp)
        {
            var m = subject.Length;
            var n = queryLw.Length;

            if (m <= 0 || n > m) return false;

            var i = -1;
            var j = -1;

            while (++j < n)
            {
                var qjLw = queryLw[j];
                var qjUp = queryUp[j];

                while (++i < m)
                {
                    var si = subject[i];
                    if (si == qjLw || si == qjUp) break;
                }

                if (i == m) return false;
            }

            return true;
        }

        public static double ScorePosition(double pos)
        {
            if (pos < CONST_POS_BONUS)
            {
                var sc = CONST_POS_BONUS - pos;
                return 100 + sc * sc;
            }

            return Math.Max(100 + CONST_POS_BONUS - pos, 0);
        }

        public static double ScoreSize(double n, double m)
        {
            return CONST_TAU_SIZE / (CONST_TAU_SIZE + Math.Abs(m - n));
        }
        
        public static double ScorePattern(int count, int len, int sameCase, bool start, bool end)
        {
            var sz = count;

            var bonus = 6;
            if (sameCase == count) bonus += 2;
            if (start) bonus += 3;
            if (end) bonus += 1;

            if (count == len)
            {
                if (start)
                {
                    if (sameCase == len)
                    {
                        sz += 2;
                    }
                    else
                    {
                        sz += 1;
                    }
                }

                if (end) bonus += 1;
            }

            return sameCase + sz * (sz + bonus);
        }

        public static double ScoreExact(int n, int m, double quality, double pos)
        {
            return 2 * n * (CONST_WM * quality + ScorePosition(pos)) * ScoreSize(n,m);
        }

        public static double ScoreCharacter(int i, int j, bool start, double acroScore, double cscScore)
        {
            var posBonus = ScorePosition(i);

            if (start)
            {
                return posBonus + CONST_WM * (((acroScore > cscScore) ? acroScore : cscScore) + 10);
            }

            return posBonus + CONST_WM * cscScore;
        }

        public static double ScoreConsecutives(string subject, string subjectLw, string query, string queryLw, int i,
            int j, bool startOfWord)
        {
            var m = subject.Length;
            var n = query.Length;
            var mi = m - i;
            var nj = n - j;
            var k = mi < nj ? mi : nj;
            var sameCase = 0;
            var sz = 0;

            if (query[j] == subject[i]) sameCase++;

            while (++sz < k && queryLw[++j] == subjectLw[++i])
            {
                if (query[j] == subject[i]) sameCase++;
            }

            if (sz < k)
            {
                i--;
            }

            return ScorePattern(sz, n, sameCase, startOfWord, ScorerUtil.IsWordEnd(i, subject, subjectLw, m));

        }
        
        public static double ScoreExactMatch(string subject, string subjectLw, string query, string queryLw, int pos, int n, int m)
        {
            var start = ScorerUtil.IsWordStart(pos, subject, subjectLw);

            if (!start)
            {
                var pos2 = subjectLw.IndexOf(queryLw, pos + 1);
                if (pos2 > -1)
                {
                    start = ScorerUtil.IsWordStart(pos2, subject, subjectLw);
                    if (start) pos = pos2;
                }
            }

            var i = -1;
            var sameCase = 0;
            while (++i < n)
            {
                if ((query.Length > (pos+i) && subject.Length > i) && (query[pos+i] == subject[i])) sameCase++;
            }

            var end = ScorerUtil.IsWordEnd(pos + n - 1, subject, subjectLw, m);

            return ScoreExact(n, m, ScorePattern(n, n, sameCase, start, end), pos);
        }


    }
}