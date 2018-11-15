using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public abstract class BaseScorer
    {
        public abstract double Score(string subject, string query, StringScorerOptions options);
    }
}