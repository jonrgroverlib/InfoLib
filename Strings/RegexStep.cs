//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with InfoLib.  If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System.Text.RegularExpressions; // for RegexOptions, MatchEvaluator

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RegexStep -->
    /// <summary>
    ///      The RegexStep class contains one step in a RegexList
    /// </summary>
    /// <remarks>production ready</remarks>
    public class RegexStep
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Pattern, Replacement, Options, MatchFn -->                                           /// <summary>What is replaced</summary>
        public string         Pattern  { get { return _pattern;  } } private string         _pattern; /// <summary>What it is replaced by</summary>
        public string         Replace  { get { return _replace;  } } private string         _replace; /// <summary>Standard replacement options</summary>
        public RegexOptions   Options  { get { return _options;  } } private RegexOptions   _options; /// <summary>Sometimes there is a match evaluator function that runs on the matched substring(s)</summary>
        public MatchEvaluator MatchFn  { get { return _matchFn;  } } private MatchEvaluator _matchFn; /// <summary>how many iterations it changes the string - usually this will be 0 or 1</summary>
        public int            Changes  { get { return _changes;  } } private int            _changes; /// <summary>change in string length</summary>
        public int            DeltaLen { get { return _deltaLen; } } private int            _deltaLen;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public RegexStep(string pattern, string replace)
        {
            _pattern = pattern;
            _replace = replace;
        }
        public RegexStep(string pattern, string replace, RegexOptions options)
        {
            _pattern = pattern;
            _replace = replace;
            _options = options;
        }
        public RegexStep(string pattern, MatchEvaluator matchFn)
        {
            _pattern = pattern;
            _replace = "[[EVAL]]";
            _matchFn = matchFn;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Opts -->
        /// <summary>
        ///      Converts _options to a handy character coded format
        /// </summary>
        /// <remarks>production ready</remarks>
        public string Opts
        {
            get
            {
                string opts = "";
                if ((_options & RegexOptions.CultureInvariant)        != 0)  opts += "a";
                if ((_options & RegexOptions.Compiled)                != 0)  opts += "c";
                if ((_options & RegexOptions.ECMAScript)              != 0)  opts += "e";
                if ((_options & RegexOptions.IgnoreCase)              != 0)  opts += "i";
                if ((_options & RegexOptions.Multiline)               != 0)  opts += "m";
                if ((_options & RegexOptions.RightToLeft)             != 0)  opts += "r";
                if ((_options & RegexOptions.Singleline)              != 0)  opts += "s";
                if ((_options & RegexOptions.IgnorePatternWhitespace) != 0)  opts += "w";
                if ((_options & RegexOptions.ExplicitCapture)         != 0)  opts += "x";


                return opts;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Process -->
        /// <summary>
        ///      Do a replacement (or a command)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public string Process(string start)
        {
            string str = start;
            //if (Regex.IsMatch(_pattern, @"^\[\[REPEAT\]\]"))
            if (Regex.IsMatch(_pattern, @"[^.]\.\.\.$"))
            { /// repeat command operation:
                string input;
                //string pattern = Regex.Replace(_pattern, @"^\[\[REPEAT\]\]", "");
                string pattern = Regex.Replace(_pattern, @"\.\.\.$", "");
                input = str;
                str = Regex.Replace(input, pattern, _replace, _options & RegexOptions.CultureInvariant);
                if (str == input)
                    _changes = 0;
                else
                {
                    int j;
                    for (j = 1; j < 100 && str != input; ++j)
                    {
                        input = str;
                        str = Regex.Replace(input, pattern, _replace, _options & RegexOptions.CultureInvariant);
                    }
                    _changes = j;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_pattern) || Regex.IsMatch(_replace, @"^\[\[[A-Z]+\]\]$"))
                { /// special command operations:
                    if (_replace == "[[BREAK]]")  // you can set a breakpoint in the regex list
                        BreakPoint();
                    if (_replace == "[[EVAL]]")  // you can use a match evaluator conforming to  'static MatchEvaluator method = delegate(Match match)'
                        str = Regex.Replace(str, _pattern, _matchFn, _options);
                }
                else /// normal operation:
                {
                    string input = str;
                    str = Regex.Replace(input, _pattern, _replace, _options);
                    if (str == input) _changes = 0;
                    else _changes = 1;
                }
            }
            _deltaLen = str.Length - start.Length;
            return str;
        }
        private void BreakPoint(){}

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "s/" + _pattern + "/" + _replace + "/" + Opts;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertToLowerCase -->
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>production ready</remarks>
        public static MatchEvaluator ConvertToLowerCase = delegate(Match match)
        {
            string v = match.ToString();
            return (v.ToLower());
            //return (v.Substring(0,2) + char.ToLower(v[2]));
        };
    }
}
