using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using dotnetFuzzAldrinPlus;
using dotnetFuzzAldrinPlus.Interfaces;
using Xunit;

namespace TestProject
{
    public class Test
    {

        [Fact]
        public void ReturnMostAccurateLimitedResultTest()
        {
            var opts = new StringScorerOptions(maxResults: 1);
            var candidates = new List<string> {"GruntFile", "filter", "bile"};

            var result = StringScorer.Filter(candidates, "file", opts);
            var words = result.Select(x => x.Word).ToList();

            Assert.Single(result);
            Assert.Equal(new [] {"GruntFile"}, words);
            
        }

        [Fact]
        public void PreferMatchWordAtBoundaryWithStringLimitTest()
        {
            var candidates = new List<string> {"0gruntfile0", "gruntfile0", "0gruntfile"};
            Assert.Equal("0gruntfile", ReturnBestMatch(candidates, "file"));
            Assert.Equal("gruntfile0", ReturnBestMatch(candidates, "grunt"));
        }

        [Fact]
        public void PreferMatchWordBoundaryWithSeparatorLimitTest()
        {
            var candidates = new List<string> { "0gruntfile0", "hello gruntfile0", "0gruntfile world" };
            Assert.Equal("0gruntfile world", ReturnBestMatch(candidates, "file"));
            Assert.Equal("hello gruntfile0", ReturnBestMatch(candidates, "grunt"));
        }

        [Fact]
        public void PreferMatchWordBoundaryWithCamelCaseLimitTest()
        {
            var candidates = new List<string> { "0gruntfile0", "helloGruntfile0", "0gruntfileWorld" };
            Assert.Equal("0gruntfileWorld", ReturnBestMatch(candidates, "file"));
            Assert.Equal("helloGruntfile0", ReturnBestMatch(candidates, "grunt"));
        }

        private string ReturnBestMatch(List<string> candidates, string query, StringScorerOptions options = null)
        {
            return Filter(candidates, query, options)[0].Word;
        }

        private IResultSubject[] Filter(List<string> candidates, string query, StringScorerOptions options = null)
        {
            return StringScorer.Filter(candidates, query, options);
        }

        [Fact]
        public void ReturnMostAccurateResultTest()
        {
            var candidates = new List<string> {"GruntFile", "filter", "bile"};

            var result = StringScorer.Filter(candidates, "file");
            var words = result.Select(x => x.Word).ToList();
            
            Assert.Equal(2, result.Length);
            Assert.Contains("GruntFile", words);
            Assert.Contains("filter", words);
        }

        [Fact]
        public void RankTest()
        {
            // "it ranks full-word > start-of-word > end-of-word > middle-of-word > split > scattered letters"

            var candidates = new List<string>
            {
                "controller x",
                "0_co_re_00 x",
                "0core0_000 x",
                "0core_0000 x",
                "0_core0_00 x",
                "0_core_000 x"
            };

            var result = StringScorer.Filter(candidates, "core");
            Assert.Equal(result[0].Word, candidates[5]);
            Assert.Equal(result[1].Word, candidates[4]);
            Assert.Equal(result[2].Word, candidates[3]);
            Assert.Equal(result[3].Word, candidates[2]);
            Assert.Equal(result[4].Word, candidates[1]);
            Assert.Equal(result[5].Word, candidates[0]);

            var result2 = StringScorer.Filter(candidates, "core x");
            Assert.Equal(result2[0].Word, candidates[5]);
            Assert.Equal(result2[1].Word, candidates[4]);
            Assert.Equal(result2[2].Word, candidates[3]);
            Assert.Equal(result2[3].Word, candidates[2]);
            Assert.Equal(result2[4].Word, candidates[1]);
            Assert.Equal(result2[5].Word, candidates[0]);
        }

        [Fact]
        public void PreferSequenceLengthTest()
        {
            // "rank middle of word case-insensitive match better than complete word not quite exact match (sequence length is king)"

            var candidates = new List<string> {"ZFILEZ", "fil e"};
            Assert.Equal("ZFILEZ", ReturnBestMatch(candidates, "file"));
        }

        [Fact]
        public void PreferSmallerHaystackTest()
        {
            var candidates = new List<string> { "core_", "core" };
            Assert.Equal("core", ReturnBestMatch(candidates, "core"));
        }


        [Fact]
        public void PreferMatchStartString1Test()
        {
            var candidates = new List<string> { "data_core", "core_data" };
            Assert.Equal("core_data", ReturnBestMatch(candidates, "core"));
        }

        [Fact]
        public void PreferMatchStartString2Test()
        {
            var candidates = new List<string> { "hello_data_core", "hello_core_data" };
            Assert.Equal("hello_core_data", ReturnBestMatch(candidates, "core"));
        }

        [Fact]
        public void PreferSingleLetterStartOfWordVsLongerQuery1Test()
        {
            var candidates = new List<string> { "Timecop: View", "Markdown Preview: Copy Html" };
            Assert.Equal("Markdown Preview: Copy Html", ReturnBestMatch(candidates, "m"));
        }

        [Fact]
        public void PreferSingleLetterStartOfWordVsLongerQuery2Test()
        {
            var candidates = new List<string> { "Welcome: Show", "Markdown Preview: Toggle Break On NewLine" };
            Assert.Equal("Markdown Preview: Toggle Break On NewLine", ReturnBestMatch(candidates, "m"));
        }

        [Fact]
        public void PreferSingleLetterStartOfWordVsLongerQuery3Test()
        {
            var candidates = new List<string> { "TODO", @"doc\README" };
            Assert.Equal(@"doc\README", ReturnBestMatch(candidates, "d"));
        }

