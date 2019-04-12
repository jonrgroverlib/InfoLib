//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify it under the terms 
// of the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with InfoLib.
// If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;                         // for Exception
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.HardData // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
	// --------------------------------------------------------------------------------------------
	/// <!-- AmbiguousResultException -->
    /// <summary>
    ///      Use this exception when a query can not find one of its tables
    /// </summary>
    public class MissingTableException : Exception
    {
        private const string _defaultMessage = "Query has attempted to access a table that does not exist.";


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public MissingTableException(          )                   : base(_defaultMessage) { }
        public MissingTableException(string msg)                   : base(msg)             { }
        public MissingTableException(string msg, Exception inner ) : base(msg, inner)      { }
        public MissingTableException(string err, string tableName) : base(Msg(err, tableName)) { }

        public static string Msg(string error, string tableName)
        {
            string table   = tableName.Trim();
            string pattern = "^.*Invalid object name '"+table+"'.*$";
            string changed = Regex.Replace(error, pattern, "", RegexOptions.Singleline);
            return (changed == "") ? "Missing table: '"+table+"'" : "False Positive";
        }

        public static void If(string errors, string candy, string tablename)
        {
            if (Regex.IsMatch(errors, "Invalid object name '"+tablename.Trim()+"'", RegexOptions.IgnoreCase))
                throw new MissingTableException(errors, tablename);
        }
    }
}
