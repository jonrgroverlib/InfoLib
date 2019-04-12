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
using System;                         // for 
using System.Collections.Generic;     // for List(1)
using System.Text;                    // for StringBuilder(6)
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Strings // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- GuiActionTrace -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>beta code - used in production once</remarks>
    public class GuiActionTrace
    {

        private List<GuiAction> _actionList;


        // ----------------------------------------------------------------------------------------
        //  Fields
        // ----------------------------------------------------------------------------------------
        public string MsgDataDescr (int row) { if (InRange(row)) return _actionList[row].MsgDataDescr;                             else return ""; }
        public string Message      (int row) { if (InRange(row)) return _actionList[row].Message;                                  else return ""; }
        public string Page         (int row) { if (InRange(row)) return Regex.Replace(_actionList[row].Page,  "^([^|])", "|$1");   else return ""; }
        public string Panel        (int row) { if (InRange(row)) return Regex.Replace(_actionList[row].Panel, "^([^|])", "||$1");  else return ""; }
        public string PageDataDescr(int row) { if (InRange(row)) return _actionList[row].PageDataDescr;                            else return ""; }


        public string PrevMessage (int row) { return Message(row - 1); }
        public string PrevPage    (int row) { return Page   (row - 1); }
        public string PrevPanel   (int row) { return Panel  (row - 1); }
        public string NextMessage (int row) { return Message(row + 1); }
        public string NextPage    (int row) { return Page   (row + 1); }
        public string NextPanel   (int row) { return Panel  (row + 1); }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public GuiActionTrace() { /* ThrowMessage.ReadyCheck(); */ _actionList = new List<GuiAction>(); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Null -->
        /// <summary>
        /// 
        /// </summary>
        public static GuiActionTrace Null { get
        {
            GuiActionTrace gat = new GuiActionTrace();
            gat._actionList = null;
            return gat;
        } }


        // ----------------------------------------------------------------------------------------
        /// <!-- IsNull -->
        /// <summary>
        /// 
        /// </summary>
        public bool IsNull { get { return (_actionList == null); } }


        // ----------------------------------------------------------------------------------------
        //  Pass-through methods
        // ----------------------------------------------------------------------------------------
        public void Add(GuiAction action) { _actionList.Add(action);  }
        public void Clear()               { _actionList.Clear();      }


        public int Count
        {
            get
            {
                if (_actionList == null)
                    return 0;
                else return _actionList.Count;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Full -->
        /// <summary>
        ///      Constructs the action trace without alteration
        /// </summary>
        /// <remarks>
        ///      traceBuilder.Append("\r\n" + Page(row) + "   " + Panel(row) + "   " + Message(row));
        /// </remarks>
        /// <param name="actionBuffer"></param>
        /// <returns></returns>
        public string Full { get
        {
            StringBuilder traceBuilder = new StringBuilder();
            string delim = "";
            for (int row = 0; row < Count; ++row)
                { traceBuilder.Append(delim + _actionList[row].ToString()); delim = "\r\n"; }
            return traceBuilder.ToString();
        } }

        // ----------------------------------------------------------------------------------------
        /// <!-- InRange -->
        /// <summary>Determines whether the input row number is in the array range</summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool InRange(int row) { return (0 <= row && row < _actionList.Count); }

    }
}
