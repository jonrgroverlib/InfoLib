using System;
using System.Text;

namespace InfoLib.Endemes
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeTermKey -->
    /// <summary>
    ///      The term key for an endeme profile
    /// </summary>
    public class EndemeTermKey
    {
        // ----------------------------------------------------------------------------------------
        //  Member
        // ----------------------------------------------------------------------------------------
        public string Value { get; set; }


        public static implicit operator EndemeTermKey(string text) { return new EndemeTermKey(text); }
        public static implicit operator string(EndemeTermKey entk) { return entk.ToString(); }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeTermKey(string str)
        {
            Value = str;
        }


        // ----------------------------------------------------------------------------------------
        //  Method
        // ----------------------------------------------------------------------------------------
        public override string ToString()
        {
            return Value;
        }
    }
}
