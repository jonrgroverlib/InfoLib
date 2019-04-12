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
using System.Collections.Generic;     // for Dictionary
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ExtractField -->
    /// <summary>
    ///      The ExtractField class maps a column in the import file to a column in the database
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class ExtractField
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
/*IC*/  public string Column         { get; set; } // import file column name
/*IF*/  public int    ImportField    { get; set; } // which items to import
/*IO*/  public int    ImportOrder    { get; set; } // the order the import columns are in (contains internal keys)
/*.L*/  public int    Length         { get; set; } // for validation
/*IP*/  public string Pattern        { get; set; } // for validation
/*I.*/  public string DataType       { get; set; } // data type groups like "string", "int", "date" etc.


/*EN*/  public string ErrorFieldName { get; set; } // contains error code For ColumnNameNotFound
/*ER*/  public string ErrorRequired  { get; set; } // contains error code For RequiredDataMissing
/*EF*/  public string ErrorFormat    { get; set; } // contains error code For ValueIsNotAnInteger or of the right format
/*EL*/  public string ErrorLookup    { get; set; } // contains error code For ValueNotFoundInXref
/*EL*/  public string ErrorLength    { get; set; } // contains error code For data length too long


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public ExtractField()
        {
            Column      = ""  ;
            ImportField = 2   ;
            ImportOrder = 0   ;
            Pattern     = ".*";
            Length      = 100 ;


            ErrorFieldName = "005";
            ErrorRequired  = "006"; 
            ErrorFormat    = "007";
            ErrorLookup    = "008";
            ErrorLength    = "009";
        }

        //  TODO: replace this with the one below
        public ExtractField(string column, int field, int order, string pattern, string dataType, int length
            , string fieldError, string requiredError, string formatError, string lookupError)
        {
            Column      = column  ;
            ImportField = field   ;
            ImportOrder = order   ;
            Pattern     = pattern ;
            Length      = length  ;
            DataType    = dataType;


            ErrorFieldName = fieldError   ;
            ErrorRequired  = requiredError; 
            ErrorFormat    = formatError  ;
            ErrorLookup    = lookupError  ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExtractField constructor -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"       >column name                          </param>
        /// <param name="fieldStatus"  >0:not present, 1:optional, 2:required</param>
        /// <param name="order"        >order in csv file                    </param>
        /// <param name="pattern"      >validation pattern                   </param>
        /// <param name="dataType"     >recommend: int, bit, date, string</param>
        /// <param name="length"       >maximum length</param>
        /// <param name="fieldError"   >error code if column name is bad</param>
        /// <param name="requiredError">error code if required data is not present</param>
        /// <param name="formatError"  >error code if the format or type is bad</param>
        /// <param name="lookupError"  >error code if the value does not exists in the lookup resource</param>
        /// <param name="lengthError"  >error code if the data is too long</param>
        public ExtractField(string column, int fieldStatus, int order, string pattern, string dataType, int length
            , string fieldError, string requiredError, string formatError, string lookupError, string lengthError)
        {
            Column         = column       ;
            ImportField    = fieldStatus  ;
            ImportOrder    = order        ;
            Pattern        = pattern      ;
            Length         = length       ;
            DataType       = dataType     ;


            ErrorFieldName = fieldError   ;
            ErrorRequired  = requiredError; 
            ErrorFormat    = formatError  ;
            ErrorLookup    = lookupError  ;
            ErrorLength    = lengthError  ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnCount -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExtractMap"></param>
        /// <returns></returns>
        public static int ColumnCount(Dictionary<string, ExtractField> ExtractMap)
        {
            int count = 0;
            foreach (string key in ExtractMap.Keys)
            {
                int value = ExtractMap[key].ImportField;
                if (value > 0) ++count;
            }
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnCount -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ImportField"></param>
        /// <returns></returns>
        public static int ColumnCount(Dictionary<string,int> ImportField)
        {
            int count = 0;
            foreach (int value in ImportField.Values)
                if (value > 0) ++count;
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HeaderLine -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ImportOrder"></param>
        /// <param name="ImportField"></param>
        /// <param name="ImportColumn"></param>
        /// <returns></returns>
        public static string HeaderLine(List<string> ImportOrder, Dictionary<string, int> ImportField, Dictionary<string, string> ImportColumn)
        {
            string str   = "";
            string delim = "";

            for (int i = 0; i < ImportOrder.Count; ++i)
            {
                string key = ImportOrder[i];
                if (ImportField[key] > 0)
                {
                    str += delim + ImportColumn[key];
                    delim = ",";
                }
            }

            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Column + "(" + ImportOrder + ")" + " " + Length + " " + Pattern;
        }
    }
}