        [Fact]
        public void PreferBetterOccurrenceThatHappensLaterInStringTest()
        {
            var candidates = new List<string> { "Test Espanol", @"Portugues" };
            Assert.Equal(@"Test Espanol", ReturnBestMatch(candidates, "es"));
        }

        [Fact]
        public void HelloWorldMatcherTest()
        {
            Assert.Equal(new int[] {0,1}, StringScorer.Match("Hello World", "he"));
            Assert.Equal(new int[] {6,7,8}, StringScorer.Match("Hello World", "wor"));
            Assert.Equal(new int[] {10}, StringScorer.Match("Hello World", "d"));
            Assert.Equal(new int[] {1,2,6,7,8}, StringScorer.Match("Hello World", "elwor"));
            Assert.Equal(new int[] {}, StringScorer.Match("Hello World", ""));
            Assert.Equal(new int[] {}, StringScorer.Match(null, "he"));
            Assert.Equal(new int[] {}, StringScorer.Match("", ""));
            Assert.Equal(new int[] {}, StringScorer.Match("", "abc"));
        }

        [Fact]
        public void PathMatcherTest()
        {
            Assert.Equal(new int[] { 0, 1, 2 }, StringScorer.Match(@"X\Y", @"X\Y"));
            Assert.Equal(new int[] { 0, 2 }, StringScorer.Match(@"X\X-x", @"X"));
            Assert.Equal(new int[] { 0, 2 }, StringScorer.Match(@"X\Y", @"XY"));
            Assert.Equal(new int[] { 2 }, StringScorer.Match(@"-\X", @"X"));
            Assert.Equal(new int[] { 0, 1, 3, 4 }, StringScorer.Match(@"XY\XY", @"XY"));
            Assert.Equal(new int[] { 2, 4 ,8, 11 }, StringScorer.Match(@"--X-Y-\-X--Y", @"XY"));

        }

        [Fact]
        public void PreferWholeWordToScatteredLettersTest()
        {
            Assert.Equal(new int[] { 12, 13, 14, 15 }, StringScorer.Match("fiddle gruntfile filler", @"file"));
            Assert.Equal(new int[] { 7, 8, 9, 10 }, StringScorer.Match("fiddle file", @"file"));
            Assert.Equal(new int[] { 8, 9, 10, 11 }, StringScorer.Match("find le file", @"file"));

        }

        [Fact]
        public void PreferExactMatchesTest()
        {
            Assert.Equal(new int[] { 12, 13, 14, 15 }, StringScorer.Match("filter gruntfile filler", @"file"));

        }

        [Fact]
        public void PreferWholeWordToScatteredLettersEventWithouthMatchesTest()
        {
            Assert.Equal(new int[] { 12, 13, 14, 15, 17 }, StringScorer.Match("fiddle gruntfile xfiller", @"filex"));
            Assert.Equal(new int[] { 7, 8, 9, 10, 12 }, StringScorer.Match("fiddle file xfiller", @"filex"));
            Assert.Equal(new int[] { 8, 9, 10, 11, 13 }, StringScorer.Match("find le file xfiller", @"filex"));

        }


        [Fact]
        public void PreferCaseSensitivesMatchesTest()
        {
            Assert.Equal(new int[] { 0, 1 , 2 }, StringScorer.Match("ccc CCC cCc CcC CCc", "ccc"));
            Assert.Equal(new int[] { 4, 5 , 6 }, StringScorer.Match("ccc CCC cCc CcC CCc", "CCC"));
            Assert.Equal(new int[] { 8, 9 , 10 }, StringScorer.Match("ccc CCC cCc CcC CCc", "cCc"));
            Assert.Equal(new int[] { 12, 13, 14 }, StringScorer.Match("ccc CCC cCc CcC CCc", "CcC"));
            Assert.Equal(new int[] { 16, 17, 18 }, StringScorer.Match("ccc CCC cCc CcC CCc", "CCc"));
        }

        [Fact]
        public void PreferCamelCaseToScatteredLettersTest()
        {
            Assert.Equal(new int[] { 0, 10, 15 }, StringScorer.Match("ImportanceTableCtrl", "itc"));
        }

        [Fact]
        public void PreferAcronymToScatteredLettersTest()
        {
            Assert.Equal(new int[] { 0, 7, 8, 9 }, StringScorer.Match("action_config", "acon"));
            Assert.Equal(new int[] { 0, 12, 13, 14 }, StringScorer.Match("application_control", "acon"));
        }

        [Fact]
        public void AccountForCaseInSelectingCamelCaseVsConsecutiveTest()
        {
            Assert.Equal(new int[] { 10, 15, 22 }, StringScorer.Match("0xACACAC: CamelControlClass.ccc", "CCC"));
            Assert.Equal(new int[] { 28, 29, 30 }, StringScorer.Match("0xACACAC: CamelControlClass.ccc", "ccc"));
        }


        [Fact]
        public void HelloWordScoreTest()
        {
            Assert.True(StringScorer.Score("Hello Word", "he") < StringScorer.Score("Hello Word", "Hello"));
            Assert.Equal(0, StringScorer.Score("Hello Word", ""));
            Assert.Equal(0, StringScorer.Score("Hello Word", null));
            Assert.Equal(0, StringScorer.Score(" ", "he"));
            Assert.Equal(0, StringScorer.Score(null, "he"));
            Assert.Equal(0, StringScorer.Score("", ""));
            Assert.Equal(0, StringScorer.Score(null, null));
            Assert.Equal(0, StringScorer.Score("",  "abc"));
        }
    }
}