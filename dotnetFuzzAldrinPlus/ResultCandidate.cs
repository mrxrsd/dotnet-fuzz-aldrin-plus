using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class ResultCandidate : IResultCandidate
    {
        private readonly ICandidate _candidate;
        

        public ResultCandidate(ICandidate candidate, double score)
        {
            _candidate = candidate;
            Score = score;
        }
        
        public double Score { get; }
        public string Word => _candidate.Word;
        public int Id => _candidate.Id;
    }
}