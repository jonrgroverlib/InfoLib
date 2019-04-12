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
using InfoLib.Strings ;               // for __.Pluralize
using System;                         // for StringSplitOptions
using System.Collections.Generic;     // for List<>
using System.Data;                    // for DataTable
using System.Drawing;                 // for Color
using System.Text.RegularExpressions; // for Regex
using System.Web;                     // for HttpResponse
//using System.Web.SessionState;        // for HttpSessionState
//using System.Web.UI;                  // for Page, IsPostBack, Session, Response
//using System.Web.UI.WebControls;      // for GridView, DropDownList, Listitem, Label

namespace InfoLib.Vertical
{
    //// --------------------------------------------------------------------------------------------
    ///// <!-- CommonWebRoutine -->
    ///// <summary>
    /////      The CommonWebRoutine class contains common UI routines
    ///// </summary>
    ///// <remarks>production ready to pre-alpha</remarks>
    //public static class CommonWebRoutine
    //{
    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- AllAccessAllowedPage -->
    //    /// <summary>
    //    ///      Determines whether a page may be viewed without logging in
    //    /// </summary>
    //    /// <param name="filename"></param>
    //    /// <returns></returns>
    //    /// <remarks>deprecated</remarks>
    //    public static bool AllAccessAllowedPage(string filename)
    //    {
    //        if (   filename == "LoginMaster.aspx"
    //            || filename == "Default.aspx"
    //            || filename == "DocumentList.aspx"
    //            || filename == "ForgotPasswordUserName.aspx"
    //            || filename == "ForgotPasswordSecretQst.aspx"
    //            || filename == "NewsItemList.aspx"
    //            || filename == "ResetPassword.aspx"
    //            || filename == "PasswordExpire.aspx"
    //            || filename == "SendEmailToHelpDesk.aspx"
    //            || filename == "UnderConstruction.aspx"
    //           )
    //            return true;
    //        else return false;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- AddFirstColumn -->
    //    /// <summary>
    //    ///      Adds an id column as the first column (which will normally be filled then hidden later on)
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="idColumn">id field in the data source</param>
    //    /// <remarks>production ready</remarks>
    //    private static void AddFirstColumn(GridView grid, string idColumn)
    //    {
    //        BoundField hidden = new BoundField();
    //        hidden.HeaderText = "Id";
    //        hidden.DataField  = idColumn;
    //        grid.Columns.Add(hidden);

    //        grid.Columns[0].Visible = true; // make it visible so it can be filled
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- AddOption -->
    //    /// <summary>
    //    ///      Does stuff
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="value"></param>
    //    /// <param name="text"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void AddOption(DropDownList drop, int value, string text)
    //    {
    //        ListItem item = new ListItem(text, TreatAs.StrValue(value, ""));
    //        if (!drop.Items.Contains(item))
    //            drop.Items.Add(item);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- DataBind -->
    //    /// <summary>
    //    ///      Does a standard databind on a drop down list returning a message if there is an error
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    private static string DataBind(DropDownList drop)
    //    {
    //        string err = "";
    //        try
    //        {
    //            drop.DataBind();
    //            err = "";
    //        }
    //        catch (Exception ex)
    //        {
    //            err = ErrorMessages.Unpack(ex);
    //        }
    //        return err;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- DataBind -->
    //    /// <summary>
    //    ///      Does a specialized DataBind on a GridView or tells why not, it's special because
    //    ///      it databinds hidden columns also
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    private static string DataBind(GridView grid)
    //    {
    //        string err = "";
    //        try
    //        {
    //            HiddenDataBind(grid, 0);
    //          //RenameHeader(grid);
    //            err = "";
    //        }
    //        catch (Exception ex)
    //        {
    //            err = ErrorMessages.Unpack(ex);
    //        }
    //        return err;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- DisplayColumnAsLink -->
    //    /// <summary>
    //    ///      Colors a GridView column as a link column
    //    /// </summary>
    //    /// <param name="grid">the grid to style</param>
    //    /// <param name="gridLinkCol">column number of the grid to use as the link</param>
    //    /// <remarks>production ready</remarks>
    //    public static void DisplayColumnAsLink(GridView grid, int gridLinkCol)
    //    {
    //        if (grid.Columns.Count > gridLinkCol)
    //        {
    //            grid.Columns[gridLinkCol].ItemStyle.ForeColor = Color.Blue;
    //           // grid.Columns[col].ItemStyle.Font.Underline = true;
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- EchoSelection -->
    //    /// <summary>
    //    ///      Tries to copy a dropdown list selection into a text box
    //    /// </summary>
    //    /// <param name="txt"></param>
    //    /// <param name="drop"></param>
    //    /// <param name="lblMessage"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    public static bool EchoSelection(TextBox txt, DropDownList drop, TextBox lblMessage)
    //    {
    //        drop.CssClass = "";
    //        lblMessage.Visible = false;
    //        bool selectionMade = (TreatAs.IntValue(drop.SelectedValue, 0) > 0);
    //        if (selectionMade)
    //            txt.Text = drop.SelectedItem.Text;

