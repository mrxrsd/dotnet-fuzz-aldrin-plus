using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus.Interfaces
{
    public interface IScorerEngine
    {
        double Score(string subject, string query, StringScorerOptions options);
    }
}