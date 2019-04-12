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
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- LoadField -->
    /// <summary>
    ///      The LoadField class maps a field to a column in the database
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class LoadField
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
/*SF*/  public int    StoreField { get; set; } // which items to store
/*.P*/  public string Param      { get; set; } // parameters to be used
/*SC*/  public string Column     { get; set; } // database storage name
/*.L*/  public int    Length     { get; set; } // maximum length of string in this column
/*ST*/  public string InTable    { get; set; } // the table to store the field into
        public string DataType   { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public LoadField()
        {
            InTable    = "";
            Column     = "";
            StoreField =  2;
            Param      = "";
            Length     = 10;
        }

        public LoadField(string tableName, string columnName, int field, string param, string dataType, int length)
        {
            InTable    = tableName ;
            Column     = columnName;
            StoreField = field     ;
            Param      = param     ;
            Length     = length    ;
            DataType   = dataType  ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StoreField.ToString() + " " + Column + "(" + InTable + ")" + " " + Length + " " + Param;
        }
    }
}