    //        return selectionMade;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Empty -->
    //    /// <summary>
    //    ///      Empties a grid
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void Empty(GridView grid)
    //    {
    //        RichDataTable empty = new RichDataTable();
    //        Fill(grid, empty.Table, "");
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Fill -->
    //    /// <summary>
    //    ///      Fills a DropDownList from a data table
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="source"></param>
    //    /// <param name="valueField"></param>
    //    /// <param name="textField"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void Fill(DropDownList drop, DataTable source, string valueField, string textField)
    //    {
    //        try
    //        {
    //            // ----------------------------------------------------------------------
    //            //  Remove current selected value in case it is not in the list when the list gets rebound
    //            // ----------------------------------------------------------------------
    //            //drop.SelectedItem.Selected = false;
    //            if (drop.Items != null) drop.Items.Clear(); // this is the one that worked
    //            drop.SelectedIndex = -1  ;
    //            drop.SelectedValue = null;
    //            drop.ClearSelection()    ;


    //            // ----------------------------------------------------------------------
    //            //  Rebind the list
    //            // ----------------------------------------------------------------------
    //            drop.DataSource     = source    ;
    //            drop.DataValueField = valueField;
    //            drop.DataTextField  = textField ;
    //            drop.DataBind()                 ;
    //        }
    //        catch (Exception rethrownException)
    //        {
    //            Throws.A(rethrownException, Throws.Actions, "P");
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Fill -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="value"></param>
    //    /// <param name="text"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void Fill(DropDownList drop, int value, string text)
    //    {
    //        ListItem found = drop.Items.FindByValue(value.ToString());
    //        if (found == null)
    //            drop.Items.Add(new ListItem(text, value.ToString()));
    //        else
    //            found.Text = text;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Fill -->
    //    /// <summary>
    //    ///      Fills the GridView from a data table
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="table"></param>
    //    /// <param name="idColumn">id field in the data source</param>
    //    /// <remarks>Assumes that the first column contains or will contain the id</remarks>
    //    /// <remarks>production ready</remarks>
    //    public static string Fill(GridView grid, DataTable table, string idColumn)
    //    {
    //        // --------------------------------------------------------------------------
    //        //  Add an id column if necessary, then ill the datagrid
    //        // --------------------------------------------------------------------------
            
    //        grid.DataSource = table;
    //        if (grid.Columns.Count == 0)
    //            { AddFirstColumn(grid, idColumn); grid.DataSource = table; }
    //        return DataBind(grid);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Fill_012 -->
    //    /// <summary>
    //    ///      Fills a dropdown with 0:No access, 1:View only, 2:Full access and setting a default
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="initialValue"></param>
    //    public static void Fill_012(DropDownList drop, string initialValue)
    //    {
    //        RichDataTable standard = new RichDataTable();
    //        standard.Add("Value", typeof(string));
    //        standard.Add("Text" , typeof(string));


    //        standard.Add("Value", "0", "Text", "No Access"  );
    //        standard.Add("Value", "1", "Text", "View Only"  );
    //        standard.Add("Value", "2", "Text", "Full Access");


    //        drop.DataSource     = standard.Table;
    //        drop.DataValueField = "Value";
    //        drop.DataTextField  = "Text";
    //        drop.DataBind();


    //        CommonWebRoutine.SetValue(drop, initialValue);
    //    }

