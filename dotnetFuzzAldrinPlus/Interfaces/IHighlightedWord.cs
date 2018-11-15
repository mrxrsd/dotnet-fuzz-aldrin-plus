namespace dotnetFuzzAldrinPlus.Interfaces
{
    public interface IHighlightedWord : ISubject
    {
        int[] Highlight { get; }
    }
}