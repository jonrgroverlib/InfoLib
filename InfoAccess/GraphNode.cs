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
	/// <!-- GraphNode -->
    /// <summary>
    ///      native L4
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class GraphNode
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public string Container2 { get; set; }
        public string Container  { get; set; }
        public string Name       { get; set; }
        public string SourceNode { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public GraphNode(string name                  ) { Init(""       , name, name); }
        public GraphNode(string container, string name) { Init(container, name, name); }


        private void Init(string container, string name, string name_2)
        {
            SourceNode = name     ;
            Name       = name     ;
            Container  = container;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Container.Trim())) return Name;
            else if (string.IsNullOrEmpty(Container2.Trim())) return Container + "." + Name;
            else return Container2 + "." + Container + "." + Name;
        }
    }
}
