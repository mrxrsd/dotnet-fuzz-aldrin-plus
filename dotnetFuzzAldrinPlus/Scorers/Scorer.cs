using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public class Scorer : BaseScorer
    {
        public const double CONST_WM = 150;
        public const double CONST_POS_BONUS = 20;
        public const double CONST_TAU_SIZE = 150;
        public const double CONST_MISS_COEFF = 0.75;

        public static double ComputeScore(string subject, string subjectLw, StringScorerQuery preparedQuery)
        {
            var query   = preparedQuery.Query;
            var queryLw = preparedQuery.QueryLw;

            var score = 0.0;
            var m = subject.Length;
            var n = query.Length;

            return 1;
        }

        
        public static double ScorePattern(int count, int i, int sameCase, bool b, bool isFullWord)
        {
            throw new System.NotImplementedException();
        }
    }
}