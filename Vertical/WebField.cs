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
using InfoLib.Endemes ;               // for EndemeSet
using InfoLib.SoftData;               // for TreatAs
using System;                         // for EventArgs
using System.Collections.Generic;     // for List
using System.Data;                    // for CommandType
using System.Drawing;                 // for Point, Color
using System.Text.RegularExpressions; // for Regex
//using System.Web.SessionState;        // for HttpSessionState
//using System.Web.UI.WebControls;      // for Button

namespace InfoLib.Vertical
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- WebField -->
    ///// <summary>
    /////      ASP.NET field handler for loosely coupled field vertical architecture
    ///// </summary>
    ///// <remarks>
    /////      Warning - this class is mostly just a toy until I get EndemeCollection working
    /////      
    /////      Notes:
    /////      1. Warning - This is not designed for speed, if you want speed, use data orientation
    /////      2. Anything additional is handled ad hoc in code behind, aspx or in css
    /////      3. not practical until I can get the EndemeCollection class below properly written
    /////      4. this class plays well with others so it can be used for either loosely or tightly coupled fields
    /////      
    ///// 
    /////      alpha toy code
    ///// </remarks>
    //public class WebField
    //{
    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- OnChangeEvent -->
    //    /// <summary>
    //    ///      A delegate for events that fire when some data is changed in a control
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="a"></param>
    //    /// <remarks>
    //    ///      Example use:
    //    ///      
    //    ///      OnChangeEvent MethodRun = chkMedicaidInd_CheckedChanged; // this saves it
    //    ///      MethodRun(new object(), new EventArgs());                // this runs it
    //    ///      MethodRun(sender, a);                                    // this too runs it
    //    ///      string name = MethodRun.Method.Name;                     // this looks at its name
    //    /// </remarks>
    //    public delegate void OnChangeEvent(object sender, EventArgs a);


    //    // ----------------------------------------------------------------------------------------
    //    //  22 Member Properties, anything additional is handled in code behind, aspx, css, or CrudModule
    //    // ----------------------------------------------------------------------------------------
    //                                                              /// <summary>'args' parameter for the OnChangedEvent</summary>     //      DB MT UI CSS Notes
    //    public EventArgs                  Args      { get; set; } /// <summary>Width/height of the text/view control</summary>       // * Q.   [M]        (should this be a List<string>?)
    //    public Point                      Box       { get; set; } /// <summary>a place to reference the CheckBox control</summary>   // * B.[D]           (intentionally denormalized, this is stored in every field) 
    //    public CheckBox                   Chk       { get; set; } /// <summary>a place to reference the DropDownList</summary>       //   C.       U 
    //    public DropDownList               Drop      { get; set; } /// <summary>OnABC_XYZChanged, is this needed?</summary>           //   L.    M  U 
    //    public OnChangeEvent              EventFn   { get; set; } /// <summary>field/column in the database</summary>                //   E.    M  U 
    //    public string                     Field     { get; set; } /// <summary>label for the field label or grid header</summary>    //   F. D       
    //    public GridView                   Grid      { get; set; } /// <summary>name of the table where the field is kept</summary>   //   A.       U 
    //    public string                     HomeTable { get; set; } /// <summary>the column the id is kept in</summary>                //   H.    M  U 
    //    public string                     IdColumn  { get; set; } /// <summary>where to look for data for a dropdownlist</summary>   // I. D              (intentionally denormalized, this is stored in every field)
    //    public RichDataTable              Join      { get; set; } /// <summary>hidden key on page for view/edit screens</summary>    //   J.    M    
    //    public HiddenField                Key       { get; set; } /// <summary>name/field/label control</summary>                    //   K. D  M         (intentionally denormalized, this is stored in every field)
    //    public Label Lbl { get { return _l; } } private Label _l; /// <summary>a place to store error messages returned</summary>    //   D.       U 
    //    public TextBox                    Msg       { get; set; } /// <summary>is the data numeric (or boolean) or not</summary>     //   M. ?       
    //    public bool                       Numeric   { get; set; } /// <summary>the order of the column in the display grid</summary> //   N. D  M  U      (is this an order by and order number or what?  I don't remember what this is heare for)
    //    public int                        Order     { get; set; } /// <summary>AutoPostBack</summary>                                //   O.       U 
    //    public bool                       PostBack  { get; set; } /// <summary>the results of a select on the field?</summary>       //   P.    M  U 
    //    public RichDataTable   /*unused*/ Query     { get; set; } /// <summary>required field control, disabled if not</summary>     // * G.      [U]
    //    public RequiredFieldValidator     Req       { get; set; } /// <summary>'sender' parameter for the onchanged event</summary>  //   R.       U  C
    //    public object                     Sender    { get; set; } /// <summary>a place to reference the TextBox control</summary>    //   S.       U      (arguments)
    //    public TextBox                    Txt       { get; set; } /// <summary>lists which other tables use rows from this table including the table name (key) and its foreign key field (value)</summary>    //   T.       U 
    //    public Dictionary<string,string>  UsedBy    { get; set; } /// <summary>the value display control on the view page</summary>  //   U. D  M    
    //    public Label                      View      { get; set; }                                                                    //   V.    M  U 

    //    /// <summary>universal information descriptor/locator</summary>    //   T.       U 


    //    // ----------------------------------------------------------------------------------------
    //    //  Properties
    //    // ----------------------------------------------------------------------------------------
    //    public string ChangeMethodName { get { return EventFn.Method.Name; } }


    //    // ----------------------------------------------------------------------------------------
    //    //  Constructors
    //    // ----------------------------------------------------------------------------------------
    //    public WebField(                                                       ) { PostBack = false;                                                   Msg = new TextBox(); }
    //    public WebField(Label lbl,                    TextBox       txt        ) { PostBack = false;              _l = lbl; Txt = txt;               Msg = new TextBox(); }
    //    public WebField(Label lbl, DropDownList drop                           ) { PostBack = false; Drop = drop; _l = lbl;                          Msg = new TextBox(); }
    //    public WebField(Label lbl, DropDownList drop, TextBox       txt        ) { PostBack = false; Drop = drop; _l = lbl; Txt = txt;               Msg = new TextBox(); }
    //    public WebField(Label lbl,                    CheckBox      chk        ) { PostBack = false;              _l = lbl;            Chk = chk;    Msg = new TextBox(); }
    //    public WebField(Label lbl, DropDownList drop, OnChangeEvent changeEvent) { PostBack = false; Drop = drop; _l = lbl; EventFn = changeEvent; Msg = new TextBox(); }
    //    public WebField(Label lbl, TextBox  txt ,          string header, string tableName, string idColumn, string columnName) { Txt = txt;                Init(lbl        , header, tableName, idColumn, columnName); }
    //    public WebField(Label lbl, GridView grid, int ord, string header, string tableName, string idColumn, string columnName) { Grid = grid; Order = ord; Init(lbl        , header, tableName, idColumn, columnName); }
    //    public WebField(                                   string header, string tableName, string idColumn, string columnName) {                           Init(new Label(), header, tableName, idColumn, columnName); }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- WebFieldMembers -->
    //    /// <summary>
    //    ///      A:rgs, B)ox, C)hk, D)rop, E)ventFn, F)ield, G)rid, H)omeTable, I)dColumn, J)umpTo, 
    //    ///      K)ey, L)bl, M)sg, N)umeric, O)rder, P)ostBack, Q)uery, R)eq, S)ender, T)xt, 
    //    ///      U)niversal, V)iewLabel 
    //    /// </summary>
    //    public static EndemeSet WebFieldMembers
    //    {
    //        get
    //        {
    //            if (_webFieldMembers == null)
    //            {
    //                _webFieldMembers = new EndemeSet("Web Field");

    //                _webFieldMembers.Add('A', "Args"     , "'args' parameter for the OnChangedEvent"                 );
    //                _webFieldMembers.Add('B', "Box"      , "Width/height of the text control"                        );
    //                _webFieldMembers.Add('C', "Chk"      , "a place to reference the CheckBox control"               );
    //                _webFieldMembers.Add('D', "Drop"     , "a place to reference the DropDownList control"           );
    //                _webFieldMembers.Add('E', "EventFn"  , "OnABC_XYZChanged, is this needed?"                       );
    //                _webFieldMembers.Add('F', "Field"    , "field/column in the database"                            );
    //                _webFieldMembers.Add('G', "Grid"     , "the grid for displaying the table"                       );
    //                _webFieldMembers.Add('H', "HomeTable", "name of the table where the field is kept"               );
    //                _webFieldMembers.Add('I', "IdColumn" , "the column the id is kept in"                            );
    //                _webFieldMembers.Add('J', "JoinTo"   , "where to look for data for a dropdownlist for this field");
    //                _webFieldMembers.Add('K', "Key"      , "hidden key on page for view/edit screens"                );
    //                _webFieldMembers.Add('L', "Lbl"      , "name/field/label control"                                );
    //                _webFieldMembers.Add('M', "Msg"      , "a place to store error messages returned"                );
    //                _webFieldMembers.Add('N', "Numeric"  , "is the data numeric (or boolean) or not"                 );
    //                _webFieldMembers.Add('O', "Order"    , "the order of the column in the display grid"             );
    //                _webFieldMembers.Add('P', "PostBack" , "AutoPostBack"                                            );
    //                _webFieldMembers.Add('Q', "Query"    , "contains the results of a select statement on the field?");
    //                _webFieldMembers.Add('R', "Req"      , "required field control, disabled if not"                 );
    //                _webFieldMembers.Add('S', "Sender"   , "'sender' parameter for the onchanged event"              );
    //                _webFieldMembers.Add('T', "Txt"      , "a place to reference the TextBox control"                );
    //                _webFieldMembers.Add('U', "UsedBy"   , "indicates which other tables use rows from this table"   );
    //                _webFieldMembers.Add('V', "View"     , "the value display control on the view page"              );
    //            }

    //            return _webFieldMembers;
    //        }
    //    }
    //    private static EndemeSet _webFieldMembers;


    //    // ----------------------------------------------------------------------------------------
    //    //  Short methods and properties
    //    // ----------------------------------------------------------------------------------------
    //                                                                                             /// <summary>Copies the field information directly to the database</summary>
    //    public void Copy    (string ConnectionStringName)        { Let(Scrape(), ConnectionStringName);                        }
    //    /// <summary>Fills a field directly from the database</summary>
    //    public void Fill    (string ConnectionStringName)        { Plaster(GetObject(ConnectionStringName));                 }   /// <summary>Plasters information to a web page field</summary>
    //    public void Plaster (object obj     )        { Txt.Text = TreatAs.StrValue(obj, ""); }   /// <summary>Runs an event method realated to a change in the field's information</summary>
    //    public void Run     (               )        { EventFn(Sender, Args);                }   /// <summary>Stores the controls of a label/textbox tuple</summary>
    //    public void SetTuple(Label lbl, TextBox txt) { SetLabel(lbl); Txt = txt;             }
    //    public void SetTupleDrop(Label lbl, DropDownList drp) { SetLabel(lbl); Drop = drp; }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- DeleteRow -->
    //    /// <summary>
    //    ///      Deletes the row that the field is in
    //    /// </summary>
    //    /// <param name="isUsedBy">a dictionary of tables and foreign keys where this row may be used</param>
    //    /// <returns></returns>
    //    public string DeleteRow(Dictionary<string,string> isUsedBy, string ConnectionStringName)
    //    {
    //        string err = "";


    //        if (Key != null)
    //        {
    //            int id = TreatAs.IntValue(this.Key.Value, 0);


    //            int useCount = 0;
    //            // --------------------------------------------------------------------------
    //            //  Test for use in another table
    //            // --------------------------------------------------------------------------
    //            if (isUsedBy != null)
    //            {
    //                foreach (string tableName in isUsedBy.Keys)
    //                {
    //                    string query = "SELECT COUNT(*) FROM [" + tableName + "] WHERE [" + isUsedBy[tableName] + "] = " + id;
    //                    RichSqlCommand checkCmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //                        , query, Throws.Actions, "P");
    //                    useCount += (int)checkCmd.ExecuteScalar(0);
    //                }
    //            }


    //            if (useCount > 0)
    //            {
    //                err = "This record is used " + useCount + " times, you may not delete it.";
    //            }
    //            else
    //            {
    //                // --------------------------------------------------------------------------
    //                //  Build a filter from the webfield
    //                // --------------------------------------------------------------------------
    //                string fromFilter = " FROM " + this.HomeTable + " WHERE " + IdColumn + " = " + id;


    //                // --------------------------------------------------------------------------
    //                //  Check to make sure only one row will be deleted by the filter
    //                // --------------------------------------------------------------------------
    //                RichSqlCommand get = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //                    , "SELECT COUNT(*)" + fromFilter
    //                    , Throws.Actions, "P")
    //                    ._AddParameter_safe("@Id", Key.Value, 32);
    //                int count = TreatAs.IntValue(get.ExecuteScalar(0), 0);


    //                // --------------------------------------------------------------------------
    //                //  Delete the row or say why not
    //                // --------------------------------------------------------------------------
    //                if (count == 1)
    //                {
    //                    RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //                        , "DELETE" + fromFilter
    //                        , Throws.Actions, "P")
    //                        ._AddParameter_safe("@Id", Key.Value, 32);
    //                    cmd.ExecuteNonQuery();
    //                    err = cmd.Errors;
    //                    //  TODO: handle foreign key constraints
    //                }
    //                else
    //                    err = "Row not found for " + this.HomeTable + ", id: " + id;
    //            }
    //        }
    //        else
    //        {
    //            err = "hidden key empty";
    //        }


    //        if (!string.IsNullOrEmpty(err.Trim()))
    //            CommonWebRoutine.ShowError(Msg, err);
    //        return err;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Get -->
    //    /// <summary>
    //    ///      Gets the data for a field from the database
    //    /// </summary>
    //    /// <returns></returns>
    //    public object GetObject(string ConnectionStringName)
    //    {
    //        RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //            , " SELECT " + Field
    //            + " FROM   " + HomeTable
    //            + " WHERE  " + IdColumn + " = @Id"
    //            , Throws.Actions, "P")
    //            ._AddParameter_safe("@Id", Key.Value, 32);
    //        object obj = cmd.ExecuteScalar(null, DateTime.Now);


    //        if (!string.IsNullOrEmpty(cmd.Errors.Trim())) { Msg.ForeColor = Color.Red; Msg.Text = cmd.Errors; }
    //        return obj;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GetRow -->
    //    /// <summary>
    //    ///      Returns the row that contains the field
    //    /// </summary>
    //    /// <returns></returns>
    //    public RichDataTable GetRow(string ConnectionStringName)
    //    {
    //        RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //            , " SELECT *"
    //            + " FROM  " + HomeTable
    //            + " WHERE " + IdColumn + " = @Id"
    //            , Throws.Actions, "P")
    //            ._AddParameter_safe("@Id", Key.Value, 32);
    //        RichDataTable table = new RichDataTable(cmd, HomeTable, IdColumn);


    //        if (!string.IsNullOrEmpty(table.Errors.Trim())) { Msg.ForeColor = Color.Red; Msg.Text = table.Errors; }
    //        return table;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Init -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="lbl"></param>
    //    /// <param name="header"></param>
    //    /// <param name="tableName"></param>
    //    /// <param name="idColumn"></param>
    //    /// <param name="columnName"></param>
    //    private void Init(Label lbl, string header, string tableName, string idColumn, string columnName)
    //    {
    //        Msg      = new TextBox();
    //        Field      = columnName;
    //        IdColumn   = idColumn;
    //        _l       = lbl;
    //        Lbl.Text   = header;
    //        PostBack   = false;
    //        HomeTable  = tableName;


    //        // --------------------------------------------------------------------------
    //        //  If it's unlabelled, it's not visible by default
    //        // --------------------------------------------------------------------------
    //        if (Lbl != null && Lbl.Parent != null)
    //        {
    //            if (string.IsNullOrEmpty(header.Trim()) && string.IsNullOrEmpty(columnName.Trim()))
    //                Lbl.Parent.Visible = false;
    //            else
    //                Lbl.Parent.Visible = true;
    //        }


    //        if (Lbl != null && Grid != null && -1 < Order && Order < Grid.Columns.Count)
    //        {
    //            Grid.Columns[Order].HeaderText = lbl.Text;
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- JoinTo -->
    //    /// <summary>
    //    ///      Sets a join to this column to build dropdown lists from later on
    //    /// </summary>
    //    /// <param name="tableName"></param>
    //    /// <param name="idColumnName"></param>
    //    /// <param name="idColumnType"></param>
    //    /// <param name="textColumn"></param>
    //    public void JoinTo(string tableName, string idColumnName, Type idColumnType, string textColumn)
    //    {
    //        Join = new RichDataTable(tableName, idColumnName);
    //        Join.Add(idColumnName, idColumnType);
    //        Join.Add(textColumn  , typeof(string));
    //        Join.SetPK(0);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Let -->
    //    /// <summary>
    //    ///      Sets the value of a field in the database
    //    /// </summary>
    //    /// <param name="str"></param>
    //    public void Let(object obj, string ConnectionStringName)
    //    {
    //        string str = TreatAs.StrValue(obj, "");

    //        RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, ConnectionStringName
    //            , " UPDATE " + HomeTable
    //            + " SET    " + Field     + " = @Value"
    //            + " WHERE  " + IdColumn  + " = @Id"
    //            , Throws.Actions, "P")
    //            ._AddParameter_safe("@Id"   , Key.Value,   32)
    //            ._AddParameter_safe("@Value", str      , 4000);
    //        cmd.ExecuteNonQuery();


    //        if (!string.IsNullOrEmpty(cmd.Errors.Trim())) { Msg.ForeColor = Color.Red; Msg.Text = cmd.Errors; }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- LookLikeLabel -->
    //    /// <summary>
    //    ///      Makes a textbox look and behave like a label
    //    /// </summary>
    //    /// <param name="lbl">remember this is a textbox that looks like a label</param>
    //    /// <param name="labelColor">color of 'label' text</param>
    //    public static void LookLikeLabel(TextBox lbl, Color labelColor)
    //    {
    //        lbl.ReadOnly    = true;
    //        lbl.ForeColor   = labelColor;
    //        lbl.BorderStyle = BorderStyle.None;
    //        lbl.Width       = new Unit("100%");      // does this work?
    //        lbl.TextMode    = TextBoxMode.MultiLine; // I need to set height also
    //        lbl.Height      = 30;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Scrape -->
    //    /// <summary>
    //    ///      Scrapes information from a web page field
    //    /// </summary>
    //    /// <returns></returns>
    //    public string Scrape()
    //    {
    //        string str = "";
    //        if      (Drop != null) str = Drop.SelectedValue;
    //        else if (Txt  != null) str = Txt.Text;
    //        return str;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetRequired -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="req"></param>
    //    /// <param name="errMessage"></param>
    //    /// <param name="onOrOff"></param>
    //    public void SetRequired(RequiredFieldValidator req, string errMessage, bool onOrOff)
    //    {
    //        Req              = req;
    //        Req.ErrorMessage = errMessage;
    //        Req.Enabled      = onOrOff;
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetLabel -->
    //    /// <summary>
    //    ///      Sets the Lbl field, saving the old text if ther is no new text
    //    /// </summary>
    //    /// <param name="lbl"></param>
    //    public void SetLabel(Label lbl)
    //    {
    //        // --------------------------------------------------------------------------
    //        //  Set the label control
    //        // --------------------------------------------------------------------------
    //        if (   Lbl != null
    //            &&  string.IsNullOrEmpty(lbl.Text.Trim())
    //            && !string.IsNullOrEmpty(Lbl.Text.Trim()))
    //        {
    //            string text = Lbl.Text;
    //            _l = lbl;
    //            Lbl.Text = text;
    //        }
    //        else
    //            _l = lbl;


    //        // --------------------------------------------------------------------------
    //        //  Set 'label' visibility, actually sets its parents visibility
    //        // --------------------------------------------------------------------------
    //        if (Lbl != null && Lbl.Parent != null)
    //        {
    //            if (string.IsNullOrEmpty(Lbl.Text.Trim()) && string.IsNullOrEmpty(Field.Trim()))
    //                Lbl.Parent.Visible = false;
    //            else
    //                Lbl.Parent.Visible = true;
    //        }
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowError -->
    //    /// <summary>
    //    ///      Treats a text box like an error message label, we use a textbox so we can give it focus
    //    /// </summary>
    //    /// <param name="lbl">remember this is a textbox that behaves like a label</param>
    //    /// <param name="message"></param>
    //    public static void ShowError(TextBox lbl, string message)
    //    {
    //        LookLikeLabel(lbl, Color.Red);
    //        lbl.Text      = message;
    //        //lbl.Text      = FirstTwoLines(message);
    //        lbl.Visible   = true;
    //        lbl.Height = 20 + (int)(message.Length / 10);
    //        lbl.Focus(); // because it is a textbox, we can give it focus
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Trace -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="session"></param>
    //    /// <param name="fieldList"></param>
    //    public static void Trace(HttpSessionState session, Dictionary<string, WebField> fieldList)
    //    {
    //        foreach (WebField field in fieldList.Values)
    //        {
    //            if      (field.Chk  != null) CommonWebRoutine.Trace(session, field.Lbl, field.Chk );
    //            else if (field.Drop != null) CommonWebRoutine.Trace(session, field.Lbl, field.Drop);
    //            else if (field.Txt  != null) CommonWebRoutine.Trace(session, field.Lbl, field.Txt );
    //            else                         CommonWebRoutine.Trace(session, field.Lbl.ToString() );
    //        }
    //    }

    //}
}