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
using InfoLib.HardData;        // for InData
using System;                         // for EventArgs
using System.Collections.Generic;     // for List
using System.Data;                    // for CommandType
using System.Data.SqlClient;          // for SqlConnection, SqlCommand, SqlDataReader
using System.Data.SqlTypes;           // for SqlDateTime, SqlInt32, SqlString
using System.Drawing;                 // for Point, Color
using System.Reflection;              // for [later use]
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ImportMapField -->
    /// <summary>
    ///      not presently used, consider this an early design document
    ///      File and data field handler for loosely coupled field vertical architecture
    /// </summary>
    /// <remarks>
    ///      This is copied from field and will changes radically as time goes on
    /// </remarks>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class ImportMapField
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- OnChangeEvent -->
        /// <summary>
        ///      A delegate for events that fire when some data is changed in a control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="a"></param>
        /// <remarks>
        ///      Example use:
        ///      
        ///      OnChangeEvent MethodRun = chkMedicaidInd_CheckedChanged; // this saves it
        ///      MethodRun(new object(), new EventArgs());                // this runs it
        ///      MethodRun(sender, a);                                    // this too runs it
        ///      string name = MethodRun.Method.Name;                     // this looks at its name
        /// </remarks>
        public delegate void OnChangeEvent(object sender, EventArgs a);


        // ----------------------------------------------------------------------------------------
        //  22 Member Properties, anything additional is handled in code behind, aspx, css, or CrudModule
        // ----------------------------------------------------------------------------------------
                                                                /// <summary></summary>     //      DB MT UI CSS Notes
        public object                    An       { get; set; } /// <summary>blank the field when updating if it is not in the import</summary>       // * Q.   [M]        (should this be a List<string>?)
        public bool                      Blank    { get; set; } /// <summary>field/column in the database</summary>                //   E.    M  U 
/*SC*/  public List<string> /* 2 */      Column   { get; set; } /// <summary>hidden key on page for view/edit screens</summary>    //   J.    M    
/*+*/   public string                    Data4Id  { get; set; } /// <summary>FieldName/Required/Format/Lookup error codes for field</summary>    //   D.       U 
/*E.*/  public List<string> /* 4 */      Err      { get; set; } /// <summary>the column in the file (or member in the object) to be downloaded</summary>           //   L.    M  U 
/*IC*/  public string                    Field    { get; set; } /// <summary></summary>    //   F. D       
        public object                    Go       { get; set; } /// <summary></summary>    //   S.       U      (arguments)
        public object                    Hi       { get; set; } /// <summary>required(2) or optional(1) to import field</summary>                                //   O.       U 
/*IF*/  public int                       Import   { get; set; } /// <summary>where to look for data for a dropdownlist</summary>   // I. D              (intentionally denormalized, this is stored in every field)
/*+*/   public DataTable                 Join     { get; set; } /// <summary>the column the id is kept in</summary>                //   H.    M  U 
/*+*/   public string                    Key      { get; set; } /// <summary>requried max length of the input/mirror/store item</summary>                    //   K. D  M         (intentionally denormalized, this is stored in every field)
/*.L*/  public List<int> /* 3 */         Length   { get; set; } /// <summary>'universal' definition/identity of field</summary>       //   C.       U 
/*ISM*/ public string                    Mapping  { get; set; } /// <summary>is the data numeric (or boolean) or not</summary>     //   M. ?       
        public bool /*notneededforparmaterizedsqlcommands*/ Numeric  { get; set; } /// <summary>field column number in the input file</summary> //   N. D  M  U      (is this an order by and order number or what?  I don't remember what this is heare for)
/*IO*/  public string                    Order    { get; set; } /// <summary></summary>      // * B.[D]           (intentionally denormalized, this is stored in every field) 
/*IP*/  public string                    Pattern  { get; set; } /// <summary>query related to the field</summary>       //   P.    M  U 
        public string                    Query    { get; set; } /// <summary>lookup xref table to convert from import number to database number</summary>     // * G.      [U]
