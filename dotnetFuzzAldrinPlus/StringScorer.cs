using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using dotnetFuzzAldrinPlus.Filter;
using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class StringScorer
    {
        public static IResultSubject[] Filter(IList<string> candidates, string query,
            StringScorerOptions options = null)
        {
            var convertedList = new List<ISubject>();
            candidates.ToList().ForEach(x=> convertedList.Add(new Subject(x)));

            return Filter(convertedList, query, options);
        }

        public static IResultSubject[] Filter(IList<ISubject> candidates, string query,
            StringScorerOptions options = null)
        {
            if (string.IsNullOrEmpty(query)) return new IResultSubject[0];
            if (candidates == null || !candidates.Any()) return new IResultSubject[0];

            options = options ?? new StringScorerOptions();
            options.Init(query);

            var result = Sieve.FilterCandidates(candidates.ToList().ConvertAll(Convert).ToArray(), query, options)
                .ToList();
                
            return result.ConvertAll(Convert).ToArray();
        }
        
        #region Helpers

        private static ISubject Convert(ICandidate candidate)
        {
            return new Subject(candidate.Word);
        }

        private static ICandidate Convert(ISubject subject)
        {
            return new Candidate(-1, subject.Word);
        }

        private static IResultSubject Convert(IResultCandidate resultCandidate)
        {
            return new ResultSubject(new Subject(resultCandidate.Word), resultCandidate.Score);
        }
        
        #endregion
    }
}