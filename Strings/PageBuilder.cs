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
    /// <!-- PageBuilder -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha or beta code</remarks>
    public class PageBuilder
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        private List<string> _line;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public PageBuilder()
        {
            _line = new List<string>();
        }
        public PageBuilder(int width, int height)
        {
            _line = new List<string>(height);
            for (int i = 0; i < height; ++i)
                _line.Add("".PadRight(width));
        }
        public PageBuilder(string output)
        {
            output = Regex.Replace(output, "\r\n\r\n", "\r\n \r\n", RegexOptions.Singleline);
            string[] line = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int height = line.Length;
            int width = 0;
            for (int i = 0; i < height; ++i)
                width = Math.Max(width, line[i].Length);


            _line = new List<string>(height);
            for (int i = 0; i < height; ++i)
                _line.Add(line[i].PadRight(width));
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public int Height { get { return _line.Count; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- AddLine -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(string line)
        {
            _line.Add(line);
            // TODO: bulk out
        }

        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Put -->
        /// <summary>
        ///      Sets the value of a character in a page
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        public void Put(int line, int col, char value)
        {
            if (line >= _line.Count)
            {
                for (int i = _line.Count; i <= line; ++i)
                    _line.Add("");
            }


            string ln = _line[line];
            if (col < ln.Length)
            {
                string afterValue = "";
                if (col+1 < ln.Length)
                    afterValue = ln.Substring(col+1);
                _line[line] = ln.Substring(0,col) + value + afterValue;
            }
            else
            {
                _line[line] = ln.PadRight(col) + value;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Put -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="page"></param>
        public void Put(int line, int col, PageBuilder page)
        {
            int width = page.Width;
            int height = page.Height;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Put(line + j, col + i, page._line[j].ToCharArray()[i]);
                }
            }
        }

        public void Put(int line, int col, string output)
        {
            PageBuilder page = new PageBuilder(output);
            Put(line, col, page);
        }


        public void PutHorzontalLine(int line                         , char value) { for (int col  = 0; col  < Width ; ++col ) Put(line, col, value);        }
        public void PutVerticalLine (int col                          , char value) { for (int line = 0; line < Height; ++line) Put(line, col, value);        }
        public void PutVerticalLine (int col, int fromLine, int toLine, char value) { for (int j = fromLine; j < Math.Min(Height,toLine); ++j) Put(j, col, value); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            string delim = "";
            for (int i = 0; i < _line.Count; ++i)
            {
                str.Append(delim + _line[i]);
                delim = "\r\n";
            }
            return str.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Width -->
        /// <summary>
        /// 
        /// </summary>
        public int Width { get
        {
            int width = 0;
            for (int i = 0; i < _line.Count; ++i)
            {
                width = Math.Max(width, _line[i].Length);
            }
            return width;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- WrapStringLoose -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxRightMargin"></param>
        /// <returns></returns>
        public static List<string> WrapStringLoose(string str, int width, int height, int maxRightMargin)
        {
            List<string> list = new List<string>();
            int area = width*height;
            char[] cha = str.Trim().PadRight(area).ToCharArray();
            int row = 0;


            for (row = 0; row < height; ++row) list.Add("");
            row = 0;
            for (int c = 0; c < area && row < height; ++c)
            {
                // ----------------------------------------------------------------------
                //  Add the character somewhere
                // ----------------------------------------------------------------------
                char ch = cha[c];
                if (list[row].Length != 0 || ch != ' ')
                {
                    list[row] += ch;
                    if      (ch == ' ' && list[row].Length > width - maxRightMargin - 1)
                                                        { if (list[row].Length > width) { Pause(); } row++; }
                    else if (list[row].Length >= width) { if (list[row].Length > width) { Pause(); } row++; }
                    else                                { if (list[row].Length > width) { Pause(); }        }
                }
            }

            for (row = 0; row < height; ++row) list[row] = list[row].PadRight(width, ' ');

            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WrapStringMedium -->
        /// <summary>
        ///      Wraps a string into an area on a page
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxRightMargin">the lower, the tighter, the larger, the looser</param>
        /// <returns>This one works the best</returns>
        public static List<string> WrapStringMedium(string str, int width, int height, int maxRightMargin)
        {
            List<string> list = new List<string>();
            int area = width*height;
            char[] cha = str.Trim().PadRight(area).ToCharArray();
            int row = 0;


            for (row = 0; row < height; ++row) list.Add("");
            row = 0;
            string word = "";
            for (int c = 0; c < area && row < height; ++c)
            {
                // ----------------------------------------------------------------------
                //  Add the character somewhere
                // ----------------------------------------------------------------------
                char ch = cha[c];
                if (ch != ' ') { word += ch; }
                else
                {
                    string pair = (list[row] + " " + word).Trim();
                    if (list[row].Length < width - maxRightMargin && pair.Length > width && word.Length > maxRightMargin*2) { list[row] = pair.Substring(0, width); row++; if (row < height) list[row] = pair.Substring(width); }
                    else if (pair.Length > width)                     { row++; if (row < height) { if (word.Length > width) { list[row] = word.Substring(0, width); row++; if (row < height) list[row] = word.Substring(width); }
                                                                                                   else                     { list[row] = word;                                                                                 } } }
                    else                                                                                                    { list[row] = pair;                                                                                 }
                    word = "";
                }
            }

            for (row = 0; row < height; ++row) list[row] = list[row].PadRight(width, ' ');
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- WrapStringTight -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static List<string> WrapStringTight(string str, int width, int height)
        {
            List<string> list = new List<string>();
            int count = list.Count;
            list.Add(str);


            for (int i = 1; i < height; ++i)
            {
                count = list.Count;
                list.Add((list[count-1]+" ".PadRight(width+1)).Substring(width));
                list[count] = Regex.Replace(list[count], "^ ", "");
            }

            for (int i = 0; i < height; ++i) { list[i] = __.SetLength(width, list[i]); }
            return list;
        }

        public void AddCorners()
        {
            for (int line = 0; line < Height; ++line)
            {
                for (int col = 0; col < Width; ++col)
                {
                    if (line > 0        && col > 0       && _line[line - 1].Substring(col, 1) == "|" && _line[line].Substring(col-1, 1) == "-") Put(line, col, '+');
                    if (line < Height-1 && col > 0       && _line[line + 1].Substring(col, 1) == "|" && _line[line].Substring(col-1, 1) == "-") Put(line, col, '+');
                    if (line > 0        && col < Width-1 && _line[line - 1].Substring(col, 1) == "|" && _line[line].Substring(col+1, 1) == "-") Put(line, col, '+');
                    if (line < Height-1 && col < Width-1 && _line[line + 1].Substring(col, 1) == "|" && _line[line].Substring(col+1, 1) == "-") Put(line, col, '+');
                }
            }
        }
    }
}
