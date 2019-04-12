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
using System;                         // for 
using System.Collections.Generic;     // for List, Dictionary
using System.Text;                    // for ?
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Split -->
    /// <summary>
    ///      The Split class is a general simple parsing results class
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class Split
    {
        // ----------------------------------------------------------------------------------------
        //  Data members
        // ----------------------------------------------------------------------------------------
        public string[] Name { get { return _name; } set { _name = value; } } private string[] _name; // a three-part name
        //public Date     Date { get { return _date; } set { _date = value; } } private Date     _date; // a Date
        public string   Num  { get { return _num;  } set { _num  = value; } } private string   _num;  // a numeric string
        public string   SSN  { get { return _ssn;  } set { _ssn  = value; } } private string   _ssn;  // a social security number
        public string   Text { get { return _text; } set { _text = value; } } private string   _text; // a phrase or sentence


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public Split()
        {
            _name = new string[3];
            _name[0] = "";
            _name[1] = "";
            _name[2] = "";
//            _date = Date.Null;
            _num  = "";
            _ssn  = "";
            _text = "";
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsDate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool ContainsDate(string text)
        {
            bool dateFound = false;


            //1. remove week days
            text = Regex.Replace(text, "(, )?(sun|mon|tues?|wed|thur?s?|fri|sat)(day|nesday|day|urday)?(, )?", " ", RegexOptions.IgnoreCase);

            //2. convert to yyyy, skip to step 5
            dateFound |= (Regex.IsMatch(text, @"[0123]?[0-9][-. /][0123]?[0-9][-. /][0-9][0-9]"));

            //3. for written months skip to step 6 - 1 birth date
            dateFound |= (Regex.IsMatch(text, @"[0123]?[0-9]t?h?[-. /]?(jan|febr?|ma[ry]|apr|ju[ln]|aug|sept?|oct|nov|dec)(\.|uary|ch|il|y|e|ust|ober|ember)?[-. /]?[12][0-9]{3}",                          RegexOptions.IgnoreCase));
            dateFound |= (Regex.IsMatch(text,                        @"(jan|febr?|ma[ry]|apr|ju[ln]|aug|sept?|oct|nov|dec)(\.|uary|ch|il|y|e|ust|ober|ember)?[-. /]?[0123]?[0-9]t?h?,?[-. /]?[12][0-9]{3}", RegexOptions.IgnoreCase));
            dateFound |= (Regex.IsMatch(text,     @"[12][0-9]{3}[-. /]?(jan|febr?|ma[ry]|apr|ju[ln]|aug|sept?|oct|nov|dec)(\.|uary|ch|il|y|e|ust|ober|ember)?[-. /]?[0123]?[0-9]t?h?",                      RegexOptions.IgnoreCase));

            //4. for years first, skip to step 6 - 1 birth date
            dateFound |= (Regex.IsMatch(text, "[12][0-9]{3}[-. /][0123]?[0-9][-. /][0123]?[0-9]"));

            //5. make two dates to look for - 2 birth dates
            dateFound |= (Regex.IsMatch(text, @"[0123]?[0-9][-. /][0123]?[0-9][-. /][12][0-9]{3}"));


            return dateFound;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertingDatesToAmerican -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ConvertingDatesToAmerican(string text)
        {
            text = Regex.Replace(text, "("+dd+")t?h?"+__+"?"+mmm          +__+"?("+yyyy+")"  , "$2 $1, $4", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "("+yyyy+")"  +__+"?"+mmm          +__+"?("+dd+")t?h?", "$2 $4, $1", RegexOptions.IgnoreCase);
            text = Regex.Replace(text,     mmm       +__+"?("+dd+")t?h?,?"+__+"?("+yyyy+")"  , "$1 $3, $4", RegexOptions.IgnoreCase);
            return text;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertingYYtoYYYY -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ConvertingYYtoYYYY(string text)
        {
            string ddmm = "[0123]?[0-9][-. /][0123]?[0-9][-. /]";
            string yy = "[0-9][0-9]";
            if (Regex.IsMatch(text, @"^" + ddmm + yy + "$") || Regex.IsMatch(text, @"^" + ddmm + yy + " ")
                || Regex.IsMatch(text, @" " + ddmm + yy + "$") || Regex.IsMatch(text, @" " + ddmm + yy + " ")
                )
            {
                string year2d = Regex.Replace(text, "^.*" + ddmm + "(" + yy + ").*$", "$1");
                string year4d = ConvertYearToFourDigits(year2d, DateTime.Now.AddYears(1).Year);
                text = Regex.Replace(text, "^(.*" + ddmm + ")" + yy + "(.*)$", "$1" + year4d + "$2");
            }
            return text;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- JoinLastName -->
        /// <summary>
        ///      Join last name with join character strings when the last name is multiple words
        /// </summary>
        /// <remarks>
        ///      StandardNameTable.Add(New Rexpr(" (DE) (LA) ([-'A-Z_]+):", " $1_$2_$3:"))
        ///      StandardNameTable.Add(New Rexpr(" (VAN|VON) (DE|DEN|DER) ([-'A-Z_]+):", " $1_$2_$3:"))
        ///      StandardNameTable.Add(New Rexpr(" ([A-Z]) (D|O) ([-A-Z_]+):", " $1 $2'$3:"))
        ///      StandardNameTable.Add(New Rexpr(" (DA|DI|DU|LA|LE|MC|ST|VAN|VON) ([-A-Z_]+):", " $1_$2:"))
        ///      StandardNameTable.Add(New Rexpr("^ (['A-Z\. ]+) (ABDUL|ABU|AL|EL) ([-A-Z_]+):", " $1 $2-$3:"))
        ///      StandardNameTable.Add(New Rexpr(" (ABDUL|ABU|AL|EL) ([-A-Z_]+):(.*;,[A-Z])", " $1-$2:$3"))
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="joinChar">usually something like a '_' or a '='</param>
        /// <returns></returns>
        public static string JoinLastName(string name, string joinChar)
        {
            // -------------------------------------------------------------------------90
            //  last name prefixes (French, Dutch, German, Irish, Italian, Spanish, Scottish, Arabic)
            // -------------------------------------------------------------------------90
            name = Regex.Replace(name, @"(['A-Z\.]+) (RANDAL) (EL)$", "$1 $2"+joinChar+"$3", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @" (DE) (LA) ([-'A-Z=]+)", " $1"+joinChar+"$2"+joinChar+"$3", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @"^(['A-Z\. ]+) (ABDUL|ABU|AL|EL) ([-A-Z_]+)", "$1 $2"+joinChar+"$3", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @" (VAN|VON) (DE|DEN|DER) ([-'A-Z=]+)", " $1"+joinChar+"$2"+joinChar+"$3", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @" (DA|DI|DU|LA|LE|MAC|MC|ST|VAN|VON) ([-'A-Z=]+)", " $1"+joinChar+"$2", RegexOptions.IgnoreCase);
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ParseText -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static Split ParseText(string searchText)
        {
            Split pt = new Split();


            string[] keywords = Split.OutDate(searchText); // this gets the date
            if (keywords[1] == "" && keywords[2] == "")
                keywords = Split.OutNum(searchText);


            // -------------------------------------------------------------------------90
            //  1. Process the first keyword if any
            //  2. Process the second keyword if any
            //  3. Process the third keyword if any
            // -------------------------------------------------------------------------90
            if (!string.IsNullOrEmpty(keywords[0]))  SettingVarsWhereMatch(keywords[0], pt);
            if (!string.IsNullOrEmpty(keywords[1]))  SettingVarsWhereMatch(keywords[1], pt);
            //{
            //    string keyword = keywords[1];
            //    pt.Date = new Date(keyword);
            //    if (Regex.IsMatch(keyword, "^[-0-9]{4,11}$")) pt.SSN = keyword;
            //}
            if (!string.IsNullOrEmpty(keywords[2]))  SettingVarsWhereMatch(keywords[2], pt);


            // -------------------------------------------------------------------------90
            //  Parse out the keyword string
            // -------------------------------------------------------------------------90
            if (string.IsNullOrEmpty(pt.Name[1])) pt.Name[1] = pt.Name[0];
            if (string.IsNullOrEmpty(pt.Name[1])) pt.Name[1] = pt.Name[2];
            if (string.IsNullOrEmpty(pt.Name[2])) pt.Name[2] = pt.Name[0];
            return pt;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SettingVarsWhereMatch -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pt"></param>
        private static void SettingVarsWhereMatch(string keyword, Split pt)
        {
//            if (ContainsDate (keyword                                              )) pt.Date = new Date(keyword);
            if (Regex.IsMatch(keyword, "^[0-9]+$"                                  )) pt.Num  = keyword;
            if (Regex.IsMatch(keyword, "^[0-9][-0-9]{2,5}[-][0-9]{1,4}$"           )) pt.SSN  = keyword;
            if (Regex.IsMatch(keyword, "^[0-9]{4}$"                                )) pt.SSN  = keyword;
            if (Regex.IsMatch(keyword, "^[-'A-Za-z. ]+$"                           )) pt.Name = Split.NameInThree(keyword);
            if (Regex.IsMatch(keyword, "^[-0-9A-Za-z, /]*[A-Za-z][-0-9A-Za-z, /]*$")) pt.Text = keyword;

            if (Regex.IsMatch(pt.Name[0], "^[0-9]+$"       )) throw new FormatException("parsing misfire");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MONddYYYY -->
        /// <summary>
        ///      
        /// </summary>
        /// <param name="text"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool MONddYYYY(string text, string[] str)
        {
            bool found = false;
            if (Regex.IsMatch(text, mmm+___+dd+",?"+___+yyyy))
            {
                str[0] = Regex.Replace(text, "^(.*)"+mon    +___+dd+",?"+___+yyyy+ ".*$" , "$1"  );
                str[1] = Regex.Replace(text, "^.*"  +mon+"("+___+dd+",?"+___+yyyy+").*$" , "$1$3");
                str[2] = Regex.Replace(text, "^.*"  +mon    +___+dd+",?"+___+yyyy+"(.*)$", "$3"  );
            }
            return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NameInTwo -->
        /// <summary>
        ///      Splits name into given and family names in a string array
        /// </summary>
        /// <remarks>
        ///      Anything more complex warrants a struct or a class
        /// </remarks>
        /// <param name="text">assumed to be last name if there is no comma or space</param>
        /// <returns>a two item string array [0]:given name [1]:last name</returns>
        public static string[] NameInTwo(string text)
        {
            string[] name = { "", "" };
            if (Regex.IsMatch(text, ","))
                name = Split.ReverseNameInTwo(text);
            else name = Split.RegularNameInTwo(text);
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NameInThree -->
        /// <summary>
        ///      Splits name into an array of three pieces - first, middle, and last
        /// </summary>
        /// <param name="textName"></param>
        /// <returns>three item string array</returns>
        public static string[] NameInThree(string textName)
        {
            string[] splitName = Split.NameInTwo(textName);
            string[] givenName = Split.GivenName(splitName[0]);
            string[] fullName  = { givenName[0], givenName[1], splitName[1] };
            return fullName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReverseNameInTwo -->
        /// <summary>
        ///      Split the name in two if the name is in reverse order separated by a comma
        /// </summary>
        /// <param name="fullName">with a comma delimiter</param>
        /// <returns>a string array of two: given name, last name</returns>
        public static string[] ReverseNameInTwo(string inputName)
        {
            string trimmedName = TrimName(inputName);
            trimmedName = JoinLastName(trimmedName, "=");


            string[] name = { trimmedName, "" };
            name[0] = Regex.Replace(trimmedName, "^.*, *", "");
            name[1] = Regex.Replace(trimmedName, ",.*$", "");
            name[1] = Regex.Replace(name[1], "=", " ");


            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RegularNameInTwo -->
        /// <summary>
        ///      Splits the name in two if the name is in standard order
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>a string array of two: given name, last name</returns>
        private static string[] RegularNameInTwo(string inputName)
        {
            string trimmedName = TrimName(inputName);
            trimmedName = JoinLastName(trimmedName, "=");
            string[] name = { trimmedName, "" };


            // -------------------------------------------------------------------------90
            //  Special case for when fullName is just a space followed by the last name
            // -------------------------------------------------------------------------90
            if (Regex.IsMatch(inputName, "^ +[-A-Za-z]+$"))
                { name[0] = "";  name[1] = trimmedName; }
            else
            {
                // ---------------------------------------------------------------------90
                //  General case - split the name in two
                // ---------------------------------------------------------------------90
                if (Regex.IsMatch(trimmedName, "[.A-Za-z] [.A-Za-z]"))
                {
                    name[1] = Regex.Replace(trimmedName, "^.* ", "");
                    name[0] = Regex.Replace(trimmedName, " " + name[1] + "$", "");
                }
            }


            name[1] = Regex.Replace(name[1], "=", " ");

            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SplitGivenName -->
        /// <summary>
        ///      Splits a person's given name into their first and middle names
        /// </summary>
        /// <param name="givenName"></param>
        /// <returns></returns>
        public static string[] GivenName(string givenName)
        {
            givenName = TrimName(givenName);


            // -------------------------------------------------------------------------90
            //  Split the name in two
            // -------------------------------------------------------------------------90
            string[] name = { givenName, "" };
            if (Regex.IsMatch(givenName, "[.A-Za-z] [.A-Za-z]"))
            {
                name[0] = Regex.Replace(givenName, " .*$", "");
                name[1] = Regex.Replace(givenName, "^" + name[0] + " ", "");
            }


            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OutDate -->
        /// <summary>
        ///      Splits a string into three pieces, the center piece being a date, also removes
        ///      embedded days of the week and converts 2 character years to 4 character years
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] OutDate(string text)
        {
            string[] str = new string[3];


            // -------------------------------------------------------------------------90
            //  Extract date substrings:
            //   1. Extract dates with week days included
            //   2. Extract American dates (Some dates are ambiguously American/European, so try American first)
            //   3. Extract East Asian dates
            //   4. Extract European dates
            //   5. Extract abbreviated dates (Assume American rather than European or East Asian)
            // -------------------------------------------------------------------------90
            // dates with week days included
            if (Split.Pattern(text, wkdy+", "+mmmm+___+dd  +",?"+___+yyyy, str)) return str; // American
            if (Split.Pattern(text, wkdy+", "+mmmm+___+ d  +",?"+___+yyyy, str)) return str; // American
            if (Split.Pattern(text, yyyy+___ +mmmm+___+dd  +", "    +wkdy, str)) return str; // East Asian
            if (Split.Pattern(text, yyyy+___ +mmmm+___+ d  +", "    +wkdy, str)) return str; // East Asian
            if (Split.Pattern(text, wkdy+" " +dd  +___+mmmm     +___+yyyy, str)) return str; // European
            if (Split.Pattern(text, wkdy+" " + d  +___+mmmm     +___+yyyy, str)) return str; // European

            // American dates
            if (Split.Pattern(text, mmmm+___ +dd+",?"+___+yyyy, str)) return str;
            if (Split.Pattern(text, mmm +___ +dd+",?"+___+yyyy, str)) return str;
            if (Split.Pattern(text, mm  +__  +dd     +__ +yyyy, str)) return str;
            if (Split.Pattern(text, m   +__  +dd     +__ +yyyy, str)) return str;
            if (Split.Pattern(text, mmmm+___ + d+",?"+___+yyyy, str)) return str;
            if (Split.Pattern(text, mmm +___ + d+",?"+___+yyyy, str)) return str;
            if (Split.Pattern(text, mm  +__  + d     +__ +yyyy, str)) return str;
            if (Split.Pattern(text, m   +__  + d     +__ +yyyy, str)) return str;

            // East Asian dates
            if (Split.Pattern(text, yyyy+___ +mmmm   +___+dd,   str)) return str;
            if (Split.Pattern(text, yyyy+___ +mmm    +___+dd,   str)) return str;
            if (Split.Pattern(text, yyyy+__  +mm     +__ +dd,   str)) return str;
            if (Split.Pattern(text, yyyy+__  +m      +__ +dd,   str)) return str;
            if (Split.Pattern(text, yyyy+___ +mmmm   +___+ d,   str)) return str;
            if (Split.Pattern(text, yyyy+___ +mmm    +___+ d,   str)) return str;
            if (Split.Pattern(text, yyyy+__  +mm     +__ + d,   str)) return str;
            if (Split.Pattern(text, yyyy+__  +m      +__ + d,   str)) return str;

            // European dates
            if (Split.Pattern(text, dd  +___ +mmmm   +___+yyyy, str)) return str;
            if (Split.Pattern(text, dd  +___ +mmm    +___+yyyy, str)) return str;
            if (Split.Pattern(text, dd  +__  +mm     +__ +yyyy, str)) return str;
            if (Split.Pattern(text, dd  +__  +m      +__ +yyyy, str)) return str;
            if (Split.Pattern(text, d   +___ +mmmm   +___+yyyy, str)) return str;
            if (Split.Pattern(text, d   +___ +mmm    +___+yyyy, str)) return str;
            if (Split.Pattern(text, d   +__  +mm     +__ +yyyy, str)) return str;
            if (Split.Pattern(text, d   +__  +m      +__ +yyyy, str)) return str;

            // Abbreviated dates (2 digit years)
            if (Split.Pattern(text, mm  +__  +dd     +__ +yy,   str)) return str;
            if (Split.Pattern(text, m   +__  +dd     +__ +yy,   str)) return str;
            if (Split.Pattern(text, mm  +__  + d     +__ +yy,   str)) return str;
            if (Split.Pattern(text, m   +__  + d     +__ +yy,   str)) return str;


            str[0] = text;
            str[1] = "";
            str[2] = "";
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OutNum -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        private static string[] OutNum(string text)
        {
            string[] str = new string[3];


            if (Split.Pattern(text, "[0-9][-0-9]{2,}[0-9]", str)) return str;


            str[0] = text;
            str[1] = "";
            str[2] = "";
            return str;
        }


        // ----------------------------------------------------------------------------------------
        //  Regex patterns
        // ----------------------------------------------------------------------------------------
        /// <summary>single digit numeric day with possible suffix, ie '3rd'</summary>
        private static string d    = "[1-9][nrst]?[dht]?";                                                          /// <summary>two digit numeric day with possible suffix, ie '18th'</summary>
        private static string dd   = "[0123][0-9]t?h?";                                                             /// <summary>three or more character week day without parens, ie SAT</summary>
        private static string wkdy = "[fmstwFMSTW][aehoruAEHORU][deintuDEINTU][nuNU]?[erER]?[sS]?[dD]?[aA]?[yY]?";  /// <summary>definite dash dot or slash</summary>
        private static string __   = "[-./]"    ;                                                                   /// <summary>possible dash dot space or slash</summary>
        private static string ___  = "[-. /]?"  ;                                                                   /// <summary>single digit numeric month</summary>
        private static string m    = "[1-9]"    ;                                                                   /// <summary>two digit numeric month</summary>
        private static string mm   = "[01][0-9]";                                                                   /// <summary>Three letter month abbreviation without parens</summary>
        private static string mmm  = "[adfjmnosADFJMNOS][aceopuACEOPU][bcglnprtvyBCGLNPRTVY]";                      /// <summary>full or partial month name without parens</summary>
        private static string mmmm = "[adfjmnosADFJMNOS][aceopuACEOPU][bcglnprtvyBCGLNPRTVY][ciortCIORT.]?[ehulEHUL]?[smSM]?[btBT]?[aAeE]?[rR]?[yY]?";        /// <summary>two digit year</summary>
        private static string yy   = "'?[0-9]{2}"  ;                                                                /// <summary>four digit year</summary>
        private static string yyyy = "[12][0-9]{3}";                                                                /// <summary>full or partial month name WITH PARENS!</summary>
        private static string mon  = @"(jan|feb|ma[ry]|apr|ju[ln]|aug|sep|oct|nov|dec)(\.|r?uary|ch|il|e|y|ust|ober|t?ember)?";


        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertYearToFourDigits -->
        /// <summary>
        ///      Converts a two digit string year to a four digit string year, given a cutoff year
        /// </summary>
        /// <param name="strYear"></param>
        /// <param name="cutYear"></param>
        /// <returns></returns>
        private static string ConvertYearToFourDigits(string strYear, int cutYear)
        {
            int year;
            int decYr = int.Parse(strYear);
            int cutYr = cutYear % 100;
            int century = (int)(cutYear / 100);
            if (decYr > cutYr) year = 100 * (century - 1) + decYr;
            else year = 100 * century + decYr;
            return year.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pattern -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool Pattern(string text, string pattern, string[] str)
        {
            if (Regex.IsMatch(pattern, @"\)"))
                throw new FormatException("no parens allowed in this particular pattern, sorry");
            bool match = false;
            if (Regex.IsMatch(text, pattern))
            {
                str[0] = "";
                str[0] = Regex.Replace(text, "^(.*?)" + pattern + ".*$", "$1");
                str[0] = Regex.Replace(str[0], " +$", "");

                str[1] = "";
                str[1] = Regex.Replace(text, "^.*?(" + pattern + ").*$", "$1");

                str[2] = "";
                str[2] = Regex.Replace(text, "^.*?" + pattern + "(.*)$", "$1");
                str[2] = Regex.Replace(str[2], "^ +", "");

                match = true;
            }

            return match;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TrimName -->
        /// <summary>
        ///      Deal with excess or missing spaces
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string TrimName(string name)
        {
            name = Regex.Replace(name, @" +$", "");
            name = Regex.Replace(name, @"^ +", "");
            name = Regex.Replace(name, @"  +", " ");
            name = Regex.Replace(name, @"\.([A-Z])", ". $1", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, @"^([A-Z][a-z]+)([A-Z][a-z]+)$", "$1 $2");
            return name;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SegmentDoc10 -->
        /// <summary>
        ///      Splits document into sections of 10 characters,
        ///      including a list of the sections of 10 characters for each section of 10 characters
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="size">10</param>
        /// <returns></returns>
        public static List<string> SegmentDoc10(string doc, int size)
        {
            char[] c = doc.ToCharArray();
            Dictionary<string,List<string>> next = new Dictionary<string,List<string>>();
            List<string> all = new List<string>();
            for (int i = 0; i < c.Length-size-size; ++i)
            {
                string str1 = "";
                string str2 = "";
                for (int j = 0; j < size; ++j)
                {
                    str1 += c[i+j];
                    str2 += c[i+j+size];
                }

                if (next.ContainsKey(str1)) next[str1].Add(str2);
                else
                {
                    all.Add(str1);
                    List<string> str = new List<string>();
                    str.Add(str2);
                    next.Add(str1,str);
                }
            }

            return all;
        }
    }
}
