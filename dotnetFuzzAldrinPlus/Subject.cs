using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class Subject : ISubject
    {

        public Subject(string word)
        {
            Word = word;
        }
        
        public string Word { get; }
    }
}