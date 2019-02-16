using System.Resources;
using dotnetFuzzAldrinPlus.Scorers;

namespace dotnetFuzzAldrinPlus.Interfaces
{
    public class StringScorerOptions
    {
        public const string DEFAULT_PATH_SEPARATOR = @"\";

        private bool _allowErros;
        private bool _usePathScoring;
        private bool _useExtensionBonus;
        private bool _wrap;
        private string _pathSeparator;
        private int? _maxInners;
        private int? _maxResults;
        private StringScorerQuery _preparedQuery;
        private IScorerEngine _scoreEngine;
        
        public StringScorerOptions(bool allowErrors = false, bool usePathScoring = true, bool useExtensionBonus = false,
            string pathSeparator = null, bool wrap = false, int? maxInners = null, int? maxResults = null)
        {
            _allowErros = allowErrors;
            _usePathScoring = usePathScoring;
            _useExtensionBonus = useExtensionBonus;
            _pathSeparator = pathSeparator ?? DEFAULT_PATH_SEPARATOR;
            _wrap = wrap;
            _maxInners = maxInners;
            _maxResults = maxResults;

            _scoreEngine =  usePathScoring ? (IScorerEngine) new PathScorer() : (IScorerEngine) new Scorer();
        }

        public bool UsePathScoring => _usePathScoring;
        public string PathSeparator => _pathSeparator;

        public bool UseExtensionBonus => _useExtensionBonus;

        public bool AllErrows => _allowErros;
        public int? MaxInners => _maxInners;

        public int? MaxResults => _maxResults;

        public IScorerEngine ScorerEngine => _scoreEngine;

        public StringScorerQuery PreparedQuery => _preparedQuery;

        public void Init(string query)
        {
            if (_preparedQuery == null || _preparedQuery.Query != query)
            {
                _preparedQuery = new StringScorerQuery(query, null, _pathSeparator );
            }
        }
    }
}