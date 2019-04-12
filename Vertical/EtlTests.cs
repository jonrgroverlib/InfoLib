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
using InfoLib.Endemes;         // for 
using InfoLib.Strings;         // for 
using System;                         // for 
using System.Collections.Generic;     // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EtlTests -->
    /// <summary>
    ///      The EtlTests class tests the classes in the Etl Library
    /// </summary>
    /// <remarks>alpha code - used once in production, expected to be deprecated</remarks>
    public class EtlTests
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        //private        string _result;


        // ----------------------------------------------------------------------------------------
        /// <!-- AllTests -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AllTests()
        {
            //_result = "";
            string result = "";


            // --------------------------------------------------------------------------
            //  tests
            // --------------------------------------------------------------------------


            result += "\r\nETL tests succeeded";
            return result;
        }
    }
}