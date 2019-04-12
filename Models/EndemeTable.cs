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
// You should have received a copy of the GNU Lesser General Public License along with
// InfoLib. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using InfoLib.Endemes;
using System;
using System.Collections.Generic;

namespace InfoLib.Models
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeTable -->
    /// <summary>
    ///      The EndemeTable class contains objects that parallel the Endeme table
    /// </summary>
    public class EndemeTable
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        //[Key]
        public long      EndemeId        { get; set; } // primary key
        public Guid      EndemeSetId     { get; set; }
        public string    EndemeString    { get; set; }
        public string    EndemeCode      { get; set; }
        public string    EndemeLabel     { get; set; }
        public string    EndemeDescr     { get; set; }
        public bool      RawSource       { get; set; } // not null
        public bool      MultipleUse     { get; set; } // not null
        public byte[]    ValueBinary     { get { return _valueBinary; } set { _valueBinary = value; } }  public byte[] _valueBinary;
        public DateTime? ValueDateTime   { get; set; }
        public double    ValueFloat      { get; set; }
        public decimal   ValueNumber     { get; set; }
        public string    ValueText       { get; set; }
        public int       EndemeProfileId { get; set; }
        public EndemeSet EndemeSet       { get; set; }
        public List<EndemeIndex> EndemeIndexList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeTable()
        {
        }

        public override string ToString()
        {
            return EndemeString + " " + EndemeId + " " + EndemeLabel + " " + ValueText + ValueNumber;
        }
    }

}
