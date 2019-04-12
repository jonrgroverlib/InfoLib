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
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DiffListLcs -->
    /// <summary>
    ///      The DiffListLcs class finds the longest common sub-sequence (not substring) of two
    ///      Lists of strings and then the diff of the two lists
    /// </summary>
    /// <remarks>
    ///      'Equals' must be defined for the objects in the list
    /// </remarks>
    /// <remarks>junk, I think</remarks>
    public class DiffListLcs
    {
        // ----------------------------------------------------------------------------------------
        ///  Members
        // ----------------------------------------------------------------------------------------
        public int    Length1 { get { return _list1.Count; } } private List<string> _list1;
        public int    Length2 { get { return _list2.Count; } } private List<string> _list2;
        public int[,] Table   { get { return _table;       } } private int[,]       _table;


        public int this[int i, int j] { get { return _table[i,j]; } }
        public int Length { get
        {
            int len1 = Length1;
            int len2 = Length2;
            if (len1 == 0 || len2 == 0) return 0;
            else return _table[len1, len2];
        } }


        // ----------------------------------------------------------------------------------------
        ///  Constructor
        // ----------------------------------------------------------------------------------------
        public DiffListLcs(List<string> list1, List<string> list2)
        {
            if (list1 == null) throw new NullReferenceException("list 1 is null in LcsList constructor");
            if (list2 == null) throw new NullReferenceException("list 2 is null in LcsList constructor");
            //if (list1.Count > 0 && list2.Count > 0 && list1[0].Equals(list2[0]))
            //    return;
            _list1 = list1;
            _list2 = list2;
            _table = Init();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int[,] Init()
        {
            // -------------------------------------------------------------------------90
            ///  Initialize LCS table
            // -------------------------------------------------------------------------90
            int len1 = Length1 + 1;
            int len2 = Length2 + 1;
            int[,] table = new int[len1, len2];
            for (int i = 0; i < len1; i++) table[i, 0] = 0;
            for (int j = 0; j < len2; j++) table[0, j] = 0;


            // -------------------------------------------------------------------------90
            ///  Fill LCS table
            // -------------------------------------------------------------------------90
            for (int i = 1; i < len1; i++) for (int j = 1; j < len2; j++)
                if      (_list1[i-1].Equals(_list2[j-1])) table[i, j] = table[i-1, j-1] + 1;
                else if (table[i, j-1] > table[i - 1, j]) table[i, j] = table[i, j-1];
                     else                                 table[i, j] = table[i-1, j];
            return table;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- GetLCS -->
        /// <summary>
        ///      Returns the longest common subsequence (not substring) of two strings
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static DiffListLcs GetLCS(List<string> list1, List<string> list2)
        {
            DiffListLcs lcs = new DiffListLcs(list1, list2);
            return lcs;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int len1 = 1 + Math.Min(Length1, 32);
            int len2 = 1 + Math.Min(Length2, 32);
            StringBuilder str = new StringBuilder(3*len1*len2);
            string rowDelim = "\r\n";
            string colDelim = " ";
            int col = 2;


            // -------------------------------------------------------------------------90
            ///  Fill table header
            // -------------------------------------------------------------------------90
            str.Append(" ");
            for (int j = 1; j < len2; j++)
                str.Append(colDelim + _list2[j-1].SetToLength(col));
            if (Length2 > len2)  str.Append("...");


            // -------------------------------------------------------------------------90
            ///  Fill table
            // -------------------------------------------------------------------------90
            for (int i = 1; i < len1; i++)
            {
                str.Append(rowDelim + _list1[i-1].SetToLength(col));
                for (int j = 1; j < len2; j++)
                    str.Append(colDelim + _table[i,j].ToString().PadLeft(col));
            }


            return str.ToString();
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Sequence -->
        /// <summary>
        ///      Goes through the specified list and returns what needs to be changed
        /// </summary>
        /// <returns></returns>
        public List<string> CommonSequence()
        {
            List<string> seq = new List<string>();
            if (Length == 0)
                return seq;
            else
            {
                string str   = ToString();
                int    i     = _table.GetLength(0) - 1;
                int    j     = _table.GetLength(1) - 1;
                int    steps = 0;


                for (int k = Length; k > 0; ++steps)
                {
                    int up   = _table[i - 1, j];
                    int left = _table[i, j - 1];
                    int here = _table[i, j];

                    ///  Note: multiple sequences occur when both 'up' == here and 'left' == here
                    if (up == here)                i--;
                    else if (left == here)              j--;
                    else { seq.Add(_list1[i - 1]); i--; j--; k--; }
                }
                seq.Reverse();
                return seq;
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- AddListForArray -->
        /// <summary>
        ///      Goes through the specified list and returns what needs to be removed to change to
        ///      the sequence
        /// </summary>
        /// <param name="num">Enter a 1 or a 2 for list1 or list2</param>
        /// <returns></returns>
        public List<string> AddListForArray(int num)
        {
            List<string> commonSeq = CommonSequence();
            int q = 0;
            int len; if (num == 1) len = Length2; else len = Length1;
            List<string> half = new List<string>(len - commonSeq.Count);


            if (num == 1) /// first list
                for (int s = 0; s < len; ++s)
                    if (q < commonSeq.Count && _list2[s] == commonSeq[q]) q++; else half.Add(_list2[s]);
            else          /// second list
                for (int s = 0; s < len; ++s)
                    if (q < commonSeq.Count && _list1[s] == commonSeq[q]) q++; else half.Add(_list1[s]);
            return half;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Diff -->
        /// <summary>
        ///      Returns the diff results for two lists of strings
        /// </summary>
        /// <returns></returns>
        public DiffList Diff()
        {
            List<string> comSeq = CommonSequence();
            int len; len = Length1 + Length2 - comSeq.Count;
            //List<DiffListItem> diff = new List<DiffListItem>(len);
            DiffList diff = new DiffList(len, "1", "2");
            int c1 = 0;
            int cc = 0;
            int c2 = 0;
            string eol = "nUll"; /// end of list


            int count = comSeq.Count;
            for (int t = 0; cc <= count; ++t)
            {
                string common;
                string next_1;
                string next_2;
                bool   equal_1;
                bool   equal_2;
                if (cc == comSeq.Count) common = eol; else common = comSeq[cc];
                if (c1 == _list1.Count) next_1 = eol; else next_1 = _list1[c1];
                if (c2 == _list2.Count) next_2 = eol; else next_2 = _list2[c2];
                equal_1 = (next_1 == common);
                equal_2 = (next_2 == common);
                if      ( equal_1 &&  equal_2) { diff.Add(new DiffItem(common,  0, "", "")); ++c1; ++cc; ++c2; }
                else if ( equal_1 && !equal_2) { diff.Add(new DiffItem(next_2,  1, "", ""));             ++c2; }
                else if (!equal_1)             { diff.Add(new DiffItem(next_1, -1, "", "")); ++c1;             }
            }


            if (diff[diff.Count-1].Str == eol)  diff.RemoveAt(diff.Count-1);


            //DiffList list = new DiffList();
            //foreach (DiffListItem item in diff)
            //{
            //    list.Add(item);
            //}
            return diff;
        }


    }
}
