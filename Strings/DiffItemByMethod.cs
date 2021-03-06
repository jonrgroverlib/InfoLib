﻿//--------------------------------------------------------------------------------------------------
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
using System;
using System.Text;

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DiffItemByMethod -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class DiffItemByMethod : IDiffItem
    {
        // ----------------------------------------------------------------------------------------
        ///  Members
        // ----------------------------------------------------------------------------------------
        private string _methodName;
        private string _parent1;     ///  in this case, the file path
        private string _parent2;     ///  in this case, the file path
        private DiffItem _item;


        // ----------------------------------------------------------------------------------------
        ///  Constructor
        // ----------------------------------------------------------------------------------------
        public DiffItemByMethod(string methodName, DiffItem item, string filePath1, string filePath2)
        {
            _methodName = methodName;
            _parent1    = filePath1;
            _parent2    = filePath2;
            _item       = item;
        }


        // ----------------------------------------------------------------------------------------
        ///  Simple methods and properties
        // ----------------------------------------------------------------------------------------
        public int    Change        { get { return _item.Change; } }  /// -1 remove, 0 nochange, +1 add
        public string Str           { get { return _methodName;  } }  /// essentially the name of the file
        public string Parent1       { get { return _parent1;     } set { _parent1 = value; } } 
        public string Parent2       { get { return _parent2;     } set { _parent2 = value; } }
        public string Parent(int x) { if (x == 1) return _parent1; else return _parent2; }


        public override string ToString()
        {
            if (_item.Change <= 0)
                return _item.Token + _parent1 + '\\' + _methodName;
            else return _item.Token + _parent2 + '\\' + _methodName;
        }


        public int Direction(int x)
        {
            if      (x == 1) return -1;
            else if (x == 2) return 1;
            else throw new IndexOutOfRangeException("the operands in a diff are labeled either 1 or 2");
        }


    }
}
