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
using InfoLib.Endemes ;        // for 
using InfoLib.HardData;        // for InData
using System.Collections.Generic;     // for Dictionary
using System.Data;                    // for DataTable, DataRow
using System.Data.SqlClient;          // for SqlConnection
using System.Data.SqlTypes;           // for 
using System.Text;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Xref -->
    /// <summary>
    ///      Cross reference tables for various things in the database and various codes used in the import files
    /// </summary>
    /// <remarks>
    ///      The reason for all of the empty catch blocks is to try to make their methods thread-safe
    ///      
    ///      alpha toy code - used once in production, expected to be deprecated
    /// </remarks>
    public static class Xref
    {
        // ------------------------------------------------------------------------------
        //  Members
        // ------------------------------------------------------------------------------
        private static NumericLookup _intGender     ; private static StringLookup  _strGender     ;
        private static NumericLookup _state         ;
        private static DataTable     _dsmCode       ;


        // ------------------------------------------------------------------------------
        //  Properties
        // ------------------------------------------------------------------------------
        public static NumericLookup IntGender      { get { if (_intGender      == null) FillIntGender     (); return _intGender     ; } }
        public static  StringLookup StrGender      { get { if (_strGender      == null) FillStrGender     (); return _strGender     ; } }
        public static NumericLookup State          { get { if (_state          == null) FillStates        (); return _state         ; } }
        public static DataTable     DsmCodeTable   { get { if (_dsmCode        == null) FillDsmCodeTable  (); return _dsmCode       ; } }


        // ------------------------------------------------------------------------------
        //  Property-like methods (property wrapper)
        // ------------------------------------------------------------------------------
        public static SqlInt32 DsmCode_Lookup(SqlString dsmCode)
        {
            if (_dsmCode == null) FillDsmCodeTable();


            // ----------------------------------------------------------------------
            //  Bail out if blank
            // ----------------------------------------------------------------------
            if (dsmCode.IsNull                               ) return SqlInt32.Null;
            if (string.IsNullOrEmpty(dsmCode.ToString().Trim())) return SqlInt32.Null;


            // ----------------------------------------------------------------------
            //  Find matches
            // ----------------------------------------------------------------------
            string code1 = Regex.Replace(dsmCode.ToString().Trim(), "'", "");
            SqlInt32 id = SqlInt32.Null;
            DataRow[] list = null;


            string code2 = "JeSuS";
            string code3 = "JeSuS";
            switch (code1.Length)
            {
                case 3  : code2 = code1 + ".0";                   code3 = code1 + ".00";                   break;
                case 6  : code2 = Regex.Replace(code1, "0$", ""); code3 = Regex.Replace(code1, "00$", ""); break;
                default : code2 = code1 + "0";                    code3 = Regex.Replace(code1, "0$" , ""); break;
            }


            // ----------------------------------------------------------------------
            //  Extract id
            // ----------------------------------------------------------------------
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '"   + code1 + "'" + " OR ConditionCode_5 = '"   + code1 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '"   + code2 + "'" + " OR ConditionCode_5 = '"   + code2 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '"   + code3 + "'" + " OR ConditionCode_5 = '"   + code3 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '''" + code1 + "'" + " OR ConditionCode_5 = '''" + code1 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '''" + code2 + "'" + " OR ConditionCode_5 = '''" + code2 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull) { list = _dsmCode.Select("ConditionCode_4TR = '''" + code3 + "'" + " OR ConditionCode_5 = '''" + code3 + "'"); if (list.Length > 0) { id = InData.GetSqlInt32(list[0], "DsmCodeId"); } }
            if (id.IsNull)
                Pause();

            return id;
        }

        private static void Pause()
        {
        }

        public static SqlInt32 State_Lookup(SqlInt32 importStateId)
        {
            if (_state == null) FillStates();
            if (importStateId.IsNull)        return SqlInt32.Null;
            else { int key = (int)importStateId;
                   if (_state.ContainsKey(key)) return _state[key];
                   else                         return SqlInt32.Null; }
        }


        // ------------------------------------------------------------------------------
        //  Public Methods
        // ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
        /// <!-- Check -->
        /// <summary>
        ///      Compares an xref to make sure all of its values are in the associated table
        /// </summary>
        /// <param name="listFromXref"></param>
        /// <param name="label"></param>
        /// <param name="tableName"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <returns></returns>
        /// <remarks>TODO: do the check the other way too</remarks>
        public static string Check(Dictionary<int,int> listFromXref, string label
            , string tableName, string keyColumn, string valueColumn, InfoAspect aspect)
        {
            Dictionary<int,int> listFromTable = InData.GetDictionaryFrom(tableName, keyColumn, valueColumn, aspect.SecondaryConnection);
            string err = "";
            string delim = "";
            foreach (int id in listFromXref.Values)
            {
                if (!listFromTable.ContainsKey(id))
                {
                    err += delim + label + " Xref ID missing from " + tableName + " table: " + id;
                    delim = "\r\n";
                }
            }

            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Check -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listFromXref"></param>
        /// <param name="label"></param>
        /// <param name="tableName"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static string Check(NumericLookup listFromXref, string label
            , string tableName, string keyColumn, string valueColumn, InfoAspect aspect)
        {
            Dictionary<int,int> listFromTable = InData.GetDictionaryFrom(tableName, keyColumn, valueColumn, aspect.SecondaryConnection);
            string err = "";
            string delim = "";
            foreach (int id in listFromXref.Values)
            {
                if (!listFromTable.ContainsKey(id))
                {
                    err += delim + label + " Xref ID missing from " + tableName + " table: " + id;
                    delim = "\r\n";
                }
            }

            return err;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DsmCode -->
        /// <summary>
        ///      Retrieves candidate dsm id's for a particular code
        /// </summary>
        /// <param name="dsmCode"></param>
        /// <returns></returns>
        public static DataRow[] DsmCode(SqlString dsmCode)
        {
            DataRow[] field = new DataRow[0];
            if (InData.DetectSqlInjection(dsmCode.ToString()))
                return field;


            if (_dsmCode == null)
            {
                FillDsmCodeTable();
            }

            if (!dsmCode.IsNull)
            {
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_5   =    '"  + dsmCode.ToString() + "' AND ConditionNameSource LIKE '%DSM%'");
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_4TR =    '"  + dsmCode.ToString() + "' AND ConditionNameSource LIKE '%DSM%'");
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_5   =    '"  + dsmCode.ToString() + "'" );
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_4TR =    '"  + dsmCode.ToString() + "'" );
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_5   LIKE '%" + dsmCode.ToString() + "%'");
                if (field.Length == 0) field = _dsmCode.Select("ConditionCode_4TR LIKE '%" + dsmCode.ToString() + "%'");
            }

            return field;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetRaceFromBits -->
        /// <summary>
        ///      Converts the race indicators from CANVaS into the raceId in VABHAS
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static SqlInt32 GetRaceFromBits(DataRow field)
        {
            // ------------------------------------------------------------------
            //  Get the bits
            // ------------------------------------------------------------------
            SqlBoolean black  = InData.GetSqlBoolean(field, "Race_Black_Ind"    );
            SqlBoolean yellow = InData.GetSqlBoolean(field, "Race_Asian_Ind"    );
            SqlBoolean brown  = InData.GetSqlBoolean(field, "Race_Hawaiian_Ind" );
            SqlBoolean red    = InData.GetSqlBoolean(field, "Race_Native_Am_Ind");
            SqlBoolean white  = InData.GetSqlBoolean(field, "Race_White_Ind"    );


            // ------------------------------------------------------------------
            //  Add the bits
            // ------------------------------------------------------------------
            int total = 0
                + (int)black .ToSqlInt32()
                + (int)yellow.ToSqlInt32()
                + (int)brown .ToSqlInt32()
                + (int)red   .ToSqlInt32()
                + (int)white .ToSqlInt32();


            // ------------------------------------------------------------------
            //  Use the bits
            // ------------------------------------------------------------------
            SqlInt32 raceId = SqlInt32.Null;
            if (total > 1)   raceId = 13;  // 13. Two or more races.
            else if (black ) raceId = 1 ;  // 1.  African American
            else if (yellow) raceId = 2 ;  // 2.  Asian
            else if (brown ) raceId = 3 ;  // 3.  Native Hawaiian
            else if (red   ) raceId = 4 ;  // 4.  American Indian
            else if (white ) raceId = 5 ;  // 5.  Caucasian
            else             raceId = 14;  // 14. Decline to Identify


            return raceId;
        }

        private static void FillDsmCodeTable()
        {
            using (SqlConnection connection = new SqlConnection(InfoAspect.OutputConnectionString))
            {
                InData.Open(connection);
                _dsmCode = InData.GetTable("DsmCode"
                    , " SELECT DsmCodeId, ConditionCode_4TR, ConditionCode_5, ConditionName, ConditionNameSource"
                    + " FROM   DsmCode"
                    , connection);
            }
        }

        private static void FillIntGender()
        {
            if (_intGender == null) try { _intGender = new NumericLookup(3); } catch { }
                                                                              // Import       INTFDB                     SANDBOX
            _intGender.Add(2, 12); // 2-Unknown -> 12-Unknown or Unspecified -> 3-Unknown or Unspecified
            _intGender.Add(3,  1); // 3-Female  -> 1 -Female                 -> 1-Female
            _intGender.Add(4,  2); // 4-Male    -> 2 -Male                   -> 2-Male
        }

        private static void FillStrGender()
        {
            if (_strGender == null) try { _strGender = new StringLookup(5); } catch { }

            _strGender.Add("F"   ,"1" ); // 1	F	Female
            _strGender.Add("M"   ,"2" ); // 2	M	Male
            _strGender.Add("NULL","12"); // 12	U	Unknown or Unspecified
            _strGender.Add("Null","12"); // 12	U	Unknown or Unspecified
            _strGender.Add(""    ,"12"); // 12	U	Unknown or Unspecified
        }

        private static void FillStates()
        {
            if (_state == null) try { _state = new NumericLookup(60); } catch { }

            _state.Add( 1,  1); //  1	AL	Alabama
            _state.Add( 2,  2); //  2	AK	Alaska
            _state.Add( 3,  3); //  3	AS	American Samoa
            _state.Add( 4,  4); //  4	AZ	Arizona
            _state.Add( 5,  5); //  5	AR	Arkansas
            _state.Add( 6,  6); //  6	CA	California
            _state.Add( 7,  7); //  7	CO	Colorado
            _state.Add( 8,  8); //  8	CT	Connecticut
            _state.Add( 9,  9); //  9	DE	Delaware
            _state.Add(10, 10); // 10	DC	District of Columbia
            _state.Add(11, 11); // 11	FM	Federated States of Micronesia
            _state.Add(12, 12); // 12	FL	Florida
            _state.Add(13, 13); // 13	GA	Georgia
            _state.Add(14, 14); // 14	GU	Guam
            _state.Add(15, 15); // 15	HI	Hawaii
            _state.Add(16, 16); // 16	ID	Idaho
            _state.Add(17, 17); // 17	IL	Illinois
            _state.Add(18, 18); // 18	IN	Indiana
            _state.Add(19, 19); // 19	IA	Iowa
            _state.Add(20, 20); // 20	KS	Kansas
            _state.Add(21, 21); // 21	KY	Kentucky
            _state.Add(22, 22); // 22	LA	Louisiana
            _state.Add(23, 23); // 23	ME	Maine
            _state.Add(24, 24); // 24	MH	Marshall Islands
            _state.Add(25, 25); // 25	MD	Maryland
            _state.Add(26, 26); // 26	MA	Massachusetts
            _state.Add(27, 27); // 27	MI	Michigan
            _state.Add(28, 28); // 28	MN	Minnesota
            _state.Add(29, 29); // 29	MS	Mississippi
            _state.Add(30, 30); // 30	MO	Missouri
            _state.Add(31, 31); // 31	MT	Montana
            _state.Add(32, 32); // 32	NE	Nebraska
            _state.Add(33, 33); // 33	NV	Nevada
            _state.Add(34, 34); // 34	NH	New Hampshire
            _state.Add(35, 35); // 35	NJ	New Jersey
            _state.Add(36, 36); // 36	NM	New Mexico
            _state.Add(37, 37); // 37	NY	New York
            _state.Add(38, 38); // 38	NC	North Carolina
            _state.Add(39, 39); // 39	ND	North Dakota
            _state.Add(40, 40); // 40	MP	Northern Mariana Islands
            _state.Add(41, 41); // 41	OH	Ohio
            _state.Add(42, 42); // 42	OK	Oklahoma
            _state.Add(43, 43); // 43	OR	Oregon
            _state.Add(44, 44); // 44	PW	Palau
            _state.Add(45, 45); // 45	PA	Pennsylvania
            _state.Add(46, 46); // 46	PR	Puerto Rico
            _state.Add(47, 47); // 47	RI	Rhode Island
            _state.Add(48, 48); // 48	SC	South Carolina
            _state.Add(49, 49); // 49	SD	South Dakota
            _state.Add(50, 50); // 50	TN	Tennessee
            _state.Add(51, 51); // 51	TX	Texas
            _state.Add(52, 52); // 52	UT	Utah
            _state.Add(53, 53); // 53	VT	Vermont
            _state.Add(54, 54); // 54	VI	Virgin Islands
            _state.Add(55, 55); // 55	VA	Virginia
            _state.Add(56, 56); // 56	WA	Washington
            _state.Add(57, 57); // 57	WV	West Virginia
            _state.Add(58, 58); // 58	WI	Wisconsin
            _state.Add(59, 59); // 59	WY	Wyoming
        }
    }
}