    //    public static string Fill_012(int value, string textFor0, string textFor1, string textFor2)
    //    {
    //        string str = "";
    //        switch (value)
    //        {
    //            case 0 : str = textFor0; break;
    //            case 1 : str = textFor1; break;
    //            case 2 : str = textFor2; break;
    //        }
    //        return str;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FirstTwoLines -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="message"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    private static string FirstNLines(int num, string message)
    //    {
    //        string str = "";
    //        string[] line = message.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //        if (line.Length > num)
    //        {
    //            str = line[0];
    //            for (int i = 1; i < num; ++i)
    //                str += "\r\n" + line[i];
    //        }
    //        else
    //            str = message;
    //        return str;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FloatingErrorDisplay -->
    //    /// <summary>
    //    ///      Handles errors from other screens
    //    /// </summary>
    //    /// <param name="lblMessage"></param>
    //    /// <param name="page"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void FloatingErrorDisplay(TextBox lblMessage, Page page)
    //    {
    //        string err = TreatAs.StrValue(page.Session["FloatingError"], "").Trim();
    //        if (!string.IsNullOrEmpty(err.Trim()))
    //        {
    //            CommonWebRoutine.ShowError(lblMessage, err);
    //            page.Session["FloatingError"] = "";
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FloatingErrorDisplay -->
    //    /// <summary>
    //    ///      Handles errors from other screens
    //    /// </summary>
    //    /// <param name="Session"></param>
    //    /// <param name="lblMessage"></param>
    //    /// <remarks>production ready</remarks>
    //    internal static void FloatingErrorDisplay(HttpSessionState Session, TextBox lblMessage)
    //    {
    //        string err = TreatAs.StrValue(Session["FloatingError"], "").Trim();
    //        if (!string.IsNullOrEmpty(err.Trim()))
    //        {
    //            CommonWebRoutine.ShowError(lblMessage, err);
    //            Session["FloatingError"] = "";
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- FloatOverTo -->
    //    /// <summary>
    //    ///      Carries an error to a new page
    //    /// </summary>
    //    /// <param name="url">page to go to</param>
    //    /// <param name="err">error to carry to it</param>
    //    /// <param name="page">originating page</param>
    //    /// <remarks>production ready</remarks>
    //    public static void FloatErrorOverTo(string url, string err, Page page)
    //    {
    //        page.Session["FloatingError"] = err;
    //        page.Response.Redirect(url);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- gridFillSimple -->
    //    /// <summary>
    //    ///      Fills a grid
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="with">The table to fill the grid with</param>
    //    /// <param name="idColumn"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void gridFillSimple(GridView grid, RichDataTable with, string idColumn, TextBox lblErr)         
    //    {
    //        if (with != null && with.Count > 0)
    //        {
    //            string err = CommonWebRoutine.Fill(grid, with.Table, idColumn);
    //            if (!string.IsNullOrEmpty(err.Trim()))
    //                CommonWebRoutine.ShowError(lblErr, err);
    //            CommonWebRoutine.Style(grid, 1);
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GridRegenerate__doPostBack -->
    //    /// <summary>
    //    ///      This is a hack to regenerate a GridView __doPostBack
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="col"></param>
    //    /// <param name="page"></param>
    //    /// <remarks>production ready</remarks>
    //    private static void GridRegenerate__doPostBack(GridView grid, Page page)
    //    {
    //        if (grid.Rows.Count > 0)
    //            GridSelectionAttachment(grid, grid.Rows[0], 0, page);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GridSelectedCell -->
    //    /// <summary>
    //    ///      Returns the cell selected in the GridView
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="arg"></param>
    //    /// <returns></returns>
    //    /// <remarks>can handle up to 100 columns, see GridSelectionAttachment</remarks>
    //    /// <remarks>production ready</remarks>
    //    public static TableCell GridSelectedCell(GridView grid, object commandArgument)
    //    {
    //        int         arg  = TreatAs.IntValue(commandArgument, 0);
    //        GridViewRow row  = grid.Rows[arg/100];
    //        TableCell   cell = row.Cells[arg%100];
    //        return cell;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GridSelectedId, RolloverSelectedId -->
    //    /// <summary>
    //    ///      Extracts the row id (usually from column 0) (also contains the selection attachment hack)
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="commandArgument"></param>
    //    /// <param name="idCol">GridView column number for the id</param>
    //    /// <param name="page"></param>
    //    /// <returns></returns>
    //    /// <remarks>
    //    ///      1. A hack is to regenerate the __doPostBack
    //    ///      2. Extract row
    //    ///      3. Get and use Level2 id
    //    /// </remarks>
    //    /// <remarks>production ready</remarks>
    //    public static int GridSelectedId0(GridView grid, object commandArgument, int idCol, Page page)
    //    {
    //        GridRegenerate__doPostBack(grid, page);
    //        GridViewRow row = SelectedRow(grid, commandArgument, 0);
    //        int id = TreatAs.IntValue(row.Cells[idCol].Text, 0);
    //        return id;
    //    }
    //    /// <summary>Extracts the row id (from the datakey) (also contains the selection attachment hack)</summary>
    //    public static int RolloverSelectedId(GridView grid, object commandArgument, Page page)
    //    {
    //        GridRegenerate__doPostBack(grid, page);
    //        int arg = TreatAs.IntValue(commandArgument, 0);
    //        int row = arg/100;
    //        return TreatAs.IntValue(grid.DataKeys[row].Value, 0);
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SelectedRow -->
    //    /// <summary>
    //    ///      Returns the row selected in the GridView
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="commandArgument"></param>
    //    /// <returns></returns>
    //    /// <remarks>can handle up to 100 columns, see GridSelectionAttachment</remarks>
    //    public static GridViewRow SelectedRow(GridView grid, object commandArgument, int offset)
    //    {
    //        string ca = commandArgument.ToString();
    //        int arg = TreatAs.IntValue(commandArgument, 0);
    //        int r = 0;
    //        if (ca.Length < 3) r = arg + offset;  // assume the column part got lost and the row part got offset
    //        else  r = arg/100;


