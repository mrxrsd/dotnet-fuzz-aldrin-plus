namespace dotnetFuzzAldrinPlus.Scorers
{
    public class AcronymResult
    {
        public double Score { get; }
        public double Pos { get; }
        public int Count { get; }

        public AcronymResult(double score, double pos, int count)
        {
            Score = score;
            Pos = pos;
            Count = count;
        }

        public static AcronymResult ScoreAcronyms(string subject, string subjectLw, string query, string queryLw)
        {
            var m = subject.Length;
            var n = query.Length;

            if (m <= 1 && n <= 1) return CreateEmptyAcronymResult();

            var count = 0;
            var sepCount = 0;
            var sumPos = 0;
            var sameCase = 0;
            double score = 0;

            var i = -1;
            var j = -1;

            while (++j < n)
            {
                var qjLw = queryLw[j];

                if (ScorerUtil.IsSeparator(qjLw))
                {
                    i = subjectLw.IndexOf(qjLw, i + 1);
                    if (i > -1)
                    {
                        sepCount++;
                        continue;
                        ;
                    }
                    else
                    {
                        break;
                    }
                }

                while (++i < m)
                {
                    if (qjLw == subjectLw[i] && ScorerUtil.IsWordStart(i, subject, subjectLw))
                    {
                        if (query[j] == subject[i]) sameCase++;
                        sumPos += i;
                        count++;
                        break;
                    }
                }

                if (i == m) break;
            }

            if (count < 2) return CreateEmptyAcronymResult();

            var isFullWord = false;

            if (count == n)
            {
                isFullWord = IsAcronymFullWord(subject, subjectLw, query, count);
            }

            score = Scorer.ScorePattern(count, n, sameCase, true, isFullWord);
            
            return new AcronymResult(score, (double) sumPos/count, count + sepCount);
        }

        private static bool IsAcronymFullWord(string subject, string subjectLw, string query, int nbAcronymInQuery)
        {
            var m = subject.Length;
            var n = query.Length;
            var count = 0;

            if (m > 12 * n) return false;

            var i = -1;

            while (++i < m)
            {
                if (ScorerUtil.IsWordStart(i, subject, subjectLw) && ++count > nbAcronymInQuery) return false;
            }

            return true;
        }

        private static AcronymResult CreateEmptyAcronymResult()
        {
            return new AcronymResult(0, 0.1, 0);
        }
    }
}