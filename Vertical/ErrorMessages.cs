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
using InfoLib.Data    ;               // for RichDataTable
using InfoLib.SoftData;               // for TreatAs
using InfoLib.Strings ;               // for __
using System;                         // for 
using System.Collections.Generic;     // for List
using System.ComponentModel;          // for Win32Exception
using System.Data;                    // for DataColumnCollection
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ErrorMessages -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code</remarks>
    public static class ErrorMessages
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- UnpackErrorMessages -->
        /// <summary>
        ///      Extracts the messages, sources and types from an exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Moved from GlobalConstants
        ///
        ///      beta code
        /// </remarks>
        public static string Unpack(Exception ex)
        {
            string err = "";
            if (ex != null)
            {
                Exception e = ex;


                // ----------------------------------------------------------------------
                //  Interpretation header
                // ----------------------------------------------------------------------
                Type exType = ex.GetType();
                //  Start by saying 'you did nothing wrong' in case this ever reaches a user
                //string intro = "You did nothing wrong, ";
                string intro = "";
                string cr = "\r\n";


                switch (exType.Name)
                {
                    case "ActivationException":
                        err = intro + "the program or data has experienced an anomaly masked by a web configuration problem:";
                        err += cr + " ---> " + ex.Message;
                        break;
                    case "SqlException":
                        err = intro + "an internal sql command has experienced an anomaly when run on the database:";
                        err += cr + " ---> " + ex.Message;
                        break;
                    default:
                        err = intro + "anomaly message: " + ex.Message;
                        break;
                }


                // ----------------------------------------------------------------------
                //  Standard header
                // ----------------------------------------------------------------------
                err += cr;
                int width = 120;


                // ----------------------------------------------------------------------
                //  Unpack the exception and inner exception messages
                // ----------------------------------------------------------------------
                string delim = "";
                while (ex != null)
                {
                    RichDataTable dt = new RichDataTable(ex, "Exception", false, ""); dt.Add(ex);
                    exType = ex.GetType();


                    err += cr + "+-" + "---------------------------------------".PadRight(width-3,'-') + "+";
                    err += cr + "| " + ("Exception Type:        " + exType.Name).PadRight(width-3)     + "|";
                    err += cr + "| " + dt.PrettyFormatRow(0, width-3, "|\r\n| ")                       + "|";
                    err += cr + "+-" + "---------------------------------------".PadRight(width-3,'-') + "+";


                    delim = cr + cr + "  <--" + cr + cr;
                    ex = ex.InnerException;
                }


                // ----------------------------------------------------------------------
                //  Add a nice footer (stacktrace)
                // ----------------------------------------------------------------------
                err += cr + "|".PadRight(width-1)+"|";
                err += cr + "|".PadRight(width-1)+"|";
                err += cr + "|Stack Trace:".PadRight(width-1)+"|";
                err += cr + __.PrettyPrintStackTrace(e.StackTrace, width);
            }
            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <summary>
        ///      Formats a table horizontally with column names across the top and rows vertically
        ///      if horizontal is true (or the opposite if horozontal is false)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="horizontal">if true: columns horizontally, rows vertically</param>
        /// <returns></returns>
        private static string PrettyPrintTable(RichDataTable dt, bool horizontal)
        {
            // go through the columns
            // insert spaces in the headers
            // find the longest column name
            // show the value and the header for those with values
            // print vertically
            return "";
        }
    }
}