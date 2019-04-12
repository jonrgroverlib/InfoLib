//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of InfoLib.
//
// InfoLib is free software: you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with InfoLib. 
// If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using System;                         // for Guid
using System.Collections.Generic;     // for Dictionary, List
using System.Data;                    // for DataColumnCollection
using System.Linq;                    // for ToList()
using System.Text;                    // for StringBuilder
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Testing
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Atom -->
    /// <summary>
    ///      Atom: a List (at) of string-object Dictionaries (om)
    /// </summary>
    /// <remarks>
    ///      I orignally intended to make each class in this file a one-liner, but Atom grew a method
    /// </remarks>
    public class Atom : List<Dictionary<string, object>>
    {
        public Atom(            ) : base(        ) { }
        public Atom(int capacity) : base(capacity) { }


        public static Atom Empty { get { return new Atom(); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- PrependEmptyItem -->
        /// <summary>
        ///      Prepends an empty item matching the table that made the atom in the first place
        /// </summary>
        /// <param name="columnList">column list from the table that made the atom</param>
        /// <returns></returns>
        public Atom PrependEmptyItem(DataColumnCollection columnList)
        {
            Proton emptyItem = new Proton(columnList.Count);

            for (int col = 0; col < columnList.Count; ++col)
            {
                DataColumn the = columnList[col];
                switch (the.DataType.Name)
                {
                    case "Boolean": emptyItem.Add(the.ColumnName, false     ); break;
                    case "Char"   : emptyItem.Add(the.ColumnName, '\0'      ); break;
                    case "Guid"   : emptyItem.Add(the.ColumnName, Guid.Empty); break;
                    case "Int"    : emptyItem.Add(the.ColumnName, 0         ); break;
                    case "Int32"  : emptyItem.Add(the.ColumnName, 0         ); break;
                    case "String" : emptyItem.Add(the.ColumnName, ""        ); break;
                    default       : emptyItem.Add(the.ColumnName, null      ); break;
                }
            }

            Insert(0,emptyItem);
            return this;
        }
    }

    
    // --------------------------------------------------------------------------------------------
    /// <summary>Carbon : a List of string-string Dictionaries</summary>
    // --------------------------------------------------------------------------------------------
    public class Carbon : List<Dictionary<string, string>>
    {
        public Carbon () : base() { }
        public Carbon (int capacity) : base(capacity) { }


        public string this[string key] { get { return this[key]; } set { this[key] = value; } }

        public void Add(string key, string value)
        {
            Dictionary<string, string> item = new Dictionary<string, string>(1);
            item.Add(key,value);
            this.Add(item);
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>Hydrogen: a List of char-char Dictionaries</summary>
    // --------------------------------------------------------------------------------------------
    public class Hydrogen : List<Dictionary<char,char>>
    {
        public Hydrogen() : base() { }
        public Hydrogen(int capacity) : base(capacity) { }

        public void Add               (char   key      , char value  ) { Dictionary<char,char> item = new Dictionary<char,char>(1); item.Add(key,value); this.Add(item);        }
        public void InitializeEntities(string list     , char value  ) { char[] item = list.ToCharArray(); for (int i = 0; i < item.Length; ++i) { Add(item[i],value); }        }
        public void Replace           (char   fromValue, char toValue) { for (int i = 0; i < this.Count; ++i) foreach (char key in this[i].Keys.ToList()) if (this[i][key] == fromValue) this[i][key] = toValue; }
        public void SetValue          (char   ofKey    , char value  ) { for (int i = 0; i < Count     ; ++i) foreach (char key in this[i].Keys.ToList()) if (key == ofKey)              this[i][key] = value;   }

        public void SetOperations(string allItems, string entities)
        {
            char[] item = allItems.ToCharArray();
            for (int i = 0; i < item.Length; ++i)
            {
                if (Regex.IsMatch(item[i].ToString(), "["+entities+"]"))
                        SetValue(item[i], 'X');  // Operations on: USOT
                else Replace('X', item[i]);   // Operations: DAEIRN
            }
        }

        public override string ToString()
        {
            string str = "";
            string delim1 = "";
            for (int i = 0; i < this.Count; ++i)
            {
                str += delim1;
                string delim2 = "";
                foreach (char k in this[i].Keys)
                {
                    str += delim2 + k + ":" + this[i][k];
                    delim2 = ", ";
                }
                delim1 = "\r\n";
            }
            return str;
        }

        public char[] KeysAmong(string allKeys)
        {
            List<char> entityList = new List<char>();
            for (int i = 0; i < this.Count; ++i)
            {
                foreach (char key in this[i].Keys)
                    if (Regex.IsMatch(key.ToString(), "["+allKeys+"]"))
                        entityList.Add(key);
            }
            string str = "";
            for (int i = 0; i < entityList.Count; ++i)
            {
                str += entityList[i];
            }
            char[] output = str.ToCharArray();
            return output;
        }

        public string ToEntityOperationEndeme(string allOperations, string allEntities)
        {
            char[] hi = this.KeysAmong(allEntities);                //  List active entities
            char[] operation = allOperations.ToCharArray();


            string endeme = "";
            for (int i = 0; i < operation.Length; ++i)
            {
                string entityStr = "";
                for (int j = 0; j < hi.Length; ++j)
                {
                    char ent = hi[j];
                    foreach (char op in this[j].Values)
                    {
                        if (op == operation[i])
                            entityStr += hi[j];
                    }
                }
                endeme += entityStr + operation[i];
            }

            return endeme;
        }
    }


    // --------------------------------------------------------------------------------------------
    //  Isotopes, atoms and elements (list of dictionaries and dictionaries of dictionaries)
    /* ----------------------------------------------------------------------------------------- */                                                              /// <summary>Isotope: a List of object-object Dictionaries</summary>
    public class Isotope  : List      <        Dictionary<object, object>> { public Isotope () : base() { } public Isotope (int capacity) : base(capacity) { } } /// <summary>Lithium: a (string-key) Dictionary of int-string Dictionaries</summary>
    public class Lithium  : Dictionary<string, Dictionary<int   , string>> { public Lithium () : base() { } public Lithium (int capacity) : base(capacity) { } } /// <summary>Krypton: a ragged 2d array of strings</summary>
    public class Krypton  : List      <        List      <        string>> { public Krypton () : base() { } public Krypton (int capacity) : base(capacity) { } }


    public class Nitrogen : Dictionary<string, List<string>>
    {
        public Nitrogen() : base() { } public Nitrogen(int capacity) : base(capacity) { }

        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public int    MaxKeyLength()                   { int max = 0; foreach (string key in this.Keys) { max = Math.Max(max, key.Length); } return max; }
        public string ToList(string key, string delim) { string str = ""; string join = ""; foreach (string item in this[key]) { str += join + item; join = delim; } return str; }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds key-value to a Nitrogen dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            if (this.ContainsKey(key))
            {
                this[key].Add(key);
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(value);
                this.Add(key, list);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Ascii -->
        /// <summary>
        ///      key list delimiter is a CR-LF
        /// </summary>
        /// <param name="delim">content list delimiter</param>
        /// <returns></returns>
        public string Ascii(string delim)
        {
            StringBuilder text = new StringBuilder();
            int maxLen = this.MaxKeyLength();
            foreach (string key in this.Keys)
            {
                text.Append("\r\n")
                    .Append(key.PadRight(maxLen+1))
                    .Append("(")
                    .Append(this.ToList(key, delim))
                    .Append(")")
                    ;
            }

            return text.ToString();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReKey -->
        /// <summary>
        ///      Renames a key, carrying along all the old data
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <param name="newValue"></param>
        public void ReKey(string oldKey, string newKey)
        {
            if ( this.ContainsKey(newKey)) throw new ArgumentException("Nitrogen already contains key '"+newKey+"'");
            if (!this.ContainsKey(oldKey)) throw new ArgumentException("Nitrogen does not contain key '"+oldKey+"'");


            if (newKey != oldKey)
            {
                List<string> tempList = this[oldKey];
                Add   (newKey, tempList);
                Remove(oldKey);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToRegexPattern -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToRegexPattern()
        {
            StringBuilder text = new StringBuilder();
            string delim = "(";
            foreach (string key in this.Keys)
                { text.Append(delim).Append(key); delim = "|"; }
            text.Append(")");

            return text.ToString();
        }
    }


    // --------------------------------------------------------------------------------------------
    //  Particles and sub-particles (dictionaries)
    /* ----------------------------------------------------------------------------------------- */                                            /// <summary>Particle: an object-object Dictionary[object,string ]</summary>
    public class Particle : Dictionary<object, object > { public Particle() : base() { } public Particle(int capacity) : base(capacity) { } }  /// <summary>Electron: a string-int     Dictionary[string,int    ]</summary>
    public class Electron : Dictionary<string, int    > { public Electron() : base() { } public Electron(int capacity) : base(capacity) { } }  /// <summary>Neutron : a string-string  Dictionary[string,string ]</summary>
    public class Neutron  : Dictionary<string, string > { public Neutron () : base() { } public Neutron (int capacity) : base(capacity) { } public void Nullify(string key) { if (ContainsKey(key) && string.IsNullOrWhiteSpace(this[key])) Remove(key); } }  /// <summary>Proton  : a  string-object Dictionary[string,object]</summary>
    public class Proton   : Dictionary<string, object > { public Proton  () : base() { } public Proton  (int capacity) : base(capacity) { } }  /// <summary>Quark   : an int-string    Dictionary[int   ,string ]</summary>
    public class Quark    : Dictionary<int   , string > { public Quark   () : base() { } public Quark   (int capacity) : base(capacity) { } }  /// <summary>Graviton: an int-DataRow   Dictionary[int   ,DataRow]</summary>
    public class Radion   : Dictionary<int   , DataRow> { public Radion  () : base() { } public Radion  (int capacity) : base(capacity) { } }  /// <summary>Graviton: a string-DataRow Dictionary[string,DataRow]</summary>
    public class Graviton : Dictionary<string, DataRow> { public Graviton() : base() { } public Graviton(int capacity) : base(capacity) { } }
}
