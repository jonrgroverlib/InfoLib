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
using System;                         // for Random
using System.Collections.Generic;     // for List
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- RegexGenerator -->
    /// <summary>
    ///      The RegexGenerator class is all about creating things from Regex Patterns
    /// </summary>
    /// <remarks>
    /// Status:
    ///      very very very incomplete
    /// </remarks>
    public class RegexGenerator
    {

        #region members and constructors


        private string _pattern;
        /// <summary>
        /// 
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public string TestPattern
        {
            get
            {
                string tp = "^^" + _pattern + "$$";
                tp = Regex.Replace(tp, @"^\^\^", "^");
                tp = Regex.Replace(tp, @"\$\$$", "$");
                return tp;
            }
        }


        private List<RegexLogStep> _steps;


        // ----------------------------------------------------------------------------------------
        /// <!-- RegexTargetGenerator constructor -->
        /// <summary>
        /// 
        /// </summary>
        public RegexGenerator()
        {
            _steps = new List<RegexLogStep>();
            _pattern = "";
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- RegexTargetGenerator constructor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public RegexGenerator(string pattern)
        {
            _steps = new List<RegexLogStep>();
            _pattern = pattern;
        }


        #endregion members and constructors

        #region methods


        // ----------------------------------------------------------------------------------------
        /// <!-- Change -->
        /// <summary>
        ///      Changes '[ABC]{3}' to 'CBB' or something
        /// </summary>
        /// <param name="r"></param>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private string Change(Random r, string input, string fromA, string fromB)
        {
            string snip = ExtractSnip(input, fromA + fromB);
            int num;


            // -------------------------------------------------------------------------90
            //  Identify parameters
            // -------------------------------------------------------------------------90
            string list = ExtractCore(snip, fromA);
            string strNum = ExtractCore(snip, fromB);
            if (!int.TryParse(strNum, out num))
                num = 0;
            char[] chars = list.ToCharArray();


            // -------------------------------------------------------------------------90
            //  Construct string from identified parameters
            // -------------------------------------------------------------------------90
            string cha = "";
            for (int i = 0; i < num; ++i)
                cha += chars[r.Next(chars.Length)];
            string output = Regex.Replace(input, fromA + fromB, cha);


            return output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Change -->
        /// <summary>
        ///      Changes [A-E] to [ABCDE] etc.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private string Change(string input, string from, Random r)
        {
            string output = input;
            string to = "";


            //TAG
            if (Regex.IsMatch(input, from))
            {
                string snip = ExtractSnip(input, from);
                string core = ExtractCore(snip, from);
                core = Regex.Replace(core, @"\\d", "0-9");
                string cha = "";


                if (Regex.IsMatch(core, "^.-.$"))
                {
                    // ---------------------------------------------------------------------90
                    //  Determine the limits
                    // ---------------------------------------------------------------------90
                    char[] coreChars = core.ToCharArray();
                    int lo = Convert.ToInt32(coreChars[0]);
                    int hi = Convert.ToInt32(coreChars[coreChars.Length - 1]);


                    // ---------------------------------------------------------------------90
                    //  Make and use the list
                    // ---------------------------------------------------------------------90
                    for (int c = lo; c <= hi; ++c)
                        cha += Convert.ToChar(c).ToString();
                    to = FillSnip(snip, cha);
                    // to = "[" + cha + "]";
                    string fr = Escape(snip);
                    output = Regex.Replace(input, fr, to);
                }
                else
                    output = input;
            }
            else
                output = input;


            _steps.Add(new RegexLogStep(input, from, to, output));


            return output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Change -->
        /// <summary>
        ///      Changes the pattern of interest from one to another using a regex pattern
        /// </summary>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private string Change(string input, string from, string to)
        {
            string output = Regex.Replace(input, from, to);
            _steps.Add(new RegexLogStep(input, from, to, output));
            return output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Change -->
        /// <summary>
        ///      Changes {3,7} to {5}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <param name="toA"></param>
        /// <param name="r"></param>
        /// <param name="toB"></param>
        /// <returns></returns>
        private string Change(string input, string from, string toA, Random r, string toB)
        {
            string output = input;
            string to = "";


            //TAG
            if (Regex.IsMatch(input, from))
            {
                string snip = ExtractSnip(input, from);
                string range = ExtractCore(snip, from);


                if (Regex.IsMatch(range, "^.,.+$"))
                {
                    // ---------------------------------------------------------------------90
                    //  Determine the limits
                    // ---------------------------------------------------------------------90
                    int max;
                    int min;
                    if (!int.TryParse(Regex.Replace(range, "^.*,", ""), out max)) max = 7;
                    if (!int.TryParse(Regex.Replace(range, ",.*$", ""), out min)) min = 0;


                    // ---------------------------------------------------------------------90
                    //  Pick the number
                    // ---------------------------------------------------------------------90
                    string strNum = "" + r.Next(min, max + 1);
                    to = FillSnip(snip, strNum);
                    string fr = Escape(snip);
                    output = Regex.Replace(input, fr, to);
                }
                else
                    output = input;
            }
            else
                output = input;


            _steps.Add(new RegexLogStep(input, from, to, output));


            return output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CharComponent -->
        /// <summary>
        ///      Returns a list of characters to make the pattern out of
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static List<char> CharComponent(string pattern)
        {
            char[] chars;
            string core = Regex.Replace(pattern, "^.*\\[(.+)\\].*$", "$1");
            core = Regex.Replace(core, "\\\\d", "0-9");


            if (!string.IsNullOrEmpty(core))
            {
                if (Regex.IsMatch(core, "^.-.$"))
                {
                    //  get the bounds
                    char[] coreChars = core.ToCharArray();
                    int lo = Convert.ToInt32(coreChars[0]);
                    int hi = Convert.ToInt32(coreChars[coreChars.Length-1]);


                    //  make the list
                    string cha = "";
                    for (int c = lo; c <= hi; ++c)
                        cha += Convert.ToChar(c).ToString();
                    chars = cha.ToCharArray();
                }
                else
                    chars = core.ToCharArray();
            }
            else
                chars = "abcdefghijklmnopqrstuvwxyz ".ToCharArray();


            return new List<char>(chars);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ChooseLength -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static int ChooseLength(string pattern, Random r)
        {
            int maxLength;
            int length;


            // ---------------------------------------------------------------------90
            //  Extract 'maximum' length
            // ---------------------------------------------------------------------90
            string max = Regex.Replace(pattern, "^.*{([0-9,]+)}.*$", "$1");
            max = Regex.Replace(max, "^[0-9],", "");
            if (!int.TryParse(max, out maxLength))
                maxLength = r.Next(2, 8) * r.Next(1, 8) * r.Next(1, 8) * r.Next(1, 8);


            // ---------------------------------------------------------------------90
            //  Calculate actual length
            // ---------------------------------------------------------------------90
            length = r.Next(1, maxLength * 2) - (int)(maxLength / 2);
            length = Math.Max(0, Math.Min(length, maxLength));


            return length;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateString -->
        /// <summary>
        ///      Creates a string to match a particular regex pattern
        /// </summary>
        /// <param name="whiteSpace">either "collapse", "replace" or "preserve"</param>
        /// <returns></returns>
        public string CreateString(Random r, string whiteSpace)
        {
            string str;
            str = CreateString(r);


            if (!Regex.IsMatch(str, _pattern))
                throw new NotImplementedException("string " + str + " does not match pattern " + _pattern);
            else
                return str;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateString -->
        /// <summary>
        ///      Handles:
        ///      [-+]?[\\d]*|[-+]?[\\d]+.[\\d]*|[-+]?[\\d]*.[\\d]+
        ///      [A-Z]{0,26}
        /// </summary>
        /// <remarks>
        ///      Normalized form:    [ABCD]{4}[WBRBT]{5}|[xfbvst]{0}[56347]{17}
        /// </remarks>
        /// <returns></returns>
        public string CreateString(Random r)
        {
            string input = _pattern;


            if (_pattern != "[-+]?[\\d]*|[-+]?[\\d]+.[\\d]*|[-+]?[\\d]*.[\\d]+"
                && _pattern != "[0-9]*"
                && _pattern != "[+]?[\\d]+"
                && _pattern != "[A-Z]{0,26}")
                BreakPoint();


            _steps.Clear();


            // -------------------------------------------------------------------------90
            //  Handle [\\d]+
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(input, @"\[[\\]d\]\+(.*)$", "[\\d]*" + r.Next(10) + "$1");
            }


            // -------------------------------------------------------------------------90
            //  Handle [\\d]*
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                switch (r.Next(3))
                {
                    case 0: input = Change(input, @"\[[\\]d\]\*(.*)$", "[\\d]*" + r.Next(10) + "$1"); break;
                    case 1: input = Change(input, @"\[[\\]d\]\*(.*)$", "" + r.Next(10) + "$1"); break;
                    case 2: input = Change(input, @"\[[\\]d\]\*(.*)$", "$1");                   break;
                }
            }


            // -------------------------------------------------------------------------90
            //  Handle ]*
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(input, @"\]\*", "]{"+r.Next(7)*r.Next(7)+"}");
            }


            // -------------------------------------------------------------------------90
            //  Handle {0,26}
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(input, "{[0-9,]+}", "{", r, "}");
            }


            // -------------------------------------------------------------------------90
            //  Handle [\\d]
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(input, @"\[\\d\]", "[0123456789]");
            }


            // -------------------------------------------------------------------------90
            //  Handle [A-Z]
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(input, @"\[[^\[\]]+\]", r);
            }


            // -------------------------------------------------------------------------90
            //  Handle [ABC]{3}
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                input = Change(r, input, @"\[[^\[\]]+\]", "{[0-9]+}");
            }


            // -------------------------------------------------------------------------90
            //  Handle [-+]?
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                string core = Regex.Replace(input, @"^.*\[([^\[\]]+)\]\?.*$", "$1");
                if (core != input)
                {
                    string pattern = @"\[" + EscapeSpecialChars(core) + @"\]\?";
                    if (r.Next(2) == 0)
                        input = Change(input, pattern+"(.*)$", "$1");
                    else
                    {
                        char[] letters = core.ToCharArray();
                        string target = "" + letters[r.Next(letters.Length)];
                        input = Change(input, pattern+"(.*)$", target+"$1");
                    }
                }
            }
            

            // -------------------------------------------------------------------------90
            //  Handle |
            // -------------------------------------------------------------------------90
            for (int i = 0; (i == 0) || (i < 100 && IsChanging()); ++i)
            {
                switch (r.Next(2))
                {
                    case 0: input = Change(input, @"^(.*)\|(.*)$", "$1"); break;
                    case 1: input = Change(input, @"^(.*)\|(.*)$", "$2"); break;
                }
            }


            // -------------------------------------------------------------------------90
            //  Check success
            // -------------------------------------------------------------------------90
            if (!Regex.IsMatch(input, _pattern))
                throw new NotImplementedException("String "+input+" does not match pattern"
                    + " " + _pattern);


            return input;
        }


        public static void BreakPoint()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CreateString -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="whiteSpace"></param>
        /// <returns></returns>
        public string CreateString(Random r, int length, string whiteSpace)
        {
            StringBuilder str = new StringBuilder(length + 1);
            List<char> chars = new List<char>();


            chars = CharComponent(_pattern);


            if (whiteSpace == "preserve" && chars.Count >= 20)
            {
                chars.Add('\r');
                chars.Add('\t');
            }


            // -------------------------------------------------------------------------90
            //  Build the string
            // -------------------------------------------------------------------------90
            int i = 0;
            char last = 'g';
            for (int j = 0; j < length + 20; ++j)
            {
                if (i >= length)
                    break;


                char cha = chars[r.Next(chars.Count)];


                switch (whiteSpace)
                {
                    case "collapse":
                        if (cha == ' ' && last == ' ') --i;
                        else str.Append(cha); break;
                    case "preserve":
                        if (cha == '\r') { str.Append(cha).Append("\n"); i++; }
                        else str.Append(cha); break;
                    case "replace":
                        if (cha == '\r' || cha == '\t') str.Append(" ");
                        else str.Append(cha); break;
                    case "": str.Append(cha); break;
                    default:
                        throw new NotSupportedException(whiteSpace + " is not a whitespace type");
                }


                i++;
                last = cha;
            }


            return str.ToString();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Escape -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snip"></param>
        /// <returns></returns>
        private static string Escape(string snip)
        {
            return Regex.Replace(snip, @"([\{\[\]\}])", @"\$1");
        }


        private static string EscapeSpecialChars(string str)
        {
            str = Regex.Replace(str, "([-+*])", "\\$1");
            return str;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractCore -->
        /// <summary>
        ///      Extracts the core of something like {3} or [ABC] or {1,27} etc.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private static string ExtractCore(string input, string from)
        {
            if (from.Substring(0, 1) == "\\")
            {
                from = Regex.Replace(from, "^(..)", "^.*$1(");
                from = Regex.Replace(from, "(..)$", ")$1.*$");
            }
            else
            {
                from = Regex.Replace(from, "^(.)", "^.*$1(");
                from = Regex.Replace(from, "(.)$", ")$1.*$");
            }
            string output = Regex.Replace(input, from, "$1");
            return output;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractSnip -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private static string ExtractSnip(string input, string from)
        {
            return Regex.Replace(input, "^.*(" + from + ").*$", "$1");
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- FillSnip -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snip"></param>
        /// <param name="filling"></param>
        /// <returns></returns>
        private static string FillSnip(string snip, string filling)
        {
            string A = snip.Substring(0, 1);
            string B = snip.Substring(snip.Length - 1, 1);
            snip = A + filling + B;
            return snip;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsChanging -->
        /// <summary>
        ///      Returns true if the target is changing
        /// </summary>
        /// <returns></returns>
        private bool IsChanging()
        {
            int count = _steps.Count;
            if (count == 0)  return true;
            RegexLogStep step = _steps[count - 1];
            return (step.Input != step.Output);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- LengthComponent -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static int LengthComponent(Random r, string pattern)
        {
            int maxLength;
            int length;


            if (Regex.IsMatch(pattern, "{[0-9]*,?[0-9]+}"))
            {
                length = ChooseLength(pattern, r);
            }
            else
            {
                if (Regex.IsMatch(pattern, "0-9"))  // an ID?
                    length = r.Next(1, 8) + r.Next(1, 8) + r.Next(1, 8);
                else
                { // prose?
                    maxLength = r.Next(2, 8) * r.Next(1, 8) * r.Next(1, 8) * r.Next(1, 8); ;
                    length = (int)(maxLength / 2);
                }
            }


            return length;
        }


        #endregion methods

    }


}