/*RM*/  public Dictionary<int,int>       xRef     { get; set; } /// <summary>required(2) or optional(1) to store field</summary>  //   R.       U  C
/*SF*/  public int                       Store    { get; set; } /// <summary>name of the table where the field is kept</summary>   //   A.       U 
/*ST+*/ public string                    Table    { get; set; } /// <summary>lists which other tables use rows from this table including the table name (key) and its foreign key field (value)</summary>    //   T.       U 
/*+*/   public Dictionary<string,string> UsedBy   { get; set; } /// <summary>related to field variable name used to build queries</summary>  //   U. D  M    
/*.P*/  public List<string> /* 2 */      Variable { get; set; }                                                                    //   V.    M  U 
        // maybe: the file from which the column is read, may be a path or a file name depending on theneeds of the application
        // maybe: row in the file from which the field was gotten


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public ImportMapField() { }
        public ImportMapField(string tableName, string idColumn, string columnName) { Init(tableName, idColumn, columnName); }


        // ----------------------------------------------------------------------------------------
        /// <!-- FieldMembers -->
        /// <summary>
        ///      A:rgs, B)ox, C)hk, D)rop, E)ventFn, F)ield, G)rid, H)omeTable, I)dColumn, J)umpTo, 
        ///      K)ey, L)bl, M)sg, N)umeric, O)rder, P)ostBack, Q)uery, R)eq, S)ender, T)xt, 
        ///      U)niversal, V)iewLabel 
        /// </summary>
        public static Dictionary<char, string> FieldMembers
        {
            get
            {
                if (_fieldMembers == null)
                {
                    _fieldMembers = new Dictionary<char, string>();

                    _fieldMembers.Add('A', "A        , ");
                    _fieldMembers.Add('B', "B        , ");
                    _fieldMembers.Add('C', "Column   , field/column in the database");
                    _fieldMembers.Add('D', "D        , ");
                    _fieldMembers.Add('E', "Err      , a place to store error messages returned");
                    _fieldMembers.Add('F', "File     , the file from which the column is read");
                    _fieldMembers.Add('G', "G        , ");
                    _fieldMembers.Add('H', "H        , ");
                    _fieldMembers.Add('I', "Import   , required(2) or optional(1) to import field");
                    _fieldMembers.Add('J', "Join     , lookup table meaning");
                    _fieldMembers.Add('K', "Key      , the actual id of the row");
                    _fieldMembers.Add('L', "L        , ");
                    _fieldMembers.Add('M', "Member   , the column in the file to be downloaded");
                    _fieldMembers.Add('N', "Numeric  , is the data numeric (or boolean) or not");
                    _fieldMembers.Add('O', "Object   , the class (object) to get the field from");
                    _fieldMembers.Add('P', "PK       , the column the id is kept in");
                    _fieldMembers.Add('Q', "Query    , query related to the field");
                    _fieldMembers.Add('R', "Row      , row in the file from which the field was gotten");
                    _fieldMembers.Add('S', "Store    , required(2) or optional(1) to store field");
                    _fieldMembers.Add('T', "Table    , name of the table where the field is kept");
                    _fieldMembers.Add('U', "UsedBy   , lists which other tables use rows from this table");
                    _fieldMembers.Add('V', "Variable , related to field variable name used to build queries");
                }

                return _fieldMembers;
            }
        }
        private static Dictionary<char, string> _fieldMembers;

        // ----------------------------------------------------------------------------------------
        /// <!-- DeleteRow -->
        /// <summary>
        ///      Deletes the row that the field is in, if the field leads to one and only one item
        /// </summary>
        /// <param name="isUsedBy">a dictionary of tables and foreign keys where this row may be used</param>
        /// <returns></returns>
        public string DeleteRow(Dictionary<string,string> isUsedBy, SqlConnection connection)
        {
            string err = "";


            if (Data4Id != null)
            {
                SqlInt32 id = InData.GetSqlInt32(this.Data4Id);


                int useCount = 0;
                // --------------------------------------------------------------------------
                //  Test for use in another table
                // --------------------------------------------------------------------------
                if (isUsedBy != null)
                {
                    foreach (string tableName in isUsedBy.Keys)
                    {
                        string query = "SELECT COUNT(*) FROM [" + tableName + "] WHERE [" + isUsedBy[tableName] + "] = " + id;
                        useCount += (int)InData.GetSqlInt32(query, connection);
                    }
                }


                if (useCount > 0)
                {
                    err = "This record is used " + useCount + " times, you may not delete it.";
                }
                else
                {
                    // --------------------------------------------------------------------------
                    //  Build a filter from the field
                    // --------------------------------------------------------------------------
                    string fromFilter = " FROM " + this.Table + " WHERE " + Key + " = " + id;


                    // --------------------------------------------------------------------------
                    //  Check to make sure only one row will be deleted by the filter
                    // --------------------------------------------------------------------------
                    List<SqlParameter> param = new List<SqlParameter>(1);
                    param.Add(new SqlParameter("@Id", Data4Id));
                    SqlInt32 count = InData.GetSqlInt32("SELECT COUNT(*)" + fromFilter, param, connection);


                    // --------------------------------------------------------------------------
                    //  Delete the row or say why not
                    // --------------------------------------------------------------------------
                    if (count == 1)
                    {
                        SqlCommand cmd = new SqlCommand("DELETE" + fromFilter, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Id", Data4Id);
                        cmd.ExecuteNonQuery();
                        err = "";
                        //  TODO: handle foreign key constraints
                    }
                    else
                        err = "Row not found for " + this.Table + ", id: " + id;
                }
            }
            else
            {
                err = "key empty";
            }

            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Get -->
        /// <summary>
        ///      Gets the data for a field from the database
        /// </summary>
        /// <returns></returns>
        public object GetObject(SqlConnection connection)
        {
            string query = "SELECT " + Column + " FROM " + Table + " WITH(NOLOCK) WHERE " + Key + " = @Id";

            List<SqlParameter> parameters = new List<SqlParameter>(1);
            parameters.Add(new SqlParameter("@Id", Data4Id));

            object obj = null;
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Id", Data4Id);
                obj = cmd.ExecuteScalar();
            }
            return obj;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetRow -->
        /// <summary>
        ///      Returns the row that contains the field
        /// </summary>
        /// <returns></returns>
        public DataTable GetRow(string tableName, SqlConnection connection)
        {
            string query = "SELECT *"
                         + " FROM  " + Table + " WITH(NOLOCK)"
                         + " WHERE " + Key + " = @Id";

            List<SqlParameter> parameters = new List<SqlParameter>(1);
            parameters.Add(new SqlParameter("@Id", Data4Id));
            DataTable table = InData.GetTable(tableName, query, parameters, connection);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="header"></param>
        /// <param name="tableName"></param>
        /// <param name="idColumn"></param>
        /// <param name="columnName"></param>
        private void Init(string tableName, string idColumn, string columnName)
        {
            Err    = new List<string>(4);
            Column = new List<string>(3); Column.Add(columnName); Column.Add(columnName); Column.Add(columnName);
            Key    = idColumn;
            Table  = tableName;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- JoinTo -->
        /// <summary>
        ///      Sets a join to this column to build dropdown lists from later on
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="idColumnName"></param>
        /// <param name="idColumnType"></param>
        /// <param name="textColumn"></param>
        public void JoinTo(string tableName, string idColumnName, Type idColumnType, string textColumn)
        {
            Join = new DataTable(tableName);
            Join.Columns.Add(idColumnName, idColumnType);
            Join.Columns.Add(textColumn  , typeof(string));
            DataColumn[] key = new DataColumn[1];
            key[0] = Join.Columns[0];
            Join.PrimaryKey = key;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Let -->
        /// <summary>
        ///      Sets the value of a field in the database
        /// </summary>
        /// <param name="str"></param>
        public void Let(object obj, SqlConnection connection)
        {
            string query = " UPDATE " + Table
                + " SET    " + Column + " = @Value"
                + " WHERE  " + Key     + " = @Id";


            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Id"   , Data4Id);
                cmd.Parameters.AddWithValue("@Value", obj);
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ResistSqlInjection -->
        /// <summary>
        ///      A fairly simple attempt to resist sql injection in dynamic queries
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static string ResistSqlInjection(string str, int maxLen)
        {
            if (str.Length > maxLen)
                str = str.Substring(0,maxLen);
            str = Regex.Replace(str, "(/[*]|[*]/)", "");

            str = Regex.Replace(str, "(xp_cmdshell|xp_enumgroups|xp_grantlogin|xp_logevent|xp_loginconfig|xp_logininfo|xp_msver|xp_revokelogin|xp_sprintf|xp_sqlmaint|xp_sscanf)", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(ASCII|CHAR)[ (]+[^()]+[)]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(sys|sysx)(column|message|object|process|server|login)(s|es)", "$1 $2 $3", RegexOptions.IgnoreCase);

            str = Regex.Replace(str, "(union)(.*)(select)"                                         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(alter|create|drop|truncate)(.*)(role|table|view)"           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(grant|revoke)(.*)(insert|select|update|delete|all)"         , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(delete|select)(.*)(from|top|[(][)])"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(insert|update)(.*)(into|set|values)"                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //str = Regex.Replace(str, "(and|if|or)( .* )(=|<>|or|and|else|exists|like|select)"    , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(order)( .*|.* )(by)"                                        , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(cmd|exec|execute)(.*)(cmd|dbo|execute|master|sys|xp_|sp_)"  , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(;)(.*)(shutdown)"                                           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            str = Regex.Replace(str, "(where)(.*)(like)"                                           , "'$1'$2$3", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            str = Regex.Replace(str, "[-]+" , "-" );
            str = Regex.Replace(str, "@+"   , "@" );
            //str = Regex.Replace(str, " = "  , " equals ");
            str = Regex.Replace(str, @"[\\]", ""  );
            str = Regex.Replace(str, "'+"   , "''");

            return str;
        }
    }
}