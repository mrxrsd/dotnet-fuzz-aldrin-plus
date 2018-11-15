using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class Candidate : ICandidate
    {
        public Candidate(int Id, string Word)
        {
            this.Id = Id;
            this.Word = Word;
        }
        
        public string Word { get; }
        public int Id { get; }
    }
}