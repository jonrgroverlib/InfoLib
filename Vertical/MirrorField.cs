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
using System.Collections.Generic;     // for Dictionary
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- MirrorField -->
    /// <summary>
    ///      The MirrorField class maps a field to a column in the database
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class MirrorField
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        //  ABCDEFGHIJKLMNOPQRSTUV|
        //  ----------------------+
        //    C         M     ST  |S. Copy/Staging/Mirror/Temp X
        //    C  F       N        |C. Column (field name)
        //      E                 |E. Error
        //       F           R    |F. Field requirement
        //          I             |I. Import X
        //             L      S   |L. Length (size)
        //              M         |M. Mapping
        //                O       |O. Order
        //       F         P      |P. Pattern (format)
        //             L     R    |R. xRef (lookup)
        //                    S   |S. Storage X
        //       F             T  |T. Table (or file)
        //                 P     V|V. Param/Variable
        //  ----------------------+
        //  ABCDEFGHIJKLMNOPQRSTUV|
        // ----------------------------------------------------------------------------------------
        public string Column   { get; set; } // the database 'mirroring' (or staging) table copy of the items actually imported
/*.P*/  public string Param    { get; set; } // the parameters used for mirroring (or staging) the import
/*.L*/  public int    Length   { get; set; } // how much of the input can be stored and how much must be truncated (if mirrored) or a validation issue (if staged)
        public bool   Active   { get; set; } // does this field get mirrored?
        public string DataType { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public MirrorField()
        {
            Column = ""   ;
            Param  = ""   ;
            Length = 100  ;
            Active = false;
        }

        public MirrorField(string column, string param, bool active, string dataType, int length)
        {
            Column   = column  ;
            Param    = param   ;
            Length   = length  ;
            Active   = active  ;
            DataType = dataType;
        }
    }
}
