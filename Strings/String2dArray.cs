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
    // ------------------------------------------------------------------------------------------
    /// <!-- String2dArray -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class String2dArray
    {
        // --------------------------------------------------------------------------------------
        //  Members
        // --------------------------------------------------------------------------------------
        private string[,] _array;             // [height,width]
        public  int    Height                 { get { return _array.GetUpperBound(0) + 1; } }
        public  int    Width                  { get { return _array.GetUpperBound(1) + 1; } }
        public  string this[int row, int col] { get { return _array[row,col]; } set { _array[row,col] = value; } }


        // --------------------------------------------------------------------------------------
        //  Constructor
        // --------------------------------------------------------------------------------------
        public String2dArray(int height, int width)
        {
            _array = new string[height,width];
        }


        // --------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Turns the ASCII cell map into an ASCII string to be displayed on the UI
        /// </summary>
        /// <returns></returns>
        public string Compose()
        {
            StringBuilder str = new StringBuilder();
            str.Append("+");
            for (int i = 0; i < _array.GetUpperBound(1) + 1; ++i)
                str.Append("---+");
            str.Append("\r\n");
            string lineDelim = "";
            for (int i = 0; i < _array.GetUpperBound(0)+1; ++i)
            {
                str.Append(lineDelim + "|" + ComposeA(i));
                lineDelim = "\r\n";
                str.Append(lineDelim + "|" + ComposeB(i));
                str.Append(lineDelim + "+" + ComposeC(i));
            }

            return str.ToString();
        }


        // --------------------------------------------------------------------------------------
        /// <!-- Compose -->
        /// <summary>
        ///      Composes one line
        /// </summary>
        /// <param name="_array"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string ComposeA(int line)
        {
            StringBuilder str = new StringBuilder();
            for (int j = 0; j < Width; ++j) str.Append(_array[line,j].Substring(0,4));
            return str.ToString();
        }
        private string ComposeB(int line)
        {
            StringBuilder str = new StringBuilder();
            for (int j = 0; j < Width; ++j) str.Append(_array[line,j].Substring(4,4));
            return str.ToString();
        }
        private string ComposeC(int line)
        {
            StringBuilder str = new StringBuilder();
            for (int j = 0; j < Width; ++j) str.Append(_array[line,j].Substring(8,4));
            return str.ToString();
        }
    }
}
