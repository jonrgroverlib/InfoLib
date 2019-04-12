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
using System.Data;                    // for many(8)
using System.Data.Common;             // for many(7), DbDataAdapter
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Data
{
    // --------------------------------------------------------------------------------------------
    /// <!-- DataReaderAdapter -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>production ready, compare with same class in HardData</remarks>
    public class DataReaderAdapter : DbDataAdapter
    {
        // ----------------------------------------------------------------------------------------
        /// <!-- FillFromReader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public int FillFromReader(DataTable dataTable, IDataReader dataReader)
        {
            if (dataReader == null)
                Throws.A(new NoNullAllowedException("FillFromReader error - dataReader is null"), Throws.Actions, "P");
            return this.Fill(dataTable, dataReader);
        }


        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping){ return null; }
        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) { return null; }
        protected override void OnRowUpdated(RowUpdatedEventArgs value) { }
        protected override void OnRowUpdating(RowUpdatingEventArgs value) { }

    }
}
