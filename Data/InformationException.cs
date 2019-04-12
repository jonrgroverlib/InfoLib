// ------------------------------------------------------------------------------------------------
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
// ------------------------------------------------------------------------------------------------
using System; // for Exception

namespace InfoLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- InformationException -->
    /// <summary>
    ///      Exception on converting data to information (or converting information to data, or working with information)
    /// </summary>
    public class InformationException : Exception
    {
        private const string _defaultMessage = "Exception on converting data to information.";

        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public InformationException(                           ) : base(_defaultMessage       ) { }
        public InformationException(string msg                 ) : base(msg                   ) { }
        public InformationException(string msg, Exception inner) : base(msg            , inner) { }
        public InformationException(            Exception inner) : base(_defaultMessage, inner) { }
    }
}