    //        GridViewRow row = null;
    //        if (0 <= r && r < grid.Rows.Count)  row = grid.Rows[r];
    //        else Throws.A(new Exception("row is " + r), Throws.Actions, "P");
    //        return row;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- GridSelectionAttachment -->
    //    /// <summary>
    //    ///      Attaches a Select attribute to each cell in the grid view row
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="row"></param>
    //    /// <param name="page"></param>
    //    /// <param name="linkCol">The column that will display the link</param>
    //    /// <remarks>can handle up to 100 columns, see SelectedRow</remarks>
    //    /// <remarks>production ready</remarks>
    //    public static void GridSelectionAttachment(GridView grid, GridViewRow row, int linkCol, Page page)
    //    {
    //        TableCellCollection cellList = row.Cells;

    //        // --------------------------------------------------------------------------
    //        //  Connect row
    //        // --------------------------------------------------------------------------
    //        for (int col = 0; col <= row.Cells.Count - 1; col++)
    //        {
    //            TableCell cell = row.Cells[col];
    //            string index = row.RowIndex.ToString() + col.ToString().PadLeft(2, '0');
    //            if (index.Length < 3)
    //                Throws.A(new Exception("index is incomplete"), Throws.Actions, "P");
    //            string value = "";
    //            if (cell.Controls.Count > 0)
    //            {
    //                if (cell.Controls[0].GetType().BaseType == typeof(LinkButton))
    //                {
    //                    LinkButton btn = ((LinkButton)cell.Controls[0]);
    //                    value = page.ClientScript.GetPostBackEventReference(grid, btn.Text + "$" + index);
    //                }
    //                else
    //                {
    //                    value = page.ClientScript.GetPostBackEventReference(grid, "Command$" + index);
    //                    cell.Attributes.Add("onclick", value);
    //                }
    //            }
    //            else
    //            {
    //                value = page.ClientScript.GetPostBackEventReference(grid, "Select$" + index);
    //                cell.Attributes.Add("onclick", value);
    //            }
    //        }


