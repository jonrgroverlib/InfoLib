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
using InfoLib.Testing;         // for RandomSource
using System;                         // for DateTime, Guid
using System.Collections.Generic;     // for Dictionary<,>
using System.Data;                    // for NoNullAllowedException
using System.Data.SqlTypes;           // for SqlBoolean, SqlByte, SqlDateTime, SqlDecimal, SqlDouble, SqlGuid, SqlInt16, SqlInt32, SqlInt64, SqlMoney, SqlSingle, SqlString, SqlXml  
using System.Linq;                    // for Select
using System.Text;                    // for UTF8Encoding
using System.Text.RegularExpressions; // for Regex
using System.Xml.Serialization;       // for XmlSerializer

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- __ -->
    /// <summary>
    ///      The __ class is a place for static methods and properties oriented on:
    ///      1. string manipulations that I do not want to take up much code real estate
    ///      2. methods to be refactored into more reasonable places as they collect (see remarks)
    /// </summary>
    /// <remarks>
    ///      Graduates: (classes generally refactored from groups of methods that collected here)
    ///        for complex base conversions     see BigNumber
    ///        for complex date handling        see Date
    ///        for complex path manipulation    see PathSlicer
    ///        for complex form writing         see Plaster
    ///        for complex form reading         see Scrape
    ///        for complex exception handling   see Throw
    ///        for simple string parsing        see the Strings namespace
    ///        for simple type conversions      see TreatAs
    ///        for testing private methods      see ReflectorTest
    ///        for syntactic candy              see Here
    /// 
    ///      Patterns:
    ///        Singleton? - no - it's just a static class, there is no resource to manage
    /// 
    /// 
    ///      production ready
    /// </remarks>
    public static class __
    {
        // ----------------------------------------------------------------------------------------
        //  Silly syntactic candy
        // ----------------------------------------------------------------------------------------
        public static string To   { get { return "to"  ; } }
        public static string With { get { return "with"; } }


        // ----------------------------------------------------------------------------------------
        //  Short Methods
        // ----------------------------------------------------------------------------------------
        public static string Gap                                                { get { return "\r\n\r\n"; } }        /// <summary>Concatenates two strings if neither is blank</summary>
        public static string ConcatIf             (string A   , string B            ) { if (!string.IsNullOrEmpty(A) && !string.IsNullOrEmpty(B)) return A + B;                      else return "";            }        /// <summary>Concatenates two strings if neither is blank</summary>
        public static string ConcatIf             (string str , int num             ) { if (num >= 0                        ) return str + num.ToString();                           else return "";            }        /// <summary>Concatenates a delimiter between two strings if either is not ""</summary>
        public static string Openface             (string A , string delim, string B) { if (string.IsNullOrEmpty(A) && string.IsNullOrEmpty(B)) return "";                           else return A + delim + B; }
        public static string FirstInk             (string A   , string B            ) { if (string.IsNullOrWhiteSpace(A)    ) return B;                                              else return A;             }        /// <summary>Converts an object to a seven character string for use in a tabbed list or table</summary>
        public static string InTab                (object obj                       ) { if (obj != null                     ) return obj.ToString().PadLeft(6, ' ').Substring(0, 6); else return "      ";      }
        public static bool   IsCsv                (string str                       ) { if (string.IsNullOrEmpty(str.Trim())) return false; else return (Regex.IsMatch(str, ",") && Regex.IsMatch(str, "[\r\n]", RegexOptions.Singleline)); } /// <summary>Determines whether the string is an xml string</summary>
        public static bool   IsXml                (string str                       ) { if (string.IsNullOrEmpty(str.Trim())) return false; else return (Regex.IsMatch(str, "</") && Regex.IsMatch(str, ">"));  }        /// <summary>Returns a string exactly as long as specified</summary>
        public static string SetLength            (int    len , string str          ) { if (str == null                     ) str = ""; str = str.PadRight(len+1, ' ').Substring(0,len);  return str;           }        /// <summary>Returns the string that is not blank, preferring the first string</summary>
        public static string Best                 (string A   , string B            ) { if (string.IsNullOrEmpty(A)         ) return B;                                              else return A;             }        /// <summary>Trims a string or returns null if null</summary>
        public static string Trim                 (string str                       ) { if (str == null                     ) return str;                                            else return str.Trim();    }        /// <summary>Truncates a string if it is longer than the max length</summary>
        public static string Truncate             (string str , int maxlen          ) { if (str.Length > maxlen             ) return str.Substring(0, maxlen);                       else return str;           }        /// <summary>Toggles a character's case</summary>
        public static char   ToggleCase           (char   c                         ) { char c2 = c; if (Char.IsUpper(c)) c2 = Char.ToLower(c); if (Char.IsLower(c)) c2 = Char.ToUpper(c); return c2;           }
        public static string CapsLock             (string str                       ) { string flipcase = new string(str.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c)) : c).ToArray()); return flipcase; }
        public static int    CommonPrefixLength   (string A, string B, bool caseSensitive) { return CommonPrefix(A, B, caseSensitive).Length;                   } /// <summary>Prepares string content for insertion into a database using SQL text command construction, Warning: has no SQL insertion resistance</summary>
        public static object DoubleQuotes         (string str                       ) { return Regex.Replace(str, "'", "''");                                   } /// <summary>Good for database queries</summary>
        public static string FixApostrophe        (string str                       ) { return Regex.Replace(str, "'", "''");                                   } /// <summary>Minimum of three integers, used only here</summary>
        public static int    Minimum              (int    a   , int b, int c        ) { return Math.Min(Math.Min(a,b),c);                                       }
        public static int    MinMax               (int    lo  , int num, int hi     ) { return Math.Min(lo, Math.Max(hi, num));                                 } /// <summary>String is null or empty (syntactic candy)</summary>
        public static bool   NoE                  (string str                       ) { return string.IsNullOrEmpty(str);                                       } /// <summary>Converts a technical identifier into a grid header / field label</summary>
        public static string PrettyLabel          (string str                       ) { return Regex.Replace(SplitIntoWords(RemoveCamelPrefix(str)), "_", " "); } /// <summary>Example:  andThisTitle --&gt; ThisTitle</summary>
        public static string RemoveCamelPrefix    (string str                       ) { return Regex.Replace(str, "^[a-z]*", "");                               }
        public static string ShuffleString        (string str , Random r            ) { return ShuffleString(new List<char>(str.ToCharArray()), r);             }
        public static bool   StringIsNullOrWhiteSpace(string str                    ) { return !StringHasContent(str);                                          } /// <summary>Converts all simple spaces into something else</summary>
        public static object RespaceForWeb(string str, char respaceAs = (char)0x00A0) { return Regex.Replace(str, " ", respaceAs.ToString());                   } /// <summary>Removes up to one quote from the beginning and ende of a string</summary>
        public static string TrimQuotes           (string str                       ) { return Regex.Replace(Regex.Replace(str, "^[\"]", ""), "[\"]$", "");     } /// <summary>Gets a file name from a file path</summary>
        public static string LastSegment          (string path                      ) { string file   = Regex.Replace(path, "^.*[/\\\\]([^/\\\\]+)$", "$1");                   return file;                           }
        public static string PascalCase           (string str                       ) { string output = TitleCase(str);                                                        return Regex.Replace(output, " ", ""); } /// <summary>Converts a Pascal case string into a title case string</summary>
        public static string SplitIntoWords       (string str                       ) { string output = str; output = Regex.Replace(output, "([a-z])([A-Z])", "$1 $2");        return output;                         } /// <summary>Underlines a string</summary>
        public static string Underline            (string str , char ul             ) { string output = str + "\r\n" + ul.ToString().PadRight(str.Length, ul);                 return output;                         }
        public static Byte[] StringToUTF8ByteArray(String str                       ) { UTF8Encoding encoding = new UTF8Encoding(); Byte[] byteArray = encoding.GetBytes(str); return byteArray;                      }
        public static string RelativePath         (string root, string filePath     ) { string pattern = Regex.Replace(root, @"[/\\]", ".") + "."; string fileName = Regex.Replace(filePath, pattern, ""); return fileName; }
        public static string RenameNamespace(string xml, string renameFrom, string renameTo, string ns3) { xml = Regex.Replace(xml, renameTo, ns3); xml = Regex.Replace(xml, renameFrom, renameTo); return xml;  }


        // ----------------------------------------------------------------------------------------
        /// <!-- Collate -->
        /// <summary>
        ///      Collates a list of strings into a string given a joining string
        /// </summary>
        /// <param name="list">the list of strings to join</param>
        /// <param name="delim">the delimiter to join them with</param>
        /// <returns>a string from the items joined by the delimiter</returns>
        /// <remarks>production ready</remarks>
        public static string Collate(List<string> list, string delim)
        {
            StringBuilder str = new StringBuilder();
            string d = "";
            foreach (string s in list) { str.Append(d + s); d = delim; }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Collate -->
        /// <summary>
        ///      Returns a string of the contents of a dictionary collated in key order
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Collate(Dictionary<char, int> hash, string join)
        {
            StringBuilder str = new StringBuilder();
            List<char> keys = KeysSorted(hash);
            string delim = "";
            foreach (char key in keys)
            {
                str.Append(delim).Append(hash[key]);
                delim = join;
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Collate -->
        /// <summary>
        ///      Collates two strings leaving out strings with no content
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="join"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        internal static string Collate(string str1, string join, string str2)
        {
            if (join == null)
            {
                if (__.StringHasContent(str1) && __.StringHasContent(str2))
                                       return str1 + str2;
                else if (str1 == null) return str2;
                else                   return str1;
            }
            else
            {
                if (__.StringHasContent(str1) && __.StringHasContent(str2))
                                       return str1 + join + str2;
                else if (str1 == null) return str2;
                else                   return str1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CommonPrefixLength -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        /// <remarks>TODO: test me</remarks>
        public static string CommonPrefix(string str1, string str2, bool caseSensitive)
        {
	        if (string.IsNullOrEmpty(str1) | string.IsNullOrEmpty(str2)) { return ""; }

	        // ---------------------------------------------------------------------------
	        //  Measure the prefix similarity
	        // ---------------------------------------------------------------------------
	        char[] c1 = str1.ToUpper().ToCharArray(); if (caseSensitive) { c1 = str1.ToCharArray(); }
	        char[] c2 = str2.ToUpper().ToCharArray(); if (caseSensitive) { c2 = str2.ToCharArray(); }
	        int len = Math.Min(c1.Length, c2.Length); int sim = 0; bool done = false;
	        for (int i = 1; i <= len && !done; i++) { if (c1[i - 1] == c2[i - 1]) { sim = i; } else { done = true; } }


	        // ---------------------------------------------------------------------------
	        //  Assemble the prefix
	        // ---------------------------------------------------------------------------
	        string output = "";
	        if (sim > 0) { for (int i = 0; i <= sim - 1; i++) { output = output + c1[i]; } return output; }
	        return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatIf -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="delim"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static StringBuilder ConcatIf(StringBuilder str1, string delim, string str2)
        {
            if (str1.Length > 0 && !string.IsNullOrEmpty(str2))
                str1.Append(delim);
            if (!string.IsNullOrEmpty(str2))
                str1.Append(str2);
            return str1;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatIf -->
        /// <summary>
        ///      Concatenates two strings if neither is blank
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="delim"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string ConcatIf(string str1, string delim, string str2)
        {
            if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
                return str1 + delim + str2;
            else
                return str1 + str2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatIf -->
        /// <summary>
        ///      Returns what strings contain content with delimiters as needed
        /// </summary>
        /// <param name="str1">string 1</param>
        /// <param name="d12">delimiter between string 1 and 2</param>
        /// <param name="str2">string 2</param>
        /// <param name="d23">delimiter between string 2 and 3</param>
        /// <param name="str3">string 3</param>
        /// <param name="d13">delimiter between string 1 and 3</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string ConcatIf(string str1, string d12, string str2, string d23, string str3, string d13)
        {
            if ( NoE(str1) &&  NoE(str2) &&  NoE(str3)) return "";
            if ( NoE(str1) &&  NoE(str2) && !NoE(str3)) return str3;
            if ( NoE(str1) && !NoE(str2) &&  NoE(str3)) return str2;
            if ( NoE(str1) && !NoE(str2) && !NoE(str3)) return str2 + d23 + str3;
            if (!NoE(str1) &&  NoE(str2) &&  NoE(str3)) return str1;
            if (!NoE(str1) &&  NoE(str2) && !NoE(str3)) return str1 + d13 + str3;
            if (!NoE(str1) && !NoE(str2) &&  NoE(str3)) return str1 + d12 + str2;
            return str1 + d12 + str2 + d23 + str3;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ConcatIf -->
        /// <summary>
        ///      Returns what strings contain content with delimiters as needed
        /// </summary>
        /// <param name="str1">string 1</param>
        /// <param name="d12">delimiter between string 1 and 2</param>
        /// <param name="str2">string 2</param>
        /// <param name="d23">delimiter between string 2 and 3</param>
        /// <param name="str3">string 3</param>
        /// <param name="d34">delimiter between string 3 and 4</param>
        /// <param name="str4">string 4</param>
        /// <param name="d14">default delimiter</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string ConcatIf(string str1, string d12, string str2, string d23, string str3
            , string d34, string str4, string d14)
        {
            if ( NoE(str1) &&  NoE(str2) &&  NoE(str3) &&  NoE(str4)) return "";
            if ( NoE(str1) &&  NoE(str2) &&  NoE(str3) && !NoE(str4)) return str4;
            if ( NoE(str1) &&  NoE(str2) && !NoE(str3) &&  NoE(str4)) return str3;
            if ( NoE(str1) &&  NoE(str2) && !NoE(str3) && !NoE(str4)) return str3 + d34 + str4;
            if ( NoE(str1) && !NoE(str2) &&  NoE(str3) &&  NoE(str4)) return str2;
            if ( NoE(str1) && !NoE(str2) &&  NoE(str3) && !NoE(str4)) return str2 + d14 + str4;
            if ( NoE(str1) && !NoE(str2) && !NoE(str3) &&  NoE(str4)) return str2 + d23 + str3;
            if ( NoE(str1) && !NoE(str2) && !NoE(str3) && !NoE(str4)) return str2 + d23 + str3 + d34 + str4;
            if (!NoE(str1) &&  NoE(str2) &&  NoE(str3) &&  NoE(str4)) return str1;
            if (!NoE(str1) &&  NoE(str2) &&  NoE(str3) && !NoE(str4)) return str1 + d14 + str4;
            if (!NoE(str1) &&  NoE(str2) && !NoE(str3) &&  NoE(str4)) return str1 + d14 + str3;
            if (!NoE(str1) &&  NoE(str2) && !NoE(str3) && !NoE(str4)) return str1 + d14 + str3 + d34 + str4;
            if (!NoE(str1) && !NoE(str2) &&  NoE(str3) &&  NoE(str4)) return str1 + d12 + str2;
            if (!NoE(str1) && !NoE(str2) &&  NoE(str3) && !NoE(str4)) return str1 + d12 + str2 + d14 + str4;
            if (!NoE(str1) && !NoE(str2) && !NoE(str3) &&  NoE(str4)) return str1 + d12 + str2 + d23 + str3;
            if (!NoE(str1) && !NoE(str2) && !NoE(str3) && !NoE(str4)) return str1 + d12 + str2 + d23 + str3 + d34 + str4;
            return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- FindClosestMatch -->
        /// <summary>
        ///      Returns the least distant key in the dictionary to the target using Levenshtein distances
        /// </summary>
        /// <param name="target"></param>
        /// <param name="listOf"></param>
        /// <returns></returns>
        /// <remarks>test me</remarks>
        public static string FindClosestMatch(string target, ref Dictionary<string, int> listOf)
        {
	        // --------------------------------------------------------------------------
	        //  Initialize Variables
	        // --------------------------------------------------------------------------
	        string bestKey  = "";
	        int    shortest = 10000;


	        // --------------------------------------------------------------------------
	        //  Pick the first key with the smallest Levenshtein distance from the target
	        // --------------------------------------------------------------------------
	        foreach (string key in listOf.Keys)
            {
		        int distance = __.LevenshteinDistance_caseSensitive(target, key);
		        if (distance < shortest || string.IsNullOrEmpty(bestKey))
                    { shortest = distance; bestKey = key; }
	        }

	        return bestKey;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetLineValue -->
        /// <summary>
        ///      Given a line, return a Y value
        /// </summary>
        /// <param name="startY"></param>
        /// <param name="endY"></param>
        /// <param name="lengthX"></param>
        /// <param name="atX"></param>
        /// <returns></returns>
        public static int GetLineValue(int startY, int endY, int lengthX, int atX)
        {
            if (!(0 <= atX && atX <= lengthX))
                throw new IndexOutOfRangeException("num must be within length range");


            // --------------------------------------------------------------------------
            //  Percent of the way from low to high
            // --------------------------------------------------------------------------
            lengthX = Math.Max(1, lengthX);
            double            pct = 0.0;
            if (startY >= endY) pct = ((double)lengthX - (double)atX) / (double)lengthX; 
            else              pct = (double)atX / (double)lengthX;


            // --------------------------------------------------------------------------
            //  Calculation
            // --------------------------------------------------------------------------
            double min  = Math.Min(startY, endY);
            double diff = Math.Abs(startY - endY);
            double val  = min + diff * pct;
            int Y = Math.Max(1, (int)(0.5 + val));

            return Y;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetPascalSpacedLabel -->
        /// <summary>
        ///      Get the Pascal spaced version of a label  
        /// </summary>
        /// <param name="label">Name to be changed</param>
        /// <returns>PascalSpaced version of the name</returns>
        /// <remarks>This is a utility method is modified from NetTiers</remarks>
        public static string GetPascalSpacedLabel(string label, string defaultValue)
		{
			if (string.IsNullOrEmpty(label)) return defaultValue;
	        Regex regex = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
		    return regex.Replace(label, " ${x}");
	    }

        // ----------------------------------------------------------------------------------------
        /// <!-- Glue -->
        /// <summary>
        ///      Concatenates four strings together, putting a deliminter between them when both are not blank
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="str3"></param>
        /// <param name="str4"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Glue(string str1, string str2, string str3, string str4, string delim)
        {
            List<string> list = new List<string>(4);
            list.Add(str1);  list.Add(str2);  list.Add(str3);  list.Add(str4);
            return Glue(list, delim);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Glue -->
        /// <summary>
        ///      Concatenates a list of strings together, putting a deliminter between them when both are not blank
        /// </summary>
        /// <param name="list"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Glue(List<string> list, string delim)
        {
            string del = "";
            string str = "";
            foreach (string part in list)
                if (!string.IsNullOrEmpty(part))
                {
                    str += del + part;
                    del = delim;
                }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Glue -->
        /// <summary>
        ///      Returns what strings contain content with delimiters as needed
        /// </summary>
        /// <param name="str1">string 1</param>
        /// <param name="d12">delimiter between string 1 and 2</param>
        /// <param name="str2">string 2</param>
        /// <param name="d23">delimiter between string 2 and 3</param>
        /// <param name="str3">string 3</param>
        /// <param name="d34">delimiter between string 3 and 4</param>
        /// <param name="str4">string 4</param>
        /// <param name="d14">default delimiter to be used between str1&4, str2&4 or str1&3</param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Glue(string str1, string d12, string str2, string d23, string str3, string d34
            , string str4, string d14)
        {
            if (  NoE(str1) &&   NoE(str2) &&   NoE(str3) &&   NoE(str4)) return ""                                         ;
            if (  NoE(str1) &&   NoE(str2) &&   NoE(str3) && ! NoE(str4)) return                                        str4;
            if (  NoE(str1) &&   NoE(str2) && ! NoE(str3) &&   NoE(str4)) return                           str3             ;
            if (  NoE(str1) &&   NoE(str2) && ! NoE(str3) && ! NoE(str4)) return                           str3 + d34 + str4;
            if (  NoE(str1) && ! NoE(str2) &&   NoE(str3) &&   NoE(str4)) return              str2                          ;
            if (  NoE(str1) && ! NoE(str2) &&   NoE(str3) && ! NoE(str4)) return              str2 + d14              + str4;
            if (  NoE(str1) && ! NoE(str2) && ! NoE(str3) &&   NoE(str4)) return              str2 + d23 + str3             ;
            if (  NoE(str1) && ! NoE(str2) && ! NoE(str3) && ! NoE(str4)) return              str2 + d23 + str3 + d34 + str4;
            if (! NoE(str1) &&   NoE(str2) &&   NoE(str3) &&   NoE(str4)) return str1                                       ;
            if (! NoE(str1) &&   NoE(str2) &&   NoE(str3) && ! NoE(str4)) return str1              + d14              + str4;
            if (! NoE(str1) &&   NoE(str2) && ! NoE(str3) &&   NoE(str4)) return str1              + d14 + str3             ;
            if (! NoE(str1) &&   NoE(str2) && ! NoE(str3) && ! NoE(str4)) return str1              + d14 + str3 + d34 + str4;
            if (! NoE(str1) && ! NoE(str2) &&   NoE(str3) &&   NoE(str4)) return str1 + d12 + str2                          ;
            if (! NoE(str1) && ! NoE(str2) &&   NoE(str3) && ! NoE(str4)) return str1 + d12 + str2 + d14              + str4;
            if (! NoE(str1) && ! NoE(str2) && ! NoE(str3) &&   NoE(str4)) return str1 + d12 + str2 + d23 + str3             ;
            if (! NoE(str1) && ! NoE(str2) && ! NoE(str3) && ! NoE(str4)) return str1 + d12 + str2 + d23 + str3 + d34 + str4;
            return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Glue -->
        /// <summary>
        ///      Returns strings not empty with delimiter if both not empty
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="delim"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static string Glue(string str1, string delim, string str2)
        {
            if (  NoE(str1) &&   NoE(str2)) return ""                 ;
            if (  NoE(str1) && ! NoE(str2)) return                str2;
            if (! NoE(str1) &&   NoE(str2)) return str1               ;
            if (! NoE(str1) && ! NoE(str2)) return str1 + delim + str2;
            return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InstancesOf -->
        /// <summary>
        ///      Returns the nubmer of instances of a character in a string
        /// </summary>
        /// <param name="cha"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static int InstancesOf(char cha, string str)
        {
            char[] array = str.ToCharArray();
            int count = 0;
            for (int i = 0; i < array.Length; ++i)
                if (array[i] == cha) ++count;
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsNull -->
        /// <summary>
        ///      Determines whether an object is one or another of the standard null values
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            // --------------------------------------------------------------------------
            //  Check for standard null values
            // --------------------------------------------------------------------------
            if (obj == null || obj == DBNull.Value)
                return true;


            // --------------------------------------------------------------------------
            //  Investigate the specific Sql types for null value
            // --------------------------------------------------------------------------
            string type = obj.GetType().ToString();
            if (Regex.IsMatch(type, "^System.Data.SqlTypes"))
                switch (type)
                {
                    case "System.Data.SqlTypes.SqlBoolean" : if (((SqlBoolean) obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlByte"    : if (((SqlByte)    obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDateTime": if (((SqlDateTime)obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDecimal" : if (((SqlDecimal) obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlDouble"  : if (((SqlDouble)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlGuid"    : if (((SqlGuid)    obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt16"   : if (((SqlInt16)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt32"   : if (((SqlInt32)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlInt64"   : if (((SqlInt64)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlMoney"   : if (((SqlMoney)   obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlSingle"  : if (((SqlSingle)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlString"  : if (((SqlString)  obj).IsNull)  return true; break;
                    case "System.Data.SqlTypes.SqlXml"     : if (((SqlXml)     obj).IsNull)  return true; break;
                }

            return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- KeysSorted -->
        /// <summary>
        ///      Returns a sorted list of the keys of a dictionary
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static List<char> KeysSorted(Dictionary<char, int> hash)
        {
            List<char> keys = new List<char>(hash.Keys.Count);
            foreach (char key in hash.Keys)
                keys.Add(key);
            keys.Sort();
            return keys;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LastNChars -->
        /// <summary>
        ///      Returns the last n characters of a string
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string LastNChars(int num, string str)
        {
            if (string.IsNullOrEmpty(str.Trim())) return "";
            if (str.Length > num)               return str.Substring(str.Length - num, num);
            else                                return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- LevenshteinDistance -->
        /// <summary>
        ///      Reutrns the Levenshtein distance between two strings, Warning: case sensitive
        /// </summary>
        /// <param name="source">the so-called 'source' string, the order doesn't matter however</param>
        /// <param name="target">the so-called 'target' string, the order doesn't matter however</param>
        /// <returns></returns>
        /// <remarks>
        ///      d has (m+1)*(n+1) values, for all i and j, d[i,j] will hold the Levenshtein distance
        ///      between the first i characters of s and the first j characters of t;
        ///      
        ///      test me
        /// </remarks>
        public static int LevenshteinDistance_caseSensitive(string source, string target)
        {
            // --------------------------------------------------------------------------
            //  Prepare the Levenshtein matrix:
            // --------------------------------------------------------------------------
            char[] src = (" " + source).ToCharArray();
            char[] tgt = (" " + target).ToCharArray();
            int[,] dist = new int[src.Length,tgt.Length]; // [m, n] // distance matrix
            for (int i = 0; i < src.Length; ++i) for (int j = 0; j < tgt.Length; ++j)
                    dist[i,j] = Math.Max(i,j);

 
            // --------------------------------------------------------------------------
            //  Calculate using the Levenshtein matrix:
            // --------------------------------------------------------------------------
            for (int j = 1; j < tgt.Length; ++j)
                for (int i = 1; i < src.Length; ++i)
                    if (src[i] == tgt[j]) dist[i,j] = dist[i-1,j-1]; // no operation required
                    else                  dist[i,j] = Minimum(dist[i-1,j]+1, dist[i,j-1]+1, dist[i-1,j-1]+1); // Min( deletion , insertion , substitution);
            return dist[src.Length-1, tgt.Length-1];
        }
        public static int LevenshteinDistance_ignoreCase(string source, string target)
        {
            if (source == null) source = ""; if (target == null) target = "";
            return LevenshteinDistance_caseSensitive(source.ToUpper(), target.ToUpper());
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Max -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Max(string str1, string str2)
        {
            if (str1 == null) return str2;
            if (str2 == null) return str1;
            if (str1.Length > str2.Length) return str1;
            if (str2.Length > str1.Length) return str2;
            StringComparer sc = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, false);
            int  c = sc.Compare(str1, str2);
            if (c == 0) return str1;
            return str2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MaxSeparation -->
        /// <summary>
        ///      Returns the longest distance between two instances of the character in a string
        /// </summary>
        /// <param name="cha"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>alpha code - use levenschtein matrix instead</remarks>
        public static int MaxSeparation(char cha, string str)
        {
            char[] array = str.ToCharArray();
            int count = 0;
            int maxCount = 0;
            for (int i = 0; i < array.Length; ++i)
                if (array[i] == cha) { maxCount = count; count = 0; }
                else count++;
            return maxCount;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MoveLetter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">string to modify</param>
        /// <param name="letter">string (letter) to move</param>
        /// <param name="toPosition">1 to n or -1 to -n</param>
        /// <returns></returns>
        public static string MoveLetter(string str, string letter, int toPosition)
        {
            //  remove the letter
            string s2 = Regex.Replace(str, letter, "");

            if (toPosition < 0)
            {
                toPosition = -toPosition;
                toPosition--; // convert 1 to N to 0 to n-1
                s2 = Regex.Replace(s2, "(.{" + toPosition + "})$", letter + "$1");
            }
            else
            {
                toPosition--; // convert 1 to N to 0 to n-1
                s2 = Regex.Replace(s2, "^(.{" + toPosition + "})", "$1" + letter);
            }

            return s2;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NearPath -->
        /// <summary>
        ///      Returns a path close to the path input
        /// </summary>
        /// <param name="whereFoundPath"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static string NearPath(string whereFoundPath, string suffixFrom, string suffixTo)
        {
            string filePath = Regex.Replace(whereFoundPath, suffixFrom + "$", suffixTo);
            if (filePath == whereFoundPath)
                filePath = "..\\" + filePath;
            return filePath;
        }

        // --------------------------------------------------------------------------------------
        /// <!-- RandomLetter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="letters"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static char RandomLetter(string letters, int tries)
        {
            Random r = RandomSource.New().Random;

            char[] alphabet = letters.ToCharArray();
            int num = r.Next(letters.Length);
            for (int i = 1; i < tries; ++i)
                num = Math.Min(num, r.Next(letters.Length));
            return alphabet[num];
        }
        public static char RandomLetter(string letters) { return RandomLetter(letters, 1); }
        public static char RandomLetter() { return RandomLetter("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pluralize -->
        /// <summary>
        ///      Returns the plural of an English word, handles the common irregulars and the common ending rules
        /// </summary>
        /// <param name="noun"></param>
        /// <returns></returns>
        /// <remarks>There are 94 rules here:
        ///      -  5 phrase, phrase-like pluralization rules
        ///      - 17 kinds of nouns that often have either no singular form or identical singular and plural forms
        ///      -  1 transitional plural
        ///      - 34 irregular plural nouns and endings
        ///      -  7 rules to try to deal with nouns ending in f
        ///      - 23 words ending in -us that do not use the Latin -us rule
        ///      -  8 common ending rules (3 Latin and 5 English)
        /// </remarks>
        /// <remarks>beta code - used once in production</remarks>
        public static string Pluralize(string noun)
        {
            // --------------------------------------------------------------------------
            //  Plurals where 'of' (and friends) are used
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "(.) (about|in|of) (the )?([a-z]+)$", RegexOptions.IgnoreCase))     // heap of sand -> heaps of sand
            {
                string str2 = Regex.Replace(noun, "^(.+) (about|in|of) (the )?([a-z]+)$", "$1s", RegexOptions.IgnoreCase);
                if (0 < str2.Length && str2.Length < noun.Length)
                {
                    return Pluralize(str2) + Regex.Replace(noun, "^(.+) (about|in|of) (the )?([a-z]+)$", " $2 $3$4", RegexOptions.IgnoreCase);
                }
            }


            // --------------------------------------------------------------------------
            //  Plurals of some hyphenated prhase-like words
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "([Pp]asser|[Rr]unner)(by|-by|up|-up)$")) return Regex.Replace(noun, "er(..)$", "ers$1");
            if (Regex.IsMatch(noun, "[Mmr]other-in-law$"  )) return Regex.Replace(noun, "other-in-law$", "others-in-law");
            if (Regex.IsMatch(noun, "[Aa]ttorney-general$")) return Regex.Replace(noun, "rney-general$", "rneys-general");
            if (Regex.IsMatch(noun, "[Cc]ourt-martial$"   )) return Regex.Replace(noun, "ourt-martial$", "ourts-martial");


            // --------------------------------------------------------------------------
            //  17 kinds of words that sometimes have either no singular form or identical singular and plural forms:
            //
            //   1. Groups                          (no singular form)
            //   2. Documentation & communication   (no singular form)
            //   3. Materials                       (no singular form)
            //   4. Connected pairs                 (no singular form)
            //
            //   5. Fields                          (no singular form)
            //   6. Activities                      (no singular form)
            //   7. Sports & recreations            (no singular form)
            //   8. Short materials                 (no singular form)
            //
            //   9. -ish languages/peoples          (identical singular and plural forms)
            //  10. Herd animals                    (identical singular and plural forms)
            //  11. Directions                      (sometimes singular form)
            //  12. Adjectives operating as nouns   (no singular form)
            //
            //  13. Human qualities                 (no singular form)
            //  14. Religions & codes of conduct    (no singular form)
            //  15. Conditions                      (no singular form)
            //  16. Values                          (no singular form)
            //
            //  17. Hard core concepts              (no singular form)
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "(cattle|clothes|dead|equipment|furniture|garbage|luggage|money|traffic)$"                                        , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(advice|counsel|data|documentation|feedback|haiku|imagery|information|literature|music|news|poetry|thanks)$"     , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(chaff|cloth|copper|dirt|electricity|flour|gold|hummus|milk|oxygen|porcelain|silver|steel|stuff|sugar|sunshine|vinyl|wool|yen)$", RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(breeches|glasses|pants|pliers|scissors|tongs)$"                                                                 , RegexOptions.IgnoreCase)) return noun;

            if (Regex.IsMatch(noun, "(engineering|health|maintenance|marketing|math|media|programming|singing|transportation)$"                       , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(applause|combat|fun|hunting|hurry|leisure|running|shopping|sleeping)$"                                          , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(bowling|cards|checkers|chess|dancing|dice|fishing|golf|hockey|soccer|swimming|tennis)$"                         , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "^(air|anger|dust|earth|heat|rage|rice|sand)$"                                                                    , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, " (air|anger|dust|earth|heat|rage|rice|sand)$"                                                                    , RegexOptions.IgnoreCase)) return noun;

            if (Regex.IsMatch(noun, "(English|Finnish|French|Spanish|Turkish)$"                                                                       , RegexOptions.IgnoreCase)) return noun;  
            if (Regex.IsMatch(noun, "(bison|buffalo|craft|deer|fish|lynx|moose|plankton|quail|salmon|sheep|squid|swine|typhus|walrus)$"               , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(abroad|downstairs|east|here|nexus|north|noir|rendezvous|south|there|upstairs|west)$"                            , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "^(beautiful|beyond|big|comfortable|few|glad|inevitable|likely|many|most|neat|possible|raw|sad|sick|unique|vast)$", RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, " (beautiful|beyond|big|comfortable|few|glad|inevitable|likely|many|most|neat|possible|raw|sad|sick|unique|vast)$", RegexOptions.IgnoreCase)) return noun;

            if (Regex.IsMatch(noun, "(courage|expertise|leadership|melancholy|might|patience|piety|wisdom)$"                                          , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(Buddhism|[Bb]ushido|[Cc]hivalry|Christianity|Hinduism|Islam|[Jj]edi|Judaism|[Ss]halom|Zen)$"                    , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(brightness|chaos|coldness|dark|darkness|deflation|hardness|inflation|softness|weather|warmth)$"                 , RegexOptions.IgnoreCase)) return noun;
            if (Regex.IsMatch(noun, "(danger|hospitality|justice|plenty|poverty|progress|safety|wealth)$"                                             , RegexOptions.IgnoreCase)) return noun;

            if (Regex.IsMatch(noun, "(conduct|ignorance|knowledge|luck|peace|providence|publicity|serendipity|violence)$"                             , RegexOptions.IgnoreCase)) return noun;


            // --------------------------------------------------------------------------
            //  1 Transitional plural (the pluralizations of these words are in transtition in English)
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "[Aa]ntenna$"     )) return Regex.Replace(noun, "ntenna$"   , "ntennae"    ); // this one is pretty complicated, because context is what determines the pluralization Engineering: antennas, Biology: antennae


            // --------------------------------------------------------------------------
            //  3 Common irregular plurals (short words that usually don't form endings)
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "^[Dd]ie$"        )) return Regex.Replace(noun, "ie$"       , "ice"        ); // dice
            if (Regex.IsMatch(noun, " [Dd]ie$"        )) return Regex.Replace(noun, "ie$"       , "ice"        );
            if (Regex.IsMatch(noun, "^[Ff]oot$"       )) return Regex.Replace(noun, "oot$"      , "eet"        ); // feet
            if (Regex.IsMatch(noun, " [Ff]oot$"       )) return Regex.Replace(noun, "oot$"      , "eet"        );
            if (Regex.IsMatch(noun, "^[Oo]x$"         )) return Regex.Replace(noun, "x$"        , "xen"        ); // oxen
            if (Regex.IsMatch(noun, " [Oo]x$"         )) return Regex.Replace(noun, "x$"        , "xen"        );


            // --------------------------------------------------------------------------
            //  2 highly irregular plurals
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "^Cow$"           )) return Regex.Replace(noun, "Cow$"      , "Kine"       );
            if (Regex.IsMatch(noun, " Cow$"           )) return Regex.Replace(noun, "Cow$"      , "Kine"       );
            if (Regex.IsMatch(noun, "^cow$"           )) return Regex.Replace(noun, "cow$"      , "kine"       );
            if (Regex.IsMatch(noun, " cow$"           )) return Regex.Replace(noun, "cow$"      , "kine"       );
            if (Regex.IsMatch(noun, "Inuk$"           )) return Regex.Replace(noun, "Inuk$"     , "Inuit"      );


            // --------------------------------------------------------------------------
            //  23 Common irregular plurals (long words that sometimes form endings)
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "[Bb]louse$"      )) return Regex.Replace(noun, "ouse$"     , "ouses"      ); // (lice but not blice)
            if (Regex.IsMatch(noun, "[Mm]ongoose$"    )) return Regex.Replace(noun, "oose$"     , "ooses"      ); // (geese but not mongeese)
                                                      
            if (Regex.IsMatch(noun, "[Bb]allista$"    )) return Regex.Replace(noun, "ista$"     , "istae"      );
            if (Regex.IsMatch(noun, "[Bb]eau$"        )) return Regex.Replace(noun, "eau$"      , "eaux"       );
            if (Regex.IsMatch(noun, "[Bb]rother$"     )) return Regex.Replace(noun, "rother$"   , "rethren"    );
            if (Regex.IsMatch(noun, "[Cc]ello$"       )) return Regex.Replace(noun, "ello$"     , "ellos"      ); // Anglicized-Italian ending rule
            if (Regex.IsMatch(noun, "[Cc]hild$"       )) return Regex.Replace(noun, "hild$"     , "hildren"    );
            if (Regex.IsMatch(noun, "[Cc]riterion$"   )) return Regex.Replace(noun, "erion$"    , "eria"       );
            if (Regex.IsMatch(noun, "[Dd]iocese$"     )) return Regex.Replace(noun, "iocese$"   , "ioceses"    ); // breaks the -ese rule below
            if (Regex.IsMatch(noun, "[Ff]emur$"       )) return Regex.Replace(noun, "emur$"     , "emora"      );
            if (Regex.IsMatch(noun, "[Gg]oose$"       )) return Regex.Replace(noun, "oose$"     , "eese"       ); // (geese but not mongeese)
            if (Regex.IsMatch(noun, "[Hh]ello$"       )) return Regex.Replace(noun, "ello$"     , "ellos"      );
            if (Regex.IsMatch(noun, "[Hh]uman$"       )) return Regex.Replace(noun, "uman$"     , "umans"      );
            if (Regex.IsMatch(noun, "[Ll]ouse$"       )) return Regex.Replace(noun, "ouse$"     , "ice"        ); // (lice but not blice)
            if (Regex.IsMatch(noun, "[Mm]ouse$"       )) return Regex.Replace(noun, "ouse$"     , "ice"        );
            if (Regex.IsMatch(noun, "[Nn]etwork$"     )) return Regex.Replace(noun, "etwork$"   , "etworks"    );
            if (Regex.IsMatch(noun, "[Oo]asis$"       )) return Regex.Replace(noun, "asis$"     , "ases"       );
            if (Regex.IsMatch(noun, "[Pp]erson$"      )) return Regex.Replace(noun, "erson$"    , "eople"      );
            if (Regex.IsMatch(noun, "[Pp]hoto$"       )) return Regex.Replace(noun, "hoto$"     , "hotos"      );
            if (Regex.IsMatch(noun, "[Pp]ossum$"      )) return Regex.Replace(noun, "ossum$"    , "ossums"     );
            if (Regex.IsMatch(noun, "[Pp]sychology$"  )) return Regex.Replace(noun, "sychology$", "sychologies");
            if (Regex.IsMatch(noun, "[Pp]iano$"       )) return Regex.Replace(noun, "iano$"     , "ianos"      ); // Anglicized-Italian ending rule
            if (Regex.IsMatch(noun, "[Ss]tomach$"     )) return Regex.Replace(noun, "tomach$"   , "tomachs"    );
            if (Regex.IsMatch(noun, "[Tt]horax$"      )) return Regex.Replace(noun, "horax$"    , "horaces"    );
            if (Regex.IsMatch(noun, "[Tt]ooth$"       )) return Regex.Replace(noun, "ooth$"     , "eeth"       );


            // --------------------------------------------------------------------------
            //  4 Common irregular plural ending rules
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "[a-z](ese|graphy|ics|ies|inum|king|ology|ware|wear|work)$", RegexOptions.IgnoreCase)) return noun; // Chinese, geography, physics, species, aluminum, talking, geology, hardware, homework
            if (Regex.IsMatch(noun, "[Mm]an$"         )) return Regex.Replace(noun, "an$"       , "en"         );  // not womans
            if (Regex.IsMatch(noun, "[Tt]wo$"         )) return Regex.Replace(noun, "wo$"       , "wos"        );  // not twoes
            if (Regex.IsMatch(noun, "[Zz]ero$"        )) return Regex.Replace(noun, "ero$"      , "eros"       );  // not zeroes


            // --------------------------------------------------------------------------
            //  7 rules for words ending in f (f pluralizations are currently in transition in English and will change over time)
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "^[Ii]f$"         )) return Regex.Replace(noun, "f$"        , "fs"         ); // ifs, not ives
            if (Regex.IsMatch(noun, " [Ii]f$"         )) return Regex.Replace(noun, "f$"        , "fs"         );
            if (Regex.IsMatch(noun, "[Hh]oof$"        )) return Regex.Replace(noun, "oof$"      , "ooves"      ); // hooves
            if (Regex.IsMatch(noun, "[Tt]hief$"       )) return Regex.Replace(noun, "hief$"     , "hieves"     ); // thieves
            if (Regex.IsMatch(noun, "([hlr]ie|oo)f$"  )) return Regex.Replace(noun, "f$"        , "fs"         ); // brief -> briefs, roof -> roofs
            if (Regex.IsMatch(noun, "(ff)$"           )) return Regex.Replace(noun, "ff$"       , "ves"        ); // staff -> staves
            if (Regex.IsMatch(noun, "([aeiloru])fe?$" )) return Regex.Replace(noun, "fe?$"      , "ves"        ); // leaf  -> leaves


            // --------------------------------------------------------------------------
            //  23 words ending in -us that do not follow the -us -> -i rule, generally because they do not have Latin origin
            // --------------------------------------------------------------------------
            if (Regex.IsMatch(noun, "^([Bb]|[Pp]l)us$")) return Regex.Replace(noun, "us$"       , "uses"       ); // English                 bus: the pluralization of bus is in transition, English has not decided between buses and busses
            if (Regex.IsMatch(noun, " ([Bb]|[Pp]l)us$")) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin       plus
            if (Regex.IsMatch(noun, "^[Oo][np]us$"    )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin       onus
            if (Regex.IsMatch(noun, " [Oo][np]us$"    )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin       opus may pluralize to opera also
            if (Regex.IsMatch(noun, "[Bb]onus$"       )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Cc]allus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Cc]ampus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Cc]aucus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //                  Algonquian
            if (Regex.IsMatch(noun, "[Cc]ensus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Cc]horus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //        Greek
            if (Regex.IsMatch(noun, "[Cc]orpus$"      )) return Regex.Replace(noun, "us$"       , "ora"        ); //             Latin
            if (Regex.IsMatch(noun, "[Ee]xodus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //        Greek
            if (Regex.IsMatch(noun, "[Hh]iatus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Ff]o?etus$"     )) return Regex.Replace(noun, "us$"       , "uses"       ); //        Greek            because it's Greek rather than Latin
            if (Regex.IsMatch(noun, "[Gg]enus$"       )) return Regex.Replace(noun, "us$"       , "era"        ); //             Latin
            if (Regex.IsMatch(noun, "[Ll]otus$"       )) return Regex.Replace(noun, "us$"       , "uses"       ); //                  Semitic
            if (Regex.IsMatch(noun, "[Oo]mnibus$"     )) return Regex.Replace(noun, "us$"       , "uses"       ); // English
            if (Regex.IsMatch(noun, "[Rr]uckus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); // English
            if (Regex.IsMatch(noun, "[Rr]umpus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); // English
            if (Regex.IsMatch(noun, "[Ss]choolbus$"   )) return Regex.Replace(noun, "us$"       , "uses"       ); // English
            if (Regex.IsMatch(noun, "[Ss]inus$"       )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
            if (Regex.IsMatch(noun, "[Ss]tatus$"      )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin     status is a word with a very complicated history, the plural may also be 'status' but not 'stati' because status ia back formation from the Latin word meaning 'to stand' and has gone thorug a couple changes in meaning [http://www.reference.com/motif/language/plural-form-of-status]
            if (Regex.IsMatch(noun, "[Vv]irus$"       )) return Regex.Replace(noun, "us$"       , "uses"       ); //             Latin
                                                                                                              
                                                                                                              
            // --------------------------------------------------------------------------                     
            //  3 Common Latin plural ending rules                                                            
            // --------------------------------------------------------------------------                     
            if (Regex.IsMatch(noun, "is$"             )) return Regex.Replace(noun, "is$"       , "es"         ); // crisis   -> crises
            if (Regex.IsMatch(noun, "um$"             )) return Regex.Replace(noun, "um$"       , "a"          ); // agendum  -> agenda
            if (Regex.IsMatch(noun, "us$"             )) return Regex.Replace(noun, "us$"       , "i"          ); // octopus  -> octopi
                                                                                                              
                                                                                                              
            // --------------------------------------------------------------------------                     
            //  4 Common English plural ending rules                                                          
            // --------------------------------------------------------------------------                     
            if (Regex.IsMatch(noun, "([aeiou])y$"     )) return Regex.Replace(noun, "y$"        , "ys"         ); // day   -> days
            if (Regex.IsMatch(noun, "(.)y$"           )) return Regex.Replace(noun, "y$"        , "ies"        ); // city  -> cities
            if (Regex.IsMatch(noun, "([aeiou])o$"     )) return Regex.Replace(noun, "o$"        , "os"         ); // radio -> radios
            if (Regex.IsMatch(noun, "(ch|o|sh|s|x)$"  )) return Regex.Replace(noun, "(.)$"      , "$1es"       ); // box   -> boxes,  


            // --------------------------------------------------------------------------
            //  1 Common default plural ending rule (80% of plurals)
            // --------------------------------------------------------------------------
            return noun + "s";                                                                                    // boat -> boats
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pluralize -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num">the number of units (unit is not pluralized if num is 1)</param>
        /// <param name="unit">The singular form of the unit to pluralize (unless num is 1)</param>
        /// <returns></returns>
        /// <remarks>beta code</remarks>
        public static string Pluralize(int num, string unit)
        {
            string str = "";
            if (num == 1 || num == -1)  str = num.ToString() + " " + unit;
            else                        str = num.ToString() + " " + Pluralize(unit);
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrettyPrintStackTrace -->
        /// <summary>
        ///      Formats the stack trace so that the method names are stacked on top of each other
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        /// <remarks>broken, needs fixing and exceptin handling - for debugging</remarks>
        public static string PrettyPrintStackTrace(string stackTrace, int width)
        {
            //string[] line = stackTrace.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            //List<string> part1 = new List<string>(line.Length);
            //List<string> part2 = new List<string>(line.Length);
            //List<string> part3 = new List<string>(line.Length);
            //for (int i = 0; i < line.Length; ++i)
            //{
            //    string fullLine = line[i];
            //    part1.Add(Regex.Replace(line[i], @"^ *at *(.*)\.[^.]+\(.*$" , "$1"));
            //    part2.Add(Regex.Replace(line[i],        @"^.*\.([^.]+)\(.*$", "$1"));
            //    part3.Add(Regex.Replace(line[i],         @"^.*\.[^.]+\("    , ""  ));
            //}
            //int max1 = 0; foreach(string item in part1) max1 = Math.Max(max1, item.Length);
            //int max2 = 0; foreach(string item in part2) max2 = Math.Max(max2, item.Length);
            //int max3 = 0; foreach(string item in part3) max3 = Math.Max(max3, item.Length);


            //StringBuilder str = new StringBuilder(stackTrace.Length *2);
            //string delim = "";
            //for (int i = 0; i < line.Length; ++i)
            //{
            //    string oneLine = "|   at " + part1[i].PadLeft(max1) + "." + part2[i].PadRight(max2) + "(" + part3[i].PadRight(max3);
            //    oneLine = oneLine.Substring(0,width-4) + "...|";
            //    oneLine = Regex.Replace(oneLine, @"  \.\.\.\|$", "     |");
            //    str.Append(delim + oneLine);

            //    delim = "\r\n";
            //}


            //return str.ToString();
            return "";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PrettyXml -->
        /// <summary>
        ///      Returns a pretty version of an xml string without changing the content,
        ///      adds and removes white space between the tags
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="indent">number of spaces per indentation</param>
        /// <param name="cr">preferred carriage return, defaults to "\r\n"</param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production - useful for debugging etc.</remarks>
        public static string PrettyXml(string xml, int indent, string cr)
        {
            // -------------------------------------------------------------------------90
            //  Split xml into tag lines
            // -------------------------------------------------------------------------90
            xml = Regex.Replace(xml, ">[\r\n\t ]*<", ">\r<");
            char[] separators = { '\r' };
            string[] line = xml.Split(separators, StringSplitOptions.RemoveEmptyEntries);



            StringBuilder xml2 = new StringBuilder();
            if (string.IsNullOrEmpty(cr)) cr = "\r\n";
            int depth = 0;
            int curr = 0;
            List<string> stack = new List<string>();
            for (int i = 0; i < line.Length; ++i)
            {
                // ---------------------------------------------------------------------90
                //  Calculate line
                // ---------------------------------------------------------------------90
                if      (Regex.IsMatch(line[i], "^</[^<>]+>$")) { depth--; curr = depth; } // depth decreases are immediate
                else if (Regex.IsMatch(line[i], "^<[^</>][^<>]*[^</>]>$")) depth++;  // handle 2+ character tags
                else if (Regex.IsMatch(line[i], "^<[^</>]>$")) depth++;  // handle single character tags
                xml2.Append(cr.PadRight(cr.Length + indent * curr) + line[i]);
                curr = depth;  // depth increases are delayed
            }
            string xml3 = Regex.Replace(xml2.ToString(), "^[\r\n]*", "");
            return xml3;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomId -->
        /// <summary>
        ///      Builds an ID where the year, month, and day each consist of one letter
        /// </summary>
        /// <remarks>
        ///      First Letter:
        ///        A 2000,2025,2050,2075      B 2001,2026,2051,2076     C 2002, 2027, 2052, 2077
        ///        ...
        ///        Y 2024,2049,2074,2099
        /// 
        ///      Second Letter: (the 2nd letter of the pair is for days after the 26th)
        ///        AB January    CD February   EF March      GH April      IJ May        KL June
        ///        MN July       OP August     QR Sept       ST Oct        UV Nov        WX Dec
        /// 
        ///      Third Letter (16):
        ///        A 1st,17th    B 2nd,18th    C 3rd,19th    D 4th,20th    E 5th, 21sh
        ///        F 6th,22nd    G 7th,23rd    H 8th,24th    I 9th,25th    J 10th,26th
        ///        K 11th,27th   L 12th,28th   M 13th,29th   N 14th,30th   O 15th,31st   P 16th   
        /// 
        ///      Third Letter (27):
        ///        A 1st,27th    B 2nd,28th    C 3rd,29th    D 4th,30th    E 5th, 31st
        ///        F 6th     G 7th     H 8th     I 9th     J 10th    K 11th    L 12th    M 13th
        ///        N 14th    O 15th    P 16th    Q 17th    R 18th    S 19th    T 20th    U 21st
        ///        V 22nd    W 23rd    X 24th    Y 25th    Z 26th
        /// 
        ///      Fourth Letter:
        ///        A 0:00    B 1:00    C 2:00    D 3:00    E 4:00
        ///        F 5:00    G 6:00    H 7:00    I 8:00    J 9:00    K 10:00   L 11:00   M 12:00
        ///        N 13:00   O 14:00   P 15:00   Q 16:00   R 17:00   S 18:00   T 19:00   U 20:00
        ///        V 21:00   W 22:00   X 23:00
        /// 
        ///      So 3/17/2011 3:30 PM would be  LEQP , so would 3/17/2036 3:10 PM
        /// </remarks>
        /// <param name="prefix"></param>
        /// <param name="date"></param>
        /// <param name="numNumerals"></param>
        /// <param name="join">string to join various the sub parts</param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public static string RandomId_EmbedDate(string prefix, DateTime date, string join, int numDigits)
        {
            int wrapday = 16;  /// must be between 16 and 26
            if (string.IsNullOrEmpty(join)) join = "";
            string year  = "" + (char)(65 + date.Year % 25);
            string month = "" + (char)(63 + date.Month * 2 + ((date.Day - 1) / wrapday)); // two letters per month
            string day   = "" + (char)(65 + (date.Day-1) % wrapday);                // the end of the month wraps back to A,B,C...
            string hour  = "" + (char)(65 + date.Hour);
            string id = year + month + day + hour;
            if (!string.IsNullOrEmpty(prefix)) id = prefix + id;
            if (numDigits > 0) id = id + join + __.RandomNumericId(numDigits);
            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RandomNumericId -->
        /// <summary>
        ///      Uses Guids to create random numeric id's
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>beta code - used once in production</remarks>
        public static string RandomNumericId(int length)
        {
            string id = "";
            int subLen = 17;
            while (length > 0)
            {
                if (length > subLen) id += RandomNumericId(subLen, subLen);
                else id += RandomNumericId(length, subLen);
                length -= subLen;
            }

            return id;
        }
        /// <summary>creates a 'random' numeric ID up to maxLen digits long using a Guid</summary>
        private static string RandomNumericId(int length, int maxLen)
        {
            if (length > maxLen) length = maxLen;
            long num = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            string id = num.ToString().PadLeft(length + 10, '0');
            id = id.Substring(id.Length - length);
            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Reverse -->
        /// <summary>
        ///      Reverses a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Reverse(string str)
        {
            if (str == null) return null;
            if (str.Length <= 1) return str;
            char[] input = str.ToCharArray();
            string output = "";
            for (int i = input.Length - 1; i >= 0; --i)
                output += input[i];
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SentenceCase -->
        /// <summary>
        ///      Converts a string to a sentence case string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string SentenceCase(string str)
        {
            string sentence = str.ToLower();
            string first = sentence.Substring(0,1);
            sentence = first.ToUpper() + sentence.Substring(1);
            return sentence;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ShuffleString -->
        /// <summary>
        ///      Returns a randomized string
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string ShuffleString(List<char> c, Random r)
        {
            int    count     = c.Count;
            int    remaining = count  ;
            string str       = ""     ;


            for (int i = 0; i < count; ++i)
            {
                int j = r.Next(remaining);
                str += c[j];
                c.RemoveAt(j);
                remaining--;
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StringHasContent -->
        /// <summary>
        ///      For systems that do not have 'string.IsNullOrWhiteSpace' yet
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringHasContent(string str)
        {
          //return (!string.IsNullOrWhiteSpace(str));
            if (str == null                     ) return false;
            if (string.IsNullOrEmpty(str.Trim())) return false;
            if (Regex.IsMatch(str, @"^\s*$")    ) return false;
            return true;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- StringTimeNow -->
        /// <summary>
        ///      Returns a string giving the current time in YYYYMMDDHHMMSS format
        /// </summary>
        /// <remarks>beta code</remarks>
        public static string StringTimeNow { get
        {
            DateTime current = DateTime.Now;
            string date = String.Format("{0}{1}{2}{3}{4}{5}"
                , current.Year
                , current.Month .ToString().PadLeft(2, '0')
                , current.Day   .ToString().PadLeft(2, '0')
                , current.Hour  .ToString().PadLeft(2, '0')
                , current.Minute.ToString().PadLeft(2, '0')
                , current.Second.ToString().PadLeft(2, '0')
                );

            return date;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- Substring -->
        /// <summary>
        ///      Returns the specified sub path of a path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="startIndex">first segment to be included</param>
        /// <param name="length">number of segments to be included</param>
        /// <returns></returns>
        public static string SubPath(string fullPath, int startIndex, int length = -1)
        {
            string split = "\\";
            string[] segment = fullPath.Split(split.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            int concatLimit = 0;
            if (length < 0)
                concatLimit = segment.Length;
            else
                concatLimit = startIndex + length;


            string subPath = "";
            string delim = "";
            for (int j = startIndex; j < concatLimit; ++j)
                { subPath += delim + segment[j]; delim = split; }

            return subPath;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Substring -->
        /// <summary>
        ///      A somewhat more robust Substring
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>production ready</remarks>
        public static string Substring(string str, int startIndex, int length)
        {
            if (str == null)                         return "".PadRight(length);
            if      (str.Length > startIndex+length) return str.Substring(startIndex,length);
            else if (str.Length > startIndex       ) return str.Substring(startIndex).PadRight(length);
            else                                     return "".PadRight(length);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TitleCase -->
        /// <summary>
        ///      Grabbed off the Internet
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>does not always work</remarks>
        public static string TitleCase(string str)
        {
            string title = "";   
            try
            {
                System.Globalization.CultureInfo cultureInfo =    
                    System.Threading.Thread.CurrentThread.CurrentCulture;   
                System.Globalization.TextInfo TextInfo = cultureInfo.TextInfo;   
                title = TextInfo.ToTitleCase(str);   
            }   
            catch  
            {   
                title = str;   
            }   
            return title;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UTF8ByteArrayToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }


        public static string SerializeToXml<T>(T obj)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
            catch { throw; }
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xml);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch { throw; }
        }



        // ----------------------------------------------------------------------------------------
        /// <!-- BreakPoint -->
        /// <summary>
        ///      This is a no-op method to be used to provide a place to set breaks to
        /// </summary>
        public static void BreakPoint()
        {
        }


        public static void Pause()
        {
        }
    }
}
