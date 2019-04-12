//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify it under the terms 
// of the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with InfoLib.
// If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System; // for Exception

namespace InfoLib.HardData
{
	// --------------------------------------------------------------------------------------------
	/// <!-- RelationalReferenceException -->
    /// <summary>
    ///      Use this exception when a foreign key points to a nonexistent row, or a row is orphaned
    /// </summary>
    /// <remarks>production ready</remarks>
    public class RelationalReferenceException : Exception
    {
        private const string _defaultMessage = "Relational database relationship error";


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public RelationalReferenceException()                            : base(_defaultMessage) { }
        public RelationalReferenceException(string msg)                  : base(msg)             { }
        public RelationalReferenceException(string msg, Exception inner) : base(msg, inner)      { }
    }
}