    //        // --------------------------------------------------------------------------
    //        //  Style id column
    //        // --------------------------------------------------------------------------
    //        bool StyleGridRowSelectAsLink = false;
    //        if (StyleGridRowSelectAsLink)
    //        {
    //            row.Cells[linkCol].Attributes.Add("onmouseover", "this.style.textDecoration='underline';");
    //            row.Cells[linkCol].Attributes.Add("onmouseout" , "this.style.textDecoration='none';"     );
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- HiddenDataBind -->
    //    /// <summary>
    //    ///      Data-binds a grid insuring that one particular hidden column is also bound
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="col"></param>
    //    /// <remarks>production ready</remarks>
    //    private static void HiddenDataBind(GridView grid, int col)
    //    {
    //        grid.Columns[0].Visible = true;
    //        try
    //        {
    //            grid.DataBind();
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        grid.Columns[0].Visible = false;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- LookLikeLabel -->
    //    /// <summary>
    //    ///      Makes a textbox look and behave like a label
    //    /// </summary>
    //    /// <param name="lbl">remember this is a textbox that looks like a label</param>
    //    /// <param name="labelColor">color of 'label' text</param>
    //    /// <remarks>production ready</remarks>
    //    public static void LookLikeLabel(TextBox lbl, Color labelColor)
    //    {
    //        lbl.ReadOnly    = true;
    //        lbl.ForeColor   = labelColor;
    //        //lbl.BackColor   = Color.White;
    //        lbl.BorderStyle = BorderStyle.None;
    //        lbl.Width       = new Unit("100%");      // does this work?
    //        lbl.TextMode    = TextBoxMode.MultiLine; // I need to set height also
    //        lbl.Height      = 30;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Plain -->
    //    /// <summary>
    //    ///      Removes the distinguishing characters after a label
    //    /// </summary>
    //    /// <param name="label"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    private static string Plain(string label)
    //    {
    //        label = Regex.Replace(label, "[ :*]+$", "");
    //        return label;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Prepend -->
    //    /// <summary>
    //    ///      Prepends an item into a dropdown list
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="value"></param>
    //    /// <param name="text"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void Prepend(DropDownList drop, string value, string text)
    //    {
    //        ListItem found = drop.Items.FindByValue(value);
    //        if (found == null)
    //        {
    //            ListItem item = new ListItem(text, value);
    //            if (drop.Items.Count > 0)
    //                drop.Items.Insert(0, item);
    //            else
    //                drop.Items.Add(item);
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetText -->
    //    /// <summary>
    //    ///      Sets teh text of the textbox based on the text selected in the dropdown
    //    /// </summary>
    //    /// <param name="txt"></param>
    //    /// <param name="drop"></param>
    //    /// <param name="defaultText"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void SetText(TextBox txt, DropDownList drop, string defaultText)
    //    {
    //        if (!string.IsNullOrEmpty(TreatAs.StrValue(drop.SelectedItem.Text, "").Trim()))
    //            txt.Text = drop.SelectedItem.Text;
    //        else
    //            txt.Text = defaultText;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetValue -->
    //    /// <summary>
    //    ///      Sets the value of a dropdown list (information oriented version)
    //    ///      adding the default value to the list if it is not yet there
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="value"></param>
    //    /// <param name="defaultValue"></param>
    //    /// <param name="defaultText"></param>
    //    /// <remarks>production ready</remarks>
    //    public static bool SetValue(DropDownList drop, string value, string defaultValue, string defaultText)
    //    {
    //        bool ok = false;
    //        if (drop != null && drop.Items != null && !string.IsNullOrEmpty(value.Trim()))
    //        {
    //            if (drop.Items.FindByValue(value) != null)
    //            {
    //                drop.SelectedValue = value;
    //                ok = true;
    //            }
    //            else
    //            {
    //                if (drop.Items.FindByValue(defaultValue) != null)
    //                {
    //                    drop.SelectedValue = defaultValue;
    //                }
    //                else
    //                {
    //                    ListItem item = new ListItem(defaultText, defaultValue);
    //                    drop.Items.Add(item);
    //                    drop.SelectedValue = defaultValue;
    //                }
    //            }
    //        }
    //        return ok;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetValue --> 
    //    /// <summary>
    //    ///      Moves the selected value in a dropdown to a textbox, resetting the dropdown
    //    /// </summary>
    //    /// <param name="txt"></param>
    //    /// <param name="drop"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void SetValue(TextBox txt, DropDownList drop)
    //    {
    //        if (TreatAs.IntValue(drop.SelectedValue, 0) > 0)
    //        {
    //            txt.Text = drop.SelectedItem.Text;
    //            //SetValue(drop, "0", "0", DropSuggestedItemsMessage);
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetValue --> 
    //    /// <summary>
    //    ///      Moves the selected value in a dropdown to a textbox, resetting the dropdown
    //    /// </summary>
    //    /// <param name="txt"></param>
    //    /// <param name="drop"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void SetValue(TextBox txt, DropDownList drop, string defaultValue)
    //    {
    //        if (!string.IsNullOrEmpty(TreatAs.StrValue(drop.SelectedValue, "").Trim()))
    //            txt.Text = drop.SelectedItem.Text;
    //        else
    //            txt.Text = defaultValue;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetValue -->
    //    /// <summary>
    //    ///      Sets the value of a dropdown list (data oriented version), returning true if the value was found in the dropdown
    //    /// </summary>
    //    /// <param name="drop"></param>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    /// <remarks>production ready</remarks>
    //    public static bool SetValue(DropDownList drop, string value)
    //    {
    //        bool ok = false;
    //        if (drop != null && drop.Items != null
    //            && !string.IsNullOrEmpty(value.Trim())
    //            && drop.Items.FindByValue(value) != null)
    //        {
    //            drop.SelectedValue = value;
    //            ok = true;
    //        }
    //        return ok;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetValueByText -->
    //    /// <summary>
    //    ///      Sets the value of a dropdown by the text input
    //    /// </summary>
    //    /// <param name="dropDocType"></param>
    //    /// <param name="text"></param>
    //    /// <remarks>production ready</remarks>
    //    public static bool SetValueByText(DropDownList drop, string text)
    //    {
    //        bool ok = false;
    //        ListItem item = null;
    //        if (drop != null && drop.Items != null)
    //        {
    //            item = drop.Items.FindByText(                text          ); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(                text.ToLower()); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(                text.ToUpper()); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(__.TitleCase   (text)         ); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(__.SentenceCase(text)         ); if (!ok && item != null) ok = SetValue(drop, item.Value);

