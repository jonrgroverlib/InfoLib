//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
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
using InfoLib.Models;                 // for 
using System;                         // for 
using System.Collections.Generic;     // for
using System.ComponentModel.DataAnnotations; // for [Key]
using System.Linq;                    // for ToList
using System.Runtime.CompilerServices;// for IndexerName
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeItem -->
    /// <summary>
    ///      The EndemeItem class contains element in an EndemeList
    /// </summary>
    /// <remarks>
    /// 
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [-------------------- ACTION ------------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    ///       _   |                                              |    |                                                          |    |                                                                       |   _     _ 
    ///        |                  EndemeActuator.cs                                                                                                                            EndemeAccess                  |       |
    ///  Info  |  |                                              |    |                                                          |    |                                              |                        |  | Info  |
    ///        |                                                                                                                                                                                                 |       |
    ///  Level |  |                                              |    |                         (objects)                        |    |       (tables)                               |                        |  | Level |
    ///        |                                                                                    +  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  +--+--+--+--+--+--+-----+------+                    |       |
    ///       _|  |                                              |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |  |_     _|
    ///                                                                                             |                                                        |  |  |  |  |  |  |            |                             
    ///       _   |                                              |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |   _     _ 
    ///        |                                                                               EndemeField (*,*,*)*n                                         |  |  |  |  |  |  |            |                    |       |
    ///        |  |                                              |    |                             |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                          +----------------+                 +----------------------+                                                        |  |  |  |  |  |  |            |                    |       |
    ///        |  |                       |                 \    |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                     EndemeActuator          \          EndemeList (1,1,1)*n   EndemeDefinition (n,n,n+m)*n                                  |  |  |  |  |  |  |            |                    |       |
    ///        |  |                      /|\                  \  |    |      |                      |                            |    |                      |  |  |  |  |  |  |            |                 |  |       |
    ///        |                      ActuatorList             +--------------------------------------+----------------------------------------+-------------+  |  |  |  |  |  |            |                    |       |
    ///  High  |  |                      /|\                     |    |      |                     /|\                           |    |       /                 |  |  |  |  |  |            |                 |  | High  |
    ///        |       +----------+-------+-------+-------+                  |                 EndemeObject (n,n,n+m)                   (endeme actuator table) |  |  |  |  |  |            |                    |       |
    ///  Level |  |    |          |       |       |       |      |    |      |                      |                            |    |  [~FACTOR,ACTUATOR]     |  |  |  |  |  |            |                 |  | Level |
    ///        |  |    |    WeightFactor  |   CharFactor  |                  |            +---------+------------------------------------------+----------------+  |  |  |  |  |            |                    |       |
    ///        |  |    |       |  |       |       |       |      |    |      |           /          |                            |    |       /                    |  |  |  |  |            |                 |  |       |
    ///        |    SetFactor  |  |  OrderFactor  |  ValueFactor             |          /      EndemeProfile (n,n,n)                    (endeme profile table)     |  |  |  |  |            |                    |       |
    ///        |  |    |  |    |  |       |       |    |  |  |   |    |      |         /            |           \|/              |    |  [stores PROFILE,OBJECT,   |  |  |  |  |            |                 |  |       |
    ///        |       |  +-------+-------+-------+----+---------------------+----------------------+    EndemeReference (n,0,0)          DEFINITION,FIELD]        |  |  |  |  |            |                    |       |
    ///        |  |    |       |          |               |  |   |    |              /             /|\             |             |    |                            |  |  |  |  |            |                 |  |       |
    ///       _|       |   WeightFormula  |               |  |                      /          EndemeItem (1,1,1)  |                                               |  |  |  |  |            |                    |_     _|
    ///           |    |                  |               |  |   |    |            /                |              |             |    |                            |  |  |  |  |            |                 |           
    ///                |            IEndemeActuatorFactor |  +-----------+  +-----+    +------+-----+              |                                               |  |  |  |  |            |                             
    ///       _   |    |                  |               |      |    |  |  |     |    |     /      |              |             |    |                            |  |  |  |  |            |                 |   _     _ 
    ///        |       |                  |               |              |  |  IEndemeItem  /       |              |                           +-------------------+  |  |  |  |            |                    |       |
    ///        |  |    |                  |               |      |    |  |  |              /        |  Endeme      |  Endeme     |    |       /                       |  |  |  |            |                 |  |       |
    ///        |       |                  |               |              EndemeValue (0,0,1)        |  TextFormat  |  GrabBag           (endeme index table)          |  |  |  |            |                    |       |
    ///        |  |    |                  |               |      |    |       |                     |       |      |       |     |    |  [indexes ENDEME,LIST]        |  |  |  |            |                 |  |       |
    ///        |       |                  +---------------+-------------------+---------------------+-------+--------------+-------------------+----------------------+  |  |  |            |                    |       |
    ///        |  |    |                                         |    |                             |              |             |    |       /                          +  |  |            |                 |  |       |
    ///        |       |                                                                       Endeme (1,1,0)      |                    (endeme table) [stores LIST,    /   +  |            |                    |       |
    ///        |  |    |                                         |    |                       /    \|/            /|\            |    |  ENDEME,QUANTIF.,ITEM,VALUE]   /   /   +            |                 |  |       |
    ///        |       +----------------------------------------------------------------------------+--------------+---------------------------+----------------------+   /   /    +--------+-------+            |       |
    ///  Low   |  |                                              |    |                     /       |                            |    |       /                          +   /     |                |         |  | Low   |
    ///        |                                                                           /   EndemeSet (1,0,0)                        (endeme set table)               |  +  RichSqlCommand   RichDataTable    |       |
    ///  Level |  |                                              |    |                   /         |                            |    |  [stores SET]                    |  |      |     |          |   |     |  | Level |
    ///        |                                                             +-----------+          +--+---------------------------------------+-------------------------+  |      |     +----------+   |        |       |
    ///        |  |                                              |    |     /|\                    /|\                           |    |       /                             |      |     |              |     |  |       |
    ///        |                                                        EndemeQuantification   EndemeCharacteristic (0,0,0)             (endeme characteristic table)       |      |  InData     Is.    |        |       |
    ///        |  |                                              |    | (0,0,0)                     |                            |    |  [stores CHARACTERISTIC]            |      |  Throws     RandomSource |  |       |
    ///        |                                                                                    +------------------------------------------+----------------------------+      |  TreatAs.   FilesIO         |       |
    ///        |  |                                              |    |                            /|\                           |    |       /                                    |             __.          |  |       |
    ///       _|                                                                               EndemeMeaning (0,0,0)                    (endeme meaning table)                 ConnectSource                     |_     _|
    ///           |                                              |    |                                                          |    |  [stores MEANING]                                                     |           
    ///           [-------------------- ACTION ------------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    ///
    ///      beta code - nearly production ready</remarks>
    public class EndemeItem : IAmAnEndemeItem
    {
        // ----------------------------------------------------------------------------------------
        //  Member properties
        /* ------------------------------------------------------------------------------------- */
        [Key]
        public long        EndemeId   { get { return _itemId    ; } set { _itemId     = value; } } long   _itemId; // endeme id of the item
        public Endeme      ItemEndeme { get; set; } // make this the parent class? no!
        public Guid        ItemKey    { get; set; }
        public string      ItemCode   { get; set; }
        public string      ItemLabel  { get; set; }
        public string      ItemDescr  { get; set; }
        public double      TempMatch  { get; set; } // ephemeral data regarding matching of the EndemeItem with another endeme
        public EndemeValue Item       { get { if (_value == null || _value.Count < 1) return null    ; else return _value[0]; }
                                        set { if (_value == null  ) _value = new List<EndemeValue>(1);
                                              if (_value.Count < 1) _value.Add(Simple(value)); else _value[0] = Simple(value);
                                      }     } private List<EndemeValue> _value;


        // ----------------------------------------------------------------------------------------
        //  Accessors
        // ----------------------------------------------------------------------------------------
        [IndexerName("IloveJesus")]
        public EndemeValue this[int i]
        {
            get { if (_value.Count > i) return _value[i]; else return null; }
            set { if (_value.Count <= i) for (int j = _value.Count; j < i; ++j) _value.Add(EndemeValue.Empty); _value.Add(value); }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------                                     
        public  EndemeItem(                            EndemeValue   value) { Init(""   , null, value                 ); }
        public  EndemeItem(                 Endeme en, EndemeValue   value) { Init(""   , en  , value                 ); }
        public  EndemeItem(string label   , Endeme en, EndemeValue   value) { Init(label, en  , value                 ); }
        public  EndemeItem(long   endemeId, Endeme en, object        value) { Init(""   , en  , new EndemeValue(value)); EndemeId = endemeId; }
        private EndemeItem(string label   , Endeme en, EndemeValue[] value) { Init(label, en  , value                 ); }


        // --------------------------------------------------------------------------------------
        //  Short methods
        // --------------------------------------------------------------------------------------
        public void          AddValue(       object obj) { EndemeValue val = new EndemeValue(obj); _value.Add   (val)   ; }
        public void          Insert  (int i, object obj) { EndemeValue val = new EndemeValue(obj); _value.Insert(i, val); }
        public void          Remove  (       object obj) { EndemeValue val = new EndemeValue(obj); _value.Remove(val)   ; }
                                     
        public bool          Contains(Endeme en        ) { if (en.EnSet == null) en.EnSet = ItemEndeme.EnSet; return ItemEndeme.Contains(en); }
        public void          Clear   (                 ) {        _value.Clear   ()      ; }
        public EndemeItem    Copy    (                 ) { EndemeItem item = new EndemeItem(ItemLabel, ItemEndeme.Copy(), _value.ToArray()); item.ItemCode = ItemCode; item.TempMatch = TempMatch; return item; }
        public void          RemoveAt(int i            ) {        _value.RemoveAt(i)     ; }
        public void          Reverse (                 ) {        _value.Reverse ()      ; }
        public EndemeValue[] ToArray (                 ) { return _value.ToArray ()      ; }


        // --------------------------------------------------------------------------------------
        //  Short properties
        // --------------------------------------------------------------------------------------
        public static EndemeItem Empty      { get { return new EndemeItem("", Endeme.Empty, EndemeValue.Empty) ; } }
        public        EndemeSet  EnSet      { get { return ItemEndeme.EnSet; } set { ItemEndeme.EnSet = value; } }
        public        int        ValueCount { get { return _value.Count                                        ; } }


        // --------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        private void Init(string label, Endeme path, EndemeValue value)
        {
            if (value != null && value.GetType().Name == "EndemeItem")
                throw new Exception("You may not put an EndemeItem   inside an EndemeItem  .");
            Init(label, path, 0.0);
            _value = new List<EndemeValue>(1); Item = value ;
        }
        private void Init(string label, Endeme path, EndemeValue[] value)
        {
            Init(label, path, 0.0);
            _value = value.ToList<EndemeValue>();  // with linq
          //_value = new List<object>(value); // without linq
        }

        // --------------------------------------------------------------------------------------
        /// <!-- Init -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="endeme"></param>
        /// <param name="match"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        private void Init(string label, Endeme endeme, double match)
        {
            if (endeme != null && endeme.EnSet == null)
                Pause();
            ItemEndeme = endeme;
            ItemLabel  = label ;
            ItemCode   = ""    ;
            TempMatch  = match ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Part -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="enItemFormatted"></param>
        /// <returns></returns>
        public static string Part(int n, string enItemFormatted)
        {
            string[] str = enItemFormatted.Split(":=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (str.Length > n) return str[n]; else return "";
        }

        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Simple -->
        /// <summary>
        ///      Absorbs undefined values (i.e values without Endemes)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static EndemeValue Simple(EndemeValue input)
        {
            EndemeValue storeValue = input;
            if (input                 != null               &&
                input.Value           != null               &&
                input.Value.GetType() == typeof(EndemeItem) &&
                ((EndemeItem)input.Value).ItemEndeme == null)
                   storeValue = ((EndemeItem)input.Value).Item;
            return storeValue;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeTableObject -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EndemeTable ToEndemeTableObject()
        {
            EndemeTable enRow = new EndemeTable();


            enRow.EndemeId      = this.EndemeId;


            enRow.EndemeSetId   = this.EnSet.EndemeSetId;
            enRow.EndemeSet     = this.EnSet;


            enRow.RawSource     = this.ItemEndeme.RawSource  ;
            enRow.MultipleUse   = this.ItemEndeme.MultipleUse;
            enRow.EndemeString  = this.ItemEndeme            ;


            enRow.EndemeCode    = this.ItemCode ;
            enRow.EndemeLabel   = this.ItemLabel;
            enRow.EndemeDescr   = this.ItemDescr;


            enRow.ValueBinary   = this.Item.BinValue ;
            enRow.ValueDateTime = this.Item.DateValue;
            enRow.ValueFloat    = this.Item.DblValue ;
            enRow.ValueNumber   = this.Item.DecValue ;
            enRow.ValueText     = this.Item.StrValue ;


            enRow.EndemeProfileId = -1;

            return enRow;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToHeaderString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ToHeaderString()
        {
            return "item:<" + ItemLabel + ">";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToMatchString -->
        private string ToMatchString()
        {
            return "("+Math.Round(TempMatch,3)+")";
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToProfileString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToProfileString()
        {
            string str = "";
            if (ItemEndeme == null)
            {
                str += "endeme item has null endeme profile";
            }
            else
            {
                str += "en:";
                str += "'";
                if (ItemEndeme.EnSet == null) str += "no set"; else str += ItemEndeme.EnSet.Label;
                str += ":";
                str += ItemEndeme;
                str += "'";
            }
            str += "("+Regex.Replace(TempMatch.ToString(),"^(....).*$", "$1")+")";
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToValueString -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ToValueString()
        {
            string output = "value:";
            if      (_value               == null) output += "?"                    ; 
            else if (_value.Count         == 0   ) output += "no values"            ;
            else if (_value.Count         >  1   ) output += _value.Count + "values";
            else if (_value[0]            == null) output += "null"                 ;
            else if (_value[0].ToString() == ""  ) output += "''"                   ;
            else                                   output += _value[0].ToString()   ; // the normal case
            return output;
        }


        // ----------------------------------------------------------------------------------------
        //  Override methods
        // ----------------------------------------------------------------------------------------
        public override string ToString()
        {
            string str = ToHeaderString() + "," + ToProfileString() + "," + ToValueString() + ToMatchString();
            return str;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HasOrder -->
        /// <summary>
        ///      Returns true if Endeme maintains the same order as the input string
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        /// <remarks>
        ///      Move this to Endeme class
        /// </remarks>
        public bool HasOrder(string order)
        {
            return ItemEndeme.HasOrderOf(new Endeme(ItemEndeme.EnSet, order, false));
        }
    }
}
