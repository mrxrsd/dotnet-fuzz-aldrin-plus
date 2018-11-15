using System.Resources;

namespace dotnetFuzzAldrinPlus.Interfaces
{
    public class StringScorerOptions
    {
        public const string DEFAULT_PATH_SEPARATOR = @"\";

        public StringScorerOptions(bool allowErrors = false, bool usePathScoring = true, bool useExtensionBonus = false,
            string pathSeparator = null, bool wrap = false, int? maxInners = null, int? maxResults = null)
        {
            
        }
    }
}