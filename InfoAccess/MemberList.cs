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
using System.Collections.Generic;     // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Info
{
	// --------------------------------------------------------------------------------------------
	/// <!-- MemberList -->
    /// <summary>
    ///      Classes have members, the MemberList class contains metadata for them
    /// </summary>
    /// <remarks>alpha code</remarks>
    public class MemberList
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        GraphNode HeadOfList;
        List<GraphEdge> LocalMember { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor, Without a name it is a class, with a name it is a member of a class
        // ----------------------------------------------------------------------------------------
        public MemberList(string myNamespace, string className                    ) { Init(myNamespace, className, ""         ); }
        public MemberList(string myNamespace, string className, string memberLabel) { Init(myNamespace, className, memberLabel); }

        private void Init(string myNamespace, string className, string memberLabel)
        {
            HeadOfList            = new GraphNode(memberLabel);
            HeadOfList.Container  = className;
            HeadOfList.Container2 = myNamespace;
            LocalMember           = new List<GraphEdge>();
        }

        public void Add(string memberClassNamespace, string memberClassName, string memberLabel)
        {
            GraphNode member = new GraphNode(memberLabel);
            member.Container = memberClassName;
            member.Container2 = memberClassNamespace;
            GraphEdge membership = new GraphEdge(member, HeadOfList);
            LocalMember.Add(membership);
        }

        public override string ToString()
        {
            string members = "";
            string delim   = "";
            foreach (GraphEdge item in LocalMember)
            {
                members += (delim + item.ToString());
                delim = ", ";
            }
            return HeadOfList.Container + " : " + members;
        }
    }
}
