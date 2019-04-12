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
using System.Collections.Generic;     // for 
using System.Data;                    // for DataTable, DataRow
using System.Data.SqlClient;          // for SqlConnection, SqlCommand, SqlDataReader
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- NumericLookup -->
    /// <summary>
    ///      The NumericLookup class implements a cross reference between integer values in a lookup
    ///      table for database with integer values in another database having the same meaning
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class NumericLookup
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        Dictionary<int,int> _xref;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        public int this[int x] { get { if (ContainsKey(x)) return _xref[x]; else return 0; } }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public NumericLookup(int capacity)
        {
            _xref = new Dictionary<int, int>(capacity);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds a new item to the xref in a threadsafe manner
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(int key, int value)
        {
            if (!_xref.ContainsKey(key)) try { _xref.Add(key, value); } catch { }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ContainsKey -->
        /// <summary>
        ///      Wraps the ContainsKey(int) method
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(int key)
        {
            return _xref.ContainsKey(key);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetDictionaryFrom -->
        /// <summary>
        ///      Converts two columns of a database table into a Dictionary<int,int> structure
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static NumericLookup GetDictionaryFrom(string tableName, string keyColumn
            , string valueColumn, SqlConnection connection)
        {
            NumericLookup list = new NumericLookup(10);
            string query
                = " SELECT " + keyColumn + ", " + valueColumn
                + " FROM "   + tableName
                + " WHERE "  + valueColumn + " IS NOT NULL";


            try
            {
                var command = new SqlCommand(query, connection);
                DataTable table = new DataTable();
                using (command)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
                for (int row = 0; row < table.Rows.Count; ++row)
                {
                    int key = 0;
                    int value = 0;
                    if (int.TryParse(table.Rows[row][keyColumn  ].ToString(), out key  ) &&
                        int.TryParse(table.Rows[row][valueColumn].ToString(), out value))
                    list.Add(key, value);
                }
            }
            catch (SqlException ex)
            {
                Pause(); 
                if (ex.InnerException != null)
                    throw new Exception("Sql connection having trouble? - " + ex.Message + " - " + ex.InnerException.Message, ex);
                else
                    throw new Exception("Sql connection having trouble? - " + ex.Message, ex);
            }
            catch (Exception ex) { Pause(); throw new Exception(ex.Message, ex); }

            return list;
        }

        private static void Pause()
        {
        }

        public Dictionary<int,int>.KeyCollection Keys { get { return _xref.Keys; } }

        public IEnumerable<int> Values { get { return _xref.Values; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToString -->
        /// <summary>
        ///      Wraps ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
