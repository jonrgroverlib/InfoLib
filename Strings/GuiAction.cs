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
using System;                         // for EventArgs(1)
using System.Reflection;              // for BindingFlags, MethodInfo
using System.Windows.Forms;           // for many(57)
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- GuiAction -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>production ready unit test code</remarks>
    public class GuiAction : EventArgs
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- Page, Panel, Message, PageDataDescr, MsgDataDescr -->
        /// <summary>
        ///      Information about the page, panel and message that the click happened in
        /// </summary>
        public string PageDataDescr { get { return _pageDataDescr; } } private string _pageDataDescr; /// <summary>The page where the action occurs</summary>
        public string Page          { get { return _page;          } } private string _page;          /// <summary>The panel where the action occurs</summary>
        public string Panel         { get { return _panel;         } } private string _panel;         /// <summary>Symbolic information for the generally literal message</summary>
        public string MsgDataDescr  { get { return _msgDataDescr;  } } private string _msgDataDescr;  /// <summary>Information about the click</summary>
        public string Message       { get { return _message;       } } private string _message;

	
        // ----------------------------------------------------------------------------------------
        //  constructors
        // ----------------------------------------------------------------------------------------
        public GuiAction(Form form                                                                   ) { Init("",                  "|"+form.Text, "",          "",        ""); }
        public GuiAction(Form form,                                             ButtonBase       btn ) { Init("",                  "|"+form.Text, "",          "",        "["+TextOf(btn)+"]"); }
        public GuiAction(Form form,              string dataDescr, string  lbl, ToolStripTextBox txt ) { Init("",                  "|"+form.Text, "",          dataDescr, lbl + ": [_'" + txt.Text + "'_]"); }
        public GuiAction(Form form, Control pnl,                                string           msg ) { Init("",                  "|"+form.Text, TextOf(pnl), "",        msg); }
        public GuiAction(Form form, Control pnl,                                CheckBox         chk ) { Init("",                  "|"+form.Text, TextOf(pnl), "",        CheckBoxMessage(chk)); }
        public GuiAction(Form form, Control pnl, string dataDescr,              RadioButton      rad ) { if (rad.Focused) Init("", "|"+form.Text, TextOf(pnl), dataDescr, "(*) "+rad.Text); }
        public GuiAction(Form form, Control pnl, string dataDescr, Control lbl, CheckedListBox   drop) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextOf(lbl)+": ["+drop.Text+"\\/]"    ); }
        public GuiAction(Form form, Control pnl, string dataDescr, Control lbl, ComboBox         drop) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextOf(lbl)+": ['"+drop.Text+"'\\/]"  ); }
        public GuiAction(Form form, Control pnl, string dataDescr, Control lbl, DateTimePicker   pick) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextOf(lbl)+": ['"+pick.Text+"'\\[]/]"); }
        public GuiAction(Form form, Control pnl, string dataDescr, Control lbl, TextBox          txt ) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextOf(lbl)+": [_'"+txt.Text+"'_]"    ); }
        public GuiAction(Form form, Control pnl, string dataDescr, Control lbl, TextBoxBase      txt ) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextBoxMessage(TextOf(lbl), txt)      ); }
        public GuiAction(Form form, Control pnl, string dataDescr, string  lbl, ComboBox         drop) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, lbl+": ['"+drop.Text+"'\\/]"          ); }
        public GuiAction(Form form, Control pnl, string dataDescr, string  lbl, DateTimePicker   pick) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, lbl+": ['"+pick.Text+"'\\[]/]"        ); }
        public GuiAction(Form form, Control pnl, string dataDescr, string  lbl, RadioButton      rad ) { if (rad.Focused) Init("", "|"+form.Text, TextOf(pnl), dataDescr, "(*) "+lbl); } // did it happen because the radio button was clicked?
        public GuiAction(Form form, Control pnl, string dataDescr, string  lbl, TextBoxBase      txt ) { Init("",                  "|"+form.Text, TextOf(pnl), dataDescr, TextBoxMessage(lbl, txt)); }
        public GuiAction(Form form,                                             ToolStripItem    item) { Init("",                  "|"+form.Text, "",          "",        "["+BestLabel(item)+"]"); }        // "[(Help)]"
        public GuiAction(Form form, ToolStripMenuItem menu,                     ToolStripItem    item) { Init("",                  "|"+form.Text, "",          "",        "["+Regex.Replace(menu.Text,"&","")+" > "+Regex.Replace(item.Text,"&","")+"]"); }
        public GuiAction(Form form, ToolStripMenuItem menu, ToolStripItem submenu, ToolStripItem item) { Init("",                  "|"+form.Text, "",          "",        "[" + TextOf(menu) + " > " + TextOf(submenu) + " > " + TextOf(item) + "]"); }
        public GuiAction(string page,            string dataDescr,              string           msg ) { Init("",                  page,          "",          dataDescr, msg); }
        public GuiAction(string page, string pnl, string dataDescr,             string           msg ) { Init("",                  page,          pnl,         dataDescr, msg); }
        public GuiAction(string pageDataDescr, Form form)                                              { Init(pageDataDescr,       "|"+form.Text, "",          "",        ""); }
        public GuiAction(string pageDataDescr, string page, string pnl, string dataDescr, string msg ) { Init(pageDataDescr,       page,          pnl,         dataDescr, msg); }


        // ----------------------------------------------------------------------------------------
        /// <!-- GuiAction -->
        /// <summary>
        ///      A GuiAction constructor for YesNoQuestion objects
        /// </summary>
        /// <remarks>
        ///      I know this is strange, inefficient and unsafe but I am doing this
        ///      to avoid a dependency between DataAtomLib.Trace and DataAtomLib.UI
        /// 
        ///      before:
        ///      Init("", "|"+dialog.Title, "", "", Regex.Replace(dialog.Text, "[\r\n]+", " ") + " ["+dialog.Answer+"]");
        /// </remarks>
        /// <param name="dialog"></param>
        public GuiAction(object dialog)
        {
            if (dialog.GetType().Name == "YesNoQuestion")
            {
                Type Class = dialog.GetType();
                BindingFlags PI = BindingFlags.Public | BindingFlags.Instance;
                string title  =  (string)      (Class.GetProperty("Title",  PI)).GetValue(dialog, null);
                string text   =  (string)      (Class.GetProperty("Text",   PI)).GetValue(dialog, null);
                string answer = ((DialogResult)(Class.GetProperty("Answer", PI)).GetValue(dialog, null)).ToString();

                Init("", "|"+title, "", "", Regex.Replace(text, "[\r\n]+", " ") + " ["+answer+"]");
            }
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- BestLabel -->
        /// <summary>
        ///      Comes up with its best guess as to the label of the widget, for example: "(Save)"
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        public static string BestLabel(ToolStripItem btn)
        {
            string best = Regex.Replace(btn.Text, "&", "");
            if (string.IsNullOrEmpty(btn.Text))
            {
                if (string.IsNullOrEmpty(btn.ToolTipText)) best = "(" + btn.Name + ")";
                else best = "(" + btn.ToolTipText + ")";
            }
            return best;
        }
        public static string BestLabel(ButtonBase btn)
        {
            string best = Regex.Replace(btn.Text, "&", "");
            if (string.IsNullOrEmpty(btn.Text))
                best = "(" + btn.Name + ")";
            return best;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- CheckBoxMessage -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chk"></param>
        /// <returns></returns>
        private static string CheckBoxMessage(CheckBox chk)
        {
            string msg;
            if (chk.Checked)  msg = "[X] " + chk.Text;
            else              msg = "[_] " + chk.Text;
            return msg;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- GatMessage -->
        /// <summary>Returns the message formatted for GAT rather than formatted literally</summary>
        /// <returns></returns>
        public string GatMessage { get
        {
            if (string.IsNullOrEmpty(_msgDataDescr)) return _message;
            else return Regex.Replace(_message, "'[^']+'", "<" + _msgDataDescr + ">");
        } }


        // ----------------------------------------------------------------------------------------
        /// <!-- GatPage -->
        /// <summary>Returns the page formatted for GAT rather than formatted literally</summary>
        /// <returns></returns>
        public string GatPage { get
        {
            if (string.IsNullOrEmpty(_pageDataDescr)) return _page;
            else return Regex.Replace(_page, "'[^']+'", "<" + _pageDataDescr + ">");
        } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageDataDescr"></param>
        /// <param name="page"></param>
        /// <param name="panel"></param>
        /// <param name="msgDataDescr"></param>
        /// <param name="message"></param>
        private void Init(string pageDataDescr, string page, string panel, string msgDataDescr, string message)
        {
            _pageDataDescr = pageDataDescr;
            _page          = page;
            _panel         = panel;
            _msgDataDescr  = msgDataDescr;
            _message       = message;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- TextBoxMessage -->
        /// <summary>
        ///      Text boxes are complicated because they can be either single line or multi-line
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        private static string TextBoxMessage(string lbl, TextBoxBase txt)
        {
            string msg = lbl + ": [_'" + txt.Text + "'_]"; if (txt.Multiline) msg = Regex.Replace(msg, @"[\[\]]", "|");
            return msg;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- TextOf -->
        private static string TextOf(Control item)
        {
            if (item == null)
                return "";
            else return Regex.Replace(item.Text, @"&(\w)", "$1");
        }
        private static string TextOf(ToolStripItem item)
        {
            if (item == null)
                return "";
            else return Regex.Replace(item.Text, @"&(\w)", "$1");
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Returns a plain vanilla version of the action
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str  = Regex.Replace(_page,  "^([^|])", "|$1")
                + "   " + Regex.Replace(_panel, "^([^|])", "||$1")
                + "   " + _message;
            return base.ToString();
        }
    }
}
