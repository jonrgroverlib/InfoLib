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
using InfoLib.HardData;        // for ImportMapping
using System;                         // for 
using System.Collections.Generic;     // for Dictionary
using System.Data;                    // for DataRow, DataTable
using System.Linq;                    // for 
using System.Data.SqlClient;          // for SqlCommand, SqlException, SqlDataReader
using System.Data.SqlTypes;           // for SqlDateTime
using System.Text.RegularExpressions; // for Regex, RegexOptions

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EtlModule -->
    /// <summary>
    ///      The EtlModule class is an attempt to make the parameterized classes generic
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class EtlModule
    {
        #region Members and constructors

        protected static void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        //  ETL members
        // ----------------------------------------------------------------------------------------
        public    Dictionary<string,ExtractField>   Extract         { get { return _extractMap  ; } } public  Dictionary<string,ExtractField>   _extractMap  ;
        public    Dictionary<string,MirrorField>    Mirror          { get { return _mirrorMap   ; } } public  Dictionary<string,MirrorField>    _mirrorMap   ; /// <summary>C)omplex, D)uplicate, E)xtract X, I)dentity column, L)oad X, M)irror X, N)umeric lookup table</summary>
        public    Dictionary<string,TransformField> Transform       { get { return _transformMap; } } public  Dictionary<string,TransformField> _transformMap;
        public    Dictionary<string,LoadField>      Load            { get { return _loadMap     ; } } public  Dictionary<string,LoadField>      _loadMap     ;
                                                                   
        public    Dictionary<string,NumericLookup>  NumberConverter { get { return _numberXref  ; } } private Dictionary<string,NumericLookup>  _numberXref  ;
        public    Dictionary<string,StringLookup>   StringConverter { get { return _stringXref  ; } } private Dictionary<string,StringLookup>   _stringXref  ;
        public    Dictionary<string,NumericLookup>  ParentConverter { get { return _parentXref  ; } } private Dictionary<string,NumericLookup>  _parentXref  ;


        public    DataTable StatusErrorLog         { get; set; } // this is being used to store the status on each imported row and the status of each error
        public    string    Name                   { get; set; } // the name of the module (for logging etc)
        protected string    MainTable              { get; set; }
        protected string    ImportItemTable        { get; set; } // name of the import item table
        protected string    RealIdColumn           { get; set; } // name of the real id column placed in the import DataTable
        protected string    MainTablePk            { get; set; }
        protected bool      UniqueIdIncludesRegion { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constrctor
        // ----------------------------------------------------------------------------------------
        public EtlModule() { }
        public EtlModule(string moduleName, int fieldCount)
        {
            // --------------------------------------------------------------------------
            //  Module name
            // --------------------------------------------------------------------------
            Name = moduleName;


            // --------------------------------------------------------------------------
            //  Transform set
            // --------------------------------------------------------------------------
            EndemeSet transformSet = TransformField.ActionSet;


            // --------------------------------------------------------------------------
            //  Value mappings
            // --------------------------------------------------------------------------
            _numberXref = new Dictionary<string,NumericLookup>(20);
            _stringXref = new Dictionary<string,StringLookup> (2) ;
            _parentXref = new Dictionary<string,NumericLookup>(4) ;


            _numberXref.Add("Gender"        , Xref.IntGender     );  _stringXref.Add("Gender"        , Xref.StrGender     );
            _numberXref.Add("State"         , Xref.State         );


            // --------------------------------------------------------------------------
            //  Script dictionaries
            // --------------------------------------------------------------------------
            _extractMap    = new Dictionary<string,ExtractField>  (fieldCount);
            _mirrorMap     = new Dictionary<string,MirrorField>   (fieldCount);
            _transformMap  = new Dictionary<string,TransformField>(fieldCount);
            _loadMap       = new Dictionary<string,LoadField>     (fieldCount);


            // --------------------------------------------------------------------------
            //  Result recording
            // --------------------------------------------------------------------------
            StatusErrorLog = new DataTable();
        }

        #endregion Members and constructors

        #region AddParameter

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad -->
        /// <summary>
        ///      Adds a parameter to a query for transforming an extract field into a load field
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad(SqlCommand cmd, string tableName, DataRow field, string transformKey, InfoAspect aspect)
        {
            TransformField transform = Transform[transformKey];
            string extractKey     = ""; ExtractField extract = null; string extractActions = "";
            string loadKey        = ""; LoadField    load    = null; string loadActions    = "";
            string mirrorActions  = "";


            try
            {
                // ----------------------------------------------------------------------
                //  Unpack transformation
                // ----------------------------------------------------------------------
                extractKey     = transform.ExtractKey    ;
                loadKey        = transform.LoadKey       ;
                extractActions = transform.ExtractActions;
                mirrorActions  = transform.MirrorActions ;
                loadActions    = transform.LoadActions   ;

                //if (transformKey == "ACTIVE")
                //    Pause();


                // ----------------------------------------------------------------------
                //  Save Parameter
                // ----------------------------------------------------------------------
                if (Load.ContainsKey(loadKey))
                {
                    load = Load[loadKey];
                    if (load.InTable == tableName)
                    {
                        if ( load.StoreField > 0       &&
                             load.InTable == tableName &&
                            !cmd.Parameters.Contains(load.Param))
                        {
                            if (Extract.ContainsKey(extractKey))
                            {
                                if (RunOkForUpdateOrInsert(loadActions, cmd.CommandText))
                                {
                                    extract = Extract[extractKey];
                                    if (Regex.IsMatch(loadActions, "[" + "B" + "C" + "D" + "I" + "K" + "L" + "M" + "R" + "V" + "]"))
                                    {
                                        if (Regex.IsMatch(loadActions, "B")                                    ) { AddParameterToLoad_blank    (cmd                                           , load, aspect.Enter()); aspect--; }  //  'Blank' value
                                        if (Regex.IsMatch(loadActions, "D")                                    ) { AddParameterToLoad_simple   (cmd           , field, extract, extractActions, load, aspect.Enter()); aspect--; }  //  Transfer 'as-is' 
                                        if (Regex.IsMatch(loadActions, "K") && !Regex.IsMatch(loadActions, "V")) { AddParameterToLoad_simple   (cmd           , field, extract, extractActions, load, aspect.Enter()); aspect--; }  //  Add a key parameter 'as-is' 
                                        if (Regex.IsMatch(loadActions, "L") && !Regex.IsMatch(loadActions, "C")) { AddParameterToLoad_intlookup(cmd           , field, extract, transform     , load, aspect.Enter()); aspect--; }  //  Do a conversion using a look-up table
                                        if (Regex.IsMatch(loadActions, "M")                                    ) { AddParameterToLoad_strlookup(cmd, tableName, field, transformKey                 , aspect.Enter()); aspect--; }  //  lookup string value
                                        if (Regex.IsMatch(loadActions, "V")                                    ) { AddParameterToLoad_value    (cmd                           , transform     , load, aspect.Enter()); aspect--; }  //  Literal value
                                    } else { if (!string.IsNullOrEmpty(extract.Column.Trim()))                     { AddParameterToLoad_simple   (cmd           , field, extract, extractActions, load, aspect.Enter()); aspect--; }} //  Transfer 'as-is'          
                                }
                            }
                            else
                            {
                                // --------------------------------------------------------------
                                //  Maybe grab the real db id stored in the extract table
                                // --------------------------------------------------------------
                                if (Regex.IsMatch(loadActions, "R"))
                                {
                                    if (RunOkForUpdateOrInsert(loadActions, cmd.CommandText))
                                    {
                                        AddParameterToLoad_realId(cmd, field, load, aspect.Enter()); aspect--;
                                    }
                                }
                                if (Regex.IsMatch(loadActions, "V"))
                                {
                                    if (RunOkForUpdateOrInsert(loadActions, cmd.CommandText))
                                        { AddParameterToLoad_value (cmd, transform, load, aspect.Enter()); aspect--; }  //  Literal value
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="load"></param>
        /// <param name="aspect"></param>
        private void AddParameterToLoad_realId(SqlCommand cmd, DataRow field, LoadField load, InfoAspect aspect)
        {
            string   extractPkColumn = "";
            SqlInt32 extractPkValue  = SqlInt32.Null;
            string   sourceFkColumn  = "";
          //string   loadPkColumn    = "";


            try
            {
                //  Check for a value in that column and throw an exception if not one
                SqlInt32 idValue = SqlInt32.Null; // InData.GetSqlInt32(field, RealIdColumn);
                if (idValue.IsNull || idValue == -1)
                {
                    // ------------------------------------------------------------------
                    //  Look in the database itself
                    // ------------------------------------------------------------------
                    extractPkColumn = ColumnName_ExtractPk(aspect.Enter()); aspect--;
                    extractPkValue  = InData.GetSqlInt32 (field, extractPkColumn);
                    sourceFkColumn  = ColumnName_SourceId(MainTable, aspect.Enter()); aspect--;
                  //loadPkColumn    = ColumnName_LoadPk  (tableName, aspect.Enter()); aspect--;


                    if (!string.IsNullOrEmpty(sourceFkColumn) &&
                        !string.IsNullOrEmpty(extractPkColumn))
                    {
                        idValue = InData.GetSqlInt32(
                              " SELECT " + MainTablePk + " FROM " + MainTable + " WITH(NOLOCK)"
                            + " WHERE "+sourceFkColumn+" = " + extractPkValue, aspect.SecondaryConnection);
                    }
                    if (idValue.IsNull)
                    {
                        throw new NoNullAllowedException("can not use a null foreign key to "+MainTable+".");
                    }
                }
                cmd.Parameters.AddWithValue(load.Param, idValue);
            }
            catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_blank -->
        /// <summary>
        ///      Adds a parameter with sort of a blank value
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="load"></param>
        private void AddParameterToLoad_blank(SqlCommand cmd, LoadField load, InfoAspect aspect)
        {
            try
            {
                switch (load.DataType)
                {
                    case "bit"    : SqlBoolean YN = false; cmd.Parameters.AddWithValue(load.Param, YN          ); break;
                    case "int"    :                        cmd.Parameters.AddWithValue(load.Param, 0           ); break;
                    case "date"   :                        cmd.Parameters.AddWithValue(load.Param, DateTime.Now); break;
                    case "string" :                        cmd.Parameters.AddWithValue(load.Param, ""          ); break;
                    default:
                        Pause();
                        break;
                }
            }
            catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_value -->
        /// <summary>
        ///      Adds a parameter with sort of a literal value "V" kept in the Xref member
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="load"></param>
        private void AddParameterToLoad_value(SqlCommand cmd, TransformField transform, LoadField load, InfoAspect aspect)
        {
            try
            {
                string value = transform.XrefLookup;

                switch (load.DataType)
                {
                    case "bit"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlBoolean (value)); break;
                    case "int"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlInt32   (value)); break;
                    case "date"   : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlDateTime(value)); break;
                    case "string" : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlString  (value)); break;
                    default:
                        Pause();
                        break;
                }
            }
            catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_lookup -->
        /// <summary>
        ///      Gets the paraeter value using one of the integer lookup dictionaries
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="extract"></param>
        /// <param name="transform"></param>
        /// <param name="load"></param>
        private void AddParameterToLoad_intlookup(SqlCommand cmd, DataRow field
            , ExtractField extract, TransformField transform, LoadField load, InfoAspect aspect)
        {
            SqlInt32 intValue = SqlInt32.Null;
            SqlInt32 newValue = SqlInt32.Null;
            string   xrefName = null;
            try
            {
                string typeConversion = extract.DataType + "," + load.DataType;
                switch (typeConversion)
                {
                    case "int,int" :
                        intValue = InData.ExtractIntValue_defaultMinusOne(field, extract.Column, load.Length);
                        if (!intValue.IsNull)
                        {
                            xrefName = transform.XrefLookup;
                            newValue = _numberXref[xrefName][(int)intValue];
                        }
                        cmd.Parameters.AddWithValue(load.Param, newValue);
                        break;
                    default:
                        Pause();
                        break;
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_simple -->
        /// <summary>
        ///      Simply adds the query parameter without testing whether it should be added
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="extract"></param>
        /// <param name="load"></param>
        private void AddParameterToLoad_simple(SqlCommand cmd, DataRow field, ExtractField extract
            , string extractActions, LoadField load, InfoAspect aspect)
        {
            try
            {
                if (Regex.IsMatch(extractActions, "T"))
                {
                    SqlString value = InData.Trim(InData.GetSqlString(field, extract.Column));
                    cmd.Parameters.AddWithValue(load.Param, InData.Truncate(value, load.Length));
                }
                else
                {
                    string typeConversion = extract.DataType + "," + load.DataType;
                    switch (typeConversion)
                    {
                        case    "bit,bit"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlInt32                    (field, extract.Column              )           ); break;
                        case    "bit,int"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlInt32                    (field, extract.Column              )           ); break;
                        case   "date,date"   : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlDateTime                 (field, extract.Column              )           ); break;
                        case    "int,bit"    : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultMinusOne(field, extract.Column , load.Length)           ); break;
                        case    "int,int"    : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultMinusOne(field, extract.Column , load.Length)           ); break;
                        case    "int,string" : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultNull    (field, extract.Column , load.Length).ToString()); break;
                        case "string,int"    : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultMinusOne(field, extract.Column , load.Length)           ); break;
                        case "string,string" : cmd.Parameters.AddWithValue(load.Param, InData.Truncate (InData.GetSqlString  (field, extract.Column), load.Length)           ); break;
                        default:
                            Pause();
                            break;
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_CreateDate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_CreateDate(SqlCommand cmd, string tableName, DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                string extractKey = Transform[transformKey].ExtractKey;
                string loadKey    = Transform[transformKey].LoadKey;


                if (Extract[extractKey].ImportField > 0)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(field[Extract[extractKey].Column]).Trim()))
                            { cmd.Parameters.AddWithValue("@Create_Date", field[Extract[extractKey].Column]); }
                    else { cmd.Parameters.AddWithValue("@Create_Date", DateTime.Now  ); }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_DsmCode -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_DsmCode(SqlCommand cmd, string tableName, DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                string extractKey = Transform[transformKey].ExtractKey;
                string loadKey    = Transform[transformKey].LoadKey;


                if (Load.ContainsKey(loadKey)          &&
                    Extract.ContainsKey(extractKey)    &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    cmd.Parameters.AddWithValue(Load[loadKey].Param
                        , Xref.DsmCode_Lookup(InData.GetSqlString(field, Extract[extractKey].Column)));
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddAnswerParameter -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformTextKey"></param>
        /// <param name="transformValueKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_Answer(SqlCommand cmd, string tableName, DataRow field, string transformTextKey, string transformValueKey, InfoAspect aspect)
        {
            try
            {
                string textKey  = Transform[transformTextKey ].ExtractKey;
                string loadKey  = Transform[transformTextKey ].LoadKey;
                string valueKey = Transform[transformValueKey].ExtractKey;


                if (Load.ContainsKey(loadKey)          &&
                    Extract.ContainsKey(textKey)       &&
                    Extract.ContainsKey(valueKey)      &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    SqlInt32 answerValue = InData.GetSqlInt32 (field, valueKey);
                    if (Regex.IsMatch(transformTextKey, answerValue.ToString()))
                    {
                        // now set the parameter
                        SqlString answerText = InData.GetSqlString(field, textKey );
                        cmd.Parameters.AddWithValue(Load[loadKey].Param, answerText);
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_Literal -->
        /// <summary>
        ///      Adds an integer parameter to the SQL command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_Literal(SqlCommand cmd, string tableName, SqlInt32 value, string transformKey, InfoAspect aspect)
        {
            try
            {
                string loadKey = Transform[transformKey].LoadKey;

                if (Load.ContainsKey(loadKey)          &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    cmd.Parameters.AddWithValue(Load[loadKey].Param, value);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_Literal -->
        /// <summary>
        ///      Adds a boolean parameter to the SQL command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_Literal(SqlCommand cmd, string tableName, SqlBoolean value, string transformKey, InfoAspect aspect)
        {
            try
            {
                string loadKey = Transform[transformKey].LoadKey;

                if (Load.ContainsKey(loadKey)          &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    cmd.Parameters.AddWithValue(Load[loadKey].Param, value);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_Literal -->
        /// <summary>
        ///      Adds a string parameter to the SQL command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_Literal(SqlCommand cmd, string tableName, SqlString value, string transformKey, InfoAspect aspect)
        {
            try
            {
                string loadKey = Transform[transformKey].LoadKey;

                if ( Load.ContainsKey(loadKey)          &&
                     Load[loadKey].InTable == tableName &&
                     Load[loadKey].StoreField > 0       &&
                    !cmd.Parameters.Contains(Load[loadKey].Param)
                   )
                {
                    cmd.Parameters.AddWithValue(Load[loadKey].Param, value);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_Literal -->
        /// <summary>
        ///      Adds a datetime parameter to the SQL command
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_Literal(SqlCommand cmd, string tableName, SqlDateTime value, string transformKey, InfoAspect aspect)
        {
            try
            {
                string loadKey = Transform[transformKey].LoadKey;

                if (Load.ContainsKey(loadKey)          &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    cmd.Parameters.AddWithValue(Load[loadKey].Param, value);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_RandomRowId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="refTableName"></param>
        /// <param name="tablePk"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_RandomRowId(SqlCommand cmd, string tableName, DataRow field
            , string transformKey, string refTableName, string tablePk, InfoAspect aspect)
        {
            try
            {
                TransformField transform = Transform[transformKey];
                string         loadKey   = transform.LoadKey;


                if (Load.ContainsKey(loadKey) && Load[loadKey].InTable == tableName)
                {
                    LoadField load = Load[loadKey];
                    if (Load[loadKey].StoreField > 0 && transform.LoadActions == "G")
                    {
                        SqlInt32 id = InData.GetRandomRowId(refTableName, tablePk, aspect.SecondaryConnection);
                        cmd.Parameters.AddWithValue(load.Param, id);
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_ModifyDate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_ModifyDate(SqlCommand cmd, string tableName, DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                string extractKey = Transform[transformKey].ExtractKey;
                string loadKey = Transform[transformKey].LoadKey;


                if (Load.ContainsKey(loadKey)          &&
                    Extract.ContainsKey(extractKey)    &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    if (Extract[extractKey].ImportField > 0) // ImportModifyDate > 0)
                    {
                        SqlDateTime modifyDate = InData.GetSqlDateTime(field, "Last_Update_Date");
                        if (modifyDate.IsNull) cmd.Parameters.AddWithValue("@Last_Update_Date", DateTime.Now);
                        else                   cmd.Parameters.AddWithValue("@Last_Update_Date", modifyDate  );
                    } else                     cmd.Parameters.AddWithValue("@Last_Update_Date", DateTime.Now);
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_PlusValue -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="offset"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_PlusValue(SqlCommand cmd, string tableName, DataRow field, string transformKey, object offset, InfoAspect aspect)
        {
            TransformField transform = Transform[transformKey];

            try
            {
                // ----------------------------------------------------------------------
                //  Unpack transformation
                // ----------------------------------------------------------------------
                string extractKey     = transform.ExtractKey    ;
                string loadKey        = transform.LoadKey       ;
                string extractActions = transform.ExtractActions;
                string mirrorActions  = transform.MirrorActions ;
                string loadActions    = transform.LoadActions   ;


                // ----------------------------------------------------------------------
                //  Save Parameter
                // ----------------------------------------------------------------------
                if (Load.ContainsKey(loadKey))
                {
                    LoadField load = Load[loadKey];
                    if (load.StoreField > 0 &&
                            load.InTable == tableName &&
                        !cmd.Parameters.Contains(load.Param))
                    if (Extract.ContainsKey(extractKey))
                    {
                        if (RunOkForUpdateOrInsert(loadActions, cmd.CommandText))
                        {
                            ExtractField extract = Extract[extractKey];
                            string typeConversion = extract.DataType + "," + load.DataType;


                            // ----------------------------------------------------------
                            //  Preprocess offset
                            // ----------------------------------------------------------
                            if (offset == null)
                            {
                                if (load.DataType == "string")
                                    offset = "";
                                else
                                    offset = 0;
                            }
                            string strOffset = offset.ToString();
                            if (Regex.IsMatch(strOffset, "^[0-9]+$"))
                            {
                                offset = int.Parse(strOffset);
                            }
                            if (Regex.IsMatch(offset.ToString(), @"\.[0-9]"))
                            {
                                offset = (int)(0.5 + (double)offset * 86400000);
                                typeConversion += ".ms";
                            }


                            switch (typeConversion)
                            {
                                case    "bit,int"     : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlInt32                    (field, extract.Column              )                + (int)offset           ); break;
                                case   "date,date"    : cmd.Parameters.AddWithValue(load.Param, ((DateTime)InData.GetSqlDateTime      (field, extract.Column              )).AddDays        ((int)offset)          ); break;
                                case   "date,date.ms" : cmd.Parameters.AddWithValue(load.Param, ((DateTime)InData.GetSqlDateTime      (field, extract.Column              )).AddMilliseconds((int)offset)          ); break;
                                case    "int,int"     : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultNull    (field, extract.Column , load.Length)                + (int)offset           ); break;
                                case    "int,string"  : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultNull    (field, extract.Column , load.Length).ToString()     +      offset.ToString()); break;
                                case "string,int"     : cmd.Parameters.AddWithValue(load.Param, InData.ExtractIntValue_defaultMinusOne(field, extract.Column , load.Length)                + (int)offset           ); break;
                                case "string,string"  : cmd.Parameters.AddWithValue(load.Param, InData.Truncate (InData.GetSqlString  (field, extract.Column), load.Length)                +      offset.ToString()); break;
                                default:
                                    Pause();
                                    break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToLoad_strlookup -->
        /// <summary>
        ///      Adds a parameter value to the load query using a string lookup table from Xref
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToLoad_strlookup(SqlCommand cmd, string tableName
            , DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                TransformField transform  = Transform[transformKey];
                string         extractKey = transform.ExtractKey;
                string         loadKey    = transform.LoadKey;


                if (Load.ContainsKey(loadKey)          &&
                    Extract.ContainsKey(extractKey)    &&
                    Load[loadKey].InTable == tableName &&
                    Load[loadKey].StoreField > 0
                   )
                {
                    // ------------------------------------------------------------------
                    //  Get the value from the extract table
                    // ------------------------------------------------------------------
                    ExtractField extract = Extract[extractKey];
                    SqlString    inValue = InData.GetSqlString(field, extract.Column);
                    string       lookup  = transform.XrefLookup;
                    string       value   = _stringXref[lookup][inValue.ToString()];


                    // ------------------------------------------------------------------
                    //  Fill the parameter
                    // ------------------------------------------------------------------
                    LoadField    load    = Load[loadKey];
                    switch (load.DataType)
                    {
                        case "bit"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlBoolean (value)); break;
                        case "date"   : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlDateTime(value)); break;
                        case "int"    : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlInt32   (value)); break;
                        case "string" : cmd.Parameters.AddWithValue(load.Param, InData.GetSqlString  (value)); break;
                        default       : Pause(); break;
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToMirror -->
        /// <summary>
        ///      Tests whether a parameter should be added then adds the parameter if so
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToMirror(SqlCommand cmd, DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                // ----------------------------------------------------------------------
                //  Unpack transformation
                // ----------------------------------------------------------------------
                if (transformKey == "IdSuffix") // || transformKey == "NumArrests")
                    Pause();          
                string extractKey     = Transform[transformKey].ExtractKey;
                string mirrorKey      = Transform[transformKey].MirrorKey ;
                string loadKey        = Transform[transformKey].LoadKey   ;

                string extractActions = Transform[transformKey].ExtractActions;
                string mirrorActions  = Transform[transformKey].MirrorActions ;
                string loadActions    = Transform[transformKey].LoadActions   ;


                // ----------------------------------------------------------------------
                //  Health check
                // ----------------------------------------------------------------------
                if (Mirror.ContainsKey(mirrorKey) && Load.ContainsKey(loadKey))
                {
                    if (!Regex.IsMatch
                         (transformKey
                         , "(SubID|Dependence|Email|Internal_ID|Phone|SourceId|UserId)"
                         , RegexOptions.IgnoreCase
                         ) &&
                        !Regex.IsMatch(Transform[transformKey].LoadActions, "M")
                       )
                    {
                        LoadField load = Load[loadKey];
                        MirrorField mirror = Mirror[mirrorKey];
                        double healthIndicator = (load.Length + mirror.Length) / (0.1 + Math.Abs(load.Length - mirror.Length));
                        if (healthIndicator < 8.0)
                            Pause();
                    }
                }


                // ----------------------------------------------------------------------
                //  Save Parameter
                // ----------------------------------------------------------------------
                if (!Regex.IsMatch(mirrorActions, "["+"C"+"D"+"I"+"N"+"]"))
                {
                    if (Extract.ContainsKey(extractKey))
                    {
                        if (Mirror.ContainsKey(mirrorKey))
                        {
                            MirrorField mirror = Mirror[mirrorKey];
                            if (mirror.Active && !cmd.Parameters.Contains(mirror.Param))
                            {
                                AddParameterToMirror_simple(cmd, field, Extract[extractKey], mirror);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToMirror_DsmCode -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="transformKey"></param>
        /// <param name="aspect"></param>
        protected void AddParameterToMirror_DsmCode(SqlCommand cmd, DataRow field, string transformKey, InfoAspect aspect)
        {
            try
            {
                string extractKey = Transform[transformKey].ExtractKey;
                string mirrorKey  = Transform[transformKey].MirrorKey;


                if (Mirror.ContainsKey(mirrorKey)   &&
                    Extract.ContainsKey(extractKey) &&
                    Mirror[mirrorKey].Column.Length > 0
                   )
                {
                    cmd.Parameters.AddWithValue(Mirror[mirrorKey].Param
                        , Xref.DsmCode_Lookup(InData.GetSqlString(field, Extract[extractKey].Column)));
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AddParameterToMirror_simple -->
        /// <summary>
        ///      Simply adds the mirror query parameter without testing whether it should be added
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="field"></param>
        /// <param name="extract"></param>
        /// <param name="mirror"></param>
        private void AddParameterToMirror_simple(SqlCommand cmd, DataRow field, ExtractField extract, MirrorField mirror)
        {
            string typeConversion = extract.DataType + "," + mirror.DataType;
            switch (typeConversion)
            {
                case "bit,bit":
                    cmd.Parameters.AddWithValue(mirror.Param, InData.GetSqlBoolean(field, extract.Column));
                    break;
                case "date,date":
                    cmd.Parameters.AddWithValue(mirror.Param, InData.GetSqlDateTime(field, extract.Column));
                    break;
                case "string,string":
                    SqlString strValue = InData.Truncate(InData.GetSqlString(field, extract.Column), mirror.Length);
                    cmd.Parameters.AddWithValue(mirror.Param, strValue);
                    break;
                case "int,int":
                    SqlString str = InData.Truncate(InData.GetSqlString(field, extract.Column), mirror.Length);
                    SqlInt32 intValue = SqlInt32.Null;
                    if (!str.IsNull)
                    {
                        if (Regex.IsMatch(str.ToString().Trim(), "^[-+0-9]+$"))
                        {
                            intValue = SqlInt32.Parse(str.ToString().Trim());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(str.ToString().Trim()))
                            {
                                intValue = -1;
                            }
                        }
                    }
                    cmd.Parameters.AddWithValue(mirror.Param, intValue);
                    break;
                default:
                    Pause();
                    break;
            }
        }

        #endregion AddParameter

        #region ColumnNames and keys

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnName_ExtractPkFromLoad -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected string ColumnName_ExtractPkFromLoad(string tableName, InfoAspect aspect)
        {
            string idColumn   = "";
            string extractKey = "";


            foreach (TransformField trans in Transform.Values)
            {
                try
                {
                    string loadKey = trans.LoadKey;
                    if (!string.IsNullOrEmpty(loadKey.Trim()) &&
                         Load.ContainsKey(loadKey))
                    {
                        LoadField load = Load[loadKey];
                        if (load.InTable == tableName)
                        {
                            if (Regex.IsMatch(trans.LoadActions, "["+"I"+"K"+"]"))
                            {
                                ExtractField extract = null;
                                extractKey   = trans.ExtractKey;
                                extract      = Extract[extractKey];
                                idColumn = extract.Column;
                            }
                        }
                    }
                } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }


            if (string.IsNullOrEmpty(idColumn.Trim()))
                throw new DataException("No id column found for " + tableName);
            return idColumn;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnName_ExtractPk -->
        /// <summary>
        ///      Finds the extract PK from the Extract list, no table is needed because there is only one import table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected string ColumnName_ExtractPk(InfoAspect aspect)
        {
            string idColumn   = "";
            string extractKey = "";
            foreach (TransformField trans in Transform.Values)
            {
                try
                {
                    if (!string.IsNullOrEmpty(trans.ExtractKey.Trim()))
                    {
                        if (Extract.ContainsKey(trans.ExtractKey))
                        {
                            if (Regex.IsMatch(trans.ExtractActions, "[" + "I" + "K" + "]"))
                            {
                                extractKey = trans.ExtractKey;
                                ExtractField extract = Extract[extractKey];
                                idColumn = extract.Column;
                            }
                        }
                    }
                } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }
            if (string.IsNullOrEmpty(idColumn.Trim()))
                throw new DataException("No id column found for extract.");
            return idColumn;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnName_LoadId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected string ColumnName_LoadId(string tableName, InfoAspect aspect)
        {
            string idColumn = "";

            //foreach (TransformField trans in Transform.Values)
            //{
            //    try
            //    {
            //        string loadKey = trans.LoadKey;
            //        if (!string.IsNullOrEmpty(loadKey) &&
            //             Load.ContainsKey(loadKey))
            //        {
            //            LoadField load = Load[loadKey];
            //            if (load.InTable == tableName) // COLUMN
            //            {
            //                if (Regex.IsMatch(trans.LoadActions, "["+"I"+"K"+"]"))
            //                {
            //                    idColumn = load.Column;
            //                }
            //            }
            //        }
            //    } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            //}

            idColumn = ColumnName_LoadFromActionPattern(tableName, "["+"I"+"K"+"]", aspect.Enter()); aspect--;
            return idColumn;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnName_SourceId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected string ColumnName_SourceId(string tableName, InfoAspect aspect)
        {
            string idColumn = "";

            //foreach (TransformField trans in Transform.Values)
            //{
            //    try
            //    {
            //        string loadKey = trans.LoadKey;
            //        if (!string.IsNullOrEmpty(loadKey) &&
            //             Load.ContainsKey(loadKey))
            //        {
            //            LoadField load = Load[loadKey];
            //            if (load.InTable == tableName) // COLUMN
            //            {
            //                if (Regex.IsMatch(trans.LoadActions, "["+"S"+"]"))
            //                {
            //                    idColumn = load.Column;
            //                }
            //            }
            //        }
            //    } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            //}

            idColumn = ColumnName_LoadFromActionPattern(tableName, "["+"S"+"]", aspect.Enter()); aspect--;
            return idColumn;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ColumnName_LoadPk -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected string ColumnName_LoadPk(string tableName, InfoAspect aspect)
        {
            string idColumn = "";

            //foreach (TransformField trans in Transform.Values)
            //{
            //    try
            //    {
            //        string loadKey = trans.LoadKey;
            //        if (!string.IsNullOrEmpty(loadKey) &&
            //             Load.ContainsKey(loadKey))
            //        {
            //            LoadField load = Load[loadKey];
            //            if (load.InTable == tableName) // COLUMN
            //            {
            //                if (Regex.IsMatch(trans.LoadActions, "["+"I"+"K"+"]"))
            //                {
            //                    idColumn = load.Column;
            //                }
            //            }
            //        }
            //    } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            //}

            idColumn = ColumnName_LoadFromActionPattern(tableName, "["+"I"+"K"+"]", aspect.Enter()); aspect--;
            return idColumn;
        }

        private string ColumnName_LoadFromActionPattern(string tableName, string pattern, InfoAspect aspect)
        {
            string column = "";

            foreach (TransformField trans in Transform.Values)
            {
                try
                {
                    string loadKey = trans.LoadKey;
                    if (!string.IsNullOrEmpty(loadKey.Trim()) &&
                         Load.ContainsKey(loadKey))
                    {
                        LoadField load = Load[loadKey];
                        if (load.InTable == tableName)
                        {
                            if (Regex.IsMatch(trans.LoadActions, pattern))
                            {
                                column = load.Column;
                            }
                        }
                    }
                } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }

            return column;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IdColumnLoadKey -->
        /// <summary>
        ///      Returns the load key for the id column
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected string IdColumnLoadKey(string tableName, InfoAspect aspect)
        {
            string key = "";

            foreach (string transformKey in Transform.Keys)
            {
                try
                {
                    string loadKey = Transform[transformKey].LoadKey;

                    if (Load.ContainsKey(loadKey)          &&
                        Load[loadKey].StoreField > 0       &&
                        Load[loadKey].InTable == tableName && 
                        Regex.IsMatch(Transform[transformKey].LoadActions, "["+"I"+"K"+"]")
                       )
                    {
                        key = loadKey;
                    }
                } catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }

            return key;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MirrorKey_RealId -->
        /// <summary>
        ///      Returns the key for the real db table fk id column for the module_import_item table "ILCM"
        /// </summary>
        /// <returns></returns>
        protected string MirrorKey_RealId()
        {
            string idkey = "";
            foreach (string key in Transform.Keys)
            {
                if (Regex.IsMatch(Transform[key].LoadActions  , "I") &&
                    Regex.IsMatch(Transform[key].MirrorActions, "["+"C"+"F"+"R"+"]")
                   )
                    idkey = Transform[key].MirrorKey;
            }

            return idkey;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TransformKey_ImportId -->
        /// <summary>
        ///      Returns the key for the id column for the module_import_item table "IM"
        /// </summary>
        /// <returns></returns>
        protected string TransformKey_ImportId()
        {
            string idkey = Name + "ImportId";
            foreach (string key in Transform.Keys)
            {
                string profile = Transform[key].MirrorActions;
                if (Regex.IsMatch(profile, "I"))
                    idkey = key;
            }
            return idkey;
        }

        #endregion ColumnNames and keys

        #region Store data

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportErrorQuery -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        internal string ImportErrorQuery(string moduleName)
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

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportErrorFill -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="importId"></param>
        /// <param name="errorCode"></param>
        /// <param name="field"></param>
        /// <param name="moduleName"></param>
        internal void ImportErrorFill(SqlCommand command, int importId, string errorCode, DataRow field, string moduleName)
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
        public void ImportErrorInsert(string query, DataTable statusErrorLog, int row
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
        public int ImportErrorsInsert(DataTable statusErrorLog, Dictionary<int, int> itemIdByLineNum
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
                            , itemIdByLineNum, moduleName, aspect.Enter("user","ImportErrorInsert")); aspect--;
                        errorCount++;
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return errorCount;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportItemInsert -->
        /// <summary>
        ///      Inserts a module import item into the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="importData"></param>
        /// <param name="row"></param>
        /// <param name="summaryId"></param>
        /// <param name="itemIdByLineNum"></param>
        /// <param name="aspect"></param>
        protected void ImportItemInsert(string query, DataTable importData, int row, int summaryId
            , Dictionary<int, int> itemIdByLineNum, string realId_mirrorKey, InfoAspect aspect)
        {
            using (SqlCommand cmd = new SqlCommand(query, aspect.SecondaryConnection))
            {
                try
                {
                    // ----------------------------------------------------------------------
                    //  Resolve parameter values
                    // ----------------------------------------------------------------------
                    DataRow  field  = importData.Rows[row];
                    SqlInt32 realId = InData.GetSqlInt32(field, RealIdColumn);
                    string   status = StatusError.CalculateImportStatus(field, StatusErrorLog, aspect.Enter()); aspect--;


                    // ----------------------------------------------------------------------
                    //  Add standard calculated parameters
                    // ----------------------------------------------------------------------
                    if (Mirror["ImportSummaryId"].Active) cmd.Parameters.AddWithValue(Mirror["ImportSummaryId"].Param, summaryId);
                    if (Mirror["ImportStatus"   ].Active) cmd.Parameters.AddWithValue(Mirror["ImportStatus"   ].Param, status   );
                    if (Mirror[realId_mirrorKey ].Active) cmd.Parameters.AddWithValue(Mirror[realId_mirrorKey ].Param, realId   );


                    // ----------------------------------------------------------------------
                    //  Add simple parameters
                    // ----------------------------------------------------------------------
                    foreach (string transformKey in Transform.Keys)
                    {
                        AddParameterToMirror(cmd, field, transformKey, aspect.Enter());
                        aspect--;
                    }


                    // ----------------------------------------------------------------------
                    //  Record item from import file
                    // ----------------------------------------------------------------------
                    int itemId     = (int)cmd.ExecuteScalar();
                    int lineNumber = int.Parse(importData.Rows[row]["FileLineNumber"].ToString());
                    itemIdByLineNum.Add(lineNumber, itemId);
                }
                catch (Exception ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportItemQuery -->
        /// <summary>
        /// 
        /// </summary>
        protected string ImportItemQuery(InfoAspect aspect)
        {
            string mirrorIdColumnKey = TransformKey_ImportId();
            string query   = "";
            string lastkey = "";
            List<string> keyOrder = new List<string>();

            try
            {
                // --------------------------------------------------------------------------
                //  Gather insert statement mirror keys
                // --------------------------------------------------------------------------
                foreach (string key in Mirror.Keys)
                    if (Mirror[key].Active            &&
                        Mirror[key].Column.Length > 0 &&
                        key != mirrorIdColumnKey
                       )
                        keyOrder.Add(key);
                string delim = "";


                // --------------------------------------------------------------------------
                //  Construct insert statement
                // --------------------------------------------------------------------------
                query += "\r\n" + " INSERT INTO dbo.["+ImportItemTable+"]";
                delim  = "\r\n" + "     ( "; foreach (string key in keyOrder) { lastkey = key; query += delim + Mirror[key].Column; delim = "\r\n     , "; }
                query += "\r\n" + "     ) OUTPUT INSERTED." + Mirror[mirrorIdColumnKey].Column;
                query += "\r\n" + " VALUES";
                delim  = "\r\n" + "     ( "; foreach (string key in keyOrder) { lastkey = key; query += delim + Mirror[key].Param ; delim = "\r\n     , "; }
                query += "\r\n" + "     )";
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return query;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportItemsInsert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="importData"></param>
        /// <param name="summaryId"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public Dictionary<int,int> ImportItemsInsert(DataTable importData, int summaryId, InfoAspect aspect)
        {
            Dictionary<int,int> itemIdByLineNum = new Dictionary<int,int>();
            int row = 0;
            string query = "";
            string idKey = "";

            try
            {
                query = ImportItemQuery(aspect.Enter()); aspect--;
                idKey = MirrorKey_RealId();
                //idKey = ImportId_transformKey();

                for (row = 0; row < importData.Rows.Count; ++row)
                {
                    SqlBoolean safe = !InData.SqlInjectionIdentified(importData.Rows[row]);
                    if (safe)
                    {
                        ImportItemInsert(query, importData, row, summaryId, itemIdByLineNum, idKey, aspect.Enter()); aspect--;
                    }
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return itemIdByLineNum;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportSummaryFill -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="importData"></param>
        /// <param name="statusErrorLog"></param>
        /// <param name="importFilePath"></param>
        protected void ImportSummaryFill(SqlCommand summary, DataTable importData
            , string importFilePath, int importRunId, InfoAspect aspect)
        {
            try
            {
                string dateNow = DateTime.Now.ToString();


                // --------------------------------------------------------------------------
                //  Get counts
                // --------------------------------------------------------------------------
                DataRow[] inserted = importData.Select("IsUpdate = 0 AND IsValid = 1");
                DataRow[] updated  = importData.Select("IsUpdate = 1 AND IsValid = 1");
                int errorCount = StatusErrorLog.Rows.Count - inserted.Length - updated.Length - updated.Length;


                summary.Parameters.AddWithValue("@Import_Date"           , dateNow              );
                summary.Parameters.AddWithValue("@Import_Type"           , "Initial"            );
                summary.Parameters.AddWithValue("@Import_Records_Total"  , importData.Rows.Count);
                summary.Parameters.AddWithValue("@Records_Inserted_Total", inserted.Length      );
                summary.Parameters.AddWithValue("@Records_Updated_Total" , updated.Length       );
                summary.Parameters.AddWithValue("@Import_Errors_Total"   , errorCount           );
                summary.Parameters.AddWithValue("@Import_File_Name"      , importFilePath       );
                summary.Parameters.AddWithValue("@Import_Run_ID"         , importRunId          );
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportSummaryInsert -->
        /// <summary>
        ///      This is the normal version of the file import summary row inserts
        /// </summary>
        /// <param name="importData"></param>
        /// <param name="importFilePath"></param>
        /// <param name="importRunId"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public int ImportSummaryInsert(DataTable importData, string summaryTable, string importFilePath, int importRunId, InfoAspect aspect)
        {
            int summaryId = 0;

            try
            {
                SqlCommand insertSummary = new SqlCommand(ImportSummaryQuery(summaryTable), aspect.SecondaryConnection);
                ImportSummaryFill(insertSummary, importData, importFilePath, importRunId, aspect.Enter()); aspect--;


                using (insertSummary)
                {
                    object obj = insertSummary.ExecuteScalar();
                    if (obj != null && int.TryParse(obj.ToString(), out summaryId)) { } else summaryId = 0;
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return summaryId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportSummaryInsert -->
        /// <summary>
        ///      This is the 'minimal' version for when something goes wrong,
        ///      Writes a summary row, when an exception has prevented the normal writing of the summary row
        /// </summary>
        /// <param name="originalImportFilePath"></param>
        /// <param name="insertedCount"></param>
        /// <param name="errorCount"></param>
        /// <param name="aspect"></param>
        public void ImportSummaryInsert(string summaryTable, DataTable importData, string originalImportFilePath, int insertedCount
            , int errorCount, bool keepHeaders, int importRunId, string copyOfprefix, InfoAspect aspect)
        {
            try
            {
                string importFilePath = originalImportFilePath;


                // --------------------------------------------------------------------------
                //  Get the import data into a table
                // --------------------------------------------------------------------------
                importFilePath = CsvFile.InsureFileHeader(originalImportFilePath, HeaderLine(), keepHeaders, copyOfprefix);


                // --------------------------------------------------------------------------
                //  Prep 1
                // --------------------------------------------------------------------------
                string     summaryQuery = ImportSummaryQuery(summaryTable);
                SqlCommand command      = new SqlCommand(summaryQuery, aspect.SecondaryConnection);
                int count = 0;
                if (importData != null)
                    count = importData.Rows.Count;


                // --------------------------------------------------------------------------
                //  Fill query parameters
                // --------------------------------------------------------------------------
                command.Parameters.AddWithValue("@Import_Date"           , DateTime.Now.ToString());
                command.Parameters.AddWithValue("@Import_Type"           , "Initial"              );
                command.Parameters.AddWithValue("@Import_Records_Total"  , count                  );
                command.Parameters.AddWithValue("@Records_Inserted_Total", insertedCount          );
                command.Parameters.AddWithValue("@Records_Updated_Total" , 0                      );
                command.Parameters.AddWithValue("@Import_Errors_Total"   , 0                      );
                command.Parameters.AddWithValue("@Import_File_Name"      , originalImportFilePath );
                command.Parameters.AddWithValue("@Import_Run_ID"         , importRunId            );


                // --------------------------------------------------------------------------
                //  Insert summary row
                // --------------------------------------------------------------------------
                using (command)
                {
                    object obj = command.ExecuteScalar();
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportSummaryQuery -->
        /// <summary>
        /// 
        /// </summary>
        protected string ImportSummaryQuery(string summaryTableName)
        {
            InfoAspect.Measuring(summaryTableName + ".ImportSummaryQuery");

            string query
                = "\r\n" + " INSERT INTO " + summaryTableName
                + "\r\n" + "     ( Import_Date"
                + "\r\n" + "     , Import_Type"
                + "\r\n" + "     , Import_Records_Total"
                + "\r\n" + "     , Records_Inserted_Total"
                + "\r\n" + "     , Records_Updated_Total"
                + "\r\n" + "     , Import_Errors_Total"
                + "\r\n" + "     , Import_File_Name"
                + "\r\n" + "     , Import_Run_ID"
                + "\r\n" + "     ) OUTPUT INSERTED.Import_Summary_ID"
                + "\r\n" + " VALUES"
                + "\r\n" + "     ( @Import_Date"
                + "\r\n" + "     , @Import_Type"
                + "\r\n" + "     , @Import_Records_Total"
                + "\r\n" + "     , @Records_Inserted_Total"
                + "\r\n" + "     , @Records_Updated_Total"
                + "\r\n" + "     , @Import_Errors_Total"
                + "\r\n" + "     , @Import_File_Name"
                + "\r\n" + "     , @Import_Run_ID"
                + "\r\n" + "     )"
                ;

            InfoAspect.Measured(summaryTableName + ".ImportSummaryQuery");
            return query;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InsertQuery -->
        /// <summary>
        ///      Returns a parameterized query string to insert a row into a module table
        /// </summary>
        /// <returns></returns>
        protected string InsertQuery(string tableName, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Initialize variables
            // --------------------------------------------------------------------------
            string loadKey          = "";
            string transformKey     = "";
            string query            = "";
            string idColumnFieldKey = "";
            int    idCount          = 0;
            bool   turnOffIdentity  = false;
            bool   identityInsert   = false;


            try
            {
                // --------------------------------------------------------------------------
                //  Select the fields to include
                // --------------------------------------------------------------------------
                List<string> keyOrder = new List<string>();
                foreach (string tKey in Transform.Keys)
                {
                    transformKey = tKey;
                    TransformField transform = Transform[transformKey];
                    loadKey      = transform.LoadKey;


                    if (Load.ContainsKey(loadKey))
                    {
                        if ( Load[loadKey].StoreField > 0       &&
                             Load[loadKey].InTable == tableName &&
                            !Regex.IsMatch(transform.LoadActions, "N"))
                        {
                            if (Regex.IsMatch(transform.LoadActions, "["+"K"+"]") &&
                                Regex.IsMatch(transform.LoadActions, "["+"I"+"]")
                               )
                            {
                                keyOrder.Add(loadKey);
                                idCount++;
                            }
                            else if (Regex.IsMatch(transform.LoadActions, "["+"I"+"]"))
                            {
                                idCount++;
                            }
                            else if (Regex.IsMatch(transform.LoadActions, "["+"K"+"]"))
                            {
                                keyOrder.Add(loadKey);
                                idCount++;
                            }
                            else if (!Regex.IsMatch(transform.ExtractActions, "N") ||
                                      Regex.IsMatch(transform.LoadActions, "["+"C"+"D"+"I"+"R"+"V"+"]")
                                    ) // either it exists in the extract or load has another source for it
                            {
                                keyOrder.Add(loadKey);
                            }
                        }
                    }
                }


                idColumnFieldKey = IdColumnLoadKey        (tableName, aspect.Enter()); aspect--;
                turnOffIdentity  = TurnOffIdentityOnInsert(tableName);
                identityInsert   = IdentityInsert         (tableName);


                if (idCount == 1)
                {
                    // ------------------------------------------------------------------
                    //  Construct the insert query
                    // ------------------------------------------------------------------
                    string delim = "";
                    if (turnOffIdentity)               query += "\r\n" + " SET IDENTITY_INSERT ["+tableName+"] ON;";
                                                       query += "\r\n" + " INSERT INTO ["+tableName+"]";
                    delim = "";                        query += "\r\n" + "     ( ";
                    foreach (string key in keyOrder) { query += delim  + Load[key].Column;             delim = "\r\n     , "; }
                                                       query += "\r\n" + "     )";
                    if (identityInsert)                query +=          " OUTPUT INSERTED." + Load[idColumnFieldKey].Column;
                                                       query += "\r\n" + " VALUES ";
                    delim = "";                        query += "\r\n" + "     ( ";
                    foreach (string key in keyOrder) { query += delim  + Load[key].Param ;             delim = "\r\n     , "; }
                                                       query += "\r\n" + "     );";
                    if (turnOffIdentity)               query += "\r\n" + " SET IDENTITY_INSERT ["+tableName+"] OFF;";
                }
                else
                    Pause();
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return query;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IdentityInsert -->
        /// <summary>
        ///      Check to see if identity is on or off in the spec.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected bool IdentityInsert(string tableName)
        {
            // --------------------------------------------------------------------------
            //  Go through the load items, look for a K and/or O,
            //  if it is O, return the identity column, if K or KO, return the id value
            // --------------------------------------------------------------------------
            bool identityInsert = false;

            foreach (string transformKey in Transform.Keys)
            {
                string loadKey = Transform[transformKey].LoadKey;
                if ( Load.ContainsKey(loadKey)                                       &&
                     Load[loadKey].StoreField > 0                                    &&
                     Load[loadKey].InTable == tableName                              &&
                     Regex.IsMatch(Transform[transformKey].LoadActions, "["+"I"+"]") && 
                    !Regex.IsMatch(Transform[transformKey].LoadActions, "["+"K"+"]")
                   )
                {
                    identityInsert = true;
                }
            }

            return identityInsert;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ParameterSanityCheck -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected string ParameterSanityCheck(SqlCommand cmd)
        {
            //bool ok = true;
            string err = "";
            SqlParameterCollection paramList = cmd.Parameters;
            foreach (SqlParameter param in paramList)
            {
                if (param.Value == null)
                {
                    //ok = false;
                    err = "Missing '"+param.ParameterName+"' parameter value.";
                }
            }
            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- TurnOffIdentityOnInsert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TurnOffIdentityOnInsert(string tableName)
        {
            bool   turnOffIdentity  = false;

            turnOffIdentity = false;
            foreach (string transformKey in Transform.Keys)
            {
                string loadKey = Transform[transformKey].LoadKey;

                if (Load.ContainsKey(loadKey)                                       &&
                    Load[loadKey].StoreField > 0                                    &&
                    Load[loadKey].InTable == tableName                              &&
                    Regex.IsMatch(Transform[transformKey].LoadActions, "["+"K"+"]") &&
                    Regex.IsMatch(Transform[transformKey].LoadActions, "["+"I"+"]")
                   )
                {
                    turnOffIdentity = true;
                }
            }

            return turnOffIdentity;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- RunOkForUpdateOrInsert -->
        /// <summary>
        ///      Some fields should only be updated some should only be inserted
        /// </summary>
        /// <param name="loadActions"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private bool RunOkForUpdateOrInsert(string loadActions, string commandText)
        {
            bool                                      run = true;
            if      (Regex.IsMatch(loadActions, "A")) run = Regex.IsMatch(commandText, "INSERT INTO ", RegexOptions.IgnoreCase);
            else if (Regex.IsMatch(loadActions, "U")) run = Regex.IsMatch(commandText, " UPDATE "     , RegexOptions.IgnoreCase);
            if      (Regex.IsMatch(loadActions, "N")) run = false;
            return run;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpdateQuery -->
        /// <summary>
        ///      Replaces the imported fields
        /// </summary>
        protected string UpdateQuery(string tableName, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Initialize variables
            // --------------------------------------------------------------------------
            string loadKey          = "";
            string transformKey     = "";
            string query            = "";
            string idColumnFieldKey = "";
            int    idCount          = 0 ;


            try
            {
                // ----------------------------------------------------------------------
                //  Select the fields to include
                // ----------------------------------------------------------------------
                List<string> keyOrder = new List<string>();
                foreach (string tKey in Transform.Keys)
                {
                    transformKey = tKey;
                    loadKey      = Transform[transformKey].LoadKey;


                    if (Load.ContainsKey(loadKey))
                    {
                        if ( Load[loadKey].StoreField > 0 &&
                             Load[loadKey].InTable == tableName &&
                            !Regex.IsMatch(Transform[transformKey].LoadActions, "N"))
                        {
                            if (Regex.IsMatch(Transform[transformKey].LoadActions, "["+"I"+"K"+"]"))
                            {
                                idColumnFieldKey = loadKey;
                                idCount++;
                            }
                            else
                                keyOrder.Add(loadKey);
                        }
                    }
                }


                if (idCount == 1)
                {
                    // ----------------------------------------------------------------------
                    //  Construct the query string
                    // ----------------------------------------------------------------------
                    query = "\r\n" + " UPDATE dbo.["+tableName+"] SET ";
                    string delim = "\r\n       ";
                    foreach (string key in keyOrder)
                    {
                        query += delim + Load[key].Column.PadRight(20) + " = " + Load[key].Param;
                        delim = "\r\n     , ";
                    }
                    query += "\r\n" + " WHERE "+Load[idColumnFieldKey].Column+" = " + Load[idColumnFieldKey].Param;
                }
                else
                    Pause();
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return query;
        }

        #endregion Store data

        #region Validate data

        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        ///      Checks an import string for errors and length
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="extractKey"></param>
        /// <param name="validate"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool Validate(EtlModule module, DataRow field, string extractKey
            , DataTable validate, InfoAspect aspect)
        {
            bool rowHasError = false;

            try
            {
                if (module.Extract[extractKey].ImportField > 0)
                {
                    // --------------------------------------------------------------------------
                    //  Look for error
                    // --------------------------------------------------------------------------
                    string errorCode = StatusError.CheckForError("-1", field, module.Extract[extractKey], aspect.Enter()); aspect--;
                                                                                                 
                                                                                                
                    // --------------------------------------------------------------------------
                    //  Record error
                    // --------------------------------------------------------------------------
                    if (!string.IsNullOrEmpty(errorCode.Trim()))
                    {
                        string errorMessage = StatusError.ErrorCodeList[errorCode];
                        StatusError.RecordStatusError(errorCode, field, module.StatusErrorLog);
                        rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                            , "ErrorCode_" + extractKey, "ErrorMessage_" + extractKey, errorCode, errorMessage, rowHasError, aspect.Level+1);
                    }

                    return rowHasError;
                }
                else
                {
                    rowHasError = false;
                    return rowHasError;
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return rowHasError;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="fieldKey"></param>
        /// <param name="dataTable"></param>
        /// <param name="validate"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool Validate(EtlModule module, DataRow field, string fieldKey
            , DataTable lookup, string column, DataTable validate, InfoAspect aspect)
        {
            bool rowHasError = false;

            if (module.Extract[fieldKey].ImportField > 0)
            {
                // --------------------------------------------------------------------------
                //  Check for error
                // --------------------------------------------------------------------------
                string errorCode = StatusError.CheckForError
                    ( module.Extract[fieldKey].ImportField   , field
                    , module.Extract[fieldKey].Column  , lookup, column
                    , module.Extract[fieldKey].ErrorFieldName
                    , module.Extract[fieldKey].ErrorRequired 
                    , module.Extract[fieldKey].ErrorFormat   
                    , module.Extract[fieldKey].ErrorLookup   
                    );


                // --------------------------------------------------------------------------
                //  Record error
                // --------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(errorCode.Trim()))
                {
                    string errorMessage = StatusError.ErrorCodeList[errorCode];
                    StatusError.RecordStatusError(errorCode, field, module.StatusErrorLog);
                    rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                        , "ErrorCode_" + fieldKey, "ErrorMessage_" + fieldKey, errorCode, errorMessage, rowHasError, aspect.Level+1);
                }

                return rowHasError;
            }
            else
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Validate -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="fieldKey"></param>
        /// <param name="lookup"></param>
        /// <param name="validate"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool Validate(EtlModule module, DataRow field, string fieldKey
            , NumericLookup lookup, DataTable validate, InfoAspect aspect)
        {
            bool rowHasError = false;

            if (module.Extract[fieldKey].ImportField > 0)
            {
                // --------------------------------------------------------------------------
                //  Look for error
                // --------------------------------------------------------------------------
                string errorCode = StatusError.CheckForErrors
                    ( module.Extract[fieldKey].ImportField, field
                    , module.Extract[fieldKey].Column     , lookup
                    , module.Extract[fieldKey].ErrorFieldName
                    , module.Extract[fieldKey].ErrorRequired 
                    , module.Extract[fieldKey].ErrorFormat   
                    , module.Extract[fieldKey].ErrorLookup, aspect.Enter()
                    );
                aspect--;
                                                                                                 
                                                                                                
                // --------------------------------------------------------------------------
                //  Record error
                // --------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(errorCode.Trim()))
                {
                    string errorMessage = StatusError.ErrorCodeList[errorCode];
                    StatusError.RecordStatusError(errorCode, field, module.StatusErrorLog);
                    rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                        , "ErrorCode_" + fieldKey, "ErrorMessage_" + fieldKey, errorCode, errorMessage, rowHasError, aspect.Level+1);
                }

                return rowHasError;
            }
            else
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValidateExists -->
        /// <summary>
        ///      Validates whether the module row has already been imported into the database
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="extractKey"></param>
        /// <param name="lookIncolumn"></param>
        /// <param name="validate"></param>
        /// <param name="rowHasError"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool ValidateExists(EtlModule module, DataRow field, string extractKey
            , string lookIncolumn, DataTable validate, ref bool rowHasError, InfoAspect aspect)
        {
            bool rowExists = true;

            try
            {
                if (module.Extract[extractKey].ImportField > 0)
                {
                    // --------------------------------------------------------------------------
                    //  Look for error and status
                    // --------------------------------------------------------------------------
                    string statusCode = StatusError.CheckForError
                        ( module.Extract[extractKey].ImportField
                        , module.Extract[extractKey].Length        , field
                        , module.Extract[extractKey].Column
                        , module.Extract[extractKey].Pattern       , module.MainTable, lookIncolumn // EXISTS TAG
                        , module.Extract[extractKey].ErrorFieldName
                        , module.Extract[extractKey].ErrorRequired 
                        , module.Extract[extractKey].ErrorFormat   
                        , module.Extract[extractKey].ErrorLookup
                        , module.Extract[extractKey].ErrorLength
                        , aspect.Enter()
                        );
                    aspect--;


                    if (statusCode == module.Extract[extractKey].ErrorLookup)
                        rowExists = false;


                    // -------------------------------------------------------------------------- // EXISTS TAG 2
                    //  Record error
                    // --------------------------------------------------------------------------
                    if (!string.IsNullOrEmpty(statusCode.Trim()) && statusCode != module.Extract[extractKey].ErrorLookup)
                    {
                        string errorMessage = StatusError.ErrorCodeList[statusCode];
                        StatusError.RecordStatusError(statusCode, field, module.StatusErrorLog);
                        rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                            , "ErrorCode_" + extractKey, "ErrorMessage_" + extractKey, statusCode, errorMessage, rowHasError, aspect.Level+1);
                        rowExists = false;
                    }
                }
                else
                {
                    rowExists = false;
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return rowExists;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValidateColumnCount -->
        /// <summary>
        ///      Validates the column count of the import file
        /// </summary>
        /// <param name="module"></param>
        /// <param name="importData"></param>
        /// <param name="statusErrorLog"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool ValidateColumnCount(EtlModule module, DataTable importData, InfoAspect aspect)
        {
            int expectedCount = module.ImportFileColumnCount;
            if (importData.Columns.Count == expectedCount + 4) return true;
            else return false;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValidateDate -->
        /// <summary>
        ///      Checks an import date for errors and range
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="fieldKey"></param>
        /// <param name="validate"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool ValidateDate(EtlModule module, DataRow field, string fieldKey
            , DateTime? mustBeAfterDate, DateTime? mustBeBeforeDate
            , DataTable validate, InfoAspect aspect)
        {
            bool rowHasError = false;

            if (module.Extract[fieldKey].ImportField > 0)
            {
                // --------------------------------------------------------------------------
                //  Look for error
                // --------------------------------------------------------------------------
                string errorCode = StatusError.CheckForError
                    ( module.Extract[fieldKey].ImportField   , field
                    , module.Extract[fieldKey].Column  
                    , module.Extract[fieldKey].Pattern , mustBeAfterDate, mustBeBeforeDate
                    , module.Extract[fieldKey].ErrorFieldName
                    , module.Extract[fieldKey].ErrorRequired 
                    , module.Extract[fieldKey].ErrorFormat   
                    , module.Extract[fieldKey].ErrorLookup   
                    , aspect.Enter()
                    );
                aspect--;
                                                                                                 
                                                                                                
                // --------------------------------------------------------------------------
                //  Record error
                // --------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(errorCode.Trim()))
                {
                    string errorMessage = StatusError.ErrorCodeList[errorCode];
                    StatusError.RecordStatusError(errorCode, field, module.StatusErrorLog);
                    rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                        , "ErrorCode_" + fieldKey, "ErrorMessage_" + fieldKey, errorCode, errorMessage, rowHasError, aspect.Level+1);
                }

                return rowHasError;
            }
            else
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ValidDate -->
        /// <summary>
        ///      Checks an import date for errors and range
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="fieldKey"></param>
        /// <param name="validate"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static bool ValidDate(EtlModule module, DataRow field, string fieldKey
            , DateTime? mustBeAfterDate, DateTime? mustBeBeforeDate, DataTable validate, InfoAspect aspect)
        {
            bool rowHasError = false;

            if (module.Extract[fieldKey].ImportField > 0)
            {
                // --------------------------------------------------------------------------
                //  Look for error
                // --------------------------------------------------------------------------
                string errorCode = StatusError.CheckForError
                    ( module.Extract[fieldKey].ImportField   , field
                    , module.Extract[fieldKey].Column  
                    , module.Extract[fieldKey].Pattern , mustBeAfterDate, mustBeBeforeDate
                    , module.Extract[fieldKey].ErrorFieldName
                    , module.Extract[fieldKey].ErrorRequired 
                    , module.Extract[fieldKey].ErrorFormat   
                    , module.Extract[fieldKey].ErrorLookup   
                    , aspect.Enter()
                    );
                aspect--;
                                                                                                 
                                                                                                
                // --------------------------------------------------------------------------
                //  Record error
                // --------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(errorCode.Trim()))
                {
                    string errorMessage = StatusError.ErrorCodeList[errorCode];
                    StatusError.RecordStatusError(errorCode, field, module.StatusErrorLog);
                    rowHasError = ImportDalCommon.AddTwoValuesTo(validate, field
                        , "ErrorCode_" + fieldKey, "ErrorMessage_" + fieldKey, errorCode, errorMessage, rowHasError, aspect.Level+1);
                }

                return rowHasError;
            }
            else
            {
                return false;
            }
        }

        #endregion Validate data

        #region Utilities

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckRowForSqlInjection -->
        /// <summary>
        ///      Checks each cell in a row in a datatable for SQL injection
        /// </summary>
        /// <param name="field">the row</param>
        /// <param name="table">the data table</param>
        /// <param name="validate">table to add error column to</param>
        /// <param name="statusErrorLog">table to add error to</param>
        public static bool CheckRowForSqlInjection(DataRow field
            , DataTable validate, DataTable statusErrorLog, DataTable table, ref bool isRowInsertedIntoErrorTable, InfoAspect aspect)
        {
            bool injectionFound = false;
            for (int cell = 0; cell < table.Columns.Count; ++cell)
            {
                if (field[cell] != null && InData.DetectSqlInjection(field[cell].ToString()))
                {
                    StatusError.RecordStatusError("303", field, statusErrorLog);

                    isRowInsertedIntoErrorTable = ImportDalCommon.AddTwoValuesTo(validate, field, "ErrorCode_DataFormat", "ErrorMessage_DataFormat"
                        , "303", StatusError.ErrorCodeList["303"], isRowInsertedIntoErrorTable, aspect.Level+1);
                    injectionFound    = true;
                    field["IsValid"]  = false;
                    field["IsUpdate"] = false;
                }
            }

            return injectionFound;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExecuteScalar -->
        /// <summary>
        ///      Returns the id created or used
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected object ExecuteScalar(SqlCommand cmd, string tableName, InfoAspect aspect)
        {
            // Check to see if identity is on or off in the spec.
            // go through the load items, look for a K and/or O
            // if it is O, return the identity column, if K or KO, return the id value
            if (IdentityInsert(tableName))
            {
                return cmd.ExecuteScalar();
            }
            else
            {
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    string fieldId = IdColumnLoadKey(tableName, aspect.Enter()); aspect--;
                    string param   = Load[fieldId].Param;
                    object id      = cmd.Parameters[param].Value;
                    return id;
                }
                else
                    return null;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HeaderLine -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string HeaderLine()
        {
            var orderedList = Extract
                .Where(x => x.Value.ImportOrder >= 0)
                .OrderBy(x => x.Value.ImportOrder)
                .Select(x => x.Value.Column)
                .ToList();

            string hdr = "";
            string delim = "";
            foreach (string key in orderedList)
            {
                hdr += delim + key;
                delim = ",";
            }
            return hdr;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ImportFileColumnCount -->
        /// <summary>
        ///      Calculates the import column count from the Extract dictionary
        /// </summary>
        public int ImportFileColumnCount { get
        {
            int count = 0;
            foreach (string key in Extract.Keys)
            {
                int value = Extract[key].ImportField;
                if (value > 0) ++count;
            }
            return count;
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckDictionaryConnections -->
        /// <summary>
        ///      Checks to make sure that the transform dictionary connects well with the other dictionaries
        /// </summary>
        /// <returns></returns>
        protected bool CheckDictionaryConnections()
        {
            bool ok = true;

            // --------------------------------------------------------------------------
            //  Check extract information in the transform dictionary
            // --------------------------------------------------------------------------
            foreach (string transformKey in Transform.Keys)
            {
                TransformField transform = Transform[transformKey];
                string extractKey = transform.ExtractKey;
                if (string.IsNullOrEmpty(extractKey.Trim()))
                {
                    if (!Regex.IsMatch(transform.ExtractActions, "N"))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Live extract field '"+transformKey+"' does not have an entry in extract dictionary.");
                    }
                }
                else
                {
                    if (!Extract.ContainsKey(extractKey) &&
                        !Regex.IsMatch(transform.LoadActions, "R"))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Transform extract key '"+transformKey+"' not found in extract dictionary.");
                    }
                }
            }

            // --------------------------------------------------------------------------
            //  Check mirror information in the transform dictionary
            // --------------------------------------------------------------------------
            foreach (string transformKey in Transform.Keys)
            {
                TransformField transform = Transform[transformKey];
                string mirrorKey = transform.MirrorKey;
                if (string.IsNullOrEmpty(mirrorKey.Trim()))
                {
                    if (!Regex.IsMatch(transform.MirrorActions, "N") &&
                        !Regex.IsMatch(transform.MirrorActions, "D")
                       )
                    {
                        ok = false;
                        throw new KeyNotFoundException("Live mirror field '"+transformKey+"' does not have an entry in mirror dictionary.");
                    }
                }
                else
                {
                    if (!Mirror.ContainsKey(mirrorKey))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Transform mirror key '"+transformKey+"' not found in mirror dictionary.");
                    }
                }
            }

            // --------------------------------------------------------------------------
            //  Check load information in the transform dictionary
            // --------------------------------------------------------------------------
            foreach (string transformKey in Transform.Keys)
            {
                TransformField transform = Transform[transformKey];
                string loadKey = transform.LoadKey;
                if (string.IsNullOrEmpty(loadKey.Trim()))
                {
                    if (!Regex.IsMatch(transform.LoadActions, "N"))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Live load field '"+transformKey+"' does not have an entry in load dictionary.");
                    }
                }
                else
                {
                    if (!Load.ContainsKey(loadKey))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Transform load key '"+transformKey+"' not found in load dictionary.");
                    }
                }
            }

            // --------------------------------------------------------------------------
            //  Check extract key coverage in the transform dictionary
            // --------------------------------------------------------------------------
            List<string> transformExtractList = Transform.Select(x => x.Value.ExtractKey).ToList();
            foreach (string transformKey in Extract.Keys)
            {
                ExtractField extract = Extract[transformKey];
                if (extract.ImportOrder >= 0 &&
                   !transformExtractList.Contains(transformKey))
                {
                    ok = false;
                    throw new KeyNotFoundException("Extract key '"+transformKey+"' not found in transform dictionary.");
                }
            }

            // --------------------------------------------------------------------------
            //  Check mirror key coverage in the transform dictionary
            // --------------------------------------------------------------------------
            List<string> transformMirrorList = Transform.Select(x => x.Value.MirrorKey).ToList();
            foreach (string transformKey in Mirror.Keys)
            {
                MirrorField mirror = Mirror[transformKey];
                if (!transformMirrorList.Contains(transformKey))
                {
                    ok = false;
                    throw new KeyNotFoundException("Mirror key '"+transformKey+"' not found in transform dictionary.");
                }
            }

            // --------------------------------------------------------------------------
            //  Check load key coverage in the transform dictionary
            // --------------------------------------------------------------------------
            List<string> transformLoadList = Transform.Select(x => x.Value.LoadKey).ToList();
            foreach (string transformKey in Load.Keys)
            {
                LoadField load = Load[transformKey];
                if (load.StoreField > 0)
                {
                    if (!transformLoadList.Contains(transformKey))
                    {
                        ok = false;
                        throw new KeyNotFoundException("Load key '"+transformKey+"' not found in transform dictionary.");
                    }
                }
                else
                {
                }
            }

            return ok;
        }

        #endregion Utilities

    }
}
