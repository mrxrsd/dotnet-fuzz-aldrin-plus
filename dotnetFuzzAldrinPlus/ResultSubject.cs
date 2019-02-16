using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class ResultSubject : IResultSubject
    {
        public ResultSubject(ISubject subject, double score)
        {
            Word = subject.Word;
            Score = score;
        }
        
        public string Word { get; }
        public double Score { get; }
    }
}