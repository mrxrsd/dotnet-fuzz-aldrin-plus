using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using dotnetFuzzAldrinPlus.Exceptions;
using dotnetFuzzAldrinPlus.Scorers;

namespace dotnetFuzzAldrinPlus
{
    public class StringScorerQuery
    {
        public const string CONST_OPT_CHAR_RGX = @"[_ \-:\/\\]";

        private string _query;
        private string _charRegex;
        private string _pathSeparator;
        private string _coreChars;
        private string _ext;
        private HashSet<int> _charCode;
        private int _depth;
        
        public StringScorerQuery(string query, string charRegex, string pathSeparator)
        {
            if (string.IsNullOrEmpty(query)) throw new StringScorerException("Query cannot be null or empty");
            
            _query = query;
            _charRegex = charRegex ?? CONST_OPT_CHAR_RGX;
            _pathSeparator = pathSeparator;
            _coreChars = GetCoreChars(query, _charRegex);
            _ext = ScorerUtil.GetExtension(QueryLw);
            _charCode = GetCharCodes(QueryLw);
            _depth = ScorerUtil.CountDir(query, query.Length, Convert.ToChar(_pathSeparator));

        }

        public string Query => _query;
        public string QueryLw => _query.ToLower();
        public string CoreLw => _coreChars.ToLower();
        public string CoreUp => _coreChars.ToUpper();
        public string Ext => _ext;
        public int Depth => _depth;
        public HashSet<int> CharCodes => _charCode;
        
        
        public string GetCoreChars(string subject, string optCharRegex = null)
        {
            return Regex.Replace(subject, optCharRegex ?? _charRegex, "");
        }

        public HashSet<int> GetCharCodes(string subject)
        {
            var len = subject.Length;
            var i = -1;
            var charCodes = new HashSet<int>();

            while (++i < len)
            {
                if (!charCodes.Contains(subject[i]))
                    charCodes.Add(subject[i]);
            }

            return charCodes;
        }
        
        
    }
}