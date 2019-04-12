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
	/// <!-- GraphEdge -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class GraphEdge // native L4
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public GraphNode SourceNode      { get; set; }
        public GraphNode DestinationNode { get; set; }
        public string    EdgeType        { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public GraphEdge(GraphNode fromNode, GraphNode toNode)
        {
            SourceNode      = fromNode;
            DestinationNode = toNode  ;
            EdgeType        = "Graph" ;
        }
        public GraphEdge(GraphNode FKColumn, GraphNode PKColumn, string edgeType)
        {
            SourceNode      = FKColumn;
            DestinationNode = PKColumn;
            EdgeType        = edgeType;
        }


        public string Connector { get
        {
            switch (EdgeType)
            {
                case "Foreign Key" : return " >- ";
                case "Inherits"    : return " : " ;
            }
            return " -> ";
        } }

        public override string ToString()
        {
            return SourceNode.ToString() + " -> " + DestinationNode.ToString();
        }
    }
}