    //            string str = __.Pluralize(text);
    //            item = drop.Items.FindByText(                str          ); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(                str.ToLower()); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(                str.ToUpper()); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(__.TitleCase   (str)         ); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //            item = drop.Items.FindByText(__.SentenceCase(str)         ); if (!ok && item != null) ok = SetValue(drop, item.Value);
    //        }
    //        return ok;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowBoolean -->
    //    /// <summary>
    //    ///      Displays boolean data in either a checkbox or a label
    //    /// </summary>
    //    /// <param name="nullable"></param>
    //    /// <param name="lbl"></param>
    //    /// <param name="chk"></param>
    //    /// <param name="txt"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void ShowBoolean(bool value, Label lbl, CheckBox chk, Label txt)
    //    {
    //        if (txt != null)  txt.Visible = false;
    //        if (chk != null)  chk.Visible = false;
    //        bool StyleViewBoolAsCheckbox = true;
    //        if (StyleViewBoolAsCheckbox)
    //        {
    //            chk.Visible  = true;
    //            chk.Checked  = TreatAs.BoolValue(value, false);
    //            lbl.CssClass = "alignwithcheckbox";
    //        }
    //        else
    //        {
    //            txt.Visible  = true;
    //            txt.Text     = TreatAs.StrValue(value, "");
    //            lbl.CssClass = "alignwithlabel";
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowError -->
    //    /// <summary>
    //    ///      Treats a text box like an error message label, we use a textbox so we can give it focus
    //    /// </summary>
    //    /// <param name="lbl">remember this is a textbox that behaves like a label</param>
    //    /// <param name="message"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void ShowError(TextBox lbl, string message)
    //    {
    //        LookLikeLabel(lbl, Color.Red);
    //        lbl.Text    = message;
    //      //lbl.Text    = FirstTwoLines(message);
    //        lbl.Visible = true;
    //        lbl.Height  = 20 + (int)(message.Length / 10);
    //        lbl.Focus(); // because it is a textbox, we can give it focus
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowError -->
    //    /// <summary>
    //    ///      Treats a text box like an error message label, we use a textbox so we can give it focus
    //    /// </summary>
    //    /// <param name="lbl">remember this is a textbox that behaves like a label</param>
    //    /// <param name="message"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void ShowError(Label lbl, string message)
    //    {
    //       // LookLikeLabel(lbl, Color.Red);
    //        lbl.ForeColor = Color.Red;
    //        lbl.Text = FirstNLines(3, message); // message; //
    //        lbl.Visible = true;
    //        lbl.Height = 20 + (int)(message.Length / 10);
    //       // lbl.Focus(); // because it is a textbox, we can give it focus
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowNormalMessage -->
    //    /// <summary>
    //    ///      Treats a text box like an normal message label, uses a textbox so we can give it focus
    //    /// </summary>
    //    /// <param name="lbl">uses a textbox for receiving focus</param>
    //    /// <param name="message"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void ShowNormalMessage(TextBox lbl, string message)
    //    {
    //        LookLikeLabel(lbl, Color.Black);
    //        lbl.Text      = FirstNLines(7, message);
    //        lbl.Visible   = true;
    //        lbl.Focus(); // because it is a textbox, we can give it focus
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ShowWarning -->
    //    /// <summary>
    //    ///      Treats a text box like an normal message label, uses a textbox to receive focus
    //    /// </summary>
    //    /// <param name="lbl">uses a textbox so it can receive focus</param>
    //    /// <param name="message"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void ShowWarning(TextBox lbl, string message)
    //    {
    //        LookLikeLabel(lbl, Color.Green);
    //        lbl.Text      = FirstNLines(7, message);
    //        lbl.Visible   = true;
    //        lbl.Focus(); // because it is a textbox, we can give it focus
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SiteColorTable -->
    //    /// <summary>
    //    ///      A list of site colors
    //    /// </summary>
    //    /// <returns></returns>
    //    /// <remarks>
    //    ///      This is very UI specific and therefore is better to keep here than in the database,
    //    ///      good arguments could be made for the opposite position though
    //    /// </remarks>
    //    public static RichDataTable SiteColorTable()
    //    {
    //        RichDataTable color = new RichDataTable("Color", "CssClass");

    //        color.Add("CssClass" , typeof(string));
    //        color.Add("ColorName", typeof(string));

    //        color.Add("CssClass", "0", "ColorName", "original"           );
    //        color.Add("CssClass", "1", "ColorName", "sound blue"         );
    //        color.Add("CssClass", "2", "ColorName", "purply blue"        );
    //        color.Add("CssClass", "3", "ColorName", "dark greyish blue"  );
    //        color.Add("CssClass", "4", "ColorName", "bluish grey"        );
    //        color.Add("CssClass", "5", "ColorName", "gentle blue"        );
    //        color.Add("CssClass", "6", "ColorName", "serious blue"       );
    //        color.Add("CssClass", "7", "ColorName", "true blue"          );
    //        color.Add("CssClass", "8", "ColorName", "greyish aqua blue"  );
    //        color.Add("CssClass", "9", "ColorName", "charcoal blue"      );
                                                                         
    //        color.Add("CssClass", "A", "ColorName", "Atlantic salmon"    );
    //        color.Add("CssClass", "B", "ColorName", "brick"              );
    //        color.Add("CssClass", "C", "ColorName", "chocolate"          );
    //        color.Add("CssClass", "D", "ColorName", "dark muddy brown"   );
    //        color.Add("CssClass", "E", "ColorName", "tan pumpkin"        );
    //        color.Add("CssClass", "F", "ColorName", "faded green"        );
    //        color.Add("CssClass", "G", "ColorName", "strong green"       );
    //        color.Add("CssClass", "H", "ColorName", "muted orange"       );
    //        color.Add("CssClass", "I", "ColorName", "template indigo"    );
    //        color.Add("CssClass", "J", "ColorName", "plumb"              );
    //        color.Add("CssClass", "K", "ColorName", "wine"               );
    //        color.Add("CssClass", "L", "ColorName", "lovely lavendar"    );
    //        color.Add("CssClass", "M", "ColorName", "mint chocolate chip");
    //        color.Add("CssClass", "N", "ColorName", "neutral grey"       );
    //        color.Add("CssClass", "O", "ColorName", "olive"              );
    //        color.Add("CssClass", "P", "ColorName", "pink"               );
    //        color.Add("CssClass", "R", "ColorName", "template red"       );

