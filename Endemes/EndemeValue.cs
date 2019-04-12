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
using InfoLib.Testing;                // for 
using System;                         // for 
using System.IO;                      // for MemoryStream
using System.Runtime.Serialization.Formatters.Binary; // for BinaryFormatter
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeValue -->
    /// <summary>
    ///      The EndemeValue class contains a value to be referenced through semantic data endemes
    /// </summary>
    /// <remarks>
    /// 
    ///      Related classes
    ///      ---------------                                                                                (1-1,1)             ((1-1,1)*n)
    ///                                                                                 +----------------+- EndemeItem ------+- EndemeList -------+
    ///                                                                                /                /                    |                    |
    ///                                                                               /                /                     |                    |
    ///                                                                              /  IEndemeItem --+                      |                    |
    ///                                                                             /                 |   IEnumerable[Guid> -+                    |  (*)
    ///                                                              (1-1-0)       /    (0-0-1)       |                      |                    +- EndemeField 
    ///                                            +--------------+- Endeme ------+     EndemeValue --+                      |                    |
    ///                                           /              /                 \                   \                     |                    |
    ///  (0-0-0)          (0-0-0)                /  (1-0-0)     /  (n-0-0)          \   (n-n-0)         \   (n-n-1)          |  ((n-n-1)*n)       | 
    ///  EndemeMeaning -- EndemeCharacteristic -+-- EndemeSet -+-- EndemeDictionary -+- EndemeProfile ---+- EndemeObject ----+- EndemeDefinition -+ - - - - - - - EndemeAccess
    ///
    ///      Pre-alpha code
    /// </remarks>
    public class EndemeValue
    {
        // ----------------------------------------------------------------------------------------
        //  Members
        // ----------------------------------------------------------------------------------------
        public  object Value     { get; set; }
        public  string Label     { get; set; }
        private Type   ValueType { get; set; }


        // ----------------------------------------------------------------------------------------
        //  Casting (implicit) - mostly primitives but including endemes
        // ----------------------------------------------------------------------------------------
        public static implicit operator EndemeValue(bool      bit   ) { return new EndemeValue(bit   , typeof(bool     )); }
        public static implicit operator EndemeValue(byte      bits  ) { return new EndemeValue(bits  , typeof(byte     )); }
        public static implicit operator EndemeValue(byte[]    binary) { return new EndemeValue(binary, typeof(byte[]   )); }
        public static implicit operator EndemeValue(char      cha   ) { return new EndemeValue(cha   , typeof(char     )); }
        public static implicit operator EndemeValue(DateTime? date  ) { return new EndemeValue(date  , typeof(DateTime?)); }
        public static implicit operator EndemeValue(decimal   dec   ) { return new EndemeValue(dec   , typeof(decimal  )); }
        public static implicit operator EndemeValue(double    number) { return new EndemeValue(number, typeof(double   )); }
        public static implicit operator EndemeValue(Endeme    en    ) { return new EndemeValue(en    , typeof(Endeme   )); }
        public static implicit operator EndemeValue(EndemeSet enSet ) { return new EndemeValue(enSet , typeof(EndemeSet)); }
        public static implicit operator EndemeValue(float     real  ) { return new EndemeValue(real  , typeof(float    )); }
        public static implicit operator EndemeValue(int       medium) { return new EndemeValue(medium, typeof(int      )); }
        public static implicit operator EndemeValue(Int16     small ) { return new EndemeValue(small , typeof(Int16    )); }
        public static implicit operator EndemeValue(long      large ) { return new EndemeValue(large , typeof(long     )); }
        public static implicit operator EndemeValue(string    text  ) { return new EndemeValue(text  , typeof(string   )); }


        // ----------------------------------------------------------------------------------------
        //  Casting (explicit) - mostly primitives but including endemes  [This needs unit testing!]
        // ----------------------------------------------------------------------------------------
        public byte[]    BinValue  { get { if (Value == null) return null; if (ValueType == typeof(byte[])) { using (MemoryStream stream = new MemoryStream()) { new BinaryFormatter().Serialize(stream, Value); return stream.ToArray(); } } else return null; } }
        public bool      BoolValue { get {               if (ValueType == typeof(bool     )) return (bool     )Value; if (Value.GetType() == typeof(bool)     )               return (bool)Value;      return false          ; } }
        public byte      ByteValue { get { byte     byt; if (ValueType == typeof(byte     )) return (byte     )Value; if (byte    .TryParse(StrValue, out byt))               return byt;              return 0              ; } }
        public char      CharValue { get {               if (ValueType == typeof(char     )) return (char     )Value; if (Value is char)                                      return (char)Value;      return '\0'           ; } }
        public DateTime? DateValue { get { DateTime dat; if (ValueType == typeof(DateTime?)) return (DateTime?)Value; if (DateTime.TryParse(StrValue, out dat))               return dat;              return null           ; } }
        public decimal   DecValue  { get { decimal  dec; if (ValueType == typeof(decimal  )) return (decimal  )Value; if (decimal .TryParse(StrValue, out dec))               return dec;              return 0.0M           ; } }
        public double    DblValue  { get { double   num; if (ValueType == typeof(double   )) return (double   )Value; if (double  .TryParse(StrValue, out num))               return num;              return 0.0            ; } }
        public EndemeSet SetValue  { get { Fix();        if (ValueType == typeof(EndemeSet)) return (EndemeSet)Value; if (Value is EndemeSet)                                 return (EndemeSet)Value; return EndemeSet.Empty; } }
        public float     FltValue  { get { float    num; if (ValueType == typeof(float    )) return (float    )Value; if (float   .TryParse(StrValue, out num))               return num;              return 0.0F           ; } }
        public int       IntValue  { get { int      med; if (ValueType == typeof(int      )) return (int      )Value; if (int     .TryParse(StrValue, out med))               return med;              return 0              ; } }
        public Int16     SmlValue  { get { Int16    sml; if (ValueType == typeof(Int16    )) return (Int16    )Value; if (Int16   .TryParse(StrValue, out sml))               return sml;              return 0              ; } }
        public long      BigValue  { get { long     big; if (ValueType == typeof(long     )) return (long     )Value; if (long    .TryParse(StrValue, out big))               return big;              return 0              ; } }
        public string    StrValue  { get {               if (ValueType == typeof(string   )) return (string   )Value; if (Value != null)                                      return Value.ToString(); return ""             ; } }
        public Endeme    EnValue   { get { Fix(Value);   if (ValueType == typeof(Endeme   )) return (Endeme   )Value; if (Value != null && Value.GetType() == typeof(Endeme)) return (Endeme)Value;    return Endeme.Empty   ; } }


        // ----------------------------------------------------------------------------------------
        //  Operators
        // ----------------------------------------------------------------------------------------
        public static bool operator == (EndemeValue lhs, EndemeValue rhs)
        {
            return ( object.ReferenceEquals(null, lhs) &&  object.ReferenceEquals(null, rhs) ||
                    !object.ReferenceEquals(null, lhs) && !object.ReferenceEquals(null, rhs) && Is.Ok(lhs.Value, Is.the_same_sets_as, rhs.Value));
        }
        public static bool operator != (EndemeValue lhs, EndemeValue rhs) { return !(lhs == rhs); }


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public EndemeValue(object obj)
        {
            Value = obj;
            if (obj != null)
                ValueType = obj.GetType();
            if (Value != null && Value.GetType() == typeof(EndemeValue)) Value = ((EndemeValue)Value).Value;
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); }
        }
        private EndemeValue(object obj, Type dataType)
        {
            Value = obj;
            ValueType = dataType;
            if (Value != null && Value.GetType() == typeof(EndemeValue)) Value = ((EndemeValue)Value).Value;
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); }
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public          EndemeValue Copy()             { EndemeValue value = new EndemeValue(Value); value.Label = Label; return value; }
        public static   EndemeValue Empty        { get { return new EndemeValue(null);                                                  } }
        public override bool        Equals(object obj) { if (obj == null || obj.GetType() != typeof(EndemeValue)) return false; return Is.Ok(this, Is.the_same_sets_as, (EndemeValue)obj); }
        public override int         GetHashCode()      { return this.ToString().GetHashCode();                                          }
        public          bool        IsString     { get { return (this.Type == typeof(string));                                          } }
        public override string      ToString()         { if (Value == null) return "[null]"; else return Value.ToString();              }
        public          Type        Type         { get { return Value.GetType();                                                        } }


        /// <summary>
        /// 
        /// </summary>
        private void Fix()
        {
            if (Value != null && Value.GetType() == typeof(EndemeValue)) { Value = ((EndemeValue)Value).Value; Pause(); } // this should not be needed
        }

        /// <summary>
        ///      This method should not be needed, it is here to preserve sanity
        /// </summary>
        /// <param name="value"></param>
        private void Fix(object value)
        {
            if (Value != null && Value.GetType() == typeof(EndemeValue))
                { Value = ((EndemeValue)Value).Value; Pause(); } // this line should not be needed
        }

        /// <summary>
        /// 
        /// </summary>
        private void Pause()
        {
        }
    }
}
