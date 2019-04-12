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

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- StringLookup -->
    /// <summary>
    ///      The StringLookup class implements a cross reference between values in a lookup
    ///      table for database with values in another database having the same meaning
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class StringLookup
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        Dictionary<string,string> _xref;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public string this[string x] { get { if (ContainsKey(x)) return _xref[x]; else return ""; } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public StringLookup(int capacity)
        {
            _xref = new Dictionary<string, string>(capacity);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a new item to the xref in a threadsafe manner
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            if (!_xref.ContainsKey(key)) try { _xref.Add(key, value); } catch { }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsKey -->
        /// <summary>
        ///      Wraps the ContainsKey(string) method
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _xref.ContainsKey(key);
        }

        public Dictionary<string,string>.KeyCollection Keys { get { return _xref.Keys; } }

        public IEnumerable<string> Values { get { return _xref.Values; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Wraps ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
