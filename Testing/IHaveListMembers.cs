//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Infolib.
//
// InfoLib is free software: you can redistribute it and/or modify it under the terms of
// the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with InfoLib.
// If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;

namespace InfoLib.Testing
{
    // ---------------------------------------------------------------------------------------------
    /// <summary>
    ///      I kept making stuff with Count and Contains so ..
    /// </summary>
    /// <remarks>
    ///      Class            | Capacity   Clear   Contains   Count | Candidates
    ///      ---------------- + --------   -----   --------   ----- + ------------------------------------------------------------------------
    ///      RichDataTable    | Capacity   Clear   Contains   Count | Ascii                      Sort   Add       CreateCsCode    ToEndeme
    ///      EndemeDefinition | Capacity   Clear   Contains   Count |         Label   RemoveAt   Sort   Found          
    ///      EndemeSet        | Capacity   Clear   Contains   Count |         Label              Sort                       
    ///      Endeme           | Capacity   Clear   Contains   Count |                                   IsEmpty   ContainsOneOf   ToCharArray
    ///      EndemeList       | Capacity   Clear   Contains   Count |         Label   RemoveAt                                         
    ///      Corpus           | Capacity   Clear   Contains   Count |         DisplayName                                         RandomLine
    ///      ---------------- + --------   -----   --------   ----- + ------------------------------------------------------------------------
    ///      EndemeProfile    |                    Contains   Count |                                   IndexOf
    ///      EnumList         |                               Count |                                   Length
    ///      FuzzyList        |                                     |                                  
    ///      FuzzyMeasure     |                                     |                                  
    ///      RegexTable       |                                     |                                  
    ///      RichSqlCommand   |                                     | Ascii           RemoveAt                                         
    ///      ---------------- + --------   -----   --------   ----- + ------------------------------------------------------------------------
    /// </remarks>
    public interface IHaveListMembers
    {
        int  Capacity { get; set; }
        void Clear    (           );
        bool Contains (string item);
        int  Count    { get;      }
    }
}