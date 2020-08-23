using System;
using System.ComponentModel.Design;
using System.Xml;
using dotnetFuzzAldrinPlus.Extensions;
using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public class PathScorer : IScorerEngine
    {
        private const double CONST_TAU_DEPTH = 20;
        private const double CONST_FILE_COEFF = 2.5;
        
        
        public double Score(string subject, string query, StringScorerOptions options)
        {
            if (!(options.AllowErrors || Scorer.IsMatch(subject, options.PreparedQuery.CoreLw, options.PreparedQuery.CoreUp)))
            {
                return 0;
            }
            var score = 0.0;

            var subjectLw = subject.ToLower();
            score = ComputeScore(subject, subjectLw, options.PreparedQuery);
            score = ScorePath(subject, subjectLw, score, options);
            return Math.Ceiling(score);
        }

        
        private double ScorePath(string subject, string subjectLw, double score, StringScorerOptions options)
        {
            if (score == 0) return 0;

            var pathSeparator = Convert.ToChar(options.PathSeparator);
            var end = subject.Length - 1;
            while (subject[end] == pathSeparator) end--;

            var basePos = subject.LastIndexOf(pathSeparator, end);
            var fileLength = end - basePos;
            var extAdjust = 1.0;

            if (options.UseExtensionBonus)
            {
                extAdjust += GetExtensionScore(subjectLw, options.PreparedQuery.Ext, basePos, end, 2);
                score *= extAdjust;
            }

            if (basePos == -1) return score;

            var depth = options.PreparedQuery.Depth;

            while (basePos > -1 && depth-- > 0) basePos = subject.LastIndexOf(options.PathSeparator, basePos - 1);

            double basePathScore = 0.0;
            
            if (basePos == -1)
            {
                basePathScore = score;
            }
            else
            {
                basePathScore = extAdjust * Scorer.ComputeScore(subject.Slice(basePos + 1, end + 1), subjectLw.Slice(basePos + 1, end + 1), options.PreparedQuery);
            }

            var alpha = 0.5 * CONST_TAU_DEPTH / (CONST_TAU_DEPTH + ScorerUtil.CountDir(subject, end + 1, Convert.ToChar(options.PathSeparator)));

            return alpha * basePathScore + (1 - alpha) * score * Scorer.ScoreSize(0, CONST_FILE_COEFF * (fileLength));
        }

        private double ComputeScore(string subject, string subjectLw, StringScorerQuery optionsPreparedQuery)
        {
            return Scorer.ComputeScore(subject, subjectLw, optionsPreparedQuery);
        }

        private double GetExtensionScore(string subject, string ext, int startPos, int endPos, int maxDepth)
        {
            if (string.IsNullOrEmpty(ext)) return 0;

            var pos = subject.LastIndexOf(".", endPos);

            var n = ext.Length;
            var m = endPos - pos;

            if (m < n)
            {
                n = m;
                m = ext.Length;
            }

            pos++;
            var matched = -1;
            while (++matched < n)
            {
                if (subject[pos + matched] != ext[matched]) break;
            }

            if (matched == 0 && maxDepth > 0)
            {
                return 0.9 * GetExtensionScore(subject, ext, startPos, pos - 2, maxDepth);
            }

            return (double) matched / m;
        }
    }
}