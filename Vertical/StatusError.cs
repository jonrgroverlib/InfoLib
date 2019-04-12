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
using InfoLib.Endemes ;        // for 
using InfoLib.HardData;        // for InData
using System;                         // for Exception
using System.Collections.Generic;     // for Dictionary
using System.Data;                    // for DataColumn, DataRow, DataTable
using System.Data.SqlClient;          // for SqlCommand, SqlDataReader
using System.Data.SqlTypes;           // for SqlInt32
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- StatusError -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class StatusError
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- CalculateImportStatus -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="statusErrorLog"></param>
        /// <returns></returns>
        internal static string CalculateImportStatus(DataRow field, DataTable statusErrorLog, InfoAspect aspect)
        {
            string importStatus = "Not Imported";

            try
            {
                string    fileLineNumber  = field["FileLineNumber"].ToString();
                DataRow[] statusErrorRow  = statusErrorLog.Select("FileLineNumber = '" + fileLineNumber + "'");


                if (statusErrorRow.Length > 0)
                {
                    string    statusErrorCode = (string)statusErrorRow[0]["Status_Error_Code"];
                    SqlBoolean isUpdate = InData.GetSqlBoolean(field, "IsUpdate");
                    if (isUpdate) { if (Regex.IsMatch(statusErrorCode, "00$")) throw new DataMisalignedException("Conflicting information about row update vs insert."); }
                    else          { if (Regex.IsMatch(statusErrorCode, "01$")) throw new DataMisalignedException("Conflicting information about row update vs insert."); }


                    if (Regex.IsMatch(statusErrorCode, "00$"    )) importStatus = "Inserted";
                    if (Regex.IsMatch(statusErrorCode, "01$"    )) importStatus = "Updated" ;
                    if (Regex.IsMatch(statusErrorCode, "35[01]$")) importStatus = "Imported";
                }
            }             
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return importStatus;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks for an error in a numeric value field and returns an error code
        ///      if an error is found
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        /// <param name="columnErrorCode"></param>
        /// <param name="nullErrorCode">null or blank but required</param>
        /// <param name="typeErrorCode">not a number</param>
        /// <param name="rangeErrorCode">out of range</param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName, long lo, long hi
            , string columnErrorCode, string nullErrorCode, string typeErrorCode, string rangeErrorCode)
        {
            InfoAspect.Measuring("CheckForError(9)");

            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))                             // Check column name existence
            {
                SqlInt64 num = InData.GetSqlInt64(field, fieldName);
                if (num.IsNull)                                                      // Check numeric type validity
                {
                    string strScore = field[fieldName].ToString();
                    if (string.IsNullOrEmpty(strScore))
                    {
                        if (required >= 2) { errorCode = nullErrorCode; }            // Check required data existence
                    }
                    else { errorCode = typeErrorCode; }
                }
                else { if (num < lo || num > hi) { errorCode = rangeErrorCode; } }   // Check data range validity
            }
            else { errorCode = columnErrorCode; }

            InfoAspect.Measured("CheckForError(9)");
            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks for an error in a numeric code field and returns an error code
        ///      if there is an error detected, otherwise returns blank
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="options"></param>
        /// <param name="columnErrorCode"></param>
        /// <param name="nullErrorCode">null or blank but required</param>
        /// <param name="typeErrorCode">not a number</param>
        /// <param name="optionErrorCode">not in list</param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName, Dictionary<int, int> options
            , string columnErrorCode, string nullErrorCode, string typeErrorCode, string optionErrorCode)
        {
            InfoAspect.Measuring("CheckForError(8a)");

            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))                                  // Check column name existence
            {
                SqlInt32 num = InData.GetSqlInt32(field, fieldName);
                if (num.IsNull)                                                         // Check numeric type validity
                {
                    string strScore = field[fieldName].ToString();
                    if (string.IsNullOrEmpty(strScore))
                    {
                        if (required >= 2)
                         { errorCode = nullErrorCode; }   // Check required data existence
                    }
                    else { errorCode = typeErrorCode; }
                }
                else { if (!options.ContainsKey((int)num))
                    { errorCode = optionErrorCode; } } // Check data range validity
            }
            else { errorCode = columnErrorCode; }

            InfoAspect.Measured("CheckForError(8a)");
            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="lookup"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIsNotAnInteger"></param>
        /// <param name="errorCodeForValueNotFoundInXref"></param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName, NumericLookup lookup
            , string errorCodeForColumnNameNotFound, string errorCodeForRequiredDataMissing
            , string errorCodeForValueIsNotAnInteger, string errorCodeForValueNotFoundInXref)
        {
            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))                                  // Check column name existence
            {
                SqlInt32 num = InData.GetSqlInt32(field, fieldName);
                if (num.IsNull)                                                         // Check numeric type validity
                {
                    string strScore = field[fieldName].ToString();
                    if (string.IsNullOrEmpty(strScore))
                    {
                        if (required >= 2)
                         { errorCode = errorCodeForRequiredDataMissing; }   // Check required data existence
                    }
                    else { errorCode = errorCodeForValueIsNotAnInteger; }
                }
                else { if (!lookup.ContainsKey((int)num))
                    { errorCode = errorCodeForValueNotFoundInXref; } } // Check data range validity
            }
            else { errorCode = errorCodeForColumnNameNotFound; }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Check in database
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="dataTable"></param>
        /// <param name="column"></param>
        /// <param name="columnErrorCode"></param>
        /// <param name="nullErrorCode"></param>
        /// <param name="formatErrorCode"></param>
        /// <param name="optionErrorCode"></param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName, DataTable dataTable, string column
            , string columnErrorCode, string nullErrorCode, string formatErrorCode, string optionErrorCode)
        {
            InfoAspect.Measuring("CheckForError(10)");

            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))                                  // Check column name existence
            {
                SqlString code = InData.GetSqlString(field, fieldName);
                if (code.IsNull || string.IsNullOrEmpty(code.ToString().Trim()))
                {
                    if (required >= 2)
                        { errorCode = nullErrorCode; }   // Check required data existence
                }
                else
                {
                    if (Regex.IsMatch(code.ToString(), "^'?[V0-9][0-9][0-9][.]?[0-9]*$", RegexOptions.IgnoreCase)) // Check data format validity
                    {
                        if (!InData.ContainsValue(dataTable, column, code))
                            { errorCode = optionErrorCode; }
                    }
                    else
                    {
                        errorCode = formatErrorCode;
                    }
                }
            }
            else { errorCode = columnErrorCode; }

            InfoAspect.Measured("CheckForError(10)");
            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks for error on a string field and returns an error code
        ///      if there is an error detected, otherwise returns blank
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="pattern"></param>
        /// <param name="columnErrorCode"></param>
        /// <param name="nullErrorCode">null or blank but required</param>
        /// <param name="typeErrorCode">not matching pattern</param>
        /// <param name="optionErrorCode">unused</param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName, string pattern
            , string columnErrorCode, string nullErrorCode, string typeErrorCode, string optionErrorCode)
        {
            InfoAspect.Measuring("CheckForError(8)");

            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))                                  // Check column name existence
            {
                string str = field[fieldName].ToString();

                // ----------------------------------------------------------------------
                //  Check for data existence and format validity
                // ----------------------------------------------------------------------
                if (!string.IsNullOrEmpty(str.Trim()))
                {
                    if (!Regex.IsMatch(str, pattern))
                    {
                        errorCode = typeErrorCode;
                    }
                } else { if (required >= 2) { errorCode = nullErrorCode  ; } }
            } else                          { errorCode = columnErrorCode; }

            InfoAspect.Measured("CheckForError(8)");
            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InsertRunError -->
        /// <summary>
        ///      Inserts a row into the Import_Run_Error table
        /// </summary>
        /// <param name="err"></param>
        /// <param name="importRunId"></param>
        public static int InsertRunError(string err, Exception ex, string contentType, int importRunId, InfoAspect aspect)
        {
            int importErrorId = 0;


            // --------------------------------------------------------------------------
            //  Build the parameterized import summary insert query
            // --------------------------------------------------------------------------
            string query
                = " INSERT INTO dbo.Import_Run_Error"
                + "     ( Level_2_ID"
                + "     , Agency_ID"
                + "     , Level_4_ID"
                + "     , Content_Type"
                + "     , Error_Type"
                + "     , Error_Description"
                + "     , Import_Run_ID"
                + "     ) OUTPUT INSERTED.Import_Error_ID"
                + " VALUES "
                + "     ( @Level_2_ID"       
                + "     , @Agency_ID"        
                + "     , @Level_4_ID"       
                + "     , @Content_Type"     
                + "     , @Error_Type"       
                + "     , @Error_Description"
                + "     , @Import_Run_ID"    
                + "     )";


            // --------------------------------------------------------------------------
            //  Run the insert
            // --------------------------------------------------------------------------
            using (SqlCommand command = new SqlCommand(query, aspect.SecondaryConnection))
            {
                // ------------------------------------------------------------------
                //  Fill the parameterized import error insert query
                // ------------------------------------------------------------------
                string errorType = "";
                if (ex != null)
                    errorType = ex.GetType().ToString();

                command.Parameters.AddWithValue("@Level_2_ID"       , 0          );
                command.Parameters.AddWithValue("@Agency_ID"        , 0          );
                command.Parameters.AddWithValue("@Level_4_ID"       , 0          );
                command.Parameters.AddWithValue("@Content_Type"     , contentType);
                command.Parameters.AddWithValue("@Error_Type"       , errorType  );
                command.Parameters.AddWithValue("@Error_Description", err        );
                command.Parameters.AddWithValue("@Import_Run_ID"    , importRunId);


                // ------------------------------------------------------------------
                //  Insert the import error row
                // ------------------------------------------------------------------
                importErrorId = (int)command.ExecuteScalar();
            }

            return importErrorId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        /// 
        /// </summary>
        public static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RecordStatusError -->
        /// <summary>
        ///      Adds an error row to the status-error log
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="importRow">The row to be imported</param>
        /// <param name="statusErrorLog"></param>
        public static void RecordStatusError(string errorCode, DataRow importRow, DataTable statusErrorLog)
        {
            InfoAspect.Measuring("RecordStatusError");

            int row = statusErrorLog.Rows.Count;

            if (Regex.IsMatch(errorCode, "03"))
                Pause();

            statusErrorLog.ImportRow(importRow);
            statusErrorLog.Rows[row]["Status_Error_Code"] = errorCode;
            if (StatusError.ErrorCodeList.ContainsKey(errorCode))
                statusErrorLog.Rows[row]["Error_Message"] = StatusError.ErrorCodeList[errorCode];
            else Pause();

            InfoAspect.Measured("RecordStatusError");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForErrors -->
        /// <summary>
        ///      Checks a code field in an import DataTable for errors
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="lookup"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIsNotAnInteger"></param>
        /// <param name="errorCodeForValueNotFoundInXref"></param>
        /// <returns></returns>
        public static string CheckForErrors(int required, DataRow field, string fieldName, Dictionary<int, int> lookup
            , string errorCodeForColumnNameNotFound, string errorCodeForRequiredDataMissing
            , string errorCodeForValueIsNotAnInteger, string errorCodeForValueNotFoundInXref, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))
            {
                string str = field[fieldName].ToString();


                // --------------------------------------------------------------------------
                //  Check data validity
                // --------------------------------------------------------------------------
                if (string.IsNullOrEmpty(str.Trim()))
                {
                    if (required >= 2)
                    {
                        errorCode = errorCodeForRequiredDataMissing;
                    }
                }
                else
                {
                    // ------------------------------------------------------------------
                    //  Check data validity
                    // ------------------------------------------------------------------
                    int id = 0;
                    if (int.TryParse(str, out id))
                    {
                        if (!lookup.ContainsKey(id))
                        {
                            errorCode = errorCodeForValueNotFoundInXref;
                        }
                    }
                    else
                    {
                        errorCode = errorCodeForValueIsNotAnInteger;
                    }
                }
            }
            else { errorCode = errorCodeForColumnNameNotFound; }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForErrors -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="lookup"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIsNotAnInteger"></param>
        /// <param name="errorCodeForValueNotFoundInXref"></param>
        /// <returns></returns>
        public static string CheckForErrors(int required, DataRow field, string fieldName, NumericLookup lookup
            , string errorCodeForColumnNameNotFound, string errorCodeForRequiredDataMissing
            , string errorCodeForValueIsNotAnInteger, string errorCodeForValueNotFoundInXref, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))
            {
                string str = field[fieldName].ToString();


                // ----------------------------------------------------------------------
                //  Check data validity
                // ----------------------------------------------------------------------
                if (!string.IsNullOrEmpty(str.Trim()) && str != "NULL")
                {
                    // ------------------------------------------------------------------
                    //  Check data validity
                    // ------------------------------------------------------------------
                    int id = 0;
                    if (int.TryParse(str, out id))
                    {
                        if (!lookup.ContainsKey(id))
                        {
                            errorCode = errorCodeForValueNotFoundInXref;
                        }
                    }
                    else                    { errorCode = errorCodeForValueIsNotAnInteger; }
                } else { if (required >= 2) { errorCode = errorCodeForRequiredDataMissing; } }
            } else                          { errorCode = errorCodeForColumnNameNotFound ; }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks a date field in a DataTable for errors
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="datePattern"></param>
        /// <param name="mustBeAfterDate"></param>
        /// <param name="mustBeBeforeDate"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIncorrectFormat"></param>
        /// <param name="errorCodeForValueOutOfRange"></param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field, string fieldName
            , string datePattern, DateTime? mustBeAfterDate, DateTime? mustBeBeforeDate
            , string errorCodeForColumnNameNotFound  , string errorCodeForRequiredDataMissing
            , string errorCodeForValueIncorrectFormat, string errorCodeForValueOutOfRange, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(fieldName))
            {
                string str = field[fieldName].ToString();


                // --------------------------------------------------------------------------
                //  Check data validity
                // --------------------------------------------------------------------------
                if (string.IsNullOrEmpty(str.Trim()))
                {
                    if (required >= 2)
                    {
                        errorCode = errorCodeForRequiredDataMissing;
                    }
                }
                else
                {
                    // ------------------------------------------------------------------
                    //  Check data validity
                    // ------------------------------------------------------------------
                    string str2 = Regex.Replace(str, "^'", "");
                    if (!string.IsNullOrEmpty(datePattern))
                    {
                        if (!Regex.IsMatch(str , datePattern) &&
                            !Regex.IsMatch(str2, datePattern)
                            )
                        {
                            errorCode = errorCodeForValueIncorrectFormat;
                        }
                    }

                    if (string.IsNullOrEmpty(errorCode.Trim()))
                    {
                        DateTime date;
                        if (DateTime.TryParse(Regex.Replace(str2, "^'", ""), out date))
                        {
                            if (mustBeAfterDate != null && date <= mustBeAfterDate)
                            {
                                errorCode = errorCodeForValueOutOfRange;
                            }
                            if (mustBeBeforeDate != null && date >= mustBeBeforeDate)
                            {
                                errorCode = errorCodeForValueOutOfRange;
                            }
                        }
                        else { errorCode = errorCodeForValueIncorrectFormat; }
                    }
                }
            } else { errorCode = errorCodeForColumnNameNotFound; }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks an imported string for errors
        /// </summary>
        /// <param name="nullValue"></param>
        /// <param name="field"></param>
        /// <param name="extract"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static string CheckForError(string nullValue, DataRow field, ExtractField extract, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            if (field.Table.Columns.Contains(extract.Column))
            {
                string str = field[extract.Column].ToString();


                // ----------------------------------------------------------------------
                //  Check data validity
                // ----------------------------------------------------------------------
                if (!string.IsNullOrEmpty(str.Trim()) && str != nullValue && str != "NULL")
                {
                    // ------------------------------------------------------------------
                    //  Check data validity
                    // ------------------------------------------------------------------
                    string str2 = Regex.Replace(str, "^'", "");
                    if (!string.IsNullOrEmpty(extract.Pattern))
                    {
                        if (!Regex.IsMatch(str.Trim() , extract.Pattern) &&
                            !Regex.IsMatch(str2.Trim(), extract.Pattern))
                        {
                            errorCode = extract.ErrorFormat;
                        }
                    }

                    if (str2.Length > extract.Length)  { errorCode = extract.ErrorLength   ; }
                } else { if (extract.ImportField >= 2) { errorCode = extract.ErrorRequired ; } }
            } else                                     { errorCode = extract.ErrorFieldName; }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      Checks a complex lookup on a derived column
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName1"></param>
        /// <param name="delim"></param>
        /// <param name="fieldName2"></param>
        /// <param name="lookup"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIncorrectFormat"></param>
        /// <param name="errorCodeForExceedsDataLength"></param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field
            , string fieldName1, string delim, string fieldName2, Dictionary<int, int> lookup, string pattern
            , string tableName, string columnName
            , string errorCodeForColumnNameNotFound, string errorCodeForRequiredDataMissing
            , string errorCodeForValueIncorrectFormat, string errorCodeForValueNotFoundInTable, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            try
            {
                string str1 = "";
                if (field.Table.Columns.Contains(fieldName1) && field.Table.Columns.Contains(fieldName2))
                {
                    object obj1 = field[fieldName1];
                    if (obj1 != null)
                        str1 = obj1.ToString();
                    string str2 = ""; //field[fieldName2].ToString();
                    object obj2 = field[fieldName2];
                    if (obj2 != null)
                        str2 = obj2.ToString();

                    if (string.IsNullOrEmpty(str1.Trim()))
                    {
                        if (required > 0)
                        {
                            errorCode = errorCodeForRequiredDataMissing;
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(str1, pattern))
                        {
                            SqlInt32  importId = InData.GetSqlInt32(field, fieldName2);
                            int       realId   = lookup[(int)importId];
                            string    fullCode = str1 + delim + realId;

                            DataTable table    = InData.GetTable(tableName
                                , " SELECT *"
                                + " FROM  " + tableName
                                + " WHERE " + columnName + " = '" + fullCode + "'"
                                , aspect.SecondaryConnection);


                            if (table.Rows.Count == 0) { errorCode = errorCodeForValueNotFoundInTable; }
                        } else                         { errorCode = errorCodeForValueIncorrectFormat; }
                    }
                } else                                 { errorCode = errorCodeForColumnNameNotFound; }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      looks to see if the particular two part field value is found in the particular table column etc.
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName1"></param>
        /// <param name="delim"></param>
        /// <param name="fieldName2"></param>
        /// <param name="lookup"></param>
        /// <param name="pattern"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="errorCodeForColumnNameNotFound"></param>
        /// <param name="errorCodeForRequiredDataMissing"></param>
        /// <param name="errorCodeForValueIncorrectFormat"></param>
        /// <param name="errorCodeForValueNotFoundInTable"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static string CheckForError(int required, DataRow field
            , string fieldName1, string delim, string fieldName2, NumericLookup lookup, string pattern
            , string tableName, string columnName
            , string errorCodeForColumnNameNotFound  , string errorCodeForRequiredDataMissing
            , string errorCodeForValueIncorrectFormat, string errorCodeForValueNotFoundInTable, InfoAspect aspect)
        {
            string errorCode = string.Empty;

            try
            {
                string str1 = "";
                if (field.Table.Columns.Contains(fieldName1) && field.Table.Columns.Contains(fieldName2))
                {
                    object obj1 = field[fieldName1];
                    if (obj1 != null)
                        str1 = obj1.ToString();
                    string str2 = ""; //field[fieldName2].ToString();
                    object obj2 = field[fieldName2];
                    if (obj2 != null)
                        str2 = obj2.ToString();

                    if (!string.IsNullOrEmpty(str1.Trim()))
                    {
                        if (Regex.IsMatch(str1, pattern))
                        {
                            SqlInt32  importId = InData.GetSqlInt32(field, fieldName2);
                            int       realId   = lookup[(int)importId];
                            string    fullCode = str1 + delim + realId;

                            DataTable table    = InData.GetTable(tableName
                                , " SELECT *"
                                + " FROM  " + tableName
                                + " WHERE " + columnName + " = '" + fullCode + "'"
                                , aspect.SecondaryConnection);


                            if (table.Rows.Count == 0) { errorCode = errorCodeForValueNotFoundInTable; }
                        } else                         { errorCode = errorCodeForValueIncorrectFormat; }
                    } else { if (required > 0)         { errorCode = errorCodeForRequiredDataMissing; } }
                } else                                 { errorCode = errorCodeForColumnNameNotFound; }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckForError -->
        /// <summary>
        ///      looks to see if the particular field value is found in the particular table column etc.
        /// </summary>
        /// <param name="required"></param>
        /// <param name="field"></param>
        /// <param name="fieldName"></param>
        /// <param name="pattern"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="errorCodeForColumnNameNotFound"  ></param>
        /// <param name="errorCodeForRequiredDataMissing" ></param>
        /// <param name="errorCodeForValueIncorrectFormat"></param>
        /// <param name="errorCodeForValueNotFoundInTable"></param>
        /// <param name="errorCodeForExceedsDataLength"   ></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        internal static string CheckForError(int required, int maxLength
            , DataRow field, string fieldName, string pattern, string tableName, string columnName
            , string errorCodeForColumnNameNotFound  , string errorCodeForRequiredDataMissing
            , string errorCodeForValueIncorrectFormat, string errorCodeForValueNotFoundInTable
            , string errorCodeForExceedsDataLength   , InfoAspect aspect)
        {
            string errorCode = string.Empty;

            try
            {
                string str = "";
                if (field.Table.Columns.Contains(fieldName))
                {
                    if (field[fieldName] != null) str = field[fieldName].ToString();

                    if (!string.IsNullOrEmpty(str.Trim()))
                    {
                        if (Regex.IsMatch(str, pattern))
                        {
                            if (str.Length <= maxLength)
                            {
                                DataTable table    = InData.GetTable(tableName
                                    , " SELECT *"
                                    + " FROM  " + tableName
                                    + " WHERE " + columnName + " = '" + str + "'"
                                    , aspect.SecondaryConnection);

                                if (table.Rows.Count == 0) { errorCode = errorCodeForValueNotFoundInTable; }
                            } else                         { errorCode = errorCodeForExceedsDataLength   ; }
                        } else                             { errorCode = errorCodeForValueIncorrectFormat; }
                    } else    { if (required > 0)          { errorCode = errorCodeForRequiredDataMissing ; } }
                } else                                     { errorCode = errorCodeForColumnNameNotFound  ; }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return errorCode;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ErrorCodeList -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> ErrorCodeList { get
        {
            InfoAspect.Measuring("ErrorCodeList");

            if (_errorCode == null || _errorCode.Count < 20)
            {
                _errorCode = new Dictionary<string, string>();
                int count = 0;

                try { _errorCode.Add("1004", "Unspecified error."                                                                      ); } catch { Pause(); count++; }
                                                                                                                                       
                //  State codes 1900-1999:                                                                                             
                try { _errorCode.Add("1950", "No error - "                         + "State" + " imported."                            ); } catch { Pause(); count++; }
                try { _errorCode.Add("1951",                                         "State" + " Already Exists."                      ); } catch { Pause(); count++; }
                try { _errorCode.Add("1952", "State import file has incorrect number of columns."                                      ); } catch { Pause(); count++; }
                try { _errorCode.Add("1953", "Invalid data format."   /* Euphemism for suspected SQL insertion attempt */              ); } catch { Pause(); count++; }
                try { _errorCode.Add("1954", "Duplicate "                          + "State" + " names are not allowed."               ); } catch { Pause(); count++; }
                try { _errorCode.Add("1955", "Incorrect import column header for " + "State"                                           ); } catch { Pause(); count++; }
                try { _errorCode.Add("1956",                                         "State" + " is a required field."                 ); } catch { Pause(); count++; }
                try { _errorCode.Add("1957", "Incorrect format for "               + "State" + ". Please use " + "VA" + "."            ); } catch { Pause(); count++; }
                try { _errorCode.Add("1958", "Incorrect lookup value for "         + "State" + ". Use ?."                              ); } catch { Pause(); count++; }
                try { _errorCode.Add("1959",                                         "State" + " must not exceed " + 2 + " characters."); } catch { Pause(); count++; }


                if (count > 0)
                    throw new Exception("Repeated error code.");
            }

            InfoAspect.Measured("ErrorCodeList");
            return _errorCode;
        } }
        private static Dictionary<string,string> _errorCode;

        // ----------------------------------------------------------------------------------------
        /// <!-- InitializeTable -->
        /// <summary>
        ///      Initializes the error log table
        /// </summary>
        /// <param name="importData"></param>
        /// <returns></returns>
        public static DataTable InitializeStatusErrorStorage(DataTable importData, string tableName, int level)
        {
            InfoAspect.Measuring("InitializeStatusErrorStorage");

            DataTable statusErrorLog = importData.Clone();
            statusErrorLog.TableName = "ErrorLog";
            statusErrorLog.Columns.Add("Status_Error_Code", typeof(string));
            statusErrorLog.Columns.Add("Error_Message"    , typeof(string));
            statusErrorLog.TableName = "StatusErrorLog";


            importData.TableName = tableName;
            importData.Columns.Add(new DataColumn("IsValid" , typeof(bool)));
            importData.Columns.Add(new DataColumn("IsUpdate", typeof(bool)));

            InfoAspect.Measured("InitializeStatusErrorStorage");
            return statusErrorLog;
        }
    }
}