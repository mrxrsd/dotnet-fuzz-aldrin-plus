using System;
using System.Collections.Generic;
using System.Linq;
using dotnetFuzzAldrinPlus;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
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
    }
}