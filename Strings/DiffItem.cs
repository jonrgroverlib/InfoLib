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
    /// <!-- DiffListItem -->
    /// <summary>
    ///      The DiffListItem class describes a particular change in a list diff results list
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class DiffItem : IDiffItem
    {
        // ----------------------------------------------------------------------------------------
        ///  Members
        // ----------------------------------------------------------------------------------------
        private string _str;
        private int    _change;  /// -1 remove, 0 nochange, +1 add
        private string _parent1;
        private string _parent2;


        // ----------------------------------------------------------------------------------------
        ///  Constructor
        // ----------------------------------------------------------------------------------------
        public DiffItem(string str, int change, string parent1, string parent2)
        {
            _str     = str;
            _change  = change;
            _parent1 = "";
            _parent2 = "";
        }


        // ----------------------------------------------------------------------------------------
        ///  Simple method properties
        // ----------------------------------------------------------------------------------------
        public string Str     { get { return _str;     } }  /// <summary>-1 remove, 0 nochange, +1 add</summary>
        public int    Change  { get { return _change;  } }
        public string Parent1 { get { return _parent1; } set { _parent1 = value; } }
        public string Parent2 { get { return _parent2; } set { _parent2 = value; } }
        public string Parent(int x) { if (x == 1) return _parent1; else return _parent2; }


        public int Direction(int x)
        {
            if      (x == 1) return -1;
            else if (x == 2) return 1;
            else throw new IndexOutOfRangeException("the operands in a diff are labeled either 1 or 2");
        }


        // ----------------------------------------------------------------------------------------
        ///  Properties
        // ----------------------------------------------------------------------------------------
        public bool   IsChange { get { if (_change == 0) return false; else return true; } }
        public string Token    { get
        {
            if      (_change == 0) return "[0]";
            else if (_change == 1) return "[+]";
            else                   return "[-]";
        } }


        // ----------------------------------------------------------------------------------------
        ///  For debugging
        // ----------------------------------------------------------------------------------------
        public override string ToString() { return Token + _str; }


    }
}
