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
using InfoLib.Endemes;                // for 
using System;                         // for Console, Exception
using System.Collections.Generic;     // for Dictionary, List
using System.Data;                    // for DataTable, DataColumn, DataRow
using System.Data.SqlClient;          // for SqlCommand, SqlDataReader
using System.Data.SqlTypes;           // for SqlInt32
using System.IO;                      // for DirectoryInfo, FileIO
using System.Linq;                    // for OrderBy
using System.Text.RegularExpressions; // for Regex, RegexOptions

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ImportDalCommon -->
    /// <summary>
    ///      The ImportDalCommon class contains methods common to the various 'parameterized' classes
    /// </summary>
    /// <remarks>alpha code - used once in production, expected to be deprecated</remarks>
    public class ImportDalCommon
    {

        // ----------------------------------------------------------------------------------------
        //  These are just breakpoints set on error conditions
        // ----------------------------------------------------------------------------------------
        private static void AddErrorCell(string errorCode, int level)
        {
            if (!StatusError.ErrorCodeList.ContainsKey(errorCode))
                StatusError.Pause();
        }

        private static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddTwoValuesTo -->
        /// <summary>
        ///      Adds two values and possibly two columns to an error table
        /// </summary>
        /// <param name="errorTable"></param>
        /// <param name="data"></param>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool AddTwoValuesTo(DataTable errorTable, DataRow data
            , string column1, string column2, string value1, string value2, bool isRowInsertedIntoErrorTable, int level)
        {
            InfoAspect.Measuring("AddTwoValuesTo");

            DataColumn col1 = new DataColumn(column1, typeof(string));
            DataColumn col2 = new DataColumn(column2, typeof(string));
            isRowInsertedIntoErrorTable = AddTwoValuesTo(errorTable, data, col1, col2, value1, value2, isRowInsertedIntoErrorTable, level+1);

            InfoAspect.Measured("AddTwoValuesTo");
            return isRowInsertedIntoErrorTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddTwoValuesTo -->
        /// <summary>
        ///      Adds two column values (and perhaps two columns if needed) to the table
        /// </summary>
        /// <param name="errorTable"></param>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <param name="data"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool AddTwoValuesTo(DataTable errorTable, DataRow data
            , DataColumn col1, DataColumn col2, string value1, string value2, bool isRowInsertedIntoErrorTable, int level)
        {
            InfoAspect.Measuring("AddTwoValuesTo(7)");

            col1.AllowDBNull = true;
            col2.AllowDBNull = true;

            if ((!errorTable.Columns.Contains(col1.ToString())) && (!errorTable.Columns.Contains(col2.ToString())))
            {
                errorTable.Columns.Add(col1);
                errorTable.Columns.Add(col2);
            }
            if (isRowInsertedIntoErrorTable == false)
            {
                errorTable.ImportRow(data);
                isRowInsertedIntoErrorTable = true;
            }
            else
            {
                // isrowinserted = true;
            }
            foreach (DataRow dtRow in errorTable.Rows)
            {
                if (dtRow["FileLineNumber"].ToString() == data["FileLineNumber"].ToString())
                {
                    AddErrorCell(value1, level+1);
                    dtRow[col1.ToString()] = value1;
                    dtRow[col2.ToString()] = value2;
                }
            }

            InfoAspect.Measured("AddTwoValuesTo(7)");
            return isRowInsertedIntoErrorTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetAllImportFilesMatching -->
        /// <summary>
        ///      Returns all the files in each directory matching the pattern and not matching the avoid pattern
        /// </summary>
        /// <param name="filePattern"></param>
        /// <param name="dirList"></param>
        /// <param name="avoidPattern">the pattern starting with the beginning of the file name (leave out starting ^ and \\)</param>
        /// <returns></returns>
        public static List<string> GetAllImportFilesMatching(string filePattern, List<DirectoryInfo> dirList
            , string avoidPattern, InfoAspect aspect)
        {
            List<string> fileName = new List<string>();

            try
            {
                foreach (DirectoryInfo directory in dirList)
                {
                    List<FileInfo> fi = new List<FileInfo>(directory.GetFiles());
                    List<FileInfo> orderedFileList = fi.OrderBy(x => x.LastWriteTime).ToList();


                    foreach (FileInfo file in orderedFileList)
                    {
                        if (Regex.IsMatch(file.Name, filePattern, RegexOptions.IgnoreCase))
                        {
                            if (!Regex.IsMatch(file.Name, "^" + avoidPattern, RegexOptions.IgnoreCase) &&
                                !Regex.IsMatch(file.Name, @"\\" + avoidPattern, RegexOptions.IgnoreCase))
                            {
                                fileName.Add(directory.Name + "\\" + file.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return fileName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetMostRecentImportFilesMatching -->
        /// <summary>
        ///      Returns the most recent file in each directory matching the pattern and not matching the avoid pattern
        /// </summary>
        /// <param name="filePattern">files to include pattern</param>
        /// <param name="dirList"></param>
        /// <param name="avoidPattern">the pattern starting with the beginning of the file name (leave out ^ and \\)</param>
        /// <returns></returns>
        public static List<string> GetMostRecentImportFilesMatching(string filePattern
            , List<DirectoryInfo> dirList, string avoidPattern, InfoAspect aspect)
        {
            List<string> fileName = new List<string>();

            try
            {
                Dictionary<string,string> newestFile = new Dictionary<string,string>();

                foreach (DirectoryInfo directory in dirList)
                {
                    List<FileInfo> fi = new List<FileInfo>(directory.GetFiles());
                    List<FileInfo> orderedFileList = fi.OrderBy(x => x.LastWriteTime).ToList();


                    foreach (FileInfo file in orderedFileList)
                    {
                        if (Regex.IsMatch(file.Name, filePattern, RegexOptions.IgnoreCase))
                        {
                            if (!Regex.IsMatch(file.Name, "^"   + avoidPattern, RegexOptions.IgnoreCase) &&
                                !Regex.IsMatch(file.Name, @"\\" + avoidPattern, RegexOptions.IgnoreCase))
                            {
                                // ----------------------------------------------------------
                                //  Replace older files (earlier in the list) with newer files of the same directory
                                // ----------------------------------------------------------
                                string dirKey = directory.Name;
                                if (!newestFile.ContainsKey(dirKey))
                                    newestFile.Add(dirKey,"");
                                newestFile[dirKey] = directory.Name + "\\" + file.Name;
                            }
                        }
                    }
                }


                foreach (string file in newestFile.Values)
                {
                    fileName.Add(file);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return fileName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InitializeInsertedDataTable -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="importData"></param>
        /// <param name="idColumnName"></param>
        /// <returns></returns>
        public static DataTable InitializeImportedDataTable(DataTable importData, string idColumnName, InfoAspect aspect)
        {
            DataTable importedData = new DataTable();

            importedData           = importData.Clone();
            importedData.TableName = "ImportedData";
            importedData.Columns.Add(new DataColumn(idColumnName, typeof(SqlInt32)));

            return importedData;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportErrorQuery -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        internal static string ImportErrorQuery(string moduleName)
        {
            string commandText
                = "\r\n" + " INSERT INTO "+moduleName+"_Import_Error"
                + "\r\n" + "     ( "+moduleName+"_Import_ID"
                + "\r\n" + "     , Error_Code"
                + "\r\n" + "     , Error_Message"
                + "\r\n" + "     ) OUTPUT INSERTED.Import_Error_ID"
                + "\r\n" + " VALUES"
                + "\r\n" + "     ( @"+moduleName+"_Import_ID"
                + "\r\n" + "     , @Error_Code"
                + "\r\n" + "     , @Error_Message"
                + "\r\n" + "     )"
                ;

            return commandText;
        }

        internal static void ImportErrorFill(SqlCommand command, int importId, string errorCode, DataRow field, string moduleName)
        {
            command.Parameters.AddWithValue("@"+moduleName+"_Import_ID", importId              );
            command.Parameters.AddWithValue("@Error_Code"              , errorCode             );
            command.Parameters.AddWithValue("@Error_Message"           , field["Error_Message"]);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportErrorInsert -->
        /// <summary>
        ///      Inserts a module import error row
        /// </summary>
        /// <param name="query"></param>
        /// <param name="statusErrorLog"></param>
        /// <param name="row"></param>
        /// <param name="itemIdByLineNum"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void ImportErrorInsert(string query, DataTable statusErrorLog, int row
            , string statusErrorCode, Dictionary<int, int> itemIdByLineNum, string moduleName, InfoAspect aspect)
        {
            SqlCommand errorInsert = new SqlCommand(query, aspect.SecondaryConnection);
            using (errorInsert)
            {
                // ------------------------------------------------------------------
                //  Insert the error into the error table
                // ------------------------------------------------------------------
                int lineNumber = int.Parse(statusErrorLog.Rows[row]["FileLineNumber"].ToString());
                int itemId     = itemIdByLineNum[lineNumber];
                ImportDalCommon.ImportErrorFill(errorInsert, itemId, statusErrorCode, statusErrorLog.Rows[row], moduleName);
                int errorId    = (int)errorInsert.ExecuteScalar();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportErrorsInsert -->
        /// <summary>
        ///      Inserts all module import errors into the database
        /// </summary>
        /// <param name="statusErrorLog"></param>
        /// <param name="itemIdByLineNum"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static int ImportErrorsInsert(DataTable statusErrorLog, Dictionary<int, int> itemIdByLineNum
            , string moduleName, InfoAspect aspect)
        {
            int errorCount = 0;

            try
            {
                string query = ImportDalCommon.ImportErrorQuery(moduleName);
                for (int row = 0; row < statusErrorLog.Rows.Count; ++row)
                {
                    // ------------------------------------------------------------------
                    //  Process the row
                    // ------------------------------------------------------------------
                    string statusErrorCode = statusErrorLog.Rows[row]["Status_Error_Code"].ToString();
                    if (!Regex.IsMatch(statusErrorCode, "0[01]$"))
                    {
                        ImportDalCommon.ImportErrorInsert(query, statusErrorLog, row, statusErrorCode
                            , itemIdByLineNum, moduleName, aspect.Enter(moduleName,"ImportErrorInsert")); aspect--;
                        errorCount++;
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return errorCount;
        }
    }
}
