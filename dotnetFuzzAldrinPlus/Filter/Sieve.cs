using System.Collections.Generic;
using System.Linq;
using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Filter
{
    public class Sieve
    {
        public static double SortCandidates(IResultCandidate a, IResultCandidate b)
        {
            return b.Score - a.Score;
        }

        public static IResultCandidate[] FilterCandidates(ICandidate[] candidates, string query,
            StringScorerOptions options)
        {
            var scoredCandidates = new List<ResultCandidate>();

            var spotLeft = candidates.Length + 1;

            if (options.MaxInners.HasValue && options.MaxInners > 0)
            {
                spotLeft = options.MaxInners.Value;
            }

            for (var i = 0; i < candidates.Length; i++)
            {
                double score = 0.0;

                score = options.ScorerEngine.Score(candidates[i].Word, query, options);

                if (score > 0)
                {
                    scoredCandidates.Add(new ResultCandidate(candidates[i], score));
                    if (--spotLeft == 0) break;
                }
            }

            var result = scoredCandidates.OrderByDescending(x => x.Score);

            if (options.MaxResults.HasValue) return result.Take(options.MaxResults.Value).ToArray();
            
            return result.ToArray();
        }
    }
}