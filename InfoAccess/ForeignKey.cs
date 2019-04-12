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
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Info
{
	// --------------------------------------------------------------------------------------------
	/// <!-- ForeignKey -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class ForeignKey
    {
        GraphNode ForeignTableColumn;
        GraphNode PrimaryTableColumn;
        GraphEdge FromForeignToPrimary;

        public ForeignKey(string tableName, string columnLabel, string toTableName, string toPrimaryKey)
        {
            ForeignTableColumn = new GraphNode(columnLabel);
            ForeignTableColumn.Container = tableName;

            PrimaryTableColumn = new GraphNode(toPrimaryKey);
            PrimaryTableColumn.Container = toTableName;

            FromForeignToPrimary = new GraphEdge(ForeignTableColumn, PrimaryTableColumn, "Foreign Key");
        }

        public override string ToString()
        {
            return ForeignTableColumn.ToString() + FromForeignToPrimary.Connector + PrimaryTableColumn.ToString();
        }
    }
}
