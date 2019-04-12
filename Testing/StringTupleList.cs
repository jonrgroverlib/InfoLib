using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoLib.Testing
{
    // --------------------------------------------------------------------------------------------
    /// <!-- SStringPairList -->
    /// <summary>
    ///      The StringPairList class uses a list of string 2-tuples to fill a combobox with Carbon
    /// </summary>
    /// <remarks>
    ///      Use this class sparingly, it is mostly designed to help with certain UI API requirements
    /// </remarks>
    public class StringPairList : List<StringPair>
    {
        // ----------------------------------------------------------------------------------------
        //  Constructor and Methods
        // ----------------------------------------------------------------------------------------
        public StringPairList(string firstItem, Carbon setList, string keyLabel)
        {
            Add(new StringPair(Guid.Empty.ToString(), firstItem));
            for (int i = 0; i < setList.Count; ++i)
                Add(new StringPair(setList[i], keyLabel));
        }
    }


    // ----------------------------------------------------------------------------------------
    /// <!-- StringTupleList -->
    /// <summary>
    ///     The StringTupleList class handles a key:label pair where both are strings
    /// </summary>
    public class StringPair
    {
        // ------------------------------------------------------------------------------------
        //  Short Members, Constructors and Methods
        // ------------------------------------------------------------------------------------
        public string Key { get; set; } public string Label { get; set; }
        public StringPair(string guid, string label) { Key = guid; Label = label; }
        public StringPair(Dictionary<string, string> tuple, string keyLabel) { foreach (KeyValuePair<string,string> item in tuple) { if (item.Key == keyLabel) Key = item.Value; else Label = item.Value; } }
        public override string ToString() { return "Key: " + Key + ", Label: " + Label; }
    }
}