    //        return color;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- Style --> 
    //    /// <summary>
    //    ///      Styles a GridView
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="linkCol">column number of grid to use as link</param>
    //    /// <remarks>production ready</remarks>
    //    public static void Style(GridView grid, int linkCol)
    //    {
    //        bool styleGridRowSelectAsLink = false;
    //        if (styleGridRowSelectAsLink)
    //        {
    //            grid.CssClass = "table";
    //            DisplayColumnAsLink(grid, linkCol);
    //        }
    //        else
    //            grid.CssClass = "rollovertable";
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- TryTo -->
    //    /// <summary>
    //    ///      A syntactic candy wrapper for a method that returns an error string,
    //    ///      TryTo displays the error string if the error message string is not empty
    //    /// </summary>
    //    /// <param name="err">the error message string</param>
    //    /// <param name="lbl">text box to display the error in</param>
    //    /// <remarks>
    //    ///      Use this sparingl,
    //    ///      It is only intended for use when there are a number of TryTo statemenst in a row
    //    /// </remarks>
    //    /// <remarks>production ready</remarks>
    //    public static string TryTo(string err, TextBox lbl)
    //    {
    //        if (!string.IsNullOrEmpty(err.Trim()))
    //            CommonWebRoutine.ShowError(lbl, err);
    //        return err;
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- SetPagesToZeroExcept -->
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="thisPage"></param>
    //    /// <param name="session"></param>
    //    /// <remarks>production ready</remarks>
    //    public static void SetPagesToZeroExcept(string thisPage, HttpSessionState session)
    //    {
    //        // --------------------------------------------------------------------------
    //        //  This is cached:
    //        // --------------------------------------------------------------------------
    //        if (_pageIndexList == null)
    //        {
    //            _pageIndexList = new List<string>();
    //            _pageIndexList.Add("AccessPageIndex"          );
    //            _pageIndexList.Add("AgencyPageIndex"          );
    //            _pageIndexList.Add("AssessmentModulePageIndex");
    //            _pageIndexList.Add("AssessmentTypePageIndex"  );
    //            _pageIndexList.Add("ConfigItemPageIndex"      );
    //            _pageIndexList.Add("ConfigurationPageIndex"   );
    //            _pageIndexList.Add("CertificationPageIndex"   );
    //            _pageIndexList.Add("ConsumerLocPageIndex"     );
    //            _pageIndexList.Add("ConsumerPageIndex"        );
    //            _pageIndexList.Add("ConsumerExportPageIndex"  );
    //            _pageIndexList.Add("ConsumerSearchPageIndex"  );
    //            _pageIndexList.Add("ConsumerRequestPageIndex" );
    //            _pageIndexList.Add("CountyPageIndex"          );
    //            _pageIndexList.Add("DocumentPageIndex"        );
    //            _pageIndexList.Add("Dsm4PageIndex"            );
    //            _pageIndexList.Add("NewsPageIndex"            );
    //            _pageIndexList.Add("DocumentTypePageIndex"    );
    //            _pageIndexList.Add("EthnicityPageIndex"       );
    //            _pageIndexList.Add("GenderPageIndex"          );
    //            _pageIndexList.Add("Level1PageIndex"          );
    //            _pageIndexList.Add("Level2PageIndex"          );
    //            _pageIndexList.Add("LocationExportPageIndex"  );
    //            _pageIndexList.Add("ProviderPageIndex"        );
    //            _pageIndexList.Add("QuestionPageIndex"        );
    //            _pageIndexList.Add("RacePageIndex"            );
    //            _pageIndexList.Add("ReferralPageIndex"        );
    //            _pageIndexList.Add("RolePageIndex"            );
    //            _pageIndexList.Add("UserPageIndex"            );
    //        }


    //        // --------------------------------------------------------------------------
    //        //  Check to see that we have not left one out
    //        // --------------------------------------------------------------------------
    //        if (!_pageIndexList.Contains(thisPage))
    //            Throws.A(new KeyNotFoundException("Tell a programmer to add "+thisPage+" to the Page Index List"
    //                + " in the 'SetPagesToZeroExcept' method."), Throws.Actions, "P");
    //        object idx = session[thisPage];


    //        for (int i = 0; i < _pageIndexList.Count; ++i)
    //        {
    //            string assocIdx = _pageIndexList[i];
    //            if (assocIdx != thisPage)
    //                session[assocIdx] = 0;
    //        }
    //    }
    //    private static List<string> _pageIndexList;


