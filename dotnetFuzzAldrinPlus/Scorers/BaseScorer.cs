using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Scorers
{
    public interface IScorerEngine
    {
        double Score(string subject, string query, StringScorerOptions options);
    }
}