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
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- GrayCode -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code</remarks>
    public class GrayCode
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Code -->
        /// <summary>
        ///      The code itself kept in an unsigned long
        /// </summary>
        public ulong Code { get { return _code; } set { _code = value; } }  private ulong _code;


        // ----------------------------------------------------------------------------------------
        //  Constructors                                                */-------------------------
        /* ------------------------------------------------------------*/ /// <summary>sets the Gray Code to zero</summary>
        public GrayCode()            { _code = 0;                      }  /// <summary>calculates Gray Code from a regular number</summary>
        public GrayCode(ulong num)   { _code = CalcCode(num);          }  /// <summary>inputs the Gray Code directly</summary>
        public GrayCode(string code) { _code = Convert.ToUInt64(code); }


        // ----------------------------------------------------------------------------------------
        /// <!-- CalcCode -->
        /// <summary>
        ///      Calculates Gray Code for a number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Approach:
        ///      bits
        ///      = ((n+1)/2)%2
        ///      + ((n+2)/4)%2
        ///      + ((n+4)/8)%2
        ///      + ((n+8)/16)%2
        ///      ...
        ///      =
        ///      + ((n+2^x)/2^(x+1))%2 where x = 0,1,2,3,4...
        ///      
        /// 
        ///      alpha or beta code
        /// </remarks>
        private static ulong CalcCode(ulong num)
        {
            return (num ^ (num >> 1));
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Count -->
        /// <summary>
        ///      Counts bits in Gray Code
        /// </summary>
        /// <remarks>alpha or beta code</remarks>
        public int Count
        {
            get
            {
                ulong code = _code;
                ulong count = 0;
                while (code > 0)
                {
                    count += code & 1;
                    code >>= 1;
                }
                return (int)count;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Decode -->
        /// <summary>
        ///      Returns the original number the Gray Code would have been calculated from
        /// </summary>
        /// <remarks>alpha or beta code</remarks>
        public ulong Decode
        {
            get
            {
                int shift;
                ulong answer, idiv;
                shift = 1;
                answer = _code;
                while (true)
                {
                    idiv = answer >> shift;
                    answer ^= idiv;
                    if (idiv <= 1 || shift == 32)
                        return answer;
                    shift <<= 1; // double number of shifts next time
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HighBit -->
        /// <summary>
        ///      Returns the position of the highest bit or -1 for a zero
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <remarks>alpha or beta code</remarks>
        public static int HighBit(ulong num)
        {
            int count = -1;
            while (num > 0) { count++; num >>= 1; }
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns the Gray Code as a string of bits
        /// </summary>
        /// <returns></returns>
        /// <remarks>alpha or beta code</remarks>
        public override string ToString()
        {
            uint firstPart = (uint)(_code/(ulong)uint.MaxValue);
            string str1 = Convert.ToString(firstPart,2).PadLeft(32, '0');
            string str2 = Convert.ToString((uint)_code,2).PadLeft(32, '0');
            return str1 + str2;
        }

    }
}