    //    // ----------------------------------------------------------------------------------------
    //    //  Trace control selectors
    //    // ----------------------------------------------------------------------------------------
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, string str)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0) TraceAction(session, str);
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, Label lbl, TextBox txt)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail == 1) Trace1(session, lbl, txt);
    //        if (detail == 2) Trace2(session, lbl, txt);
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, Label lbl, DropDownList drop)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail == 1) Trace1(session, lbl, drop);
    //        if (detail == 2) Trace2(session, lbl, drop);
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, Button btn)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0) Trace1(session, btn);
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, string str, Button btn)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0) TraceAction(session, str);
    //        if (detail > 0) Trace1(session, btn);
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, Label lbl, CheckBox chk)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0)
    //        {
    //            if (chk.Checked) TraceX(session, lbl);
    //            else             TraceO(session, lbl);
    //        }
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, CheckBox chk)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0)
    //        {
    //            if (chk.Checked) TraceX(session, chk);
    //            else             TraceO(session, chk);
    //        }
    //    }
    //    /// <remarks>alpha code</remarks>
    //    public static void TracePage(HttpSessionState session, Label lblTitle)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0) TracePage1(session, lblTitle);
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- TraceAction -->
    //    /// <summary>
    //    ///      Adds a line to the user trace list
    //    /// </summary>
    //    /// <param name="session"></param>
    //    /// <param name="str"></param>
    //    /// <remarks>alpha code</remarks>
    //    private static void TraceAction(HttpSessionState session, string str)
    //    {
    //        if (session["TraceDetail"] != null)
    //        {
    //            if (session["GuiActionTrace"] == null)
    //                session["GuiActionTrace"] = new List<string>();
    //            List<string> gat = (List<string>)session["GuiActionTrace"];
    //            gat.Add(str);
    //            session["GuiActionTrace"] = gat;
    //        }
    //    }


    //    // ----------------------------------------------------------------------------------------
    //    //  Simple Trace controls            alpha code
    //    // ----------------------------------------------------------------------------------------
    //    private static void Trace1(HttpSessionState session,            Button       btn ) { TraceAction(session,                          "["    + btn.Text                      +    "]" + ", "); }
    //    private static void Trace1(HttpSessionState session, Label lbl, TextBox      txt ) { TraceAction(session, Plain(lbl.Text) + ": " + "[_<"  + Plain(lbl.Text)               +  ">_]" + ", "); }
    //    private static void Trace2(HttpSessionState session, Label lbl, TextBox      txt ) { TraceAction(session, Plain(lbl.Text) + ": " + "[_<"  + Plain(txt.Text)               +  ">_]" + ", "); }
    //    private static void Trace1(HttpSessionState session, Label lbl, DropDownList drop) { TraceAction(session, Plain(lbl.Text) + ": " + "[<"   + Plain(lbl.Text)               +">\\/]" + ", "); }
    //    private static void Trace2(HttpSessionState session, Label lbl, DropDownList drop) { TraceAction(session, Plain(lbl.Text) + ": " + "[<"   + Plain(drop.SelectedItem.Text) +">\\/]" + ", "); }
    //    private static void TraceX(HttpSessionState session, Label lbl                   ) { TraceAction(session,                          "[X] " + Plain(lbl.Text)                        + ", "); }
    //    private static void TraceO(HttpSessionState session, Label lbl                   ) { TraceAction(session,                          "[_] " + Plain(lbl.Text)                        + ", "); }
    //    private static void TraceX(HttpSessionState session,            CheckBox     chk ) { TraceAction(session,                          "[X] " + Plain(chk.Text)                        + ", "); }
    //    private static void TraceO(HttpSessionState session,            CheckBox     chk ) { TraceAction(session,                          "[_] " + Plain(chk.Text)                        + ", "); }
    //    private static void TracePage1(HttpSessionState session, Label lblTitle          ) { TraceAction(session,                          "|"    + Plain(lblTitle.Text)                   + ", "); }


    //    /// <remarks>alpha code</remarks>
    //    public static void Trace(HttpSessionState session, HttpRequest request, string parameter)
    //    {
    //        int detail = TreatAs.IntValue(session["TraceDetail"], 0);
    //        if (detail > 0)
    //        {
    //            string code = request.QueryString[parameter];
    //            switch (code)
    //            { // top level M:RCR menu, S:state menu, A:local admin menu, C:consumer menu, R:report menu, U:user menu
    //                case "MX"  : case "mx"  : Trace(session, "[" + "RCR"                           + " Administrator"                                                                            + "]"); break;
    //            }
    //        }
    //    }

    //    // ----------------------------------------------------------------------------------------
    //    /// <!-- ValidateDate --> 
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="strDate"></param>
    //    /// <returns></returns>
    //    /// <remarks>alpha code</remarks>
    //    public static bool ValidateDate(string strDate)
    //    {
    //        try
    //        {
    //            //string reg = "^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$"
    //            String date = Convert.ToDateTime(String.Format("{0:MM/dd/yyyy}", strDate)).ToShortDateString();

    //            DateTime format;
    //            bool blCorrectFormat = DateTime.TryParseExact(date, new[] { "MM/dd/yyyy", "M/d/yyyy" }, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out format);

    //            //DateTime d = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //}
}

