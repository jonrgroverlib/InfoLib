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
using InfoLib.Data    ;               // for RichSqlCommand
using InfoLib.HardData;               // for ResistSqlInjection
using InfoLib.SoftData;               // for TreatAs
using InfoLib.Strings ;               // for __.Pluralize, __.PrettyLabel
using System;                         // for EventArgs
using System.Collections.Generic;     // for List
using System.Data;                    // for CommandType
using System.Drawing;                 // for Point, Color
using System.Linq;                    // for OrderBy
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex
//using System.Web.UI.WebControls;      // for Button

namespace InfoLib.Vertical
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- CrudModule -->
    ///// <summary>
    /////      A CRUD screen handling class for vertical architectures
    ///// </summary>
    ///// <remarks>
    /////      Do not use this for any modules that are complex:
    /////      - it does not handle modules with numeric columns
    /////      - it does not handle modules with date columns
    /////      - it only sorts by one column
    /////      - it does however handle simple dropdown lookup tables
    /////      
    /////      alpha toy code
    ///// </remarks>
    //public class CrudModule
    //{
    //    private enum ModuleScreen { Add, Edit, Load, View }


    //    // ----------------------------------------------------------------------------------------
    //    //  Members
    //    // ----------------------------------------------------------------------------------------
    //                                                                 /// <summary>module label and title</summary>
    //    public string                       Label      { get; set; } /// <summary>column for searching and naming</summary>
    //    public string                       NameColumn { get; set; } /// <summary>column for ordering (and searching?)</summary>
    //    public string                       OrderBy    { get; set; } /// <summary>module path and page prefix</summary>
    //    public string                       Path       { get; set; }
    //    public Dictionary<string, WebField> ForAdd     { get; set; }
    //    public Dictionary<string, WebField> ForEdit    { get; set; }
    //    public Dictionary<string, WebField> ForLoad    { get; set; }
    //    public Dictionary<string, WebField> ForView    { get; set; } /// <summary>indicates which other tables use rows from this table including the table name (key) and its foreign key field (value)</summary>
    //    public Dictionary<string,string>    IsUsedBy   { get; set; }


    //    // ----------------------------------------------------------------------------------------
    //    //  Constructor
    //    // ----------------------------------------------------------------------------------------
    //    public CrudModule(string moduleLabel)
    //    {
    //        Label    = moduleLabel;
    //        ForAdd   = new Dictionary<string,WebField>();
    //        ForEdit  = new Dictionary<string,WebField>();
    //        ForLoad  = new Dictionary<string,WebField>();
    //        ForView  = new Dictionary<string,WebField>();
    //        IsUsedBy = new Dictionary<string,string>  ();
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    //  Short properties and methods
    //    // ----------------------------------------------------------------------------------------
    //    public List<WebField> FieldListForAdd  { get { return ListFor(ModuleScreen.Add ); } }
    //    public List<WebField> FieldListForEdit { get { return ListFor(ModuleScreen.Edit); } }
    //    public List<WebField> FieldListForLoad { get { return ListFor(ModuleScreen.Load); } }
    //    public List<WebField> FieldListForView { get { return ListFor(ModuleScreen.View); } }


    //    public void PlasterEdit(string   conn  ) { List<WebField> field = FieldListForEdit; CrudModule.Plaster(field, CrudModule.Get(field, conn), field[0].Msg); }
    //    public void PlasterView(string   conn  ) { List<WebField> field = FieldListForView; CrudModule.Plaster(field, CrudModule.Get(field, conn), field[0].Msg); }
    //    public void SetGrid    (GridView grid  ) { List<WebField> field = FieldListForLoad; for (int i = 0; i < field.Count; ++i) field[i].Grid = grid; }
    //    public void SetMessage (TextBox  lblMsg) { SetMessageC(lblMsg); SetMessageR(lblMsg); SetMessageU(lblMsg); SetMessageD(lblMsg); }


    //    public void SetMessageC(TextBox  lblMsg) { List<WebField> field = FieldListForAdd ; for (int i = 0; i < field.Count; ++i) field[i].Msg = lblMsg; }
    //    public void SetMessageR(TextBox  lblMsg) { List<WebField> field = FieldListForEdit; for (int i = 0; i < field.Count; ++i) field[i].Msg = lblMsg; }
    //    public void SetMessageU(TextBox  lblMsg) { List<WebField> field = FieldListForLoad; for (int i = 0; i < field.Count; ++i) field[i].Msg = lblMsg; }
    //    public void SetMessageD(TextBox  lblMsg) { List<WebField> field = FieldListForView; for (int i = 0; i < field.Count; ++i) field[i].Msg = lblMsg; }


    //    private static void SetKey (List      <WebField>         field    , HiddenField txtId           ) { for (int i = 0; i < field.Count; ++i) field[i].Key    = txtId;    }
    //    public  static void SetKey (Dictionary<string, WebField> fieldList, HiddenField txtId, int rowId) { txtId.Value = rowId.ToString(); SetKey (CrudModule.ToList(fieldList), txtId   ); }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ToList -->
    //    /// <summary>
    //    ///      Converts a WebField dictionary to a WebField list
    //    /// </summary>
    //    /// <param name="dictionary"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    private static List<WebField> ToList(Dictionary<string, WebField> dictionary)
    //    {
    //        List<WebField> list = new List<WebField>(dictionary.Count);
    //        foreach (WebField item in dictionary.Values)
    //            list.Add(item);
    //        return list;
    //    }
    //    private List<WebField> ListFor(ModuleScreen screen)
    //    {
    //        List<WebField> list = null;
    //        switch (screen)
    //        {
    //            case ModuleScreen.Add  : list = ToList(ForAdd ); break;
    //            case ModuleScreen.Edit : list = ToList(ForEdit); break;
    //            case ModuleScreen.Load : list = ToList(ForLoad); break;
    //            case ModuleScreen.View : list = ToList(ForView); break;
    //            default : throw new NotImplementedException("CrudModule.ToList");
    //        }
    //        return list;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- CheckRequired -->
    //    /// <summary>
    //    ///      Determines whether all required fields are filled
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public static bool CheckRequired(List<WebField> field)
    //    {
    //        bool ok = true;
    //        for (int i = 0; i < field.Count; ++i)
    //            if (field[i].Txt != null
    //                && field[i].Req != null && field[i].Req.Enabled
    //                && string.IsNullOrEmpty(field[i].Txt.Text.Trim()))
    //                    ok = false;
    //        return ok;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Display -->
    //    /// <summary>
    //    ///      Displays a table in a grid
    //    /// </summary>
    //    /// <param name="table"></param>
    //    /// <remarks>alpha code</remarks>
    //    public void Display(RichDataTable table)
    //    {
    //        List<WebField> field = FieldListForLoad;                                       
    //        if (table != null && table.Count > 0 && field.Count > 0)                                                 
    //        {
    //            DataControlFieldCollection column = field[0].Grid.Columns;
    //            ((BoundField)column[0]).DataField = field[0].IdColumn;


    //            List<WebField> list = field
    //                .OrderBy(x => x.Order)
    //                .Select(x => x)
    //                .ToList();


    //            for (int i = 0; i < list.Count && i <= column.Count; ++i)
    //            {
    //                ((BoundField)column[i+1]).DataField = list[i].Field;
    //                ((BoundField)column[i+1]).HeaderText = list[i].Lbl.Text;
    //                ((BoundField)column[i+1]).Visible = true;
    //            }


    //            string err = CommonWebRoutine.Fill(field[0].Grid, table.Table, field[0].IdColumn);
    //            if (!string.IsNullOrEmpty(err.Trim()))                                              
    //                CommonWebRoutine.ShowError(field[0].Msg, err);                                  
    //            CommonWebRoutine.Style(field[0].Grid, 1);                                         
    //        }                                                                                     
    //        else                                                                                  
    //            CommonWebRoutine.ShowWarning(field[0].Msg                                         
    //                , "No " + __.Pluralize(Label).ToLower() + " found");                   
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FieldColumnList -->
    //    /// <summary>
    //    ///      Returns a comma delimited string list of the columns but not the id column
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public static string FieldColumnList(List<WebField> field)
    //    {
    //        string list = "";
    //        string delim = "";
    //        for (int i = 0; i < field.Count; ++i)
    //        {
    //            list += delim + field[i].Field;
    //            delim = ", ";
    //        }
    //        return list;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FindRecordsLike -->
    //    /// <summary>
    //    ///      Returns the records similar to the keyword
    //    /// </summary>
    //    /// <param name="keyword"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public RichDataTable FindRecordsLike(string keyword, string ConnectionStringName)
    //    {
    //        List<WebField> field = FieldListForLoad;
    //        string columnList = CrudModule.FieldColumnList(field);
    //        int id = TreatAs.IntValue(keyword, 0);


    //        RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //            , "\r\n" + " SELECT "   + field[0].IdColumn + ", " + columnList
    //            + "\r\n" + " FROM "     + field[0].HomeTable + " WITH(NOLOCK)"
    //            + "\r\n" + " WHERE "    + NameColumn + " LIKE '%" + InData.ResistSqlInjection(keyword,30) + "%'"
    //            + "\r\n" + "     OR "   + field[0].IdColumn + " = " + id.ToString()
    //            + "\r\n" + " ORDER BY " + OrderBy
    //            , Throws.Actions, "P");
    //        return new RichDataTable(cmd, "Certification", field[0].IdColumn);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Get -->
    //    /// <summary>
    //    ///      Selects the fields in the row identified by the internal WebField id
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public static RichDataTable Get(List<WebField> field, string ConnectionStringName)
    //    {
    //        StringBuilder query = new StringBuilder("SELECT");


    //        string delim = " ";
    //        for (int i = 0; i < field.Count; ++i)
    //            { query.Append(delim + field[i].Field);  delim = ", "; }


    //        int rowId = TreatAs.IntValue(field[0].Key.Value, 0);
    //        query.Append("\r\n FROM " + field[0].HomeTable);
    //        query.Append("\r\n WHERE " + field[0].IdColumn + " = " + rowId);


    //        RichSqlCommand cmd   = new RichSqlCommand(CommandType.Text, ConnectionStringName, query.ToString(), Throws.Actions, "P");
    //        RichDataTable  table = new RichDataTable(cmd, field[0].HomeTable, field[0].IdColumn);


    //        return table;
    //    }
    //    public static RichDataTable Get(Dictionary<string, WebField> field, string conn) { return Get(CrudModule.ToList(field), conn); }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Get -->
    //    /// <summary>
    //    ///      Returns all rows inteh module table
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public RichDataTable GetAll(string ConnectionStringName)
    //    {
    //        List<WebField> field = FieldListForLoad;
    //        string columnList = CrudModule.FieldColumnList(field);


    //        RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //            , "\r\n" + " SELECT "   + field[0].IdColumn + ", " + columnList
    //            + "\r\n" + " FROM "     + field[0].HomeTable
    //            + "\r\n" + " ORDER BY " + OrderBy
    //            , Throws.Actions, "P");
    //        return new RichDataTable(cmd, Label, "");
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Insert -->
    //    /// <summary>
    //    ///      Inserts the fields returning the id of the row inserted
    //    /// </summary>
    //    /// <param name="fieldList"></param>
    //    /// <returns></returns>
    //    /// <remarks>assumes identy primary key column TODO: fix so it works without an identity column also</remarks>
    //    /// <remarks>alpha code</remarks>
    //    public object Insert(object defaultId, string ConnectionStringName)
    //    {
    //        object id = null;
    //        Dictionary<string, WebField> fieldList = ForAdd;

    //        List<WebField> field = CrudModule.ToList(fieldList);
    //        bool ok = CheckRequired(field);


    //        if (ok)
    //        {
    //            // --------------------------------------------------------------------------
    //            //  Construct the insert query (columns clause)
    //            // --------------------------------------------------------------------------
    //            string query = "\r\n" + "INSERT INTO [" + field[0].HomeTable + "]";
    //            string delim = "\r\n" + "        ( ";
    //            for (int i = 0; i < field.Count; ++i)
    //            {
    //                if (!string.IsNullOrEmpty(field[i].Field.Trim()))
    //                    { query += delim + "[" + field[i].Field + "]"; delim = "\r\n" + "        , "; }
    //            }
    //            query += "\r\n" + "        ) Output Inserted.[" + field[0].IdColumn + "]";


    //            // --------------------------------------------------------------------------
    //            //  Construct the insert query (values clause)
    //            // --------------------------------------------------------------------------
    //            delim = "\r\n" + " VALUES ( ";
    //            for (int i = 0; i < field.Count; ++i)
    //            {
    //                if (!string.IsNullOrEmpty(field[i].Field.Trim()))
    //                {
    //                    // --------------------------------------------------------------
    //                    //  Get value to store
    //                    // --------------------------------------------------------------
    //                    string value = field[i].Txt.Text;
    //                    if (field[i].Drop != null && field[i].Drop.Items != null && field[i].Drop.Items.Count > 0)
    //                        value = field[i].Drop.SelectedValue.ToString();


    //                    // --------------------------------------------------------------
    //                    //  Add value to store
    //                    // --------------------------------------------------------------
    //                    if (field[i].Numeric)
    //                        query += delim + " " + TreatAs.IntValue(value, 0) + " ";
    //                    else query += delim + "'" + InData.ResistSqlInjection(value, 255).Trim() + "'";
    //                    delim = "\r\n" + "        , ";
    //                }
    //            }
    //            query += "\r\n" + "        )";


    //            // --------------------------------------------------------------------------
    //            //  Run the insert query
    //            // --------------------------------------------------------------------------
    //            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName, query, Throws.Actions, "P");
    //            id = cmd.ExecuteScalar(defaultId, DateTime.Now);
    //        }


    //        if (id == null)
    //            CommonWebRoutine.ShowError(field[0].Msg, "Missing required value");
    //        if (id == defaultId)
    //            CommonWebRoutine.ShowError(field[0].Msg, "Default id used");
    //        return id;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Plaster -->
    //    /// <summary>
    //    ///      Plasters a data table into the page
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <param name="table"></param>
    //    /// <param name="lblMessage"></param>
    //    /// <remarks>alpha code</remarks>
    //    public static void Plaster(List<WebField> field, RichDataTable table, TextBox lblMessage)
    //    {
    //        if (table.Count > 0)
    //        {
    //            string rowId = table.StrValue(0, field[0].IdColumn, "0");
    //            if (string.IsNullOrEmpty(field[0].Key.Value.Trim()))
    //                field[0].Key.Value = table.StrValue(0, field[0].IdColumn, "0");
    //            if (field[0] != null && field[0].Txt  != null) field[0].Txt.Text  = table.StrValue(0, field[0].Field, "");
    //            if (field[0] != null && field[0].View != null) field[0].View.Text = table.StrValue(0, field[0].Field, "");


    //            for (int i = 1; i < field.Count; ++i)
    //            {
    //                if (field[i] != null)
    //                {
    //                    if (field[i].Txt  != null) field[i].Txt.Text  = table.StrValue(0, field[i].Field, "");
    //                    if (field[i].View != null) field[i].View.Text = table.StrValue(0, field[i].Field, "");
    //                }
    //            }


    //            //if (field[2] != null && field[2].Txt  != null) field[2].Txt.Text  = table.StrValue(0, field[2].Field, "");
    //            //if (field[2] != null && field[2].View != null) field[2].View.Text = table.StrValue(0, field[2].Field, "");
    //        }
    //        else
    //            CommonWebRoutine.ShowError(lblMessage, "Certification "+field[0].Key.Value+" not found.");
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetFields -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="fieldList"></param>
    //    /// <remarks>alpha code</remarks>
    //    public static void SetFields(Dictionary<string, WebField> fieldList, string requiredSymbol, string requiredMessage, string DropInitialSelectMessage, string ConnectionStringName)
    //    {
    //        foreach (WebField field in fieldList.Values)
    //        {
    //            // ----------------------------------------------------------------------
    //            //  Set label
    //            // ----------------------------------------------------------------------
    //            if (field.Lbl != null && string.IsNullOrEmpty(field.Lbl.Text.Trim()))
    //            {
    //                if      (!string.IsNullOrEmpty(field.Field.Trim()))  field.Lbl.Text = __.PrettyLabel(field.Field); 
    //                else                                                    field.Lbl.Text = __.PrettyLabel(field.Lbl.ID);
    //                if (field.Req != null && field.Req.Enabled)
    //                    field.Lbl.Text += requiredSymbol;
    //            }


    //            // ----------------------------------------------------------------------
    //            //  Set text box
    //            // ----------------------------------------------------------------------
    //            if (field.Txt != null)
    //            {
    //                if (field.Box != null)
    //                {
    //                    if (field.Box.X > 7) field.Txt.Width  = field.Box.X;
    //                    if (field.Box.Y > 7)
    //                    {
    //                        field.Txt.Height = field.Box.Y;
    //                        if (field.Box.Y > 30)
    //                            field.Txt.TextMode = TextBoxMode.MultiLine;
    //                    }
    //                }
    //            }


    //            // ----------------------------------------------------------------------
    //            //  Set required field controls
    //            // ----------------------------------------------------------------------
    //            if (field.Req != null && field.Req.Enabled)
    //            {
    //                field.Req.Text      = requiredMessage; //"required field";
    //                field.Req.ForeColor = Color.Red;
    //                //if (string.IsNullOrEmpty(field.HaveToHave.ControlToValidate))
    //                //    field.HaveToHave.ControlToValidate = field.Box.ID;
    //            }


    //            if (field.Join != null)
    //            {
    //                string query = "";
    //                query = "SELECT " + field.Join.Columns[0].ColumnName + ", " + field.Join.Columns[1].ColumnName
    //                    + " FROM "+field.Join.TableName
    //                    + " ORDER BY "+field.Join.Columns[1].ColumnName;
    //                RichSqlCommand cmd = new RichSqlCommand(CommandType.Text
    //                    , ConnectionStringName
    //                    , query, Throws.Actions, "P");
    //                field.Join = new RichDataTable(cmd, field.Join.TableName, "");


    //                CommonWebRoutine.Fill(field.Drop, field.Join.Table, field.Join.Columns[0].ColumnName, field.Join.Columns[1].ColumnName);
    //                CommonWebRoutine.Prepend(field.Drop, "0", DropInitialSelectMessage);
    //            }
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Update -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <returns>error string</returns>
    //    /// <remarks>alpha code</remarks>
    //    public string Update(string ConnectionStringName)
    //    {
    //        List<WebField> field = ListFor(ModuleScreen.Edit);

    //        bool ok = CheckRequired(field);
    //        RichSqlCommand cmd = null;

    //        if (ok)
    //        {
    //            string query = "\r\n" + " UPDATE [" + field[0].HomeTable + "] SET";
    //            string delim = "\r\n" + "        ";
    //            for (int i = 0; i < field.Count; ++i)
    //            {
    //                if (!string.IsNullOrEmpty(field[i].Field.Trim()))
    //                {
    //                    // --------------------------------------------------------------
    //                    //  Get value to store
    //                    // --------------------------------------------------------------
    //                    string value = field[i].Txt.Text;
    //                    if (field[i].Drop != null && field[i].Drop.Items != null && field[i].Drop.Items.Count > 0)
    //                        value = field[i].Drop.SelectedValue.ToString();


    //                    query += delim + "[" + field[i].Field + "] = " + "'" + InData.ResistSqlInjection(value, 255).Trim() + "'";
    //                    delim = "\r\n" + "      , ";
    //                }
    //            }
    //            query += "\r\n" + " WHERE "+field[0].IdColumn+" = " + field[0].Key.Value;


    //            cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //                , query
    //                , Throws.Actions, "P");
    //            cmd.ExecuteNonQuery();
    //        }
    //        else
    //            return "other than one record found";


    //        if (!string.IsNullOrEmpty(cmd.Errors.Trim()))
    //            CommonWebRoutine.ShowError(field[0].Msg, cmd.Errors);

    //        return cmd.Errors;
    //    }
    //}
}