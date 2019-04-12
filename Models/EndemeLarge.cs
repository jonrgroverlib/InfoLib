﻿using InfoLib.Endemes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Linq;
using System.Text;

namespace InfoLib.Models
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeLarge -->
    /// <summary>
    ///      The EndemeLarge class contains objects that parallel the EndemeLarge table
    /// </summary>
    public class EndemeLarge
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
      //[Key]
        public long      EndemeId        { get; set; }  // primary key
        public Guid      EndemeSetId     { get; set; }
        public string    EndemeString    { get; set; }
        public string    EndemeCode      { get; set; }
        public string    EndemeLabel     { get; set; }
        public string    EndemeDescr     { get; set; }
        public bool      RawValue        { get; set; }
        public bool      RawSource       { get; set; } // not null
        public bool      MultipleUse     { get; set; }
        public byte[]    ValueBinary     { get; set; }
        public DateTime  ValueDateTime   { get; set; }
        public double    ValueFloat      { get; set; }
        public decimal   ValueNumber     { get; set; }
        public string    ValueText       { get; set; }
        public int       EndemeProfileId { get; set; }
        public EndemeSet EndemeSet       { get; set; }
        public List<EndemeIndex> EndemeIndexList { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeLarge()
        {
        }

    }

}
