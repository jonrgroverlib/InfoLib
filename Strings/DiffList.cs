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
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DiffList -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class DiffList
    {
        // ----------------------------------------------------------------------------------------
        ///  Members
        // ----------------------------------------------------------------------------------------
        private List<DiffItem> _diff;
        public string Parent1  { get { return _parent1; } set { _parent1 = value; } } private string _parent1;   ///  the location of 1
        public string Parent2  { get { return _parent2; } set { _parent2 = value; } } private string _parent2;   ///  the location of 2


        // ----------------------------------------------------------------------------------------
        ///  Constructors
        // ----------------------------------------------------------------------------------------
        public DiffList(                     string loc1, string loc2) { _diff = new List<DiffItem>();         _parent1 = loc1; _parent2 = loc2; }
        public DiffList(int capacity,        string loc1, string loc2) { _diff = new List<DiffItem>(capacity); _parent1 = loc1; _parent2 = loc2; }
        public DiffList(List<DiffItem> diff, string loc1, string loc2) { _diff = diff;                         _parent1 = loc1; _parent2 = loc2; }


        // ----------------------------------------------------------------------------------------
        ///  Pass-through methods and properties
        // ----------------------------------------------------------------------------------------
        public DiffItem this[int i]  { get { return _diff[i];       } }
        public int          Count    { get { return _diff.Count;    } }
        public void         RemoveAt(int index) { _diff.RemoveAt(index); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(DiffItem item)
        {
            _diff.Add(item);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ChangeCount -->
        /// <summary>
        ///      Returns number of changes in diff list (vs number of non-changes)
        /// </summary>
        public int ChangeCount { get
        {
            int changes = 0;
            int count = _diff.Count;
            for (int i = 0; i < count; ++i)
                if (_diff[i].IsChange) ++changes;
            return changes;
        } }


    }
}
