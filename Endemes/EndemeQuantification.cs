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
using System.Collections.Generic;     // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- EndemeQuantification -->
    /// <summary>
    ///      The EndemeQuantification class - When it comes to endemes numbers are not usually important but sometimes they are
    /// </summary>
    /// <remarks>production ready</remarks>
    public class EndemeQuantification
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public Dictionary<char, double> Raw   { get; set; } // you can dump raw values into here and use them to build the endeme later
        public Dictionary<char, double> Calc  { get; set; } // you can build values here from the endeme


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeQuantification()
        {
            Raw = new Dictionary<char, double>(26);
            for (char c = 'A'; c <= 'Z'; ++c) Raw.Add(c, 0);
            Calc = new Dictionary<char, double>(26);
            for (char c = 'A'; c <= 'Z'; ++c) Calc.Add(c, 0);
        }


        // ----------------------------------------------------------------------------------------
        //  Properties
        // ----------------------------------------------------------------------------------------
        public IEnumerable<char> RawKeys      { get { return Raw.Keys  ; } }
        public IEnumerable<char> CalcKeys     { get { return Calc.Keys ; } }
        public string            AsciiDisplay { get { return ToString(); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = "Raw:";
            foreach (char c in Raw.Keys) { str += c + ")" + (int)(Raw[c]*10)/10 + ","; }
            str += " Calc:";
            foreach (char c in Calc.Keys) { str += c + ")" + (int)(Calc[c]*10)/10 + ","; }
            return str;
        }

    }
}
