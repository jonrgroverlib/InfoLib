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
using System.Collections.Generic;     // for 
//using System.Linq;                    // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- IntExtension -->
    /// <summary>
    ///      The IntExtension class contains int extension methods
    /// </summary>
    /// <remarks>alpha code</remarks>
    public static class IntExtension
    {
        public static string ToPaddedString(this int chr, int padding)
        {
            string str;
            if (chr >= 128)
                str = "\\x" + ((int)chr).ToString("x");
            else
            {
                str = ((char)chr).ToString();
                switch (str)
                {
                    case "\0": str = "\\0"; break;
                    case "\a": str = "\\a"; break;
                    case "\b": str = "\\b"; break;
                    case "\f": str = "\\f"; break;
                    case "\n": str = "\\n"; break;
                    case "\r": str = "\\r"; break;
                    case "\t": str = "\\t"; break;
                    case "\v": str = "\\v"; break;
                }
            }
            return str.PadLeft(padding);
        }
    }


    // --------------------------------------------------------------------------------------------
    /// <!-- DiffStringExtension -->
    /// <summary>
    ///      The DiffStringExtension class contains string extension methods
    /// </summary>
    public static class DiffStringExtension
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- EndsWith -->
        /// <summary>
        ///      Checks whether a word in a string ends with a particular pattern
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool EndsWith(this string text, string pattern)
        {
            if (Regex.IsMatch(text, pattern+" ")
                || Regex.IsMatch(text, "^[A-Za-z]*"+pattern+"$"))
                return true;
            else return false;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ReverseString -->
        /// <summary>
        ///      Returns the reverse of the input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- SetToLength -->
        /// <summary>
        ///      Sets a string to a particular length, pads or truncates it as necessary
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string SetToLength(this string str, int n)
        {
            if (string.IsNullOrEmpty(str))
                return "".PadRight(n);
            if (str.Length > n)
                return str.Substring(0,n);
            else return str.PadRight(n);
        }
    }
}
