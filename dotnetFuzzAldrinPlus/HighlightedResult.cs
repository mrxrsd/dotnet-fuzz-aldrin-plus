using System;
using System.Collections.Generic;
using System.Text;
using dotnetFuzzAldrinPlus.Interfaces;

namespace dotnetFuzzAldrinPlus
{
    public class HighlightedResult : IHighlightedWord
    {
        private readonly string _word;
        private readonly int[] _highlight;

        public HighlightedResult(string word, int[] highlight)
        {
            _word = word;
            _highlight = highlight;
        }

        public int[] Highlight => _highlight;

        public string Word => _word;

    }
}
