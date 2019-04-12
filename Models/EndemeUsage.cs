using InfoLib.Endemes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Linq;
using System.Text;

namespace InfoLib.Models
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeUsage -->
    /// <summary>
    ///      The EndemeUsage class contains objects that parallel the EndemeUsage table
    /// </summary>
    public class EndemeUsage
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        [Key]
        public int       EndemeUsageId       { get; set; }  // primary key
        public Guid      EndemeSetId         { get; set; }
        public string    TableName           { get; set; }
        public string    TableEndemeFkColumn { get; set; }
        public string    TablePkColumn       { get; set; }
        public string    TableRowLabel       { get; set; }
        public string    ExtractLabelB       { get; set; }
        public EndemeSet EndemeSet           { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeUsage()
        {
        }

    }

}
