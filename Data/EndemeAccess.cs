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
using InfoLib.Models  ;               // for EndemeTable
using InfoLib.Endemes ;               // for many
using InfoLib.HardData;               // for ConnectSource
using InfoLib.SoftData;               // for TreatAs.StrValue
using InfoLib.Strings ;               // for __.CommonPrefixLength, __.LevenshteinDistance
using InfoLib.Testing ;               // for Is.Trash
using System;                         // for Guid
using System.Collections.Generic;     // for List EndemeSetOf
using System.Data;                    // for CommandType, ConnectionState, DataTable
using System.Data.SqlClient;          // for SqlConnection, SqlTransaction
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Data
{
	// ---------------------------------------------------------------------------------------------
	/// <!-- EndemeAccess -->
    /// <summary>
    ///      The EndemeAccess class is a more recent, stripped down and better tested version of EndemeDataAccess1
    /// </summary>
	public class EndemeAccess
	{
        // -----------------------------------------------------------------------------------------
        //  Members
        // -----------------------------------------------------------------------------------------
        public static string SetIdColumn    { get { return m_setIdColumn   ; } } private static string m_setIdColumn    = "EndemeSetId"   ; // I'm going to want to change this at some point and make it consistent across all endeme tables:: EndemeSetID
        public static string SetLabelColumn { get { return m_setLabelColumn; } } private static string m_setLabelColumn = "EndemeSetLabel";


        public        string Errors         { get { return _errors; }
                                              set { _errors = value; } } private string _errors;
        internal      InfoAspect _aspect    { get; set; }
        internal      Result     _result;

        public const  string LaEndemeMain    = "dbo.Endeme              ";
        public const  string LaEndemeChar    = "dbo.EndemeCharacteristic";
        public const  string LaEndemeLarge   = "dbo.EndemeLarge         ";
        public const  string LaEndemeSet     = "dbo.EndemeSet           ";
        public const  string LaEndemeProfile = "dbo.EndemeProfile       ";
        public const  string LaEndemeIndex   = "dbo.EndemeIndex         ";
        public const  string LaEndemeUsage   = "dbo.EndemeUsage         ";
        public const  string LaEndemeMeaning = "dbo.EndemeMeaning       ";


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeAccess()
        {
            _errors = "";
        }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        /* ------------------------------------------------------------------------------------- */ /// <summary>Deletes all the characteristics for an endeme set, leaving the header intact</summary>
		private int ClearSet(Guid setId, string conn) { return RichDataTable.AttemptToDelete(conn, "FROM " + LaEndemeChar + " WHERE "+SetIdColumn+" = " + "'" + setId.ToString() + "'", 32); }

        #region Endeme Characteristic table methods

        // ----------------------------------------------------------------------------------------
        /// <!-- AttachEndemeSubSet -->
        /// <summary>
        ///      Makes an endeme set a subset by attaching it to a characteristic of another endeme set
        /// </summary>
        /// <param name="enCharId"></param>
        /// <param name="subEnSetId"></param>
        /// <param name="conn"></param>
        public void AttachEndemeSubSet(Guid enCharId, Guid subEnSetId, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " UPDATE " + LaEndemeChar + " SET CharacteristicIsASet = '"+subEnSetId+"'"
                + "\r\n" + " WHERE  EndemeCharacteristicID = '"+enCharId+"'"
                , Throws.Actions, "P");


            cmd.ExecuteNonQuery();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AttachEndemeSubSet -->
        /// <summary>
        ///      Makes an endeme set a subset by attaching it to a characteristic of another endeme set
        /// </summary>
        /// <param name="enCharId"></param>
        /// <param name="subEnSetId"></param>
        /// <param name="conn"></param>
        public void AttachEndemeSubSet_old(Guid enCharId, Guid subEnSetId, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " UPDATE " + LaEndemeChar + " SET CharacteristicIsASet = '"+subEnSetId+"'"
                + "\r\n" + " WHERE  charId = '"+enCharId+"'"
                , Throws.Actions, "P");


            cmd.ExecuteNonQuery();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DeleteCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enChar"></param>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public int DeleteCharacteristic(EndemeCharacteristic enChar, Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            return RichDataTable.DeleteOneRow("FROM " + LaEndemeChar + " WHERE "+SetIdColumn+" = '" + setId.ToString() + "' AND CharacteristicLetter = '" + enChar.Letter + "'"
                , connection, trx);
        }

        public int ReUpEndemeSet(EndemeSet enSet, InfoAspect aspect)
        {
            int ok = 0;

            SqlConnection connection = ConnectSource.Connection(aspect.SecondaryConn);
            try                  { ok = UpdateEndemeSet(enSet, enSet.EndemeSetId, connection, null); }
            catch (Exception ex) { Is.Trash(ex); throw; }
            finally              { connection.Dispose(); }
            return ok;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetCharacteristicId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageSetId"></param>
        /// <param name="characteristicLabel"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public Guid GetCharacteristicId(Guid ageSetId, string characteristicLabel, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " SELECT EndemeCharacteristicID"
                + "\r\n" + " FROM " + LaEndemeChar + " WITH(NOLOCK)"
                + "\r\n" + " WHERE   CharacteristicLabel = '"+characteristicLabel+"'"
                + "\r\n" + "     AND "+SetIdColumn+" = '"+ageSetId.ToString()+"'"
                , Throws.Actions, "P");


            RichDataTable table = new RichDataTable(cmd, "OneChar", SetIdColumn);
            Guid charId = table.GuidValue(0, "CharId", Guid.Empty);
            return charId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetCharacteristicId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageSetId"></param>
        /// <param name="characteristicLabel"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public Guid GetCharacteristicId_old(Guid ageSetId, string characteristicLabel, string conn)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, conn
                , "\r\n" + " SELECT CharId"
                + "\r\n" + " FROM " + LaEndemeChar + " WITH(NOLOCK)"
                + "\r\n" + " WHERE   EndemeCharacteristicLabel = '"+characteristicLabel+"'"
                + "\r\n" + "     AND EndemeSetId = '"+ageSetId.ToString()+"'"
                , Throws.Actions, "P");


            RichDataTable table = new RichDataTable(cmd, "OneChar", SetIdColumn);
            Guid charId = table.GuidValue(0, "CharId", Guid.Empty);
            return charId;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ToCharacteristic -->
        /// <summary>
        ///      Inserts an endeme characteristic into a database, used by InsertEndemeSet
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="letter"></param>
        /// <param name="code"></param>
        /// <param name="label"></param>
        /// <param name="descr"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>
        ///      beta quality code, tested as part of InsertEndemeSet
        ///      
        /// 
        ///      How to get here:
        ///      
        ///      (application or controller)
        ///          |
        ///      ToteEndemeSet
        ///          |
        ///      ToteEndemeSet
        ///          |
        ///      InsertCharacteristic
        public Guid ToCharacteristic(Guid setId, char letter, string code, string label, string descr, SqlConnection connection, SqlTransaction trx)
        {
            // --------------------------------------------------------------------------
            //  Build the command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO " + LaEndemeChar
                + "\r\n" + "        (EndemeCharLetter,  EndemeSetId,  EndemeCharCode,  EndemeCharLabel,  EndemeCharDescr)"
                + "\r\n" + " VALUES (@Letter         , @SetID      , @Code          , @Label          , @Descr          )"
                , Throws.Actions, "PR")
                ._AddParameter_char("@Letter", letter                          )
                ._AddParameter     ("@SetID" , setId                           )
                ._AddParameter_safe("@Code"  , TreatAs.StrValue(code , ""),   8)
                ._AddParameter_safe("@Label" , label                      , 128)
                ._AddParameter_safe("@Descr" , TreatAs.StrValue(descr, "")     )
                ;


            int ok = cmd.ExecuteNonQuery();
            if (ok == 0) throw new DataException("ToCharacteristic failed");
            return setId;
        }
        public Guid ToCharacteristic(EndemeCharacteristic enChar, Guid setId, SqlConnection connection, SqlTransaction trx)
            { if (enChar != null) { return ToCharacteristic(setId, enChar.Letter, enChar.Code, enChar.Label, enChar.Descr, connection, trx); } else { return Guid.Empty; } }

        // -----------------------------------------------------------------------------------------
        /// <!-- UpdateCharacteristic -->
        /// <summary>
        ///      Updates an endeme characteristic in the database
        /// </summary>
        /// <param name="enChar"></param>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int UpdateCharacteristic(EndemeCharacteristic enChar, Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " UPDATE " + LaEndemeChar
                + "\r\n" + " SET     CharacteristicCode   = @Code"
                + "\r\n" + "     ,   CharacteristicLabel  = @Label"
                + "\r\n" + "     ,   CharacteristicDescr  = @Descr"
                + "\r\n" + " WHERE   EndemeSetID          = @SetID"
                + "\r\n" + "     AND CharacteristicLetter = @Letter"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@Code"  , enChar.Code  )
                ._AddParameter_safe("@Label" , enChar.Label )
                ._AddParameter_safe("@Descr" , enChar.Descr )
                ._AddParameter     ("@SetID" , setId        )
                ._AddParameter_char("@Letter", enChar.Letter);


            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region Endeme Set table methods

        // ----------------------------------------------------------------------------------------
        /// <!-- ConvertSetToTable -->
        /// <summary>
        ///      Converts the sets produced by OnEndemeSet into RichDataTables
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, beta quality code</remarks>
        public RichDataTable ConvertSetToTable(EndemeSet enSet, string tableName, bool IncludeEmptyCharacteristics = false)
        {
	        // --------------------------------------------------------------------------
	        //  Initialize output table
	        // --------------------------------------------------------------------------
	        RichDataTable output = new RichDataTable(tableName, SetIdColumn);
	        output.Add(SetIdColumn           , typeof(Guid)  );
	        output.Add("EndemeSetLabel"      , typeof(string));
	        output.Add("CharacteristicLetter", typeof(char)  );
	        output.Add("CharacteristicCode"  , typeof(string));
	        output.Add("CharacteristicLabel" , typeof(string));
	        output.Add("CharacteristicDescr" , typeof(string));


	        for (int c = 0; c < enSet.Count; c++)
            {
                // ----------------------------------------------------------------------
                //  Add row to table
                // ----------------------------------------------------------------------
                EndemeCharacteristic enChar = enSet.Characteristics()[c];

                if (IncludeEmptyCharacteristics | __.StringHasContent(enChar.Label)) {
        	        int row = output.Add();
        	        output[row][SetIdColumn           ] = enSet.SetId  ;
        	        output[row]["EndemeSetLabel"      ] = enSet.Label  ;
        	        output[row]["CharacteristicLetter"] = enChar.Letter;
        	        output[row]["CharacteristicCode"  ] = enChar.Code  ;
        	        output[row]["CharacteristicLabel" ] = enChar.Label ;
        	        output[row]["CharacteristicDescr" ] = enChar.Descr ;
                }
	        }

	        return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExEndemeSet -->
        /// <summary>
        ///      Deletes an endeme set
        /// </summary>
        /// <param name="enSet"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public int ExEndemeSet(EndemeSet enSet, SqlConnection connection, SqlTransaction trx)
        {
            int result = -1;

            //  get the set ID
            Guid enSetID = Guid.Empty;
            bool setFound = false;
            if (enSet.SetId == Guid.Empty) { enSetID = GetEndemeSetId(enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
            else { enSetID = OnEndemeSet(enSet.SetId, connection, trx).SetId; }


            // --------------------------------------------------------------------------
            //  Delete the characeristics and the header
            // --------------------------------------------------------------------------
            if (enSetID != Guid.Empty)
            {
                result = RichDataTable.DeleteSomeRows("FROM " + LaEndemeChar + " WHERE "+SetIdColumn+" = '" + enSetID.ToString() + "'", 24, connection, trx);
                result = RichDataTable.DeleteOneRow  ("FROM " + LaEndemeSet + "  WHERE "+SetIdColumn+" = '" + enSetID.ToString() + "'"    , connection, trx);
            }

            return result;
        }
        public int DeleteEndemeSet(EndemeSet enSet, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); int result = ExEndemeSet(enSet, connection, null); connection.Dispose(); return result; }

        // ----------------------------------------------------------------------------------------
        /// <!-- EduceSetHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setLabel"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public Guid EduceSetHeader(string setLabel, SqlConnection connection, SqlTransaction trx)
        {

            // --------------------------------------------------------------------------
            //  Initialize Variables
            // --------------------------------------------------------------------------
            Guid setId = Guid.Empty;
            RichSqlCommand cmdRead = null;
            RichSqlCommand cmdInsert = null;

            try
            {
                // -----------------------------------------------------------------------
                //  try to get a table containing the endeme set ID
                // -----------------------------------------------------------------------
                cmdRead = new RichSqlCommand(CommandType.Text, connection, trx
                    , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE "+SetLabelColumn+" = @SetLabel"
                    , Throws.Actions, "P")
                    ._AddParameter_safe("@SetLabel", setLabel);


                RichDataTable table = new RichDataTable(cmdRead, "EndemeSet", trx, SetIdColumn);


                if ((table.Count < 1))
                {
                    // -------------------------------------------------------------------
                    //  Build command to create the set if it does not yet exist
                    // -------------------------------------------------------------------
                    cmdInsert = new RichSqlCommand(CommandType.Text, connection, null
                        , " INSERT INTO " + LaEndemeSet + " (" + SetIdColumn + ", "+SetLabelColumn+") VALUES (@EnSetId ,@SetLabel)"
                        , Throws.Actions, "P")
                        ._AddParameter     ("@EnSetId", Guid.NewGuid())
                        ._AddParameter_safe("@SetLabel", setLabel);
                    cmdInsert.ExecuteNonQuery();

                    table = new RichDataTable(cmdRead, "EndemeSet", trx, SetIdColumn);
                }

                // -----------------------------------------------------------------------
                //  Extract Endeme Set ID
                // -----------------------------------------------------------------------
                setId = table.GuidValue(0, SetIdColumn, Guid.Empty);

            }
            catch (Exception ex) { Is.Trash(ex); }
            finally
            {
                if (cmdRead   != null) { cmdRead  .Dispose(); }
                if (cmdInsert != null) { cmdInsert.Dispose(); }
            }

            return setId;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeSetId"></param>
        /// <param name="enAspect"></param>
        /// <returns></returns>
        public EndemeSet OnEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichDataTable table = ReinEndemeSet(endemeSetId, aspect);
            EndemeSet enSet = ReonEndemeSet(table, 0, true);
            return enSet;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        ///      Converts a table produced by ConvertSetToTable into an endeme set
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, beta quality code</remarks>
        public EndemeSet OnEndemeSet(DataTable dt)
        {
            RichDataTable rt = new RichDataTable                      (dt, "CharacteristicLetter"            ) ;
            EndemeSet     es = new EndemeSet            ( rt.GuidValue(0 , SetIdColumn           , Guid.Empty)
                                                        , rt.StrValue (0 , "EndemeSetLabel"      , ""        ));
            for (int rw = 0; rw < rt.Count; rw++) es.Add( rt.CharValue(rw, "CharacteristicLetter", ' '       )
                                                        , rt.StrValue (rw, "CharacteristicCode"  , ""        )
                                                        , rt.StrValue (rw, "CharacteristicLabel" , ""        )
                                                        , rt.StrValue (rw, "CharacteristicDescr" , ""        ));
            return es;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        ///      Returns an endeme set
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public EndemeSet OnEndemeSet(Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            EndemeSet enSet = EndemeSet.Empty;
            RichSqlCommand cmd = null;

            try
            {
                string setNameColumn = "EndemeSetLabel";

                // --------------------------------------------------------------------------
                //  Retrieve the endeme set data
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT s." + SetIdColumn + ", s."+SetLabelColumn+" AS " + setNameColumn + ", c.*"
                    + "\r\n" + " FROM                " + LaEndemeSet  + " AS s WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeChar + " AS c WITH(NOLOCK) ON c."+SetIdColumn+" = s." + SetIdColumn
                    + "\r\n" + " WHERE s." + SetIdColumn + " = @SetID"
                    , Throws.Actions, "PR")
                    ._AddParameter("@SetID", setId);
                RichDataTable source = new RichDataTable(cmd, "EndemeSet", trx, SetIdColumn);


                // --------------------------------------------------------------------------
                //  Construct the endeme set from the retrieved data
                // --------------------------------------------------------------------------
                enSet = new EndemeSet(setId, source.StrValue(0, setNameColumn, ""));

                for (int row = 0; row < source.Count; row++)
                {
                    enSet.Add
                        ( source.CharValue(row, "CharacteristicLetter", ' ')
                        , source.StrValue (row, "CharacteristicCode"  , "" )
                        , source.StrValue (row, "CharacteristicLabel" , "" )
                        , source.StrValue (row, "CharacteristicDescr" , "" )
                        );
                }
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { cmd.Dispose(); cmd = null; }
            return enSet;
        }
        public EndemeSet GetEndemeSet(Guid setId, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); EndemeSet enSet = OnEndemeSet(setId, connection, null); connection.Dispose(); return enSet; }

        // -----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        ///      Returns an endeme set from the database
        /// </summary>
        /// <param name="enSetName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public EndemeSet OnEndemeSet(string enSetName, string conn)
        {
            EndemeSet enSet = EndemeSet.Empty; RichSqlCommand cmd = null;

            try
            {
                string setNameColumn = "EndemeSetLabel";

                // --------------------------------------------------------------------------
                //  Retrieve the endeme set data
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, conn
                    , "\r\n" + " SELECT s." + SetIdColumn + ", s.EndemeSetLabel AS " + setNameColumn + ", c.*"
                    + "\r\n" + " FROM                " + LaEndemeSet  + " AS s WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeChar + " AS c WITH(NOLOCK) ON c."+SetIdColumn+" = s." + SetIdColumn
                    + "\r\n" + " WHERE s.EndemeSetLabel = @SetName"
                    , Throws.Actions, "PR")
                    ._AddParameter_safe("@SetName", enSetName);


                RichDataTable table = new RichDataTable(cmd, "EndemeSet", SetIdColumn);


                // --------------------------------------------------------------------------
                //  Construct the endeme set from the retrieved data
                // --------------------------------------------------------------------------
                enSet = new EndemeSet(table.GuidValue(0, SetIdColumn, Guid.Empty), table.StrValue(0, setNameColumn, ""));

                for (int row = 0; row < table.Count; row++)
                {
                    enSet.Add
                        ( table.CharValue(row, "EndemeCharLetter", ' ')
                        , table.StrValue (row, "EndemeCharCode"  , "" )
                        , table.StrValue (row, "EndemeCharLabel" , "" )
                        , table.StrValue (row, "EndemeCharDescr" , "" )
                        );
                }
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { cmd.Dispose(); cmd = null; }
            return enSet;
        }
        public EndemeSet OnEndemeSet(string enSetName, InfoAspect info) { return OnEndemeSet(enSetName, info.SecondaryConn); }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSetHeader -->
        /// <summary>
        ///      Returns a table contining the header row only for the endeme set
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public RichDataTable GetEndemeSetHeader(Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE " + SetIdColumn + " = @SetID"
                , Throws.Actions, "PR")
                ._AddParameter("@SetID", setId);
            RichDataTable header = new RichDataTable(cmd, "EndemeSetHeader", trx, SetIdColumn);


            header.IdColumn = SetIdColumn;
            return header;
        }
        public RichDataTable GetEndemeSetHeader(Guid setId, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); RichDataTable table = GetEndemeSetHeader(setId, connection, null); connection.Dispose(); return table; }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeSetId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public Guid GetEndemeSetId(string label, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "SELECT " + SetIdColumn + " FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE "+SetLabelColumn+" = @SetName"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@SetName", label);
            RichDataTable table = new RichDataTable(cmd, "EndemeSet", trx, SetIdColumn);


            Guid setId = table.GuidValue(0, SetIdColumn, Guid.Empty);
            return setId;
        }
        public Guid GetEndemeSetID(string label, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); Guid id = GetEndemeSetId(label, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InRetrieveAllSets -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>alpha quality code</remarks>
        public RichDataTable InRetrieveAllSets(SqlConnection connection, SqlTransaction trx)
        {
            // --------------------------------------------------------------------------
            //  Build the query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " SELECT s."+SetLabelColumn+" AS SetLabel         , c.CharacteristicLetter AS Letter"
                + "\r\n" + "     , c.CharacteristicCode AS Code , c.CharacteristicLabel"
                + "\r\n" + "     , c.CharacteristicDescr        , s." + SetIdColumn + ", c.EndemeCharacteristicID"
                + "\r\n" + " FROM                " + LaEndemeSet  + " AS s WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeChar + " AS c WITH(NOLOCK) ON c.EndemeSetID = s." + SetIdColumn
                + "\r\n" + " ORDER BY s."+SetLabelColumn+", c.CharacteristicLetter"
                , Throws.Actions, "PR");
            RichDataTable all = new RichDataTable(cmd, "EndemeSet", trx, SetIdColumn);


            all.IdColumn = "EndemeCharacteristicID";
            return all;
        }
        public RichDataTable RetrieveAllSets(string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); RichDataTable table = InRetrieveAllSets(connection, null); connection.Dispose(); return table; }

        // -----------------------------------------------------------------------------------------
        /// <!-- OntoEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public EndemeSet OntoEndemeSet(EndemeSet input, InfoAspect aspect)
        {
            EndemeSet enSet = input;
            Guid endemeSetId = InEndemeSetIdOfEndemeSetLabel(enSet.Label, aspect);
            if (endemeSetId == Guid.Empty) { ToteEndemeSet(enSet, aspect.SecondaryConn);                   }
            else                           { enSet = ReonEndemeSet(ReinEndemeSet(endemeSetId, aspect), 0); }
            return enSet;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ToteEndemeSet -->
        /// <summary>
        ///      Stores an endeme set in the database including its characteristics using a transaction to make sure it is complete
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>
        ///      You should either save the whole endeme set, or if something goes wrong, none of it
        /// 
        ///      beta quality code, unit tested
        ///      
        ///      How to get here:
        ///      
        ///      (application or controller)
        ///          |
        ///      ToteEndemeSet
        ///          |
        ///      ToteEndemeSet
        /// </remarks>
        public int ToteEndemeSet(EndemeSet enSet, SqlConnection connection, SqlTransaction inputTrx)
        {
            // --------------------------------------------------------------------------
            //  Error check input endeme set member values
            // --------------------------------------------------------------------------
            if (enSet == null) return -1;
            if (enSet.EndemeSetId == Guid.Empty)
                throw new ArgumentException("To store an EndemeSet in the database, you must choose the EndemeSetId first");
            if (string.IsNullOrWhiteSpace(enSet.Label))
                throw new ArgumentException("To store an EndemeSet in the database, it must have a label");


            // --------------------------------------------------------------------------
            //  Handle the connection
            // --------------------------------------------------------------------------
            bool closeConn = false;
            bool commitTrx = false;
            if (connection.State == ConnectionState.Closed)
                { InData.Open(connection); closeConn = true; commitTrx = true; }


            // --------------------------------------------------------------------------
            //  This should be in a transaction
            // --------------------------------------------------------------------------
            SqlTransaction trx = default(SqlTransaction);
            if (inputTrx == null) { trx = InData.Begin(connection); commitTrx = true ; }
            else                  { trx = inputTrx;                                    }

            Guid setId = Guid.Empty;
            int upOk = -1;

            try
            {
                // -----------------------------------------------------------------------
                //  Create the endeme set
                // -----------------------------------------------------------------------
                upOk = ToEndemeSetHeader(enSet.EndemeSetId, enSet.Label, enSet.Code, enSet.Version, connection, trx);
                if (upOk > 0)
                {
                    setId = enSet.EndemeSetId;
                    char[] alphabet = enSet.UnsortedAlphabet;
                    EndemeCharacteristic cha;
                    foreach (char c in alphabet)
                        { cha = enSet[c]; if (cha != null) { ToCharacteristic(setId, cha.Letter, cha.Code, cha.Label, cha.Descr, connection, trx); } }
                }


                // -----------------------------------------------------------------------
                //  Complete the process
                // -----------------------------------------------------------------------
                if (commitTrx && upOk > 0)        { InData.Commit(trx)  ; }
            }                                     
            catch (Exception ex) { if (commitTrx) { InData.Rollback(trx); } setId = Guid.Empty; Is.Trash(ex); upOk = -1; }
            finally              { if (commitTrx) { trx = null          ; }
                                   if (commitTrx && closeConn && !(connection.State == ConnectionState.Closed)) { InData.Close(connection); } }
            return upOk;
        } /// <remarks>transaction created in inner ToteEndemeSet when passed transaction is null</remarks>
        public Guid ToteEndemeSet(EndemeSet enSet, string conn)
        {
            SqlConnection connection = ConnectSource.Connection(conn);
            Guid id = Guid.NewGuid();
            ToteEndemeSet(enSet.WithId(id), connection, null);
            connection.Dispose();
            return id;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSetHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setLabel"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code</remarks>
        public int ToEndemeSetHeader(Guid setId, string setLabel, string setCode, string setVersion, SqlConnection connection, SqlTransaction trx)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO " + LaEndemeSet
                + "\r\n" + "        (EndemeSetId, EndemeSetCode, EndemeSetLabel, EndemeSetVersion)"
                + "\r\n" + " VALUES (@SetID, @Code, @Label, @Version)"
                , Throws.Actions, "PR")
                ._AddParameter     ("@SetID"  , setId     )
                ._AddParameter_safe("@Code"   , setCode   )
                ._AddParameter_safe("@Label"  , setLabel  )
                ._AddParameter_safe("@Version", setVersion)
                ;


            int didOk = cmd.ExecuteNonQuery();
            return didOk;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpdateEndemeSet -->
        /// <summary>
        ///      Updates an endeme set in the database
        /// </summary>
        /// <param name="enSetNew"></param>
        /// <param name="setId"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>endeme set labels are not expected to change</remarks>
        public int UpdateEndemeSet(EndemeSet enSetNew, Guid setId, SqlConnection connection, SqlTransaction trx)
        {
            // --------------------------------------------------------------------------
            //  Resolve context
            // --------------------------------------------------------------------------
            EndemeSet enSetCurrent = OnEndemeSet(setId, connection, trx);                  List<char> currAlphabet = new List<char>(enSetCurrent.UnsortedAlphabet);
            Endeme    enCombined   = enSetNew.RandomEndeme() + enSetCurrent.RandomEndeme(); List<char> newAlphabet  = new List<char>(enSetNew.UnsortedAlphabet    );
            char[]    allAlphabet  = enCombined.ToCharArray();
            int       result = 0;


            //  endeme set labels are not expected to change


            // --------------------------------------------------------------------------
            //  Go through each candidate charcteristic and update, delete or insert it
            // --------------------------------------------------------------------------
            for (int i = 0; i <= allAlphabet.Length; i++)
            {
                char letter = allAlphabet[i];
                if ( currAlphabet.Contains(letter) &  newAlphabet.Contains(letter)) { UpdateCharacteristic(enSetNew[letter]    , setId, connection, trx);               }
                if ( currAlphabet.Contains(letter) & !newAlphabet.Contains(letter)) { DeleteCharacteristic(enSetCurrent[letter], setId, connection, trx); result += -1; }
                if (!currAlphabet.Contains(letter) &  newAlphabet.Contains(letter)) { ToCharacteristic(enSetNew[letter]    , setId, connection, trx); result += +1; }
            }

            return result;
        }

        public void UpEndemeSet(Guid endemeSetId, string endemeSetDescr, string endemeSetResource, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE EndemeSet SET EndemeSetDescr = @Descr, EndemeSetResource = @Resource WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@Descr"      , endemeSetDescr)
                ._AddParameter_safe("@Resource"   , endemeSetResource)
                ._AddParameter     ("@EndemeSetId", endemeSetId)
                ;


            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Endeme table methods

        // -----------------------------------------------------------------------------------------
        /// <!-- AtomicEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public Endeme AtomicEndeme(RichDataTable table)
        {
            Endeme en = OnEndeme(table, 0);
            en.EnSet = OnEndemeSet(table, 0);
            for (int row = 0; row < table.Count; ++row)
            {
                en.EnSet.Add(OnEndemeCharacteristic(table, row));
            }

            return en;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ClosestEndemeTo -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enSet"></param>
        /// <returns></returns>
        /// <remarks>Not really data access, more of a complex parsing method, alpha quality code</remarks>
        public Endeme ClosestEndemeTo(string str, EndemeSet enSet)
        {
	        // --------------------------------------------------------------------------
	        //  Initialize results
	        // --------------------------------------------------------------------------
	        int maxLen = 0;
	        EndemeCharacteristic maxChar = new EndemeCharacteristic();
	        char[] alphabet = enSet.UnsortedAlphabet;
            EndemeCharacteristic enChar;
            int len;


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for best label prefix match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = __.CommonPrefixLength(str, enChar.Label, false);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for best code prefix match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = __.CommonPrefixLength(str, enChar.Code, false);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }


	        if (maxLen < 2)
            {
                // ----------------------------------------------------------------------
                //  Look for closest Levenschtein distance match
                // ----------------------------------------------------------------------
                maxLen = 0;
                maxChar = new EndemeCharacteristic();
                for (int i = 0; i <= alphabet.Length - 1; i++)
                {
        	        enChar = enSet[alphabet[i]]; len = Math.Max(str.Length, enChar.Label.Length) - __.LevenshteinDistance_caseSensitive(str, enChar.Label);
        	        if (len > maxLen) { maxLen = len; maxChar = enChar; }
                }
	        }

	        return new Endeme(enSet, maxChar.Letter.ToString(), true);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        /// <remarks>unit tested</remarks>
        public Endeme GetEndeme(long endemeId, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            Endeme en = Endeme.Empty;
            RichSqlCommand cmd = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Build Query
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT e.*"
                    + "\r\n" + " FROM " + toEndemeTable + " AS e WITH(NOLOCK)"
                    + "\r\n" + " WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeId", endemeId);


                RichDataTable table = new RichDataTable(cmd, "Endeme", trx, "EndemeId");
                Guid setId = table.GuidValue(0, SetIdColumn, Guid.Empty);
                en = new Endeme(OnEndemeSet(setId, connection, trx), table.StrValue(0, "CharString", ""), true);
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { if (cmd != null) { cmd.Dispose(); } }

            return en;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndeme -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public Endeme GetEndeme(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            Endeme en = Endeme.Empty;
            RichSqlCommand cmd = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Build Query
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "\r\n" + " SELECT e.*, s.*"
                    + "\r\n" + " FROM                "                      + toEndemeTable    + " AS e WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN "+aspect.SecondaryDb+"." + LaEndemeSet + " AS s WITH(NOLOCK) ON s.EndemeSetId = e.EndemeSetId"
                    + "\r\n" + " WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeId", endemeId);


                RichDataTable table = new RichDataTable(cmd, "Endeme", "EndemeId");
                en = OnEndeme(table, 0);
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { if (cmd != null) { cmd.Dispose(); } }

            return en;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- GetEndemeId -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLabel"></param>
        /// <param name="setId"></param>
        /// <param name="endemeLabel"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public List<long> GetEndemeId(string listLabel, Guid setId, string endemeLabel, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            List<long> endemeId = new List<long>();
            RichSqlCommand cmd = null;

            try
            {
                // --------------------------------------------------------------------------
                //  Build Query
                // --------------------------------------------------------------------------
                cmd = new RichSqlCommand(CommandType.Text, connection, trx
                    , "\r\n" + " SELECT e.*"
                    + "\r\n" + " FROM " + toEndemeTable + " AS e WITH(NOLOCK)"
                    + "\r\n" + " WHERE EndemeSetID = @SetID AND EndemeListLabel = @ListLabel AND EndemeLabel = @EndemeLabel"
                    , Throws.Actions, "PR")
                    ._AddParameter     ("@SetID"      , setId      )
                    ._AddParameter_safe("@ListLabel"  , listLabel  )
                    ._AddParameter_safe("@EndemeLabel", endemeLabel);
                RichDataTable table = new RichDataTable(cmd, "Endeme", trx, "EndemeId");


                endemeId = table.ToList("EndemeId", -1L);
            }
            catch (Exception ex) { Is.Trash(ex); }
            finally { if (cmd != null) { cmd.Dispose(); } }

            return endemeId;
        }

        // -----------------------------------------------------------------------------------------
        /// <!--  InsertEndeme_simple -->
        /// <summary>
        ///       Stores the endeme in the database (method assumes that the endeme set id is good)
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="en"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>
        ///       The non-simple version should also do a check to make sure the endeme set is in
        ///       the database too and store the endeme set as well if not
        /// 
        ///       beta quality code
        /// </remarks>
        private long InsertEndeme_simple(Guid setId, Endeme en, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            RichDataTable header = GetEndemeSetHeader(setId, connection, trx);


            // --------------------------------------------------------------------------
            //  Construct command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " DECLARE @Table AS TABLE (EndemeId BIGINT )"
                + "\r\n" + " INSERT INTO " + toEndemeTable
                + "\r\n" + "        ( CharString, "+SetIdColumn+", DerivedSetLabel,  EndemeLabel) OUTPUT INSERTED.EndemeId INTO @Table"
                + "\r\n" + " VALUES (@CharString,  @EndemeSetId  ,   @DerivedLabel, @EndemeLabel)"
                + "\r\n" + " SELECT EndemeId FROM @Table"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@CharString"  , en                                    )
                ._AddParameter     ("@EndemeSetId" , setId                                 )
                ._AddParameter_safe("@DerivedLabel", header.StrValue(0, SetLabelColumn, ""))
                ._AddParameter_safe("@EndemeLabel" , ""                                    );


            long endemeId = cmd.ExecuteScalar(0);
            return endemeId;
        }
        private long InsertEndeme_simple(Guid setId, Endeme en, string toEndemeTable, string conn)
        { SqlConnection connection = ConnectSource.Connection(conn); long id = InsertEndeme_simple(setId, en, toEndemeTable, connection, null); connection.Dispose(); return id; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeItem -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLabel"></param>
        /// <param name="enItem"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public List<long> InsertEndemeItem(string listLabel, EndemeItem enItem, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            List<long> endemeId = new List<long>();


            if (enItem == null || enItem.ItemEndeme == null || enItem.ItemEndeme.EnSet == null || enItem.ItemEndeme.EnSet.Count == 0)
            {
                return endemeId;
            }
            else
            {
                // -----------------------------------------------------------------------
                //  Educe endeme set id
                // -----------------------------------------------------------------------
                EndemeSet enSet = enItem.ItemEndeme.EnSet;
                RichDataTable header;
                bool setFound = false;
                if (enSet.SetId == Guid.Empty)                   { enSet.SetId = GetEndemeSetId    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
                else {                                             header      = GetEndemeSetHeader(enSet.SetId   , connection, trx);                                         // If there is a set id, Look for the set by guid.
                        if (header == null || header.Count == 0) { enSet.SetId = GetEndemeSetId    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // If the set id is not good, Look for the set by name
                        else                                     {                                                                 setFound = true;                         } // the normal case
                     }
                if (!setFound) { ToteEndemeSet(enSet.WithId(Guid.NewGuid()), connection, trx); }


                // -----------------------------------------------------------------------
                //  Educe the endeme item
                // -----------------------------------------------------------------------
                endemeId = GetEndemeId(listLabel, enSet.SetId, enItem.ItemLabel, toEndemeTable, connection, trx);
                if (endemeId.Count < 1)
                    endemeId.Add(InsertEndemeItem_simple(listLabel, enSet.SetId, enItem, toEndemeTable, connection, trx));
            }

            return endemeId;
        }
        public List<long> InsertEndemeItem(string listLabel, EndemeItem enItem, string toEndemeTable, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); List<long> endemeId = InsertEndemeItem(listLabel, enItem, toEndemeTable, connection, null); connection.Dispose(); return endemeId; }

        // -----------------------------------------------------------------------------------------
        /// <!-- InsertEndemeItem_simple -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="enItem"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        private long InsertEndemeItem_simple(string listLabel, Guid setId, EndemeItem enItem, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            RichDataTable header = GetEndemeSetHeader(setId, connection, trx);
            long endemeId = -1;
            if (enItem.ItemKey != Guid.Empty) endemeId = enItem.EndemeId;


            // --------------------------------------------------------------------------
            //  Construct command
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, connection, trx
                , "\r\n" + " INSERT INTO " + toEndemeTable
                + "\r\n" + "        (  EndemeId,  CharString, " + SetIdColumn + ", DerivedSetLabel,  EndemeLabel, EndemeListLabel)"
                + "\r\n" + " VALUES ( @EndemeId, @CharString,    @EndemeSetId    ,   @DerivedLabel, @EndemeLabel,      @ListLabel)"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeId"    , endemeId                              )
                ._AddParameter_safe("@CharString"  , enItem.ItemEndeme                     )
                ._AddParameter     ("@EndemeSetId" , setId                                 )
                ._AddParameter_safe("@DerivedLabel", header.StrValue(0, SetLabelColumn, ""))
                ._AddParameter_safe("@EndemeLabel" , enItem.ItemLabel                      )
                ._AddParameter_safe("@ListLabel"   , listLabel                        , 128);


            cmd.ExecuteNonQuery();
            return endemeId;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- OnEndeme -->
        /// <summary>
        ///      Builds an endeme including the endeme set if it is there
        /// </summary>
        /// <param name="table"></param>
        /// <param name="enRow"></param>
        /// <returns></returns>
        public Endeme OnEndeme(RichDataTable table, int enRow, bool dispose = false)
        {
            // --------------------------------------------------------------------------
            //  Build the basic endeme
            // --------------------------------------------------------------------------
            Endeme en = new Endeme( table.StrValue (enRow, "EndemeString", ""   )
                                  , table.BoolValue(enRow, "RawSource"   , false));         // _string, _index, RawSource
            if (table.Contains("EndemeSetLabel")) { en.EnSet = OnEndemeSet(table, enRow); } // EnSet, Size
            en.SetRawQuant(32,-1);                                                          // Quant
            en.MultipleUse = table.BoolValue(enRow, "MultipleUse", false);                  // MultipleUse


            // --------------------------------------------------------------------------
            //  If characteristics are there, add them to the endeme set
            // --------------------------------------------------------------------------
            Guid? setId = null; // = en.EnSet?.EndemeSetId;
            if (en.EnSet == null) { setId = null; }
            else { setId = en.EnSet.EndemeSetId; }


            if (setId != null) for (int row = 0; row < table.Count; ++row)
            {
                if (table.GuidValue(row, "EndemeSetId") == setId &&
                    table.Contains ("EndemeCharLetter"))
                {
                    // ------------------------------------------------------------------
                    //  Add characteristic
                    // ------------------------------------------------------------------
                    EndemeCharacteristic enChar = OnEndemeCharacteristic(table, row);
                    if (!en.EnSet.Contains(enChar.Letter))
                        { en.EnSet.Add(enChar); }
                }
            }


            if (dispose)
                table.Dispose();
            return en;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- RescueEndemes -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldSetID"></param>
        /// <param name="newSetLabel"></param>
        /// <param name="minRows"></param>
        /// <param name="maxRows"></param>
        /// <param name="connection"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public int RescueEndemes(Guid oldSetID, string newSetLabel, int minRows, int maxRows, SqlConnection connection, SqlTransaction trx)
        {
            Guid newSetID = GetEndemeSetId(newSetLabel, connection, trx);
            return RichDataTable.UpdateRowsIn("Endeme", "Set SetID = '" + newSetID.ToString() + "'", "WHERE SetID = '" + oldSetID.ToString() + "'", minRows, maxRows, connection, trx);
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- SomeTaEndemeTable -->
        /// <summary>
        ///      What does this do?
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public RichDataTable SomeTaEndemeTable(string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT TOP 0 EndemeId, EndemeSetId, EndemeString, RawSource, MultipleUse FROM " + toEndemeTable
                + "\r\n" + " UNION ALL"
                + "\r\n" + " SELECT"
                + "\r\n" + "      0  AS EndemeId"
                + "\r\n" + "     ,'00000000-0000-0000-0000-000000000000' AS EndemeSetId"
                + "\r\n" + "     ,'' AS EndemeString"
                + "\r\n" + "     ,0  AS RawSource"
                + "\r\n" + "     ,0  AS MultipleUse"
                , Throws.Actions, "PR");


            RichDataTable table = new RichDataTable(cmd, "Endeme", "EndemeId");
            return table;
        }

        // -----------------------------------------------------------------------------------------
        /// <!-- ToEndeme -->
        /// <summary>
        ///      This non-simple version does a check to make sure the endeme set is in the database,
        ///      finds the endeme set id if it is and stores the endeme set in the database if it is not
        /// </summary>
        /// <param name="en"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <remarks>beta quality code, unit tested</remarks>
        public long ToEndeme(Endeme en, string toEndemeTable, SqlConnection connection, SqlTransaction trx)
        {
            EndemeSet enSet = en.EnSet;
            RichDataTable header;


            // check for a real endeme set
            if (enSet == null || enSet.Count == 0) 
            {
                return 0;
            }
            else
            {
                // -----------------------------------------------------------------------
                //  Educe endeme set id
                // -----------------------------------------------------------------------
                bool setFound = false;
                if (enSet.SetId == Guid.Empty)                   { enSet.SetId = GetEndemeSetId    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // Look for set by name
                else {                                             header      = GetEndemeSetHeader(enSet.SetId, connection, trx);                                            // If there is a set id, Look for the set by guid.
                        if (header == null || header.Count == 0) { enSet.SetId = GetEndemeSetId    (enSet.Label, connection, trx); setFound = !(enSet.SetId == Guid.Empty); } // If the set id is not good, Look for the set by name
                        else                                     {                                                                 setFound = true;                         } // the normal case
                     }
                if (!setFound) { ToteEndemeSet(enSet.WithId(Guid.NewGuid()), connection, trx); }


                // -----------------------------------------------------------------------
                //  Insert endeme
                // -----------------------------------------------------------------------
                return InsertEndeme_simple(enSet.SetId, en, toEndemeTable, connection, trx);
            }
        }
        public long InsertEndeme(Endeme en, string toEndemeTable, string conn)
            { SqlConnection connection = ConnectSource.Connection(conn); long endemeId = ToEndeme(en, toEndemeTable, connection, null); connection.Dispose(); return endemeId; }

        // -----------------------------------------------------------------------------------------
        /// <!-- UpdateEndeme -->
        /// <summary>
        ///      Updates an endeme in the database
        /// </summary>
        /// <param name="en"></param>
        /// <param name="endemeId"></param>
        /// <param name="aspect"></param>
        /// <returns>standard ExecuteNonQuery result</returns>
        /// <remarks>alpha quality code, unit tested</remarks>
        public int UpdateEndeme(Endeme en, long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichDataTable some = SomeTaEndemeTable(toEndemeTable, aspect);
            some.Rows[0]["EndemeId"    ] = endemeId        ;
            some.Rows[0]["EndemeSetId" ] = en.EnSet.EndemeSetId;
            some.Rows[0]["EndemeString"] = en              ;
            some.Rows[0]["MultipleUse" ] = en.MultipleUse  ;
            some.Rows[0]["RawSource"   ] = en.RawSource    ;

            int result = SoupEndemeTable(some, toEndemeTable, aspect);

            return result;
        }

        public int UpEndeme(long endemeId, Guid endemeSetId, Electron endeme, bool rawSource, bool multipleUse, string toEndemeTable, InfoAspect aspect)
        {
            InfoAspect aspect2 = new InfoAspect(aspect.Project, aspect.Sender, aspect.SecondaryConn, aspect.PrimaryConn, aspect.Root, aspect.Level);
            EndemeSet  enSet   = OnEndemeSet(InEndemeSet(endemeSetId, aspect2), 0, true);
            Endeme     en      = new Endeme(enSet, endeme, true);
            en.RawSource       = rawSource;
            en.MultipleUse     = multipleUse;


            int result = UpdateEndeme(en, endemeId, toEndemeTable, aspect);
            return result;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="multipleUse"></param>
        /// <param name="rawSource"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public int UpEndemeHeader(long endemeId, bool multipleUse, bool rawSource, string toEndemeTable, InfoAspect aspect)
        {
            Endeme en      = OnInEndeme(endemeId, toEndemeTable, aspect);
            en.MultipleUse = multipleUse;
            en.RawSource   = rawSource;
            int count      = UpEndeme(endemeId, en, toEndemeTable, aspect);
            return count;
        }

        private Guid InEndemeSetIdOfEndeme(long endemeId, InfoAspect aspect)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeHeader -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="en"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public int UpEndeme(long endemeId, Endeme en, string toEndemeTable, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeId = " + endemeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(toEndemeTable, whereClause, aspect.PrimaryConn))
                {
                    case 0: break;
                    case 1:
                        if (en.EnSet == null)
                        {
                            cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                                , "\r\n" + " UPDATE " + toEndemeTable
                                + "\r\n" + " SET   EndemeString = @EndemeString"
                                + "\r\n" + "     , RawSource    = @RawSource"
                                + "\r\n" + "     , MultipleUse  = @MultipleUse"
                                + "\r\n" + whereClause
                                , Throws.Actions, "P")
                                ._AddParameter_safe("@EndemeString", en            , 24)
                                ._AddParameter     ("@RawSource"   , en.RawSource      )
                                ._AddParameter     ("@MultipleUse" , en.MultipleUse    )
                                ;


                            cmd.ExecuteNonQuery();
                            if (__.StringHasContent(cmd.Errors))
                                throw new ApplicationException("UpEndemeTable: " + cmd.Errors);
                            count = 1;
                        }
                        else
                        {
                            cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                                , "\r\n" + " UPDATE " + toEndemeTable
                                + "\r\n" + " SET   EndemeSetId  = @EndemeSetId"
                                + "\r\n" + "     , EndemeString = @EndemeString"
                                + "\r\n" + "     , RawSource    = @RawSource"
                                + "\r\n" + "     , MultipleUse  = @MultipleUse"
                                + "\r\n" + whereClause
                                , Throws.Actions, "P")
                                ._AddParameter_null("@EndemeSetId" , en.EnSet.EndemeSetId)
                                ._AddParameter_safe("@EndemeString", en              , 24)
                                ._AddParameter     ("@RawSource"   , en.RawSource        )
                                ._AddParameter     ("@MultipleUse" , en.MultipleUse      )
                                ;


                            cmd.ExecuteNonQuery();
                            if (__.StringHasContent(cmd.Errors))
                                throw new ApplicationException("UpEndemeTable: " + cmd.Errors);
                            count = 1;
                        }
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeId " + endemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        public Endeme OnInEndeme(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT EndemeId, EndemeSetId, EndemeString, RawSource, MultipleUse FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "Endeme", "EndemeId");
            Endeme en = OnEndeme(table, 0);
            EndemeSet enSet = new EndemeSet(table.GuidValue(0, "EndemeSetId", Guid.Empty), "Dummy");
            return en;
        }

        public RichDataTable MetaEndemeTable(string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + "     SELECT TOP 0 * FROM " + toEndemeTable
                + "\r\n" + " UNION ALL"
                + "\r\n" + "     SELECT"
                + "\r\n" + "          0    AS EndemeId"
                + "\r\n" + "         ,'00000000-0000-0000-0000-000000000000' AS EndemeSetId"
                + "\r\n" + "         ,''   AS EndemeString"
                + "\r\n" + "         ,NULL AS EndemeCode"
                + "\r\n" + "         ,''   AS EndemeLabel"
                + "\r\n" + "         ,NULL AS EndemeDescr"
                + "\r\n" + "         ,0    AS RawSource"
                + "\r\n" + "         ,0    AS MultipleUse"
                + "\r\n" + "         ,NULL AS ValueBinary"
                + "\r\n" + "         ,NULL AS ValueDateTime"
                + "\r\n" + "         ,NULL AS ValueFloat"
                + "\r\n" + "         ,NULL AS ValueNumber"
                + "\r\n" + "         ,NULL AS ValueText"
                + "\r\n" + "         ,NULL AS EndemeProfileId"
                + "\r\n" + "         ,0    AS Aat"
                + "\r\n" + "         ,0    AS Bat"
                + "\r\n" + "         ,0    AS Cat"
                + "\r\n" + "         ,0    AS Dat"
                + "\r\n" + "         ,0    AS Eat"
                + "\r\n" + "         ,0    AS Fat"
                + "\r\n" + "         ,0    AS Gat"
                + "\r\n" + "         ,0    AS Hat"
                + "\r\n" + "         ,0    AS Iat"
                + "\r\n" + "         ,0    AS Jat"
                + "\r\n" + "         ,0    AS Kat"
                + "\r\n" + "         ,0    AS Lat"
                + "\r\n" + "         ,0    AS Mat"
                + "\r\n" + "         ,0    AS Nat"
                + "\r\n" + "         ,0    AS Oat"
                + "\r\n" + "         ,0    AS Pat"
                + "\r\n" + "         ,0    AS Qat"
                + "\r\n" + "         ,0    AS Rat"
                + "\r\n" + "         ,0    AS Sat"
                + "\r\n" + "         ,0    AS Tat"
                + "\r\n" + "         ,0    AS Uat"
                + "\r\n" + "         ,0    AS Vat"
                + "\r\n" + "         ,0    AS Wat"
                + "\r\n" + "         ,0    AS Xat"
                , Throws.Actions, "PR");


            RichDataTable table = new RichDataTable(cmd, "Endeme", "EndemeId");
            return table;
        }

        #endregion



        //  new tables

        #region EndemeTable table methods
        // ----------------------------------------------------------------------------------------
        //  Endeme table methods
        // ----------------------------------------------------------------------------------------
        public int               ExEndeme                    (long        endemeId       , string toEndemeTable, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + toEndemeTable + " WHERE EndemeId = " + endemeId, aspect.PrimaryConn); return count; }
        public int               ExEndemeTable               (long        endemeId       , string toEndemeTable, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + toEndemeTable + " WHERE EndemeId = " + endemeId, aspect.PrimaryConn); return count; }
        public List<EndemeTable> AtMareEndemeTableOf         (int         endemesetId    , string toEndemeTable, InfoAspect aspect) { return AtOnMareEndemeTable(MareEndemeTableOf           (endemesetId    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndeme       (long        endemeId       , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndeme       (endemeId       , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeSet    (Guid        endemeSetId    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeSet    (endemeSetId    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeString (string      endemeString   , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeString (endemeString   , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeCode   (string      endemeCode     , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeCode   (endemeCode     , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeLabel  (string      endemeLabel    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeLabel  (endemeLabel    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeDescr  (string      endemeDescr    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeDescr  (endemeDescr    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfRawSource    (bool        rawSource      , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfRawSource    (rawSource      , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfMultipleUse  (bool        multipleUse    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfMultipleUse  (multipleUse    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueBinary  (byte[]      valueBinary    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueBinary  (valueBinary    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueDateTime(DateTime    valueDateTime  , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueDateTime(valueDateTime  , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueFloat   (double      valueFloat     , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueFloat   (valueFloat     , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueNumber  (decimal     valueNumber    , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueNumber  (valueNumber    , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfValueText    (string      valueText      , string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfValueText    (valueText      , toEndemeTable, aspect)); }
        public List<EndemeTable> AtEndemeTableOfEndemeProfile(int         endemeProfileId, string toEndemeTable, InfoAspect aspect) { return AtEndemeTable      (InEndemeTableOfEndemeProfile(endemeProfileId, toEndemeTable, aspect)); }
        public EndemeTable       OnEndemeTable               (long        endemeId       , string toEndemeTable, InfoAspect aspect) { return OnEndemeTable      (InEndemeTable               (endemeId       , toEndemeTable, aspect), 0, true); }
        public EndemeTable       OnIntoEndemeTable           (EndemeTable enRow          , string toEndemeTable, InfoAspect aspect) { return OnEndemeTable      (IntoEndemeTable             (enRow, toEndemeTable, aspect), 0, true); }
        public Atom              AtomMaEndemeTableOf         (Guid        endemeSetId    , string toEndemeTable, InfoAspect aspect) { return MainEndemeTableOf  (endemeSetId, toEndemeTable, aspect).ToAtom(); }
        public RichDataTable     IntoEndemeTable             (EndemeTable enRow          , string toEndemeTable, InfoAspect aspect) { if (IsEndemeTable(enRow.EndemeId, toEndemeTable, aspect)) return InEndemeTable(enRow.EndemeId, toEndemeTable, aspect); else { return InEndemeTable(ToEndemeTable(enRow, toEndemeTable, aspect), toEndemeTable, aspect); } }
        public int               UpToEndemeTable             (EndemeTable enRow          , string toEndemeTable, InfoAspect aspect) { if (IsEndemeTable(enRow.EndemeId, toEndemeTable, aspect)) return UpEndemeTable(enRow         , toEndemeTable, aspect); else { return (int)ToEndemeTable(              enRow                   , toEndemeTable, aspect); } }

        public static void AccessEndemeTable()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeTable_test -->
        /// <summary>
        ///      Tests the ToEndemeTable method
        /// </summary>
        public void ToEndemeTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeTableAccess";
            string adoMethod = "ToEndemeTable";
            string toEndemeTable = LaEndemeMain;
            Random r = RandomSource.New().Random;
            long   endemeId = -1;
            EndemeAccess ende = new EndemeAccess();


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeTable enRowTo = ende.AnneEndemeTable(r);
            try
            {
                endemeId = ende.ToEndemeTable(enRowTo, toEndemeTable, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeTable enRowFrom = ende.OnEndemeTable(endemeId, toEndemeTable, _aspect);
            ende.AssertEqualContent(enRowFrom, enRowTo, adoClass, adoMethod);


            ende.ExEndemeTable(endemeId, toEndemeTable,_aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeTable_test -->
        /// <summary>
        ///      Tests the Up Endeme Table method
        /// </summary>
        public void UpEndemeTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass      = "EndemeTableAccess";
            string adoMethod     = "UpEndemeTable";
            string toEndemeTable = LaEndemeMain;
            Random r             = RandomSource.New().Random;
            EndemeTable enRow1   = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeId with a newly created endemeTable
                // ----------------------------------------------------------------------
                long        endemeId = ende.AnIdOfEndemeTable(toEndemeTable, _aspect);
                enRow1               = ende.OnEndemeTable(endemeId, toEndemeTable, _aspect);
                EndemeTable enRow2   = ende.AnneEndemeTable(r);
                enRow2.EndemeId = enRow1.EndemeId;
                ende.UpEndemeTable(enRow2, toEndemeTable, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeTable enRow3 = ende.OnEndemeTable(endemeId, toEndemeTable, _aspect);
                Assert.That(enRow3.EndemeId, Is.equal_to, enRow2.EndemeId, adoClass, adoMethod + " test update existing endemeTable");
                ende.AssertEqualContent  (enRow3, enRow2, adoClass, adoMethod);
                ende.AssertUnequalContent(enRow3, enRow1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeTable, did the update fail?
                // ----------------------------------------------------------------------
                EndemeTable enRow4  = ende.AnneEndemeTable(r);
                enRow4.EndemeId     = ende.HiIdOfEndemeTable(toEndemeTable, _aspect) + 1;
                int         count   = ende.UpEndemeTable(enRow4, toEndemeTable, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeTable");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeTable(enRow1, toEndemeTable, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeTable -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endeme Table table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public int AnIdOfEndemeTable(string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT EndemeId FROM " + toEndemeTable + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeTableId = new RichDataTable(cmd, "EndemeTableId", "EndemeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeTableId.Count > 0) return (int)endemeTableId.ToList("EndemeId", -1)[r.Next(endemeTableId.Count)];
                else return -1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeTable -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeTable object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeTable</returns
        private EndemeTable AnneEndemeTable(Random r)
        {
            EndemeTable enRow = new EndemeTable
              //{ EndemeId        = go.HiIdOfEndemeId() + 1
                { EndemeSetId     = Guid.NewGuid()
                , EndemeString    = r.Next(10000).ToString()
                , EndemeCode      = r.Next(10000).ToString()
                , EndemeLabel     = r.Next(10000).ToString()
                , EndemeDescr     = r.Next(10000).ToString()
                , RawSource       = true
                , MultipleUse     = true
                , ValueBinary     = null
                , ValueDateTime   = DateTime.Now
                , ValueFloat      = r.Next(10000)
                , ValueNumber     = r.Next(10000)
                , ValueText       = r.Next(10000).ToString()
                , EndemeProfileId = r.Next(10000)
                };
            return enRow;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="enRow"    >endeme Table being tested</param>
        /// <param name="tgt"      >endeme Table being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeTable enRow, EndemeTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(enRow.EndemeSetId    , Is.equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(enRow.EndemeString   , Is.equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(enRow.EndemeCode     , Is.equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(enRow.EndemeLabel    , Is.equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(enRow.EndemeDescr    , Is.equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
            Assert.That(enRow.RawSource      , Is.equal_to, tgt.RawSource      , adoClass, adoMethod + " RawSource"      );
            Assert.That(enRow.MultipleUse    , Is.equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(enRow.ValueBinary    , Is.equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(enRow.ValueDateTime  , Is.equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(enRow.ValueFloat     , Is.equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(enRow.ValueNumber    , Is.equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(enRow.ValueText      , Is.equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(enRow.EndemeProfileId, Is.equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="enRow"    >endeme Table being tested</param>
        /// <param name="tgtEnRow"      >endeme Table being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeTable enRow, EndemeTable tgtEnRow, string adoClass, string adoMethod)
        {
            Assert.That(enRow.EndemeSetId      , Is.not_equal_to, tgtEnRow.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(enRow.EndemeString     , Is.not_equal_to, tgtEnRow.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(enRow.EndemeCode       , Is.not_equal_to, tgtEnRow.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(enRow.EndemeLabel      , Is.not_equal_to, tgtEnRow.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(enRow.EndemeDescr      , Is.not_equal_to, tgtEnRow.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
          //Assert.That(endemeTable.RawSource  , Is.not_equal_to, tgt.RawSource           , adoClass, adoMethod + " RawSource"      );
          //Assert.That(endemeTable.MultipleUse, Is.not_equal_to, tgt.MultipleUse         , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(enRow.ValueBinary      , Is.not_equal_to, tgtEnRow.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(enRow.ValueDateTime    , Is.not_equal_to, tgtEnRow.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(enRow.ValueFloat       , Is.not_equal_to, tgtEnRow.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(enRow.ValueNumber      , Is.not_equal_to, tgtEnRow.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(enRow.ValueText        , Is.not_equal_to, tgtEnRow.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(enRow.EndemeProfileId  , Is.not_equal_to, tgtEnRow.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeTable -->
        /// <summary>
        ///     Returns a list of Endeme Table objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of Endeme Table objects</returns>
        public List<EndemeTable> AtEndemeTable(RichDataTable table)
        {
            List<EndemeTable> list = new List<EndemeTable>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeTable(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeTable -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endeme Table objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endeme Table objects"></param>
        public List<EndemeTable> AtOnMareEndemeTable(RichDataTable table)
        {
            List<EndemeTable> endemeTableList = new List<EndemeTable>(table.Count);
            Dictionary<long,EndemeTable> found = new Dictionary<long,EndemeTable>();


            for (int row = 0; row < table.Count; ++row)
            {
                long endemeId = table.IntValue(row, "EndemeId", -1);
                EndemeTable enRow = null;

                if (!found.ContainsKey(endemeId))
                {
                    enRow = OnEndemeTable(table, row);

                  //endemeTable.EndemeIndexList = new List<EndemeIndex>();
                  //endemeTable.EndemeSet   = (new EndemeSetAccess(ConnectionString)).OnEndemeSet(table, row);
                    endemeTableList.Add(enRow);
                    found.Add(endemeId, enRow);
                }
                else
                {
                    enRow = found[endemeId];
                }

              //endemeTable.EndemeIndexList.Add((new EndemeIndexAccess(ConnectionString)).OnEndemeIndex(table, row));
            }

            return endemeTableList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeTable -->
        /// <summary>
        ///      Copies an endemeTable
        /// </summary>
        /// <param name="enRow">endeme Table to copy</param>
        public static EndemeTable CpEndemeTable(string toEndemeTable, EndemeTable enRow)
        {
            EndemeTable enRowCopy = new EndemeTable();

            enRowCopy.EndemeId        = enRow.EndemeId       ;
            enRowCopy.EndemeSetId     = enRow.EndemeSetId    ;
            enRowCopy.EndemeString    = enRow.EndemeString   ;
            enRowCopy.EndemeCode      = enRow.EndemeCode     ;
            enRowCopy.EndemeLabel     = enRow.EndemeLabel    ;
            enRowCopy.EndemeDescr     = enRow.EndemeDescr    ;
            enRowCopy.RawSource       = enRow.RawSource      ;
            enRowCopy.MultipleUse     = enRow.MultipleUse    ;
            enRowCopy.ValueBinary     = enRow.ValueBinary    ;
            enRowCopy.ValueDateTime   = enRow.ValueDateTime  ;
            enRowCopy.ValueFloat      = enRow.ValueFloat     ;
            enRowCopy.ValueNumber     = enRow.ValueNumber    ;
            enRowCopy.ValueText       = enRow.ValueText      ;
            enRowCopy.EndemeProfileId = enRow.EndemeProfileId;

            return enRowCopy;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeTableCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the Endeme table
        /// </summary>
        /// <returns>a count of rows in the Endeme table</returns>
        public int EndemeTableCt(string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT COUNT(*) FROM " + toEndemeTable + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeTable: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeTable -->
        /// <summary>
        ///      Enables an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        public void DoEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + toEndemeTable + " SET EntryState = 1 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeTable -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endeme table
        /// </summary>
        /// <returns>Maximum Endeme.EndemeId</returns>
        public long HiIdOfEndemeTable(string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT MAX(EndemeId) AS EndemeId FROM " + toEndemeTable + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            long endemeId = cmd.ExecuteScalar(0);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeTable: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTable -->
        /// <summary>
        ///      Returns the indicated row (In) the Endeme table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId"  , endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndeme -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeId column
        /// </summary>
        /// <param name="endemeId">value in EndemeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeId</returns>
        public RichDataTable InEndemeTableOfEndeme(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId" , endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueDateTime -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueDateTime column
        /// </summary>
        /// <param name="valueDateTime">value in ValueDateTime column</param>
        /// <returns>a table of rows related to the specifed value of ValueDateTime</returns>
        public RichDataTable InEndemeTableOfValueDateTime(DateTime valueDateTime, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE ValueDateTime = @ValueDateTime"
                , Throws.Actions, "PR")
                ._AddParameter_date("@ValueDateTime" , valueDateTime, SqlDbType.DateTime);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf10EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueFloat -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueFloat column
        /// </summary>
        /// <param name="valueFloat">value in ValueFloat column</param>
        /// <returns>a table of rows related to the specifed value of ValueFloat</returns>
        public RichDataTable InEndemeTableOfValueFloat(double valueFloat, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE ValueFloat = @ValueFloat"
                , Throws.Actions, "PR")
                ._AddParameter_null("@ValueFloat" , valueFloat);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf11EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueNumber -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueNumber column
        /// </summary>
        /// <param name="valueNumber">value in ValueNumber column</param>
        /// <returns>a table of rows related to the specifed value of ValueNumber</returns>
        public RichDataTable InEndemeTableOfValueNumber(decimal valueNumber, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE ValueNumber = @ValueNumber"
                , Throws.Actions, "PR")
                ._AddParameter_null("@ValueNumber" , valueNumber);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf12EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueText -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueText column
        /// </summary>
        /// <param name="valueText">value in ValueText column</param>
        /// <returns>a table of rows related to the specifed value of ValueText</returns>
        public RichDataTable InEndemeTableOfValueText(string valueText, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE ValueText = @ValueText"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@ValueText" , valueText);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf13EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeTableOfEndemeProfile(int endemeProfileId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeProfileId" , endemeProfileId);
            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf14EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeTableOfEndemeSet(Guid endemeSetId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeString -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeString column
        /// </summary>
        /// <param name="endemeString">value in EndemeString column</param>
        /// <returns>a table of rows related to the specifed value of EndemeString</returns>
        public RichDataTable InEndemeTableOfEndemeString(string endemeString, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeString = @EndemeString"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeString" , endemeString);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeCode -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeCode column
        /// </summary>
        /// <param name="endemeCode">value in EndemeCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCode</returns>
        public RichDataTable InEndemeTableOfEndemeCode(string endemeCode, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeCode = @EndemeCode"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeCode" , endemeCode);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeLabel -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeLabel column
        /// </summary>
        /// <param name="endemeLabel">value in EndemeLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLabel</returns>
        public RichDataTable InEndemeTableOfEndemeLabel(string endemeLabel, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeLabel = @EndemeLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeLabel" , endemeLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfEndemeDescr -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the EndemeDescr column
        /// </summary>
        /// <param name="endemeDescr">value in EndemeDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeDescr</returns>
        public RichDataTable InEndemeTableOfEndemeDescr(string endemeDescr, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeDescr = @EndemeDescr"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeDescr" , endemeDescr);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfRawSource -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the RawSource column
        /// </summary>
        /// <param name="rawSource">value in RawSource column</param>
        /// <returns>a table of rows related to the specifed value of RawSource</returns>
        public RichDataTable InEndemeTableOfRawSource(bool rawSource, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE RawSource = @RawSource"
                , Throws.Actions, "PR")
                ._AddParameter_null("@RawSource" , rawSource);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfMultipleUse -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the MultipleUse column
        /// </summary>
        /// <param name="multipleUse">value in MultipleUse column</param>
        /// <returns>a table of rows related to the specifed value of MultipleUse</returns>
        public RichDataTable InEndemeTableOfMultipleUse(bool multipleUse, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE MultipleUse = @MultipleUse"
                , Throws.Actions, "PR")
                ._AddParameter("@MultipleUse" , multipleUse);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeTableOfValueBinary -->
        /// <summary>
        ///      Returns the rows (In) the Endeme table filtered by a value (Of) the ValueBinary column
        /// </summary>
        /// <param name="valueBinary">value in ValueBinary column</param>
        /// <returns>a table of rows related to the specifed value of ValueBinary</returns>
        public RichDataTable InEndemeTableOfValueBinary(byte[] valueBinary, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE ValueBinary = @ValueBinary"
                , Throws.Actions, "PR")
                ._AddParameter_byte("@ValueBinary" , valueBinary);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf9EndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InicEndemeTable -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public RichDataTable InicEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            string mainDb = ConnectSource.DataBaseFrom(aspect.PrimaryConn);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT c.*, e.*, s.*"
                + "\r\n" + " FROM " + mainDb + "." + toEndemeTable + " AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet  + " AS s WITH(NOLOCK) ON s.EndemeSetId = e.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeChar + " AS c WITH(NOLOCK) ON c.EndemeSetId = s.EndemeSetId"
                + "\r\n" + " WHERE EndemeId = @EndemeId"
                + "\r\n" + " ORDER BY (CHARINDEX(c.EndemeCharLetter, e.EndemeString) + 25) % 26"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "Endeme", "EndemeCharacteristicId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeTable -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the Endeme table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "SELECT * FROM " + toEndemeTable + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeId", endemeId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one Endeme Table with EndemeId " + endemeId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeTable -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of endeme Table rows with their joined parent data</returns>
        public RichDataTable MainEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, e.*"
                + "\r\n" + " FROM                " + toEndemeTable + " AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet +" AS s WITH(NOLOCK) ON s.EndemeSetId = e.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeTableOf -->
        /// <summary>
        ///      Returns a endeme Table list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endeme Table rows with their joined parent data</returns>
        public RichDataTable MainEndemeTableOf(Guid endemeSetId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, e.*"
                + "\r\n" + " FROM                " + toEndemeTable + " AS e  WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet +" AS s WITH(NOLOCK) ON s.EndemeSetId = e.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeTableOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeTableOf -->
        /// <summary>
        ///      Returns a endeme Table list (Of) a p(a)rent endemeset with (re)trieved endeme Table and endemeindex data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endeme Table rows with their joined parent data</returns>
        public RichDataTable MareEndemeTableOf(int endemeSetId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, e.*, i.*"
                + "\r\n" + " FROM                " + toEndemeTable     + " AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet  + " AS s WITH(NOLOCK) ON s.EndemeSetId = e.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeIndex+ " AS i WITH(NOLOCK) ON i.EndemeId    = e.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeTableOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeTable -->
        /// <summary>
        ///      Creates a (Ne)w Endeme Table (o)bject from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawSource"      ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new Endeme Table object</returns>
        public EndemeTable NeonEndemeTable
            ( long     endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawSource
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            )
        {
            EndemeTable enRow = new EndemeTable
                { EndemeId        = endemeId
                , EndemeSetId     = endemeSetId
                , EndemeString    = endemeString
                , EndemeCode      = endemeCode
                , EndemeLabel     = endemeLabel
                , EndemeDescr     = endemeDescr
                , RawSource       = rawSource
                , MultipleUse     = multipleUse
                , ValueBinary     = valueBinary
                , ValueDateTime   = valueDateTime
                , ValueFloat      = valueFloat
                , ValueNumber     = valueNumber
                , ValueText       = valueText
                , EndemeProfileId = endemeProfileId
                };
            return enRow;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeTable -->
        /// <summary>
        ///      Converts a row in the Endeme data table into a Endeme Table (O)bject
        /// </summary>
        /// <param name="table">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeTable OnEndemeTable(RichDataTable table, int row, bool dispose = false)
        {
            EndemeTable enRow = new EndemeTable();
            enRow.EndemeId        = table.IntValue (row, "EndemeId"       , -1          );
            enRow.EndemeSetId     = table.GuidValue(row, "EndemeSetId"    , Guid.Empty  );
            enRow.EndemeString    = table.StrValue (row, "EndemeString"   , ""          );
            enRow.EndemeCode      = table.StrValue (row, "EndemeCode"     , ""          );
            enRow.EndemeLabel     = table.StrValue (row, "EndemeLabel"    , ""          );
            enRow.EndemeDescr     = table.StrValue (row, "EndemeDescr"    , ""          );
            enRow.RawSource       = table.BoolValue(row, "RawSource"      , false       );
            enRow.MultipleUse     = table.BoolValue(row, "MultipleUse"    , false       );
            enRow.ValueBinary     = table.ByteValue(row, "ValueBinary"                  );
            enRow.ValueDateTime   = table.DateValue(row, "ValueDateTime"  , DateTime.Now);
            enRow.ValueFloat      = table.RealValue(row, "ValueFloat"     , -1.0        );
            enRow.ValueNumber     = table.DecValue (row, "ValueNumber"    , -1.0M       );
            enRow.ValueText       = table.StrValue (row, "ValueText"      , ""          );
            enRow.EndemeProfileId = table.IntValue (row, "EndemeProfileId", -1          );

            if (dispose) table.Dispose();
            return enRow;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeTable -->
        /// <summary>
        ///      Returns a table of a Endeme Table (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of Endeme Table rows with their joined details</returns>
        public RichDataTable ReinEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT e.*, i.*"
                + "\r\n" + " FROM                " + toEndemeTable  + " AS e WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeIndex + " AS i WITH(NOLOCK) ON i.EndemeId = e.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND e.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeTable", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeTable -->
        /// <summary>
        ///      Fill the endeme only, (so)me parts (on) an Endeme Table object by hook or by crook
        /// </summary>
        /// <param name="en"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        /// <remarks>
        ///      The challenge is the the state of the endeme set in the endeme:
        ///      There might not be one; if there is, it might not be in the database,
        ///      or it might be missing its label, or if it is in the database and its is is missing;
        ///      there might be more than one with the same label etc.
        ///      
        ///      This method will actually put the endeme set in the database if its id is missing
        ///      and its label is not in the database, this is a scary side effect
        ///      
        ///      We assume that there is a databse because we are working with the Endeme Table class
        ///      whose purpose is to interface between endeme classes and the endeme database table
        /// </remarks>
        public EndemeTable NeonEndemeTable(Endeme en, string toEndemeTable, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Simple parts
            // --------------------------------------------------------------------------
            EndemeTable enRowOut  = new EndemeTable();
            enRowOut.EndemeString = en               ;
            enRowOut.RawSource    = en.RawSource  ;
            enRowOut.MultipleUse  = en.MultipleUse;


            // --------------------------------------------------------------------------
            //  Resolve endeme set id and make sure the endeme set exists inthe database
            // --------------------------------------------------------------------------
            EndemeSet es = en.EnSet;

            if (es == null || string.IsNullOrWhiteSpace(es.Label))
            {
                enRowOut.EndemeSetId = Guid.Empty;
            }
            else if (es.EndemeSetId != null && es.EndemeSetId != Guid.Empty && __.StringHasContent(es.Label))
            {
                // ----------------------------------------------------------------------
                //  Normal case
                // ----------------------------------------------------------------------
                enRowOut.EndemeSetId = es.EndemeSetId;
                {
                    // ------------------------------------------------------------------
                    //  Make sure the endeme set is filled (do this occasionally)
                    // ------------------------------------------------------------------
                    RichDataTable enChar = InEndemeCharacteristicOfEndemeSet(enRowOut.EndemeSetId, aspect);
                    if (enChar.Count == 0)
                        ToEndemeCharacteristicAtEndemeSet(es, aspect);
                }
            }
            else
            {
                RichDataTable table = InEndemeSetOfEndemeSetLabel(es.Label, aspect);
                if (table.Count == 1)
                {
                    // ------------------------------------------------------------------
                    //  Extract the id and make sure the endeme set is filled
                    // ------------------------------------------------------------------
                    es.EndemeSetId = table.GuidValue(0, "EndemeSetId", Guid.Empty);
                    enRowOut.EndemeSetId = es.EndemeSetId;
                    RichDataTable enChar = InEndemeCharacteristicOfEndemeSet(enRowOut.EndemeSetId, aspect);
                    if (enChar.Count == 0)
                        ToEndemeCharacteristicAtEndemeSet(es, aspect);
                }
                else if (table.Count == 0)
                {
                    // ------------------------------------------------------------------
                    //  Probably creates new endeme set in the database, Make sure the endeme set is filled
                    // ------------------------------------------------------------------
                    es.EndemeSetId = ToEndemeSet(es, aspect);
                    enRowOut.EndemeSetId = es.EndemeSetId;
                    ToEndemeCharacteristicAtEndemeSet(es, aspect);
                }
                else throw new AmbiguousResultException("There are multiple endeme sets with the label '" + es.Label + "' and this program can't handle it.");
            }

            return enRowOut;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToSoEndemeTable -->
        /// <summary>
        ///      Send just (so)me of Endeme Table class fields (to) the endeme table
        /// </summary>
        /// <param name="enPartial"></param>
        /// <param name="ia"></param>
        /// <returns></returns>
        /// <remarks>
        ///      The Endeme table stores data for various different classes, this method inserts
        ///      only the data relevant to the Endeme class into the Endeme table
        /// </remarks>
        public long ToSoEndemeTable(EndemeTable enRow, string toEndemeTable, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " DECLARE @Table AS TABLE ( EndemeId BIGINT )"
                + "\r\n" + " INSERT INTO " + toEndemeTable
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeString"
                + "\r\n" + "        , RawSource"
                + "\r\n" + "        , MultipleUse"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeId INTO @Table"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeString"
                + "\r\n" + "        , @RawSource"
                + "\r\n" + "        , @MultipleUse"
                + "\r\n" + "        )"
                + "\r\n" + " SELECT EndemeId FROM @Table"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeSetId" , enRow.EndemeSetId     )
                ._AddParameter_safe("@EndemeString", enRow.EndemeString, 24)
                ._AddParameter_null("@RawSource"   , enRow.RawSource       )
                ._AddParameter     ("@MultipleUse" , enRow.MultipleUse     );


            long endemeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("Error in SoToEndemeTable: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeTable -->
        /// <summary>
        ///      Inserts an endeme object in(To) the database
        /// </summary>
        /// <param name="endemeTable">endeme Table to insert into database</param>
        /// <returns>the Id of the inserted Endeme</returns>
        public long ToEndemeTable(EndemeTable enRow, string toTable, InfoAspect aspect)
        {
            if (enRow.EndemeString == null)  enRow.EndemeString = "";

            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = null;
            if (new DateTime(1776,7,4) <= enRow.ValueDateTime && enRow.ValueDateTime < new DateTime(7654,3,21))
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "\r\n" + " DECLARE @Table AS TABLE ( EndemeId BIGINT )"
                    + "\r\n" + " INSERT INTO " + toTable
                    + "\r\n" + "        ( EndemeSetId"
                    + "\r\n" + "        , EndemeString"
                    + "\r\n" + "        , EndemeCode"
                    + "\r\n" + "        , EndemeLabel"
                    + "\r\n" + "        , EndemeDescr"
                    + "\r\n" + "        , RawSource"
                    + "\r\n" + "        , MultipleUse"
                    + "\r\n" + "        , ValueBinary"
                    + "\r\n" + "        , ValueDateTime"
                    + "\r\n" + "        , ValueFloat"
                    + "\r\n" + "        , ValueNumber"
                    + "\r\n" + "        , ValueText"
                    + "\r\n" + "        , EndemeProfileId"
                    + "\r\n" + "        ) OUTPUT INSERTED.EndemeId INTO @Table"
                    + "\r\n" + " VALUES ( @EndemeSetId"
                    + "\r\n" + "        , @EndemeString"
                    + "\r\n" + "        , @EndemeCode"
                    + "\r\n" + "        , @EndemeLabel"
                    + "\r\n" + "        , @EndemeDescr"
                    + "\r\n" + "        , @RawSource"
                    + "\r\n" + "        , @MultipleUse"
                    + "\r\n" + "        , @ValueBinary"
                    + "\r\n" + "        , @ValueDateTime"
                    + "\r\n" + "        , @ValueFloat"
                    + "\r\n" + "        , @ValueNumber"
                    + "\r\n" + "        , @ValueText"
                    + "\r\n" + "        , @EndemeProfileId"
                    + "\r\n" + "        )"
                    + "\r\n" + " SELECT EndemeId FROM @Table"
                    , Throws.Actions, "PR")
                    ._AddParameter_null("@EndemeSetId"    , enRow.EndemeSetId                        )
                    ._AddParameter_safe("@EndemeString"   , enRow.EndemeString   ,   24              )
                    ._AddParameter_safe("@EndemeCode"     , enRow.EndemeCode     ,    8              )
                    ._AddParameter_safe("@EndemeLabel"    , enRow.EndemeLabel    ,   64              )
                    ._AddParameter_safe("@EndemeDescr"    , enRow.EndemeDescr    ,   -1              )
                    ._AddParameter_null("@RawSource"      , enRow.RawSource                          )
                    ._AddParameter     ("@MultipleUse"    , enRow.MultipleUse                        )
                    ._AddParameter_byte("@ValueBinary"    , enRow.ValueBinary                        )
                    ._AddParameter_date("@ValueDateTime"  , enRow.ValueDateTime  , SqlDbType.DateTime)
                    ._AddParameter_null("@ValueFloat"     , enRow.ValueFloat                         )
                    ._AddParameter_null("@ValueNumber"    , enRow.ValueNumber                        )
                    ._AddParameter_safe("@ValueText"      , enRow.ValueText      ,   -1              )
                    ._AddParameter_null("@EndemeProfileId", enRow.EndemeProfileId                    );
            }
            else
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "\r\n" + " DECLARE @Table AS TABLE ( EndemeId BIGINT )"
                    + "\r\n" + " INSERT INTO " + toTable
                    + "\r\n" + "        ( EndemeSetId"
                    + "\r\n" + "        , EndemeString"
                    + "\r\n" + "        , EndemeCode"
                    + "\r\n" + "        , EndemeLabel"
                    + "\r\n" + "        , EndemeDescr"
                    + "\r\n" + "        , RawSource"
                    + "\r\n" + "        , MultipleUse"
                    + "\r\n" + "        , ValueBinary"
                    + "\r\n" + "        , ValueFloat"
                    + "\r\n" + "        , ValueNumber"
                    + "\r\n" + "        , ValueText"
                    + "\r\n" + "        , EndemeProfileId"
                    + "\r\n" + "        ) OUTPUT INSERTED.EndemeId INTO @Table"
                    + "\r\n" + " VALUES ( @EndemeSetId"
                    + "\r\n" + "        , @EndemeString"
                    + "\r\n" + "        , @EndemeCode"
                    + "\r\n" + "        , @EndemeLabel"
                    + "\r\n" + "        , @EndemeDescr"
                    + "\r\n" + "        , @RawSource"
                    + "\r\n" + "        , @MultipleUse"
                    + "\r\n" + "        , @ValueBinary"
                    + "\r\n" + "        , @ValueFloat"
                    + "\r\n" + "        , @ValueNumber"
                    + "\r\n" + "        , @ValueText"
                    + "\r\n" + "        , @EndemeProfileId"
                    + "\r\n" + "        )"
                    + "\r\n" + " SELECT EndemeId FROM @Table"
                    , Throws.Actions, "PR")
                    ._AddParameter_null("@EndemeSetId"    , enRow.EndemeSetId        )
                    ._AddParameter_safe("@EndemeString"   , enRow.EndemeString   , 24)
                    ._AddParameter_safe("@EndemeCode"     , enRow.EndemeCode     ,  8)
                    ._AddParameter_safe("@EndemeLabel"    , enRow.EndemeLabel    , 64)
                    ._AddParameter_safe("@EndemeDescr"    , enRow.EndemeDescr    , -1)
                    ._AddParameter_null("@RawSource"      , enRow.RawSource          )
                    ._AddParameter     ("@MultipleUse"    , enRow.MultipleUse        )
                    ._AddParameter_byte("@ValueBinary"    , enRow.ValueBinary        )
                    ._AddParameter_null("@ValueFloat"     , enRow.ValueFloat         )
                    ._AddParameter_null("@ValueNumber"    , enRow.ValueNumber        )
                    ._AddParameter_safe("@ValueText"      , enRow.ValueText      , -1)
                    ._AddParameter_null("@EndemeProfileId", enRow.EndemeProfileId    );
            }


            long endemeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("Error in ToEndemeTable: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeTable -->
        /// <summary>
        ///      Inserts in(To) the Endeme Table table a (ne)w endeme Table built from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawSource"      ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new Endeme Table object</returns>
        public EndemeTable ToneEndemeTable
            ( long     endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawSource
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            , string   toEndemeTable
            , InfoAspect aspect)
        {
            EndemeTable enRow = NeonEndemeTable
                ( endemeId
                , endemeSetId
                , endemeString
                , endemeCode , endemeLabel, endemeDescr
                , rawSource  , multipleUse
                , valueBinary, valueDateTime, valueFloat, valueNumber, valueText
                , endemeProfileId
                );
            enRow.EndemeId = ToEndemeTable(enRow, toEndemeTable, aspect);
            return enRow;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeTable -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeTable
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public void UnEndemeTable(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + toEndemeTable + " SET EntryState = 4 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeTable -->
        /// <summary>
        ///      (Up)dates a row in the Endeme table from an Endeme Table object
        /// </summary>
        /// <param name="enRow">endeme Table to update</param>
        /// <returns>the count of the updated endeme Table rows"></param>
        public int UpEndemeTable(EndemeTable enRow, string toEndemeTable, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeId = " + enRow.EndemeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(toEndemeTable, whereClause, aspect.PrimaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                            , "\r\n" + " UPDATE " + toEndemeTable
                            + "\r\n" + " SET   EndemeSetId     = @EndemeSetId"
                            + "\r\n" + "     , EndemeString    = @EndemeString"
                            + "\r\n" + "     , EndemeCode      = @EndemeCode"
                            + "\r\n" + "     , EndemeLabel     = @EndemeLabel"
                            + "\r\n" + "     , EndemeDescr     = @EndemeDescr"
                            + "\r\n" + "     , RawSource       = @RawSource"
                            + "\r\n" + "     , MultipleUse     = @MultipleUse"
                            + "\r\n" + "     , ValueBinary     = @ValueBinary"
                            + "\r\n" + "     , ValueDateTime   = @ValueDateTime"
                            + "\r\n" + "     , ValueFloat      = @ValueFloat"
                            + "\r\n" + "     , ValueNumber     = @ValueNumber"
                            + "\r\n" + "     , ValueText       = @ValueText"
                            + "\r\n" + "     , EndemeProfileId = @EndemeProfileId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeId"       , enRow.EndemeId                           )
                            ._AddParameter_null("@EndemeSetId"    , enRow.EndemeSetId                        )
                            ._AddParameter_safe("@EndemeString"   , enRow.EndemeString   ,                 24)
                            ._AddParameter_safe("@EndemeCode"     , enRow.EndemeCode     ,                  8)
                            ._AddParameter_safe("@EndemeLabel"    , enRow.EndemeLabel    ,                 64)
                            ._AddParameter_safe("@EndemeDescr"    , enRow.EndemeDescr    ,                 -1)
                            ._AddParameter_null("@RawSource"      , enRow.RawSource                          )
                            ._AddParameter     ("@MultipleUse"    , enRow.MultipleUse                        )
                            ._AddParameter_byte("@ValueBinary"    , enRow.ValueBinary                        )
                            ._AddParameter_date("@ValueDateTime"  , enRow.ValueDateTime  , SqlDbType.DateTime)
                            ._AddParameter_null("@ValueFloat"     , enRow.ValueFloat                         )
                            ._AddParameter_null("@ValueNumber"    , enRow.ValueNumber                        )
                            ._AddParameter_safe("@ValueText"      , enRow.ValueText      ,                 -1)
                            ._AddParameter_null("@EndemeProfileId", enRow.EndemeProfileId                    )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeTable: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeId " + enRow.EndemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SoupEndemeTable -->
        /// <summary>
        ///      (Up)dates (So)me of the columns of a row in the Endeme table from an Endeme Table object
        /// </summary>
        /// <param name="endemeTable">endeme Table to update</param>
        /// <returns>the count of the updated endeme Table rows"></param>
        public int SoupEndemeTable(RichDataTable some, string toEndemeTable, InfoAspect aspect)
        {
            long           endemeId    = some.LongValue(0, "EndemeId", 0);
            RichDataTable  all         = InEndemeTable(endemeId, toEndemeTable, aspect);
            string         whereClause = "WHERE EndemeId = " + endemeId.ToString();
            RichSqlCommand cmd         = null;
            int            count       = 0;


            try
            {
                switch (all.Count)
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                            , "\r\n" + " UPDATE " + toEndemeTable
                            + "\r\n" + " SET   EndemeSetId     = @EndemeSetId"
                            + "\r\n" + "     , EndemeString    = @EndemeString"
                            + "\r\n" + "     , EndemeCode      = @EndemeCode"
                            + "\r\n" + "     , EndemeLabel     = @EndemeLabel"
                            + "\r\n" + "     , EndemeDescr     = @EndemeDescr"
                            + "\r\n" + "     , RawSource       = @RawSource"
                            + "\r\n" + "     , MultipleUse     = @MultipleUse"
                            + "\r\n" + "     , ValueBinary     = @ValueBinary"
                            + "\r\n" + "     , ValueDateTime   = @ValueDateTime"
                            + "\r\n" + "     , ValueFloat      = @ValueFloat"
                            + "\r\n" + "     , ValueNumber     = @ValueNumber"
                            + "\r\n" + "     , ValueText       = @ValueText"
                            + "\r\n" + "     , EndemeProfileId = @EndemeProfileId"
                            + "\r\n" + " WHERE EndemeId = @EndemeId"
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeId"       , endemeId)
                            ._AddParameter_null("@EndemeSetId"    , some.GuidValue(0, "EndemeSetId"    , all.GuidValue(0, "EndemeSetId"    , Guid.Empty  )))
                            ._AddParameter_safe("@EndemeString"   , some.StrValue (0, "EndemeString"   , all.StrValue (0, "EndemeString"   , ""          )))
                            ._AddParameter_safe("@EndemeCode"     , some.StrValue (0, "EndemeCode"     , all.StrValue (0, "EndemeCode"     , ""          )))
                            ._AddParameter_safe("@EndemeLabel"    , some.StrValue (0, "EndemeLabel"    , all.StrValue (0, "EndemeLabel"    , ""          )))
                            ._AddParameter_safe("@EndemeDescr"    , some.StrValue (0, "EndemeDescr"    , all.StrValue (0, "EndemeDescr"    , ""          )))
                            ._AddParameter_null("@RawSource"      , some.BoolValue(0, "RawSource"      , all.BoolValue(0, "RawSource"      , false       )))
                            ._AddParameter     ("@MultipleUse"    , some.BoolValue(0, "MultipleUse"    , all.BoolValue(0, "MultipleUse"    , false       )))
                            ._AddParameter_byte("@ValueBinary"    , some.ByteValue(0, "ValueBinary"    , all.ByteValue(0, "ValueBinary"                  )))
                            ._AddParameter_date("@ValueDateTime"  , some.DateValue(0, "ValueDateTime"  , all.DateValue(0, "ValueDateTime"  , DateTime.Now)), SqlDbType.DateTime)
                            ._AddParameter_null("@ValueFloat"     , some.RealValue(0, "ValueFloat"     , all.RealValue(0, "ValueFloat"     , 0.0         )))
                            ._AddParameter_null("@ValueNumber"    , some.DecValue (0, "ValueNumber"    , all.DecValue (0, "ValueNumber"    , 0.0M        )))
                            ._AddParameter_safe("@ValueText"      , some.StrValue (0, "ValueText"      , all.StrValue (0, "ValueText"      , ""          )))
                            ._AddParameter_null("@EndemeProfileId", some.LongValue(0, "EndemeProfileId", all.LongValue(0, "EndemeProfileId", 0           )))
                            ;


                        count = cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeTable: " + cmd.Errors);
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeId " + endemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion Endeme Table table

        #region EndemeCharacteristic table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeCharacteristic table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeCharacteristic> AtMareEndemeCharacteristicOf            (Guid                 endemeSetId                   , InfoAspect aspect) { return AtOnMareEndemeCharacteristic(MareEndemeCharacteristicOf(endemeSetId, aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharLetter(char                 enCharLetter                  , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharLetter(enCharLetter, aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeSet       (Guid                 enSetId                       , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeSet       (enSetId     , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharCode  (string               enCharCode                    , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharCode  (enCharCode  , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharLabel (string               enCharLabel                   , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharLabel (enCharLabel , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharDescr (string               enCharDescr                   , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharDescr (enCharDescr , aspect)); }
        public List<EndemeCharacteristic> AtEndemeCharacteristicOfEndemeCharIsASet(Guid                 enCharIsASet                  , InfoAspect aspect) { return AtEndemeCharacteristic(InEndemeCharacteristicOfEndemeCharIsASet(enCharIsASet, aspect)); }
        public Endeme                     AtomicEndeme                            (long                 endemeId, string toEndemeTable, InfoAspect aspect) { return AtomicEndeme          (InicEndemeTable                         (endemeId, toEndemeTable, aspect)); }
        public Atom                       AtomMaEndemeCharacteristicOf            (Guid                 enSetId                       , InfoAspect aspect) { return MainEndemeCharacteristicOf(enSetId, aspect).ToAtom(); }
        public void                       ExEndemeCharacteristic                  (Guid                 enSetId , char   enCharLetter , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + LaEndemeChar + " WHERE EndemeSetId = '"+enSetId.ToString()+"' AND EndemeCharLetter = " + enCharLetter, aspect.SecondaryConn); }
        public RichDataTable              IntoEndemeCharacteristic                (EndemeCharacteristic enChar                        , InfoAspect aspect) { if (   IsEndemeCharacteristic(enChar.EndemeCharLetter, aspect)) return InEndemeCharacteristic(enChar.EndemeSetId, enChar.EndemeCharLetter, aspect); else { ToEndemeCharacteristic(enChar, aspect); return InEndemeCharacteristic(enChar.EndemeSetId, enChar.EndemeCharLetter, aspect); } }
        public EndemeCharacteristic       OnInEndemeCharacteristic                (Guid                 enSetId , char   enCharLetter , InfoAspect aspect) { return OnEndemeCharacteristic(InEndemeCharacteristic  (enSetId, enCharLetter, aspect), 0, true); }
        public EndemeCharacteristic       OnIntoEndemeCharacteristic              (EndemeCharacteristic enChar                        , InfoAspect aspect) { return OnEndemeCharacteristic(IntoEndemeCharacteristic(enChar  , aspect), 0, true); }
        public int                        UpToEndemeCharacteristic                (EndemeCharacteristic enChar                        , InfoAspect aspect) { if (   IsEndemeCharacteristic(enChar.EndemeCharLetter, aspect)) return UpEndemeCharacteristic(enChar, aspect); else { ToEndemeCharacteristic(enChar, aspect); return 1; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeCharacteristic_test -->
        /// <summary>
        ///      Tests the ToEndemeCharacteristic method
        /// </summary>
        public void ToEndemeCharacteristic_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeCharacteristicAccess";
            string adoMethod = "ToEndemeCharacteristic";
            Random r         = RandomSource.New().Random;
            char   enCha     = ' ';
            EndemeAccess acc = new EndemeAccess();
            Guid   endemeSetId = Guid.Empty;


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeCharacteristic endemeCharacteristicTo = acc.AnneEndemeCharacteristic(r);
            endemeSetId = endemeCharacteristicTo.EndemeSetId;
            enCha       = endemeCharacteristicTo.EndemeCharLetter;
            try
            {
                acc.ToEndemeCharacteristic(endemeCharacteristicTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(acc.Errors)) Assert.Crash(acc.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(enCha, Is.greater_than, -1, adoClass, adoMethod);
            EndemeCharacteristic endemeCharacteristicFrom = acc.OnInEndemeCharacteristic(endemeSetId, enCha, _aspect);
            acc.AssertEqualContent(endemeCharacteristicFrom, endemeCharacteristicTo, adoClass, adoMethod);


            acc.ExEndemeCharacteristic(endemeSetId, enCha, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeCharacteristic_test -->
        /// <summary>
        ///      Tests the UpEndemeCharacteristic method
        /// </summary>
        public void UpEndemeCharacteristic_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass                            = "EndemeCharacteristicAccess";
            string adoMethod                           = "UpEndemeCharacteristic";
            Random r                          = RandomSource.New().Random;
            EndemeCharacteristic endemeCharacteristic1 = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endeme Char Letter with a newly created endemeCharacteristic
                // ----------------------------------------------------------------------
                List<object> id = ende.AnIdOfEndemeCharacteristic(_aspect);
                Guid    endemeSetId      = TreatAs.GuidValue(id[0], Guid.Empty);
                char    enCha = TreatAs.CharValue(id[1], ' '       );
                endemeCharacteristic1                        = ende.OnInEndemeCharacteristic(endemeSetId, enCha, _aspect);
                EndemeCharacteristic  endemeCharacteristic2  = ende.AnneEndemeCharacteristic(r);
                endemeCharacteristic2.EndemeCharLetter = endemeCharacteristic1.EndemeCharLetter;
                ende.UpEndemeCharacteristic(endemeCharacteristic2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeCharacteristic endemeCharacteristic3 = ende.OnInEndemeCharacteristic(endemeSetId, enCha, _aspect);
                Assert.That(endemeCharacteristic3.EndemeCharLetter, Is.equal_to, endemeCharacteristic2.EndemeCharLetter, adoClass, adoMethod + " test update existing endemeCharacteristic");
                ende.AssertEqualContent  (endemeCharacteristic3, endemeCharacteristic2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeCharacteristic3, endemeCharacteristic1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeCharacteristic, did the update fail?
                // ----------------------------------------------------------------------
                EndemeCharacteristic  endemeCharacteristic4  = ende.AnneEndemeCharacteristic(r);
                endemeCharacteristic4.EndemeCharLetter       = ende.HiIdOfEndemeCharacteristic(_aspect);
                int                   count                  = ende.UpEndemeCharacteristic(endemeCharacteristic4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeCharacteristic");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeCharacteristic(endemeCharacteristic1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpStringOfEndeme -->
        /// <summary>
        ///      Updates the endeme string in an existing endeme
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="endeme"></param>
        /// <param name="aspect"></param>
        /// <returns>ExecuteNonQuery  output</returns>
        public int UpStringOfEndeme(long endemeId, string endeme, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE Endeme SET EndemeString = @EndemeString WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeString", endeme)
                ._AddParameter     ("@EndemeId"    , endemeId);


            int output = cmd.ExecuteNonQuery();
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsage -->
        /// <summary>
        ///      Looks in the EndemeUsage table for the usage of an endeme set
        /// </summary>
        /// <param name="endemeSetId"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public RichDataTable InEndemeUsage(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "", "");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeCharacteristic -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeCharacteristic table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public List<object> AnIdOfEndemeCharacteristic(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT EndemeSetId, EndemeCharLetter FROM " + LaEndemeChar + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeCharacteristicId = new RichDataTable(cmd, "EndemeCharacteristicId", "EndemeCharLetter"))
            {
                List<object> output = new List<object>(1);
                Random r = RandomSource.New().Random;
                if (endemeCharacteristicId.Count > 0)
                {
                    int row = r.Next(endemeCharacteristicId.Count);
                    output.Add(endemeCharacteristicId.GuidValue(row, "EndemeSetId"     , Guid.Empty));
                    output.Add(endemeCharacteristicId.CharValue(row, "EndemeCharLetter", ' '       ));
                    return output;
                }
                else return output;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeCharacteristic -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeCharacteristic object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeCharacteristic</returns>
        private EndemeCharacteristic AnneEndemeCharacteristic(Random r)
        {
            EndemeCharacteristic endemeCharacteristic = new EndemeCharacteristic
                { EndemeCharLetter     = "ABCDEFGHIJKLMNOPQRSTUV".ToCharArray()[r.Next(22)]
                , EndemeSetId          = Guid.NewGuid()
                , EndemeCharCode       = r.Next(10000).ToString()
                , EndemeCharLabel      = r.Next(10000).ToString()
                , EndemeCharDescr      = r.Next(10000).ToString()
                , EndemeCharIsASet     = Guid.NewGuid()
                };
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic being tested</param>
        /// <param name="tgt"      >endemeCharacteristic being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeCharacteristic endemeCharacteristic, EndemeCharacteristic tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeCharacteristic.EndemeCharCode      , Is.equal_to, tgt.EndemeCharCode      , adoClass, adoMethod + " EndemeCharCode"      );
            Assert.That(endemeCharacteristic.EndemeCharLabel     , Is.equal_to, tgt.EndemeCharLabel     , adoClass, adoMethod + " EndemeCharLabel"     );
            Assert.That(endemeCharacteristic.EndemeCharDescr     , Is.equal_to, tgt.EndemeCharDescr     , adoClass, adoMethod + " EndemeCharDescr"     );
            Assert.That(endemeCharacteristic.EndemeCharIsASet    , Is.equal_to, tgt.EndemeCharIsASet    , adoClass, adoMethod + " EndemeCharIsASet"    );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic being tested</param>
        /// <param name="tgt"      >endemeCharacteristic being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeCharacteristic endemeCharacteristic, EndemeCharacteristic tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeCharacteristic.EndemeCharCode      , Is.not_equal_to, tgt.EndemeCharCode      , adoClass, adoMethod + " EndemeCharCode"      );
            Assert.That(endemeCharacteristic.EndemeCharLabel     , Is.not_equal_to, tgt.EndemeCharLabel     , adoClass, adoMethod + " EndemeCharLabel"     );
            Assert.That(endemeCharacteristic.EndemeCharDescr     , Is.not_equal_to, tgt.EndemeCharDescr     , adoClass, adoMethod + " EndemeCharDescr"     );
            Assert.That(endemeCharacteristic.EndemeCharIsASet    , Is.not_equal_to, tgt.EndemeCharIsASet    , adoClass, adoMethod + " EndemeCharIsASet"    );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeCharacteristic -->
        /// <summary>
        ///     Returns a list of EndemeCharacteristic objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeCharacteristic objects</returns>
        public List<EndemeCharacteristic> AtEndemeCharacteristic(RichDataTable table)
        {
            List<EndemeCharacteristic> list = new List<EndemeCharacteristic>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeCharacteristic(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeCharacteristic -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeCharacteristic objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeCharacteristic objects"></param>
        public List<EndemeCharacteristic> AtOnMareEndemeCharacteristic(RichDataTable table)
        {
            List<EndemeCharacteristic> endemeCharacteristicList = new List<EndemeCharacteristic>(table.Count);
            Dictionary<int,EndemeCharacteristic> found = new Dictionary<int,EndemeCharacteristic>();


            for (int row = 0; row < table.Count; ++row)
            {
                int enCha = table.IntValue(row, "EndemeCharLetter", -1);
                EndemeCharacteristic endemeCharacteristic = null;

                if (!found.ContainsKey(enCha))
                {
                    endemeCharacteristic = OnEndemeCharacteristic(table, row);
                    endemeCharacteristic.EndemeMeaningList = new List<EndemeMeaning>();
                    endemeCharacteristicList.Add(endemeCharacteristic);
                    found.Add(enCha, endemeCharacteristic);
                }
                else
                {
                    endemeCharacteristic = found[enCha];
                }
            }

            return endemeCharacteristicList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeCharacteristic -->
        /// <summary>
        ///      Copies an endemeCharacteristic
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to copy</param>
        public static EndemeCharacteristic CpEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic)
        {
            EndemeCharacteristic output = new EndemeCharacteristic();

            output.EndemeCharLetter = endemeCharacteristic.EndemeCharLetter;
            output.EndemeSetId      = endemeCharacteristic.EndemeSetId;
            output.EndemeCharCode   = endemeCharacteristic.EndemeCharCode;
            output.EndemeCharLabel  = endemeCharacteristic.EndemeCharLabel;
            output.EndemeCharDescr  = endemeCharacteristic.EndemeCharDescr;
            output.EndemeCharIsASet = endemeCharacteristic.EndemeCharIsASet;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeCharacteristicCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeCharacteristic table
        /// </summary>
        /// <returns>a count of rows in the EndemeCharacteristic table</returns>
        public int EndemeCharacteristicCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT COUNT(*) FROM " + LaEndemeChar + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeCharacteristic: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeCharacteristic -->
        /// <summary>
        ///      Enables an EndemeCharacteristic
        /// </summary>
        /// <param name="enCha">the primary key</param>
        public void DoEndemeCharacteristic(Guid endemeSetId, char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , " UPDATE " + LaEndemeChar + " SET MeaningActive = 1"
                + " WHERE EndemeSetId = @EndemeSetId AND EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId"     , endemeSetId     )
                ._AddParameter("@EndemeCharLetter", enCha);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeCharacteristic: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeCharacteristic -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemecharacteristic table
        /// </summary>
        /// <returns>Maximum EndemeCharacteristic.EndemeCharLetter</returns>
        public char HiIdOfEndemeCharacteristic(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT EndemeCharLetter FROM " + LaEndemeChar + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            char enCha = TreatAs.CharValue(cmd.ExecuteScalar(""),' ');
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeCharacteristic: " + cmd.Errors);
            return enCha;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristic -->
        /// <summary>
        ///      Looks in Returns the indicated row (In) the EndemeCharacteristic table
        /// </summary>
        /// <param name="enChaLetter">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeCharacteristic(Guid endemeSetId, char enChaLetter, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK)"
                + "\r\n" + " WHERE EndemeSetId = @EndemeSetId AND EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeSetId"     , endemeSetId)
                ._AddParameter_char("@EndemeCharLetter", enChaLetter);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharLetter -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharLetter column
        /// </summary>
        /// <param name="enCha">value in EndemeCharLetter column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharLetter</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharLetter(char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter_char("@EndemeCharLetter", enCha);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf1EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf2EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharCode column
        /// </summary>
        /// <param name="endemeCharCode">value in EndemeCharCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharCode</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharCode(string endemeCharCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharCode = @EndemeCharCode"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeCharCode" , endemeCharCode);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf3EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharLabel column
        /// </summary>
        /// <param name="endemeCharLabel">value in EndemeCharLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharLabel</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharLabel(string endemeCharLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharLabel = @EndemeCharLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeCharLabel" , endemeCharLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf4EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharDescr column
        /// </summary>
        /// <param name="endemeCharDescr">value in EndemeCharDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharDescr</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharDescr(string endemeCharDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharDescr = @EndemeCharDescr"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeCharDescr" , endemeCharDescr);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf5EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeCharacteristicOfEndemeCharIsASet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeCharacteristic table filtered by a value (Of) the EndemeCharIsASet column
        /// </summary>
        /// <param name="endemeCharIsASet">value in EndemeCharIsASet column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCharIsASet</returns>
        public RichDataTable InEndemeCharacteristicOfEndemeCharIsASet(Guid endemeCharIsASet, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharIsASet = @EndemeCharIsASet"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeCharIsASet" , endemeCharIsASet);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors)) throw new ApplicationException("InOf6EndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeCharacteristic -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeCharacteristic table
        /// </summary>
        /// <param name="enCha">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeCharacteristic(char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                    , "SELECT * FROM " + LaEndemeChar + " WITH(NOLOCK) WHERE EndemeCharLetter = @EndemeCharLetter"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeCharLetter", enCha);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeCharacteristic with EndemeCharLetter " + enCha.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeCharacteristic -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeCharacteristic
        /// </summary>
        /// <param name="enCha">the primary key</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MainEndemeCharacteristic(char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, c.*"
                + "\r\n" + " FROM                " + LaEndemeChar + " AS c WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet +" AS s WITH(NOLOCK) ON s.EndemeSetId = c.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND c.EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeCharLetter", enCha);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeCharacteristicOf -->
        /// <summary>
        ///      Returns a endemeCharacteristic list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MainEndemeCharacteristicOf(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, c.*"
                + "\r\n" + " FROM                " + LaEndemeChar + " AS c WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet +" AS s WITH(NOLOCK) ON s.EndemeSetId = c.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND c.EndemeSetId      = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeSetId"     , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeCharacteristicOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeCharacteristicOf -->
        /// <summary>
        ///      Returns the endeme characteristics of an endeme
        /// </summary>
        /// <param name="endemeId"></param>
        /// <param name="toEndemeTable">Specifies which Endeme table to work with</param>
        /// <param name="aspect">DatabaseB may be different from databaseA</param>
        /// <returns></returns>
        public RichDataTable MainEndemeCharacteristicOf(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT 22 - ((CHARINDEX(c.EndemeCharLetter, e.EndemeString) + 22) % 23) AS EndemeCharOrd"
                + "\r\n" + "     , c.*, s.*, e.*"
                + "\r\n" + " FROM                "                            + LaEndemeChar + " AS c WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN "                            + LaEndemeSet  + " AS s WITH(NOLOCK) ON s.EndemeSetId = c.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN " + aspect.SecondaryDb + "." + toEndemeTable     + " AS e WITH(NOLOCK) ON e.EndemeSetId = c.EndemeSetId"
                + "\r\n" + " WHERE e.EndemeId = @EndemeId"
                + "\r\n" + " ORDER BY (CHARINDEX(c.EndemeCharLetter, e.EndemeString) + 25) % 26"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "CharOfEndeme", "CharLetter");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeCharacteristicOf -->
        /// <summary>
        ///      Returns a endemeCharacteristic list (Of) a p(a)rent endemeset with (re)trieved endemeCharacteristic and endememeaning data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeCharacteristic rows with their joined parent data</returns>
        public RichDataTable MareEndemeCharacteristicOf(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, c.*, m.*"
                + "\r\n" + " FROM                " + LaEndemeChar    + " AS c WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet     + " AS s WITH(NOLOCK) ON s.EndemeSetId      = c.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeMeaning + " AS m WITH(NOLOCK) ON m.EndemeCharLetter = c.EndemeCharLetter"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND c.EndemeSetId      = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeSetId"     , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeCharacteristicOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeCharacteristic -->
        /// <summary>
        ///      Creates a (Ne)w EndemeCharacteristic (o)bject from member values
        /// </summary>
        /// <param name="enCha"    ></param>
        /// <param name="endemeSetId"         ></param>
        /// <param name="endemeCharCode"      ></param>
        /// <param name="endemeCharLabel"     ></param>
        /// <param name="endemeCharDescr"     ></param>
        /// <param name="endemeCharIsASet"    ></param>
        /// <returns>the new EndemeCharacteristic object</returns>
        public EndemeCharacteristic NeonEndemeCharacteristic
            ( char     enCha
            , Guid     endemeSetId
            , string   endemeCharCode
            , string   endemeCharLabel
            , string   endemeCharDescr
            , Guid     endemeCharIsASet
            )
        {
            EndemeCharacteristic endemeCharacteristic = new EndemeCharacteristic
                { EndemeCharLetter = enCha
                , EndemeSetId      = endemeSetId
                , EndemeCharCode   = endemeCharCode
                , EndemeCharLabel  = endemeCharLabel
                , EndemeCharDescr  = endemeCharDescr
                , EndemeCharIsASet = endemeCharIsASet
                };
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeCharacteristic -->
        /// <summary>
        ///      Converts a row in the EndemeCharacteristic data table into a EndemeCharacteristic (O)bject
        /// </summary>
        /// <param name="table"  >a table containing columns to build a batch object</param>
        /// <param name="row"    >the row to convert</param>
        /// <param name="dispose">whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeCharacteristic OnEndemeCharacteristic(RichDataTable table, int row, bool dispose = false)
        {
            EndemeCharacteristic enChar = new EndemeCharacteristic();
            enChar.EndemeCharLetter = table.CharValue(row, "EndemeCharLetter", ' '       );
            enChar.EndemeSetId      = table.GuidValue(row, "EndemeSetId"     , Guid.Empty);
            enChar.EndemeCharCode   = table.StrValue (row, "EndemeCharCode"  , ""        );
            enChar.EndemeCharLabel  = table.StrValue (row, "EndemeCharLabel" , ""        );
            enChar.EndemeCharDescr  = table.StrValue (row, "EndemeCharDescr" , ""        );
            enChar.EndemeCharIsASet = table.GuidValue(row, "EndemeCharIsASet", Guid.Empty);

            if (dispose) table.Dispose();
            return enChar;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeCharacteristic -->
        /// <summary>
        ///      Returns a table of a EndemeCharacteristic (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="enCha">the primary key</param>
        /// <returns>a table of EndemeCharacteristic rows with their joined details</returns>
        public RichDataTable ReinEndemeCharacteristic(char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT c.*, m.*"
                + "\r\n" + " FROM                " + LaEndemeChar    + " AS c WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeMeaning + " AS m WITH(NOLOCK) ON m.EndemeCharLetter = c.EndemeCharLetter"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND c.EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter_char("@EndemeCharLetter", enCha);


            RichDataTable table = new RichDataTable(cmd, "EndemeCharacteristic", "EndemeCharLetter");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeCharacteristic: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeCharacteristic -->
        /// <summary>
        ///      Inserts an endemeCharacteristic object in(To) the database
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to insert into database</param>
        /// <returns>the Id of the inserted EndemeCharacteristic</returns>
        public int ToEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " INSERT INTO " + LaEndemeChar
                + "\r\n" + "        ( EndemeCharLetter"
                + "\r\n" + "        , EndemeSetId"
                + "\r\n" + "        , EndemeCharCode"
                + "\r\n" + "        , EndemeCharLabel"
                + "\r\n" + "        , EndemeCharDescr"
                + "\r\n" + "        , EndemeCharIsASet"
                + "\r\n" + "        )"
                + "\r\n" + " VALUES ( @EndemeCharLetter"
                + "\r\n" + "        , @EndemeSetId"
                + "\r\n" + "        , @EndemeCharCode"
                + "\r\n" + "        , @EndemeCharLabel"
                + "\r\n" + "        , @EndemeCharDescr"
                + "\r\n" + "        , @EndemeCharIsASet"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                ._AddParameter_char("@EndemeCharLetter", endemeCharacteristic.EndemeCharLetter      )
                ._AddParameter     ("@EndemeSetId"     , endemeCharacteristic.EndemeSetId           )
                ._AddParameter_safe("@EndemeCharCode"  , endemeCharacteristic.EndemeCharCode  ,    8)
                ._AddParameter_safe("@EndemeCharLabel" , endemeCharacteristic.EndemeCharLabel ,   64)
                ._AddParameter_safe("@EndemeCharDescr" , endemeCharacteristic.EndemeCharDescr , 4096)
                ._AddParameter_null("@EndemeCharIsASet", endemeCharacteristic.EndemeCharIsASet      );


            int count = cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeCharacteristic: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeCharacteristicAtEndemeSet -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="es"></param>
        /// <param name="aspect"></param>
        private void ToEndemeCharacteristicAtEndemeSet(EndemeSet es, InfoAspect aspect)
        {
            if (es.EndemeSetId == Guid.Empty)
                throw new ArgumentException("Endeme set is missing its id");


            List<char> charList = es.LettersUnsorted;

            for (int i = 0; i < charList.Count; ++i)
            {
                char c = charList[i];
                EndemeCharacteristic enChar = es[c];
                RichDataTable table = InEndemeCharacteristic(es.EndemeSetId, c, aspect);
                if (table.Count == 0)
                    ToEndemeCharacteristic(enChar, aspect);
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToFieldOfEndemeCharacteristic -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSetId"></param>
        /// <param name="enCharId"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="aspect"></param>
        public void ToFieldOfEndemeCharacteristic(string enSetId, string enCharId, string column, string value, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE EndemeCharacteristic SET "+column+" = @Value WHERE EndemeSetId = @EnSetId AND EndemeCharLetter = @EnCharId"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@Value"   , value)
                ._AddParameter     ("@EnSetId" , TreatAs.GuidValue(enSetId, Guid.Empty))
                ._AddParameter_safe("@EnCharId", enCharId)
                ;
            cmd.ExecuteNonQuery();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeCharacteristic -->
        /// <summary>
        ///     Inserts in(To) the EndemeCharacteristic table a (ne)w endemeCharacteristic built from member values
        /// </summary>
        /// <param name="enCha"               ></param>
        /// <param name="endemeSetId"         ></param>
        /// <param name="endemeCharCode"      ></param>
        /// <param name="endemeCharLabel"     ></param>
        /// <param name="endemeCharDescr"     ></param>
        /// <param name="endemeCharIsASet"    ></param>
        /// <returns>the new EndemeCharacteristic object</returns>
        public EndemeCharacteristic ToneEndemeCharacteristic
            ( char     enCha
            , Guid     endemeSetId
            , string   endemeCharCode
            , string   endemeCharLabel
            , string   endemeCharDescr
            , Guid     endemeCharIsASet
            , InfoAspect aspect)
        {
            EndemeCharacteristic endemeCharacteristic = NeonEndemeCharacteristic
                ( enCha
                , endemeSetId
                , endemeCharCode
                , endemeCharLabel
                , endemeCharDescr
                , endemeCharIsASet
                );
            ToEndemeCharacteristic(endemeCharacteristic, aspect);
            return endemeCharacteristic;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeCharacteristic -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeCharacteristic
        /// </summary>
        /// <param name="enCha">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public void UnEndemeCharacteristic(char enCha, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE " + LaEndemeChar + " SET MeaningActive = 0 WHERE EndemeCharLetter = @EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeCharLetter", enCha);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeCharacteristic: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeCharacteristic -->
        /// <summary>
        ///      (Up)dates a row in the EndemeCharacteristic table from a EndemeCharacteristic object
        /// </summary>
        /// <param name="endemeCharacteristic">endemeCharacteristic to update</param>
        /// <returns>the count of the updated endemeCharacteristic rows"></param>
        public int UpEndemeCharacteristic(EndemeCharacteristic endemeCharacteristic, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeCharLetter = " + endemeCharacteristic.EndemeCharLetter.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(LaEndemeChar, whereClause, aspect.SecondaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                            , "\r\n" + " UPDATE " + LaEndemeChar
                            + "\r\n" + " SET   EndemeSetId      = @EndemeSetId"
                            + "\r\n" + "     , EndemeCharCode   = @EndemeCharCode"
                            + "\r\n" + "     , EndemeCharLabel  = @EndemeCharLabel"
                            + "\r\n" + "     , EndemeCharDescr  = @EndemeCharDescr"
                            + "\r\n" + "     , EndemeCharIsASet = @EndemeCharIsASet"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter_char("@EndemeCharLetter", endemeCharacteristic.EndemeCharLetter      )
                            ._AddParameter     ("@EndemeSetId"     , endemeCharacteristic.EndemeSetId           )
                            ._AddParameter_safe("@EndemeCharCode"  , endemeCharacteristic.EndemeCharCode  ,    8)
                            ._AddParameter_safe("@EndemeCharLabel" , endemeCharacteristic.EndemeCharLabel ,   64)
                            ._AddParameter_safe("@EndemeCharDescr" , endemeCharacteristic.EndemeCharDescr , 4096)
                            ._AddParameter_null("@EndemeCharIsASet", endemeCharacteristic.EndemeCharIsASet      )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException(cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeCharacteristicId " + endemeCharacteristic.EndemeCharLetter.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeCharacteristic table

        #region EndemeIndex table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeIndex table methods
        // ----------------------------------------------------------------------------------------
      //public List<EndemeIndex> AtMareEndemeIndexOf             (int                       endemetableId      , InfoAspect aspect) { return AtOnMareEndemeIndex(MareEndemeIndexOf(endemetableId, aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeKey        (int                       endemeKeyId        , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeKey          (endemeKeyId        , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeLarge      (int                       endemeLargeId      , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeLarge        (endemeLargeId      , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfMatchStrength    (int                       matchStrength      , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfMatchStrength      (matchStrength      , aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfDocumentTransform(int                       documentTransformId, InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfDocumentTransform  (documentTransformId, aspect)); }
        public List<EndemeIndex> AtEndemeIndexOfEndemeSet        (Guid                      endemeSetId        , InfoAspect aspect) { return AtEndemeIndex(InEndemeIndexOfEndemeSet          (endemeSetId        , aspect)); }
        public Atom              AtomMaEndemeIndexOf             (int endemetableId, string toEndemeTable      , InfoAspect aspect) { return MainEndemeIndexOf(endemetableId, toEndemeTable, aspect).ToAtom(); }
        public void              ExEndemeIndex                   (int endemeKeyId  , long   endemeLargeId      , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + LaEndemeIndex + " WHERE EndemeKeyId = " + endemeKeyId + " AND EndemeLargeId = " + endemeLargeId, aspect.PrimaryConn); }
        public RichDataTable     IntoEndemeIndex                 (EndemeIndex               endemeIndex        , InfoAspect aspect) { if (   IsEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect)) return InEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect); else { return InEndemeIndex(ToEndemeIndex(endemeIndex, aspect), aspect); } }
        public EndemeIndex       OnInEndemeIndex                 (int endemeKeyId  , long   endemeLargeId      , InfoAspect aspect) { return OnEndemeIndex(InEndemeIndex  (endemeKeyId, endemeLargeId, aspect), 0, true); }
        public EndemeIndex       OnIntoEndemeIndex               (EndemeIndex               endemeIndex        , InfoAspect aspect) { return OnEndemeIndex(IntoEndemeIndex(endemeIndex  , aspect), 0, true); }
        public int               UpToEndemeIndex                 (EndemeIndex               endemeIndex        , InfoAspect aspect) { if (   IsEndemeIndex(endemeIndex.EndemeKeyId, endemeIndex.EndemeLargeId, aspect)) return UpEndemeIndex(endemeIndex, aspect); else { ToEndemeIndex(endemeIndex, aspect); return 1; } }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeIndex_test -->
        /// <summary>
        ///      Tests the ToEndemeIndex method
        /// </summary>
        public void ToEndemeIndex_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeIndexAccess";
            string adoMethod = "ToEndemeIndex";
            Random r = RandomSource.New().Random;
            List<int> endemeIndexId = new List<int>();
            EndemeAccess ende = new EndemeAccess();


            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeIndex endemeIndexTo = ende.AnneEndemeIndex(r);
            try
            {
                endemeIndexId = ende.ToEndemeIndex(endemeIndexTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeIndexId[0], Is.greater_than, -1, adoClass, adoMethod);
            EndemeIndex endemeIndexFrom = ende.OnInEndemeIndex(endemeIndexId[0], endemeIndexId[1], _aspect);
            ende.AssertEqualContent(endemeIndexFrom, endemeIndexTo, adoClass, adoMethod);


            ende.ExEndemeIndex(endemeIndexId[0], endemeIndexId[1], _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeIndex_test -->
        /// <summary>
        ///      Tests the UpEndemeIndex method
        /// </summary>
        public void UpEndemeIndex_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeIndexAccess";
            string adoMethod         = "UpEndemeIndex";
            Random r                 = RandomSource.New().Random;
            EndemeIndex endemeIndex1 = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeKeyId with a newly created endemeIndex
                // ----------------------------------------------------------------------
                List<int>    endemeKeyId  = ende.AnIdOfEndemeIndex(_aspect);
                endemeIndex1              = ende.OnInEndemeIndex(endemeKeyId[0], endemeKeyId[1], _aspect);
                EndemeIndex  endemeIndex2 = ende.AnneEndemeIndex(r);
                endemeIndex2.EndemeKeyId  = endemeIndex1.EndemeKeyId;
                ende.UpEndemeIndex(endemeIndex2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeIndex endemeIndex3 = ende.OnInEndemeIndex(endemeKeyId[0], endemeKeyId[1], _aspect);
                Assert.That(endemeIndex3.EndemeKeyId, Is.equal_to, endemeIndex2.EndemeKeyId, adoClass, adoMethod + " test update existing endemeIndex");
                ende.AssertEqualContent  (endemeIndex3, endemeIndex2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeIndex3, endemeIndex1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeIndex, did the update fail?
                // ----------------------------------------------------------------------
                EndemeIndex  endemeIndex4  = ende.AnneEndemeIndex(r);
                endemeIndex4.EndemeKeyId   = ende.HiIdOfEndemeKey  (_aspect) + 1;
                endemeIndex4.EndemeLargeId = ende.HiIdOfEndemeIndexLarge(_aspect) + 1;
                int          count         = ende.UpEndemeIndex(endemeIndex4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeIndex");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeIndex(endemeIndex1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeIndex -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeIndex table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public List<int> AnIdOfEndemeIndex(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT TOP 1000000 EndemeKeyId, EndemeLargeId FROM " + LaEndemeIndex + " WITH(NOLOCK) ORDER BY EndemeLargeId DESC"
                , Throws.Actions, "P");


            using (RichDataTable endemeIndex = new RichDataTable(cmd, "EndemeIndexId", "EndemeKeyId, EndemeLargeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeIndex.Count > 0)
                {
                    int row = r.Next(endemeIndex.Count);
                    List<int> endemeIndexId = new List<int>(2);
                    endemeIndexId.Add(endemeIndex.IntValue(row, "EndemeKeyId"  , -1));
                    endemeIndexId.Add(endemeIndex.IntValue(row, "EndemeLargeId", -1));
                    return endemeIndexId;
                }
                else return new List<int>();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeIndex -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeIndex object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeIndex</returns>
        private EndemeIndex AnneEndemeIndex(Random r)
        {
            EndemeIndex endemeIndex = new EndemeIndex
                { EndemeKeyId         = HiIdOfEndemeKey  (_aspect) + 1
                , EndemeLargeId       = HiIdOfEndemeIndexLarge(_aspect) + 1
                , MatchStrength       = r.Next(10000)
                , DocumentTransformId = r.Next(10000)
                , EndemeSetId         = Guid.NewGuid()
                };
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeIndex">endemeIndex being tested</param>
        /// <param name="tgt"      >endemeIndex being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeIndex endemeIndex, EndemeIndex tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeIndex.MatchStrength      , Is.equal_to, tgt.MatchStrength      , adoClass, adoMethod + " MatchStrength"      );
            Assert.That(endemeIndex.DocumentTransformId, Is.equal_to, tgt.DocumentTransformId, adoClass, adoMethod + " DocumentTransformId");
            Assert.That(endemeIndex.EndemeSetId        , Is.equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeIndex">endemeIndex being tested</param>
        /// <param name="tgt"      >endemeIndex being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeIndex endemeIndex, EndemeIndex tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeIndex.MatchStrength      , Is.not_equal_to, tgt.MatchStrength      , adoClass, adoMethod + " MatchStrength"      );
            Assert.That(endemeIndex.DocumentTransformId, Is.not_equal_to, tgt.DocumentTransformId, adoClass, adoMethod + " DocumentTransformId");
            Assert.That(endemeIndex.EndemeSetId        , Is.not_equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeIndex -->
        /// <summary>
        ///     Returns a list of EndemeIndex objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeIndex objects</returns>
        public List<EndemeIndex> AtEndemeIndex(RichDataTable table)
        {
            List<EndemeIndex> list = new List<EndemeIndex>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeIndex(table, row));
            return list;
        }

        //// ----------------------------------------------------------------------------------------
        ///// <!-- AtOnMareEndemeIndex -->
        ///// <summary>
        /////      Converts a DataTable to a list of parent and detail extended endemeIndex objects
        ///// </summary>
        ///// <param name="table">the table to convert containin also parent and child data</param>
        ///// <returns>a list of parent and detail extended endemeIndex objects"></param>
        //public List<EndemeIndex> AtOnMareEndemeIndex(RichDataTable table)
        //{
        //    List<EndemeIndex> endemeIndexList = new List<EndemeIndex>(table.Count);
        //    Dictionary<string,EndemeIndex> found = new Dictionary<string,EndemeIndex>();


        //    for (int row = 0; row < table.Count; ++row)
        //    {
        //        int endemeKeyId   = table.IntValue(row, "EndemeKeyId"  , -1);
        //        long endemeLargeId = table.IntValue(row, "EndemeLargeId", -1);
        //        EndemeIndex endemeIndex = null;

        //        if (!found.ContainsKey(endemeKeyId + "-" + endemeLargeId))
        //        {
        //            endemeIndex = OnEndemeIndex(table, row);

        //            endemeIndex.EndemeLargeList = new List<EndemeLarge>();
        //            endemeIndex.EndemeTable = OnEndemeTable(table, row);
        //            endemeIndexList.Add(endemeIndex);
        //            found.Add(endemeKeyId + "-" + endemeLargeId, endemeIndex);
        //        }
        //        else
        //        {
        //            endemeIndex = found[endemeKeyId + "-" + endemeLargeId];
        //        }

        //        endemeIndex.EndemeLargeList.Add(OnEndemeLarge(table, row));
        //    }

        //    return endemeIndexList;
        //}

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeIndex -->
        /// <summary>
        ///      Copies an endemeIndex
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to copy</param>
        public static EndemeIndex CpEndemeIndex(EndemeIndex endemeIndex)
        {
            EndemeIndex output = new EndemeIndex();

            output.EndemeKeyId         = endemeIndex.EndemeKeyId;
            output.EndemeLargeId       = endemeIndex.EndemeLargeId;
            output.MatchStrength       = endemeIndex.MatchStrength;
            output.DocumentTransformId = endemeIndex.DocumentTransformId;
            output.EndemeSetId         = endemeIndex.EndemeSetId;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeIndexCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeIndex table
        /// </summary>
        /// <returns>a count of rows in the EndemeIndex table</returns>
        public int EndemeIndexCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT COUNT(*) FROM " + LaEndemeIndex + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeIndex: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeKey -->
        /// <summary>
        ///      Returns the (Hi)ghest Key (Id) (Of) the endemeindex table
        /// </summary>
        /// <returns>Maximum EndemeIndex.EndemeKeyId</returns>
        public int HiIdOfEndemeKey(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT MAX(EndemeKeyId) AS EndemeKeyId FROM " + LaEndemeIndex + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeKeyId = cmd.ExecuteScalar(0);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeIndexKey: " + cmd.Errors);
            return endemeKeyId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeLarge -->
        /// <summary>
        ///      Returns the (Hi)ghest Large (Id) (Of) the endemeindex table
        /// </summary>
        /// <returns>Maximum EndemeIndex.EndemeKeyId</returns>
        public int HiIdOfEndemeIndexLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT MAX(EndemeLargeId) AS EndemeLargeId FROM " + LaEndemeIndex + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeLargeId = cmd.ExecuteScalar(0);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeLarge: " + cmd.Errors);
            return endemeLargeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndex -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeIndex table
        /// </summary>
        /// <param name="endemeKeyId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeIndex(int endemeKeyId, long endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeKeyId"  , endemeKeyId  )
                ._AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable InEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return InEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeKey -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeKeyId column
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of rows related to the specifed value of EndemeKeyId</returns>
        public RichDataTable InEndemeIndexOfEndemeKey(int endemeKeyId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeKeyId" , endemeKeyId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeLarge -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeLargeId column
        /// </summary>
        /// <param name="endemeLargeId">value in EndemeLargeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLargeId</returns>
        public RichDataTable InEndemeIndexOfEndemeLarge(int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeLargeId" , endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfMatchStrength -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the MatchStrength column
        /// </summary>
        /// <param name="matchStrength">value in MatchStrength column</param>
        /// <returns>a table of rows related to the specifed value of MatchStrength</returns>
        public RichDataTable InEndemeIndexOfMatchStrength(int matchStrength, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE MatchStrength = @MatchStrength"
                , Throws.Actions, "PR")
                ._AddParameter_null("@MatchStrength" , matchStrength);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfDocumentTransform -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the DocumentTransformId column
        /// </summary>
        /// <param name="documentTransformId">value in DocumentTransformId column</param>
        /// <returns>a table of rows related to the specifed value of DocumentTransformId</returns>
        public RichDataTable InEndemeIndexOfDocumentTransform(int documentTransformId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE DocumentTransformId = @DocumentTransformId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@DocumentTransformId" , documentTransformId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeIndexOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeIndex table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeIndexOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeLargeId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeIndex -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeIndex table
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "SELECT * FROM " + LaEndemeIndex + " WITH(NOLOCK) WHERE EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeKeyId"  , endemeKeyId)
                    ._AddParameter("@EndemeLargeId", endemeLargeId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeIndex with EndemeKeyId " + endemeKeyId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }
        public bool IsEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return IsEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeIndex -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeIndex
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MainEndemeIndex(int endemeKeyId, int endemeLargeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT e.*, i.*"
                + "\r\n" + " FROM                " + LaEndemeIndex + " AS i WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + toEndemeTable      + " AS e WITH(NOLOCK) ON e.EndemeId = i.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND i.EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeKeyId"  , endemeKeyId)
                ._AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable MainEndemeIndex(List<int> endemeIndexId, string toEndemeTable, InfoAspect aspect) { return MainEndemeIndex(endemeIndexId[0], endemeIndexId[1], toEndemeTable, aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeIndexOf -->
        /// <summary>
        ///      Returns a endemeIndex list (Of) a p(a)rent endeme table with its endeme table data
        /// </summary>
        /// <param name="endemetableId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MainEndemeIndexOf(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT e.*, i.*"
                + "\r\n" + " FROM                " + LaEndemeIndex + " AS i WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + toEndemeTable      + " AS e WITH(NOLOCK) ON e.EndemeId = i.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND i.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId, EndemeLargeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeIndexOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeIndexOf -->
        /// <summary>
        ///      Returns a endemeIndex list (Of) a p(a)rent endeme table with (re)trieved endemeIndex and endemelarge data
        /// </summary>
        /// <param name="endemetableId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeIndex rows with their joined parent data</returns>
        public RichDataTable MareEndemeIndexOf(long endemeId, string toEndemeTable, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT e.*, i.*, l.*"
                + "\r\n" + " FROM                " + LaEndemeIndex + " AS i WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + toEndemeTable      + " AS e WITH(NOLOCK) ON e .EndemeId    = i.EndemeId"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeLarge + " AS l WITH(NOLOCK) ON l.EndemeKeyId = i.EndemeKeyId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND i.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeIndexOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeIndex -->
        /// <summary>
        ///      Creates a (Ne)w EndemeIndex (o)bject from member values
        /// </summary>
        /// <param name="endemeKeyId"        ></param>
        /// <param name="endemeLargeId"      ></param>
        /// <param name="matchStrength"      ></param>
        /// <param name="documentTransformId"></param>
        /// <param name="endemeSetId"        ></param>
        /// <returns>the new EndemeIndex object</returns>
        public EndemeIndex NeonEndemeIndex
            ( int      endemeKeyId
            , int      endemeLargeId
            , int      matchStrength
            , int      documentTransformId
            , Guid     endemeSetId
            )
        {
            EndemeIndex endemeIndex = new EndemeIndex
                { EndemeKeyId         = endemeKeyId
                , EndemeLargeId       = endemeLargeId
                , MatchStrength       = matchStrength
                , DocumentTransformId = documentTransformId
                , EndemeSetId         = endemeSetId
                };
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeIndex -->
        /// <summary>
        ///      Converts a row in the EndemeIndex data table into a EndemeIndex (O)bject
        /// </summary>
        /// <param name="endemeIndexTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeIndex OnEndemeIndex(RichDataTable endemeIndexTable, int row, bool dispose = false)
        {
            EndemeIndex endemeIndex = new EndemeIndex();
            endemeIndex.EndemeKeyId         = endemeIndexTable.IntValue (row, "EndemeKeyId"        , -1        );
            endemeIndex.EndemeLargeId       = endemeIndexTable.IntValue (row, "EndemeLargeId"      , -1        );
            endemeIndex.MatchStrength       = endemeIndexTable.IntValue (row, "MatchStrength"      , -1        );
            endemeIndex.DocumentTransformId = endemeIndexTable.IntValue (row, "DocumentTransformId", -1        );
            endemeIndex.EndemeSetId         = endemeIndexTable.GuidValue(row, "EndemeSetId"        , Guid.Empty);

            if (dispose) endemeIndexTable.Dispose();
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeIndex -->
        /// <summary>
        ///      Returns a table of a EndemeIndex (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeKeyId"  >first part of the primary key</param>
        /// <param name="endemeLargeId">second part of the primary key</param>
        /// <returns>a table of EndemeIndex rows with their joined details</returns>
        public RichDataTable ReinEndemeIndex(int endemeKeyId, int endemeLargeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT i.*, l.*"
                + "\r\n" + " FROM                " + LaEndemeIndex + " AS i WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeLarge + " AS l WITH(NOLOCK) ON l.EndemeKeyId = i.EndemeKeyId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND i.EndemeKeyId = @EndemeKeyId AND EndemeLargeId = @EndemeLargeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeKeyId"  , endemeKeyId  )
                ._AddParameter("@EndemeLargeId", endemeLargeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeIndex", "EndemeKeyId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeIndex: " + cmd.Errors);
            return table;
        }
        public RichDataTable ReinEndemeIndex(List<int> endemeIndexId, InfoAspect aspect) { return ReinEndemeIndex(endemeIndexId[0], endemeIndexId[1], aspect); }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeIndex -->
        /// <summary>
        ///      Inserts an endemeIndex object in(To) the database
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to insert into database</param>
        /// <returns>the Id of the inserted EndemeIndex</returns>
        public List<int> ToEndemeIndex(EndemeIndex endemeIndex, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " INSERT INTO " + LaEndemeIndex
                + "\r\n" + "        ( EndemeKeyId"
                + "\r\n" + "        , EndemeLargeId"
                + "\r\n" + "        , MatchStrength"
                + "\r\n" + "        , DocumentTransformId"
                + "\r\n" + "        , EndemeSetId"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeKeyId"
                + "\r\n" + " VALUES ( @EndemeKeyId"
                + "\r\n" + "        , @EndemeLargeId"
                + "\r\n" + "        , @MatchStrength"
                + "\r\n" + "        , @DocumentTransformId"
                + "\r\n" + "        , @EndemeSetId"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeKeyId"        , endemeIndex.EndemeKeyId        )
                ._AddParameter     ("@EndemeLargeId"      , endemeIndex.EndemeLargeId      )
                ._AddParameter_null("@MatchStrength"      , endemeIndex.MatchStrength      )
                ._AddParameter_null("@DocumentTransformId", endemeIndex.DocumentTransformId)
                ._AddParameter_null("@EndemeSetId"        , endemeIndex.EndemeSetId        );


            int endemeIndexId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeIndex: " + cmd.Errors);
            List<int> id = new List<int>();
            id.Add(endemeIndex.EndemeKeyId);
            id.Add(endemeIndex.EndemeLargeId);
            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeIndex -->
        /// <summary>
        ///     Inserts in(To) the EndemeIndex table a (ne)w endemeIndex built from member values
        /// </summary>
        /// <param name="endemeKeyId"        ></param>
        /// <param name="endemeLargeId"      ></param>
        /// <param name="matchStrength"      ></param>
        /// <param name="documentTransformId"></param>
        /// <param name="endemeSetId"        ></param>
        /// <returns>the new EndemeIndex object</returns>
        public EndemeIndex ToneEndemeIndex
            ( int      endemeKeyId
            , int      endemeLargeId
            , int      matchStrength
            , int      documentTransformId
            , Guid     endemeSetId
            , InfoAspect aspect)
        {
            EndemeIndex endemeIndex = NeonEndemeIndex
                ( endemeKeyId
                , endemeLargeId
                , matchStrength
                , documentTransformId
                , endemeSetId
                );
            ToEndemeIndex(endemeIndex, aspect);
            return endemeIndex;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeIndex -->
        /// <summary>
        ///      (Up)dates a row in the EndemeIndex table from a EndemeIndex object
        /// </summary>
        /// <param name="endemeIndex">endemeIndex to update</param>
        /// <returns>the count of the updated endemeIndex rows"></param>
        public int UpEndemeIndex(EndemeIndex endemeIndex, InfoAspect aspect)
        {
            string whereClause
                = " WHERE   EndemeKeyId   = " + endemeIndex.EndemeKeyId  .ToString()
                + "     AND EndemeLargeId = " + endemeIndex.EndemeLargeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(LaEndemeIndex, whereClause, aspect.PrimaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                            , "\r\n" + " UPDATE " + LaEndemeIndex
                            + "\r\n" + " SET   EndemeLargeId       = @EndemeLargeId"
                            + "\r\n" + "     , MatchStrength       = @MatchStrength"
                            + "\r\n" + "     , DocumentTransformId = @DocumentTransformId"
                            + "\r\n" + "     , EndemeSetId         = @EndemeSetId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeKeyId"        , endemeIndex.EndemeKeyId        )
                            ._AddParameter     ("@EndemeLargeId"      , endemeIndex.EndemeLargeId      )
                            ._AddParameter_null("@MatchStrength"      , endemeIndex.MatchStrength      )
                            ._AddParameter_null("@DocumentTransformId", endemeIndex.DocumentTransformId)
                            ._AddParameter_null("@EndemeSetId"        , endemeIndex.EndemeSetId        )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeIndex: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeIndexId " + endemeIndex.EndemeKeyId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeIndex table

        /* #region EndemeLarge table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeLarge table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeLarge> AtMareEndemeLargeOf         (int         endemesetId    , InfoAspect aspect) { return AtOnMareEndemeLarge(MareEndemeLargeOf(endemesetId, aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndeme       (long        endemeId       , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndeme         (endemeId       , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeSet    (Guid        endemeSetId    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeSet      (endemeSetId    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeString (string      endemeString   , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeString   (endemeString   , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeCode   (string      endemeCode     , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeCode     (endemeCode     , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeLabel  (string      endemeLabel    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeLabel    (endemeLabel    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeDescr  (string      endemeDescr    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeDescr    (endemeDescr    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfRawSource    (bool        rawSource      , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfRawSource      (rawSource      , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfMultipleUse  (bool        multipleUse    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfMultipleUse    (multipleUse    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueBinary  (byte[]      valueBinary    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueBinary    (valueBinary    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueDateTime(DateTime    valueDateTime  , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueDateTime  (valueDateTime  , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueFloat   (double      valueFloat     , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueFloat     (valueFloat     , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueNumber  (decimal     valueNumber    , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueNumber    (valueNumber    , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfValueText    (string      valueText      , InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfValueText      (valueText      , aspect)); }
        public List<EndemeLarge> AtEndemeLargeOfEndemeProfile(int         endemeProfileId, InfoAspect aspect) { return AtEndemeLarge(InEndemeLargeOfEndemeProfile  (endemeProfileId, aspect)); }
        public Atom              AtomMaEndemeLargeOf         (int         endemesetId    , InfoAspect aspect) { return MainEndemeLargeOf(endemesetId, aspect).ToAtom(); }
        public void              ExEndemeLarge               (long        endemeId       , InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + ENDEME_LARGE_TABLE + " WHERE EndemeId = " + endemeId, aspect.PrimaryConn); }
        public RichDataTable     IntoEndemeLarge             (EndemeLarge endemeLarge    , InfoAspect aspect) { if (   IsEndemeLarge(endemeLarge.EndemeId, aspect)) return InEndemeLarge(endemeLarge.EndemeId, aspect); else { return InEndemeLarge(ToEndemeLarge(endemeLarge, aspect), aspect); } }
        public EndemeLarge       OnInEndemeLarge             (long        endemeId       , InfoAspect aspect) { return OnEndemeLarge(InEndemeLarge  (endemeId, aspect), 0, true); }
        public EndemeLarge       OnIntoEndemeLarge           (EndemeLarge endemeLarge    , InfoAspect aspect) { return OnEndemeLarge(IntoEndemeLarge(endemeLarge  , aspect), 0, true); }
        public int               UpToEndemeLarge             (EndemeLarge endemeLarge    , InfoAspect aspect) { if (   IsEndemeLarge(endemeLarge.EndemeId, aspect)) return UpEndemeLarge(endemeLarge        , aspect); else { return ToEndemeLarge(endemeLarge, aspect); } }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeLarge_test -->
        /// <summary>
        ///      Tests the ToEndemeLarge method
        /// </summary>
        public void ToEndemeLarge_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeLargeAccess";
            string adoMethod = "ToEndemeLarge";
            Random r = RandomSource.New().Random;
            long   endemeId = -1;
            EndemeAccess ende = new EndemeAccess();

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeLarge endemeLargeTo = ende.AnneEndemeLarge(r);
            try
            {
                endemeId = ende.ToEndemeLarge(endemeLargeTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeLarge endemeLargeFrom = ende.OnInEndemeLarge(endemeId, _aspect);
            ende.AssertEqualContent(endemeLargeFrom, endemeLargeTo, adoClass, adoMethod);


            ende.ExEndemeLarge(endemeId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeLarge_test -->
        /// <summary>
        ///      Tests the UpEndemeLarge method
        /// </summary>
        public void UpEndemeLarge_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeLargeAccess";
            string adoMethod         = "UpEndemeLarge";
            Random r                 = RandomSource.New().Random;
            EndemeLarge endemeLarge1 = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeId with a newly created endemeLarge
                // ----------------------------------------------------------------------
                long   endemeId = ende.AnIdOfEndemeLarge(_aspect);
                endemeLarge1               = ende.OnInEndemeLarge(endemeId, _aspect);
                EndemeLarge  endemeLarge2  = ende.AnneEndemeLarge(r);
                endemeLarge2.EndemeId = endemeLarge1.EndemeId;
                ende.UpEndemeLarge(endemeLarge2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeLarge endemeLarge3 = ende.OnInEndemeLarge(endemeId, _aspect);
                Assert.That(endemeLarge3.EndemeId, Is.equal_to, endemeLarge2.EndemeId, adoClass, adoMethod + " test update existing endemeLarge");
                ende.AssertEqualContent  (endemeLarge3, endemeLarge2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeLarge3, endemeLarge1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeLarge, did the update fail?
                // ----------------------------------------------------------------------
                EndemeLarge  endemeLarge4  = ende.AnneEndemeLarge(r);
                endemeLarge4.EndemeId      = ende.HiIdOfEndemeLarge(_aspect) + 1;
                int          count         = ende.UpEndemeLarge(endemeLarge4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeLarge");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeLarge(endemeLarge1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeLarge -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeLarge table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public int AnIdOfEndemeLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT EndemeId FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeLargeId = new RichDataTable(cmd, "EndemeLargeId", "EndemeId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeLargeId.Count > 0) return (int)endemeLargeId.ToList("EndemeId", -1)[r.Next(endemeLargeId.Count)];
                else return -1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeLarge -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeLarge object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeLarge</returns>
        private EndemeLarge AnneEndemeLarge(Random r)
        {
            EndemeLarge endemeLarge = new EndemeLarge
              //{ EndemeId        = go.HiIdOfEndemeId() + 1
                { EndemeSetId     = Guid.NewGuid()
                , EndemeString    = r.Next(10000).ToString()
                , EndemeCode      = r.Next(10000).ToString()
                , EndemeLabel     = r.Next(10000).ToString()
                , EndemeDescr     = r.Next(10000).ToString()
                , RawSource        = true
                , MultipleUse     = true
                , ValueBinary     = null
                , ValueDateTime   = DateTime.Now
                , ValueFloat      = r.Next(10000)
                , ValueNumber     = r.Next(10000)
                , ValueText       = r.Next(10000).ToString()
                , EndemeProfileId = r.Next(10000)
                };
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeLarge">endemeLarge being tested</param>
        /// <param name="tgt"      >endemeLarge being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeLarge endemeLarge, EndemeLarge tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeLarge.EndemeSetId    , Is.equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeLarge.EndemeString   , Is.equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeLarge.EndemeCode     , Is.equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeLarge.EndemeLabel    , Is.equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeLarge.EndemeDescr    , Is.equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
            Assert.That(endemeLarge.RawSource      , Is.equal_to, tgt.RawSource      , adoClass, adoMethod + " RawSource"      );
            Assert.That(endemeLarge.MultipleUse    , Is.equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeLarge.ValueBinary    , Is.equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeLarge.ValueDateTime  , Is.equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeLarge.ValueFloat     , Is.equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeLarge.ValueNumber    , Is.equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeLarge.ValueText      , Is.equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeLarge.EndemeProfileId, Is.equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeLarge">endemeLarge being tested</param>
        /// <param name="tgt"      >endemeLarge being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeLarge endemeLarge, EndemeLarge tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeLarge.EndemeSetId    , Is.not_equal_to, tgt.EndemeSetId    , adoClass, adoMethod + " EndemeSetId"    );
            Assert.That(endemeLarge.EndemeString   , Is.not_equal_to, tgt.EndemeString   , adoClass, adoMethod + " EndemeString"   );
            Assert.That(endemeLarge.EndemeCode     , Is.not_equal_to, tgt.EndemeCode     , adoClass, adoMethod + " EndemeCode"     );
            Assert.That(endemeLarge.EndemeLabel    , Is.not_equal_to, tgt.EndemeLabel    , adoClass, adoMethod + " EndemeLabel"    );
            Assert.That(endemeLarge.EndemeDescr    , Is.not_equal_to, tgt.EndemeDescr    , adoClass, adoMethod + " EndemeDescr"    );
          //Assert.That(endemeLarge.RawSource      , Is.not_equal_to, tgt.RawSource      , adoClass, adoMethod + " RawSource"      );
          //Assert.That(endemeLarge.MultipleUse    , Is.not_equal_to, tgt.MultipleUse    , adoClass, adoMethod + " MultipleUse"    );
            Assert.That(endemeLarge.ValueBinary    , Is.not_equal_to, tgt.ValueBinary    , adoClass, adoMethod + " ValueBinary"    );
            Assert.That(endemeLarge.ValueDateTime  , Is.not_equal_to, tgt.ValueDateTime  , adoClass, adoMethod + " ValueDateTime"  );
            Assert.That(endemeLarge.ValueFloat     , Is.not_equal_to, tgt.ValueFloat     , adoClass, adoMethod + " ValueFloat"     );
            Assert.That(endemeLarge.ValueNumber    , Is.not_equal_to, tgt.ValueNumber    , adoClass, adoMethod + " ValueNumber"    );
            Assert.That(endemeLarge.ValueText      , Is.not_equal_to, tgt.ValueText      , adoClass, adoMethod + " ValueText"      );
            Assert.That(endemeLarge.EndemeProfileId, Is.not_equal_to, tgt.EndemeProfileId, adoClass, adoMethod + " EndemeProfileId");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeLarge -->
        /// <summary>
        ///     Returns a list of EndemeLarge objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeLarge objects</returns>
        public List<EndemeLarge> AtEndemeLarge(RichDataTable table)
        {
            List<EndemeLarge> list = new List<EndemeLarge>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeLarge(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeLarge -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeLarge objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeLarge objects"></param>
        public List<EndemeLarge> AtOnMareEndemeLarge(RichDataTable table)
        {
            List<EndemeLarge> endemeLargeList = new List<EndemeLarge>(table.Count);
            Dictionary<long,EndemeLarge> found = new Dictionary<long,EndemeLarge>();


            for (int row = 0; row < table.Count; ++row)
            {
                long endemeId = table.IntValue(row, "EndemeId", -1);
                EndemeLarge endemeLarge = null;

                if (!found.ContainsKey(endemeId))
                {
                    endemeLarge = OnEndemeLarge(table, row);

                    endemeLarge.EndemeIndexList = new List<EndemeIndex>();
                    endemeLargeList.Add(endemeLarge);
                    found.Add(endemeId, endemeLarge);
                }
                else
                {
                    endemeLarge = found[endemeId];
                }

                endemeLarge.EndemeIndexList.Add(OnEndemeIndex(table, row));
            }

            return endemeLargeList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeLarge -->
        /// <summary>
        ///      Copies an endemeLarge
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to copy</param>
        public static EndemeLarge CpEndemeLarge(EndemeLarge endemeLarge)
        {
            EndemeLarge output = new EndemeLarge();

            output.EndemeId        = endemeLarge.EndemeId    ;
            output.EndemeSetId     = endemeLarge.EndemeSetId ;
            output.EndemeString    = endemeLarge.EndemeString;
            output.EndemeCode      = endemeLarge.EndemeCode  ;
            output.EndemeLabel     = endemeLarge.EndemeLabel ;
            output.EndemeDescr     = endemeLarge.EndemeDescr ;
            output.RawSource       = endemeLarge.RawSource   ;
            output.MultipleUse     = endemeLarge.MultipleUse ;
            output.ValueBinary     = endemeLarge.ValueBinary ;
            output.ValueDateTime   = endemeLarge.ValueDateTime;
            output.ValueFloat      = endemeLarge.ValueFloat  ;
            output.ValueNumber     = endemeLarge.ValueNumber ;
            output.ValueText       = endemeLarge.ValueText   ;
            output.EndemeProfileId = endemeLarge.EndemeProfileId;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeLargeCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeLarge table
        /// </summary>
        /// <returns>a count of rows in the EndemeLarge table</returns>
        public int EndemeLargeCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT COUNT(*) FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeLarge: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeLarge -->
        /// <summary>
        ///      Enables an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        public void DoEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + ENDEME_LARGE_TABLE + " SET N/A = 1 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeLarge: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeLarge -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemelarge table
        /// </summary>
        /// <returns>Maximum EndemeLarge.EndemeId</returns>
        public long HiIdOfEndemeLarge(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT MAX(EndemeId) AS EndemeId FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            long endemeId = cmd.ExecuteScalar(0);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("HiIdOfEndemeLarge: " + cmd.Errors);
            return endemeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLarge -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeLarge table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId"  , endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndeme -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeId column
        /// </summary>
        /// <param name="endemeId">value in EndemeId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeId</returns>
        public RichDataTable InEndemeLargeOfEndeme(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId" , endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueDateTime -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueDateTime column
        /// </summary>
        /// <param name="valueDateTime">value in ValueDateTime column</param>
        /// <returns>a table of rows related to the specifed value of ValueDateTime</returns>
        public RichDataTable InEndemeLargeOfValueDateTime(DateTime valueDateTime, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE ValueDateTime = @ValueDateTime"
                , Throws.Actions, "PR")
                ._AddParameter_date("@ValueDateTime" , valueDateTime, SqlDbType.DateTime);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf10EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueFloat -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueFloat column
        /// </summary>
        /// <param name="valueFloat">value in ValueFloat column</param>
        /// <returns>a table of rows related to the specifed value of ValueFloat</returns>
        public RichDataTable InEndemeLargeOfValueFloat(double valueFloat, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE ValueFloat = @ValueFloat"
                , Throws.Actions, "PR")
                ._AddParameter_null("@ValueFloat" , valueFloat);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf11EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueNumber -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueNumber column
        /// </summary>
        /// <param name="valueNumber">value in ValueNumber column</param>
        /// <returns>a table of rows related to the specifed value of ValueNumber</returns>
        public RichDataTable InEndemeLargeOfValueNumber(decimal valueNumber, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE ValueNumber = @ValueNumber"
                , Throws.Actions, "PR")
                ._AddParameter_null("@ValueNumber" , valueNumber);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf12EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueText -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueText column
        /// </summary>
        /// <param name="valueText">value in ValueText column</param>
        /// <returns>a table of rows related to the specifed value of ValueText</returns>
        public RichDataTable InEndemeLargeOfValueText(string valueText, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE ValueText = @ValueText"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@ValueText" , valueText);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf13EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeLargeOfEndemeProfile(int endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeProfileId" , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf14EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeLargeOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeString -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeString column
        /// </summary>
        /// <param name="endemeString">value in EndemeString column</param>
        /// <returns>a table of rows related to the specifed value of EndemeString</returns>
        public RichDataTable InEndemeLargeOfEndemeString(string endemeString, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeString = @EndemeString"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeString" , endemeString);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeCode column
        /// </summary>
        /// <param name="endemeCode">value in EndemeCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeCode</returns>
        public RichDataTable InEndemeLargeOfEndemeCode(string endemeCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeCode = @EndemeCode"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeCode" , endemeCode);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeLabel column
        /// </summary>
        /// <param name="endemeLabel">value in EndemeLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeLabel</returns>
        public RichDataTable InEndemeLargeOfEndemeLabel(string endemeLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeLabel = @EndemeLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeLabel" , endemeLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfEndemeDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the EndemeDescr column
        /// </summary>
        /// <param name="endemeDescr">value in EndemeDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeDescr</returns>
        public RichDataTable InEndemeLargeOfEndemeDescr(string endemeDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeDescr = @EndemeDescr"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeDescr" , endemeDescr);
            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfRawSource -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the RawSource column
        /// </summary>
        /// <param name="rawSource">value in RawSource column</param>
        /// <returns>a table of rows related to the specifed value of RawSource</returns>
        public RichDataTable InEndemeLargeOfRawSource(bool rawSource, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE RawSource = @RawSource"
                , Throws.Actions, "PR")
                ._AddParameter_null("@RawSource" , rawSource);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfMultipleUse -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the MultipleUse column
        /// </summary>
        /// <param name="multipleUse">value in MultipleUse column</param>
        /// <returns>a table of rows related to the specifed value of MultipleUse</returns>
        public RichDataTable InEndemeLargeOfMultipleUse(bool multipleUse, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE MultipleUse = @MultipleUse"
                , Throws.Actions, "PR")
                ._AddParameter("@MultipleUse" , multipleUse);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeLargeOfValueBinary -->
        /// <summary>
        ///      Returns the rows (In) the EndemeLarge table filtered by a value (Of) the ValueBinary column
        /// </summary>
        /// <param name="valueBinary">value in ValueBinary column</param>
        /// <returns>a table of rows related to the specifed value of ValueBinary</returns>
        public RichDataTable InEndemeLargeOfValueBinary(byte[] valueBinary, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE ValueBinary = @ValueBinary"
                , Throws.Actions, "PR")
                ._AddParameter_byte("@ValueBinary" , valueBinary);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf9EndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeLarge -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeLarge table
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "SELECT * FROM " + ENDEME_LARGE_TABLE + " WITH(NOLOCK) WHERE EndemeId = @EndemeId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeId", endemeId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeLarge with EndemeId " + endemeId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeLarge -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MainEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, l.*"
                + "\r\n" + " FROM                " + ENDEME_LARGE_TABLE + " AS l WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + ENDEME_SET_TABLE   + " AS s WITH(NOLOCK) ON s.EndemeSetId = l.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND l.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeLargeOf -->
        /// <summary>
        ///      Returns a endemeLarge list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MainEndemeLargeOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, l.*"
                + "\r\n" + " FROM                " + ENDEME_LARGE_TABLE + " AS l WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + ENDEME_SET_TABLE  +" AS s WITH(NOLOCK) ON s.EndemeSetId = l.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND l.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MainEndemeLargeOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MareEndemeLargeOf -->
        /// <summary>
        ///      Returns a endemeLarge list (Of) a p(a)rent endemeset with (re)trieved endemeLarge and endemeindex data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeLarge rows with their joined parent data</returns>
        public RichDataTable MareEndemeLargeOf(int endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT s.*, l.*, i.*"
                + "\r\n" + " FROM                " + ENDEME_LARGE_TABLE + " AS l WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + ENDEME_SET_TABLE  +" AS s WITH(NOLOCK) ON s.EndemeSetId = l.EndemeSetId"
                + "\r\n" + "     LEFT OUTER JOIN " + ENDEME_INDEX_TABLE + " AS i WITH(NOLOCK) ON i.EndemeId    = l.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND l.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("MareEndemeLargeOf: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeLarge -->
        /// <summary>
        ///      Creates a (Ne)w EndemeLarge (o)bject from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawSource"      ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeLarge object</returns>
        public EndemeLarge NeonEndemeLarge
            ( long     endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawSource
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            )
        {
            EndemeLarge endemeLarge = new EndemeLarge
                { EndemeId        = endemeId
                , EndemeSetId     = endemeSetId
                , EndemeString    = endemeString
                , EndemeCode      = endemeCode
                , EndemeLabel     = endemeLabel
                , EndemeDescr     = endemeDescr
                , RawSource       = rawSource
                , MultipleUse     = multipleUse
                , ValueBinary     = valueBinary
                , ValueDateTime   = valueDateTime
                , ValueFloat      = valueFloat
                , ValueNumber     = valueNumber
                , ValueText       = valueText
                , EndemeProfileId = endemeProfileId
                };
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeLarge -->
        /// <summary>
        ///      Converts a row in the EndemeLarge data table into a EndemeLarge (O)bject
        /// </summary>
        /// <param name="endemeLargeTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeLarge OnEndemeLarge(RichDataTable endemeLargeTable, int row, bool dispose = false)
        {
            EndemeLarge endemeLarge = new EndemeLarge();
            endemeLarge.EndemeId        = endemeLargeTable.IntValue (row, "EndemeId"       , -1          );
            endemeLarge.EndemeSetId     = endemeLargeTable.GuidValue(row, "EndemeSetId"    , Guid.Empty  );
            endemeLarge.EndemeString    = endemeLargeTable.StrValue (row, "EndemeString"   , ""          );
            endemeLarge.EndemeCode      = endemeLargeTable.StrValue (row, "EndemeCode"     , ""          );
            endemeLarge.EndemeLabel     = endemeLargeTable.StrValue (row, "EndemeLabel"    , ""          );
            endemeLarge.EndemeDescr     = endemeLargeTable.StrValue (row, "EndemeDescr"    , ""          );
            endemeLarge.RawSource       = endemeLargeTable.BoolValue(row, "RawSource"      , false       );
            endemeLarge.MultipleUse     = endemeLargeTable.BoolValue(row, "MultipleUse"    , false       );
            endemeLarge.ValueBinary     = endemeLargeTable.ByteValue(row, "ValueBinary"                  );
            endemeLarge.ValueDateTime   = endemeLargeTable.DateValue(row, "ValueDateTime"  , DateTime.Now);
            endemeLarge.ValueFloat      = endemeLargeTable.RealValue(row, "ValueFloat"     , -1.0        );
            endemeLarge.ValueNumber     = endemeLargeTable.DecValue (row, "ValueNumber"    , -1.0M       );
            endemeLarge.ValueText       = endemeLargeTable.StrValue (row, "ValueText"      , ""          );
            endemeLarge.EndemeProfileId = endemeLargeTable.IntValue (row, "EndemeProfileId", -1          );

            if (dispose) endemeLargeTable.Dispose();
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeLarge -->
        /// <summary>
        ///      Returns a table of a EndemeLarge (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <returns>a table of EndemeLarge rows with their joined details</returns>
        public RichDataTable ReinEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT l.*, i.*"
                + "\r\n" + " FROM                " + ENDEME_LARGE_TABLE + " AS l WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + ENDEME_INDEX_TABLE + " AS i WITH(NOLOCK) ON i.EndemeId = l.EndemeId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND l.EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            RichDataTable table = new RichDataTable(cmd, "EndemeLarge", "EndemeId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeLarge: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeLarge -->
        /// <summary>
        ///      Inserts an endemeLarge object in(To) the database
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to insert into database</param>
        /// <returns>the Id of the inserted EndemeLarge</returns>
        public int ToEndemeLarge(EndemeLarge endemeLarge, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " DECLARE @Table AS TABLE ( EndemeId BIGINT )"
                + "\r\n" + " INSERT INTO " + ENDEME_LARGE_TABLE
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeString"
                + "\r\n" + "        , EndemeCode"
                + "\r\n" + "        , EndemeLabel"
                + "\r\n" + "        , EndemeDescr"
                + "\r\n" + "        , RawSource"
                + "\r\n" + "        , MultipleUse"
                + "\r\n" + "        , ValueBinary"
                + "\r\n" + "        , ValueDateTime"
                + "\r\n" + "        , ValueFloat"
                + "\r\n" + "        , ValueNumber"
                + "\r\n" + "        , ValueText"
                + "\r\n" + "        , EndemeProfileId"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeId INTO @Table"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeString"
                + "\r\n" + "        , @EndemeCode"
                + "\r\n" + "        , @EndemeLabel"
                + "\r\n" + "        , @EndemeDescr"
                + "\r\n" + "        , @RawSource"
                + "\r\n" + "        , @MultipleUse"
                + "\r\n" + "        , @ValueBinary"
                + "\r\n" + "        , @ValueDateTime"
                + "\r\n" + "        , @ValueFloat"
                + "\r\n" + "        , @ValueNumber"
                + "\r\n" + "        , @ValueText"
                + "\r\n" + "        , @EndemeProfileId"
                + "\r\n" + "        )"
                + "\r\n" + " SELECT EndemeId FROM @Table"
                , Throws.Actions, "PR")
              //._AddParameter     ("@EndemeId"       , endemeLarge.EndemeId                           )
                ._AddParameter_null("@EndemeSetId"    , endemeLarge.EndemeSetId                        )
                ._AddParameter_safe("@EndemeString"   , endemeLarge.EndemeString   ,   24)
                ._AddParameter_safe("@EndemeCode"     , endemeLarge.EndemeCode     ,    8)
                ._AddParameter_safe("@EndemeLabel"    , endemeLarge.EndemeLabel    ,   64)
                ._AddParameter_safe("@EndemeDescr"    , endemeLarge.EndemeDescr    ,   -1)
                ._AddParameter_null("@RawSource"      , endemeLarge.RawSource                          )
                ._AddParameter     ("@MultipleUse"    , endemeLarge.MultipleUse                        )
                ._AddParameter_byte("@ValueBinary"    , endemeLarge.ValueBinary                        )
                ._AddParameter_date("@ValueDateTime"  , endemeLarge.ValueDateTime  , SqlDbType.DateTime)
                ._AddParameter_null("@ValueFloat"     , endemeLarge.ValueFloat                         )
                ._AddParameter_null("@ValueNumber"    , endemeLarge.ValueNumber                        )
                ._AddParameter_safe("@ValueText"      , endemeLarge.ValueText      ,   -1)
                ._AddParameter_null("@EndemeProfileId", endemeLarge.EndemeProfileId                    );


            int endemeLargeId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeLarge: " + cmd.Errors);
            return endemeLargeId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeLarge -->
        /// <summary>
        ///     Inserts in(To) the EndemeLarge table a (ne)w endemeLarge built from member values
        /// </summary>
        /// <param name="endemeId"       ></param>
        /// <param name="endemeSetId"    ></param>
        /// <param name="endemeString"   ></param>
        /// <param name="endemeCode"     ></param>
        /// <param name="endemeLabel"    ></param>
        /// <param name="endemeDescr"    ></param>
        /// <param name="rawSource"      ></param>
        /// <param name="multipleUse"    ></param>
        /// <param name="valueBinary"    ></param>
        /// <param name="valueDateTime"  ></param>
        /// <param name="valueFloat"     ></param>
        /// <param name="valueNumber"    ></param>
        /// <param name="valueText"      ></param>
        /// <param name="endemeProfileId"></param>
        /// <returns>the new EndemeLarge object</returns>
        public EndemeLarge ToneEndemeLarge
            ( long     endemeId
            , Guid     endemeSetId
            , string   endemeString
            , string   endemeCode
            , string   endemeLabel
            , string   endemeDescr
            , bool     rawSource
            , bool     multipleUse
            , byte[]   valueBinary
            , DateTime valueDateTime
            , double   valueFloat
            , decimal  valueNumber
            , string   valueText
            , int      endemeProfileId
            , InfoAspect aspect)
        {
            EndemeLarge endemeLarge = NeonEndemeLarge
                ( endemeId
                , endemeSetId
                , endemeString
                , endemeCode
                , endemeLabel
                , endemeDescr
                , rawSource
                , multipleUse
                , valueBinary
                , valueDateTime
                , valueFloat
                , valueNumber
                , valueText
                , endemeProfileId
                );
            endemeLarge.EndemeId = ToEndemeLarge(endemeLarge, aspect);
            return endemeLarge;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeLarge -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeLarge
        /// </summary>
        /// <param name="endemeId">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public void UnEndemeLarge(long endemeId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + ENDEME_LARGE_TABLE + " SET N/A = 0 WHERE EndemeId = @EndemeId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeId", endemeId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeLarge: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeLarge -->
        /// <summary>
        ///      (Up)dates a row in the EndemeLarge table from a EndemeLarge object
        /// </summary>
        /// <param name="endemeLarge">endemeLarge to update</param>
        /// <returns>the count of the updated endemeLarge rows"></param>
        public int UpEndemeLarge(EndemeLarge endemeLarge, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeId = " + endemeLarge.EndemeId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(ENDEME_LARGE_TABLE, whereClause, aspect.PrimaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                            , "\r\n" + " UPDATE " + ENDEME_LARGE_TABLE
                            + "\r\n" + " SET   EndemeSetId     = @EndemeSetId"
                            + "\r\n" + "     , EndemeString    = @EndemeString"
                            + "\r\n" + "     , EndemeCode      = @EndemeCode"
                            + "\r\n" + "     , EndemeLabel     = @EndemeLabel"
                            + "\r\n" + "     , EndemeDescr     = @EndemeDescr"
                            + "\r\n" + "     , RawSource       = @RawSource"
                            + "\r\n" + "     , MultipleUse     = @MultipleUse"
                            + "\r\n" + "     , ValueBinary     = @ValueBinary"
                            + "\r\n" + "     , ValueDateTime   = @ValueDateTime"
                            + "\r\n" + "     , ValueFloat      = @ValueFloat"
                            + "\r\n" + "     , ValueNumber     = @ValueNumber"
                            + "\r\n" + "     , ValueText       = @ValueText"
                            + "\r\n" + "     , EndemeProfileId = @EndemeProfileId"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeId"       , endemeLarge.EndemeId                           )
                            ._AddParameter_null("@EndemeSetId"    , endemeLarge.EndemeSetId                        )
                            ._AddParameter_safe("@EndemeString"   , endemeLarge.EndemeString   ,   24)
                            ._AddParameter_safe("@EndemeCode"     , endemeLarge.EndemeCode     ,    8)
                            ._AddParameter_safe("@EndemeLabel"    , endemeLarge.EndemeLabel    ,   64)
                            ._AddParameter_safe("@EndemeDescr"    , endemeLarge.EndemeDescr    ,   -1)
                            ._AddParameter_null("@RawSource"      , endemeLarge.RawSource                          )
                            ._AddParameter     ("@MultipleUse"    , endemeLarge.MultipleUse                        )
                            ._AddParameter_byte("@ValueBinary"    , endemeLarge.ValueBinary                        )
                            ._AddParameter_date("@ValueDateTime"  , endemeLarge.ValueDateTime  , SqlDbType.DateTime)
                            ._AddParameter_null("@ValueFloat"     , endemeLarge.ValueFloat                         )
                            ._AddParameter_null("@ValueNumber"    , endemeLarge.ValueNumber                        )
                            ._AddParameter_safe("@ValueText"      , endemeLarge.ValueText      ,   -1)
                            ._AddParameter_null("@EndemeProfileId", endemeLarge.EndemeProfileId                    )
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeLarge: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeLargeId " + endemeLarge.EndemeId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeLarge table
        */

        #region EndemeSet table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeSet table methods
        // ----------------------------------------------------------------------------------------// toreendemeset
        public List<EndemeSet> AtEndemeSetOfEndemeSet        (Guid   endemeSetId      , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSet        (endemeSetId      , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetCode    (string endemeSetCode    , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetCode    (endemeSetCode    , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetLabel   (string endemeSetLabel   , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetLabel   (endemeSetLabel   , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetDescr   (string endemeSetDescr   , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetDescr   (endemeSetDescr   , aspect)); }
        public List<EndemeSet> AtEndemeSetOfDefaultEndeme    (string defaultEndeme    , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfDefaultEndeme    (defaultEndeme    , aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetResource(string endemeSetResource, InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetResource(endemeSetResource, aspect)); }
        public List<EndemeSet> AtEndemeSetOfEndemeSetVersion (string endemeSetVersion , InfoAspect aspect) { return AtEndemeSet(InEndemeSetOfEndemeSetVersion (endemeSetVersion , aspect)); }
        public void            ExEndemeSet    (Guid      endemeSetId, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + LaEndemeSet + " WHERE EndemeSetId = '" + endemeSetId + "'", aspect.SecondaryConn); }
        public RichDataTable   IntoEndemeSet  (EndemeSet endemeSet  , InfoAspect aspect) { if   (!IsEndemeSet(endemeSet.EndemeSetId, aspect)) ToEndemeSet(endemeSet, aspect); return InEndemeSet(endemeSet.SetId, aspect); }
        public EndemeSet       OnInEndemeSet  (Guid      endemeSetId, InfoAspect aspect) { return OnEndemeSet(InEndemeSet  (endemeSetId, aspect), 0, true); }
        public EndemeSet       OnIntoEndemeSet(EndemeSet endemeSet  , InfoAspect aspect) { return OnEndemeSet(IntoEndemeSet(endemeSet  , aspect), 0, true); }
        public Guid            UpToEndemeSet  (EndemeSet endemeSet  , InfoAspect aspect) { if   ( IsEndemeSet(endemeSet.EndemeSetId, aspect)) UpEndemeSet(endemeSet, aspect); else ToEndemeSet(endemeSet, aspect); return endemeSet.SetId; }

        //  A breakpoint for when the endeme set table gets accessed
        public static void AccessEndemeSet()
        {
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet_test -->
        /// <summary>
        ///      Tests the ToEndemeSet method
        /// </summary>
        public void ToEndemeSet_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeSetAccess";
            string adoMethod = "ToEndemeSet";
            Random r = RandomSource.New().Random;
            Guid   endemeSetId = Guid.Empty;
            EndemeAccess ende = new EndemeAccess();

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeSet endemeSetTo = ende.AnneEndemeSet(r);
            endemeSetId = endemeSetTo.SetId;
            try
            {
                ende.ToEndemeSet(endemeSetTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeSetId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeSet endemeSetFrom = ende.OnInEndemeSet(endemeSetId, _aspect);
            ende.AssertEqualContent(endemeSetFrom, endemeSetTo, adoClass, adoMethod);


            ende.ExEndemeSet(endemeSetId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeSet_test -->
        /// <summary>
        ///      Tests the UpEndemeSet method
        /// </summary>
        public void UpEndemeSet_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass      = "EndemeSetAccess";
            string adoMethod     = "UpEndemeSet";
            Random r               = RandomSource.New().Random;
            EndemeSet endemeSet1 = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeSetId with a newly created endemeSet
                // ----------------------------------------------------------------------
                Guid    endemeSetId = ende.AnIdOfEndemeSet(_aspect);
                endemeSet1             = ende.OnInEndemeSet(endemeSetId, _aspect);
                EndemeSet  endemeSet2  = ende.AnneEndemeSet(r);
                endemeSet2.EndemeSetId = endemeSet1.EndemeSetId;
                ende.UpEndemeSet(endemeSet2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeSet endemeSet3 = ende.OnInEndemeSet(endemeSetId, _aspect);
                Assert.That(endemeSet3.EndemeSetId, Is.equal_to, endemeSet2.EndemeSetId, adoClass, adoMethod + " test update existing endemeSet");
                ende.AssertEqualContent  (endemeSet3, endemeSet2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeSet3, endemeSet1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeSet, did the update fail?
                // ----------------------------------------------------------------------
                EndemeSet  endemeSet4  = ende.AnneEndemeSet(r);
                endemeSet4.EndemeSetId = Guid.NewGuid();
                int        count       = ende.UpEndemeSet(endemeSet4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeSet");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeSet(endemeSet1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeSet -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeSet table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public Guid AnIdOfEndemeSet(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT EndemeSetId FROM " + LaEndemeSet + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeSetId = new RichDataTable(cmd, "EndemeSetId", "EndemeSetId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeSetId.Count > 0) return (Guid)endemeSetId.ToList("EndemeSetId", Guid.NewGuid())[r.Next(endemeSetId.Count)];
                else return Guid.NewGuid();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeSet -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeSet object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeSet</returns>
        private EndemeSet AnneEndemeSet(Random r)
        {
            EndemeSet endemeSet = new EndemeSet
                { EndemeSetId       = Guid.NewGuid()
                , EndemeSetCode     = r.Next(10000).ToString()
                , EndemeSetLabel    = r.Next(10000).ToString()
                , EndemeSetDescr    = r.Next(10000).ToString()
                , DefaultEndeme     = r.Next(10000).ToString()
                , EndemeSetResource = r.Next(10000).ToString()
                , EndemeSetVersion  = r.Next(10000).ToString()
                };
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeSet">endemeSet being tested</param>
        /// <param name="tgt"      >endemeSet being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeSet endemeSet, EndemeSet tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeSet.EndemeSetCode    , Is.equal_to, tgt.EndemeSetCode    , adoClass, adoMethod + " EndemeSetCode"    );
            Assert.That(endemeSet.EndemeSetLabel   , Is.equal_to, tgt.EndemeSetLabel   , adoClass, adoMethod + " EndemeSetLabel"   );
            Assert.That(endemeSet.EndemeSetDescr   , Is.equal_to, tgt.EndemeSetDescr   , adoClass, adoMethod + " EndemeSetDescr"   );
            Assert.That(endemeSet.DefaultEndeme    , Is.equal_to, tgt.DefaultEndeme    , adoClass, adoMethod + " DefaultEndeme"    );
            Assert.That(endemeSet.EndemeSetResource, Is.equal_to, tgt.EndemeSetResource, adoClass, adoMethod + " EndemeSetResource");
            Assert.That(endemeSet.EndemeSetVersion , Is.equal_to, tgt.EndemeSetVersion , adoClass, adoMethod + " EndemeSetVersion" );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeSet">endemeSet being tested</param>
        /// <param name="tgt"      >endemeSet being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeSet endemeSet, EndemeSet tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeSet.EndemeSetCode    , Is.not_equal_to, tgt.EndemeSetCode    , adoClass, adoMethod + " EndemeSetCode"    );
            Assert.That(endemeSet.EndemeSetLabel   , Is.not_equal_to, tgt.EndemeSetLabel   , adoClass, adoMethod + " EndemeSetLabel"   );
            Assert.That(endemeSet.EndemeSetDescr   , Is.not_equal_to, tgt.EndemeSetDescr   , adoClass, adoMethod + " EndemeSetDescr"   );
            Assert.That(endemeSet.DefaultEndeme    , Is.not_equal_to, tgt.DefaultEndeme    , adoClass, adoMethod + " DefaultEndeme"    );
            Assert.That(endemeSet.EndemeSetResource, Is.not_equal_to, tgt.EndemeSetResource, adoClass, adoMethod + " EndemeSetResource");
            Assert.That(endemeSet.EndemeSetVersion , Is.not_equal_to, tgt.EndemeSetVersion , adoClass, adoMethod + " EndemeSetVersion" );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeSet -->
        /// <summary>
        ///     Returns a list of EndemeSet objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeSet objects</returns>
        public List<EndemeSet> AtEndemeSet(RichDataTable table)
        {
            List<EndemeSet> list = new List<EndemeSet>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeSet(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeSet -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeSet objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeSet objects"></param>
        public List<EndemeSet> AtOnMareEndemeSet(RichDataTable table)
        {
            List<EndemeSet> endemeSetList = new List<EndemeSet>(table.Count);
            Dictionary<int,EndemeSet> found = new Dictionary<int,EndemeSet>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeSetId = table.IntValue(row, "EndemeSetId", -1);
                EndemeSet endemeSet = null;

                if (!found.ContainsKey(endemeSetId))
                {
                    endemeSet = OnEndemeSet(table, row);

                    endemeSet.EndemeCharacteristicList = new List<EndemeCharacteristic>();
                    endemeSetList.Add(endemeSet);
                    found.Add(endemeSetId, endemeSet);
                }
                else
                {
                    endemeSet = found[endemeSetId];
                }

                endemeSet.EndemeCharacteristicList.Add(OnEndemeCharacteristic(table, row));
            }

            return endemeSetList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtomoidEndemeSet -->
        /// <summary>
        ///      Returns a list of all endeme sets as a list of atomic string tuples (Carbon)
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns>all endeme set labels and ids in label and reverse version order</returns>
        public Carbon AtomoidEndemeSet(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT EndemeSetId, EndemeSetLabel FROM EndemeSet ORDER BY EndemeSetLabel, EndemeSetVersion DESC"
                , Throws.Actions, "P");
            RichDataTable table = new RichDataTable(cmd, "EndemeSetTuple", "EndemeSetId");
            return table.ToCarbon();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeSet -->
        /// <summary>
        ///      Copies an endemeSet
        /// </summary>
        /// <param name="endemeSet">endemeSet to copy</param>
        public static EndemeSet CpEndemeSet(EndemeSet endemeSet)
        {
            EndemeSet output = new EndemeSet();

            output.EndemeSetId       = endemeSet.EndemeSetId;
            output.EndemeSetCode     = endemeSet.EndemeSetCode;
            output.EndemeSetLabel    = endemeSet.EndemeSetLabel;
            output.EndemeSetDescr    = endemeSet.EndemeSetDescr;
            output.DefaultEndeme     = endemeSet.DefaultEndeme;
            output.EndemeSetResource = endemeSet.EndemeSetResource;
            output.EndemeSetVersion  = endemeSet.EndemeSetVersion;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeSetCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeSet table
        /// </summary>
        /// <returns>a count of rows in the EndemeSet table</returns>
        public int EndemeSetCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT COUNT(*) FROM " + LaEndemeSet + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeSet: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeSet -->
        /// <summary>
        ///      Enables an EndemeSet
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        public void DoEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE " + LaEndemeSet + " SET N/A = 1 WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeSet: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSet -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeSet table
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId"  , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeSetOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetCode -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetCode column
        /// </summary>
        /// <param name="endemeSetCode">value in EndemeSetCode column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetCode</returns>
        public RichDataTable InEndemeSetOfEndemeSetCode(string endemeSetCode, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetCode = @EndemeSetCode"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetCode" , endemeSetCode);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetLabel column
        /// </summary>
        /// <param name="endemeSetLabel">value in EndemeSetLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetLabel</returns>
        public RichDataTable InEndemeSetOfEndemeSetLabel(string endemeSetLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetLabel = @EndemeSetLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetLabel" , endemeSetLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetLabel -->
        /// <summary>
        ///      Returns the first endeme set Id from the EndemeSet table filtered by a value (Of) the EndemeSetLabel column
        /// </summary>
        /// <param name="endemeSetLabel">value in EndemeSetLabel column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetLabel</returns>
        public Guid InEndemeSetIdOfEndemeSetLabel(string endemeSetLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT Top 1 EndemeSetId FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetLabel = @EndemeSetLabel ORDER BY EndemeSetVersion DESC"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetLabel" , endemeSetLabel);
            Guid enSetId = cmd.ExecuteScalar(Guid.Empty);
            return enSetId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetDescr -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetDescr column
        /// </summary>
        /// <param name="endemeSetDescr">value in EndemeSetDescr column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetDescr</returns>
        public RichDataTable InEndemeSetOfEndemeSetDescr(string endemeSetDescr, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetDescr = @EndemeSetDescr"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetDescr" , endemeSetDescr);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfDefaultEndeme -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the DefaultEndeme column
        /// </summary>
        /// <param name="defaultEndeme">value in DefaultEndeme column</param>
        /// <returns>a table of rows related to the specifed value of DefaultEndeme</returns>
        public RichDataTable InEndemeSetOfDefaultEndeme(string defaultEndeme, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE DefaultEndeme = @DefaultEndeme"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@DefaultEndeme" , defaultEndeme);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetResource -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetResource column
        /// </summary>
        /// <param name="endemeSetResource">value in EndemeSetResource column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetResource</returns>
        public RichDataTable InEndemeSetOfEndemeSetResource(string endemeSetResource, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetResource = @EndemeSetResource"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetResource" , endemeSetResource);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeSetOfEndemeSetVersion -->
        /// <summary>
        ///      Returns the rows (In) the EndemeSet table filtered by a value (Of) the EndemeSetVersion column
        /// </summary>
        /// <param name="endemeSetVersion">value in EndemeSetVersion column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetVersion</returns>
        public RichDataTable InEndemeSetOfEndemeSetVersion(string endemeSetVersion, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetVersion = @EndemeSetVersion"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetVersion" , endemeSetVersion);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeSet -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeSet table
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                    , "SELECT * FROM " + LaEndemeSet + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeSetId", endemeSetId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeSet with EndemeSetId " + endemeSetId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeSet -->
        /// <summary>
        ///      Creates a (Ne)w EndemeSet (o)bject from member values
        /// </summary>
        /// <param name="endemeSetId"      ></param>
        /// <param name="endemeSetCode"    ></param>
        /// <param name="endemeSetLabel"   ></param>
        /// <param name="endemeSetDescr"   ></param>
        /// <param name="defaultEndeme"    ></param>
        /// <param name="endemeSetResource"></param>
        /// <param name="endemeSetVersion" ></param>
        /// <returns>the new EndemeSet object</returns>
        public EndemeSet NeonEndemeSet
            ( Guid     endemeSetId
            , string   endemeSetCode
            , string   endemeSetLabel
            , string   endemeSetDescr
            , string   defaultEndeme
            , string   endemeSetResource
            , string   endemeSetVersion
            )
        {
            EndemeSet endemeSet = new EndemeSet
                { EndemeSetId       = endemeSetId
                , EndemeSetCode     = endemeSetCode
                , EndemeSetLabel    = endemeSetLabel
                , EndemeSetDescr    = endemeSetDescr
                , DefaultEndeme     = defaultEndeme
                , EndemeSetResource = endemeSetResource
                , EndemeSetVersion  = endemeSetVersion
                };
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeSet -->
        /// <summary>
        ///      Converts a row in the EndemeSet data table into a EndemeSet (O)bject
        /// </summary>
        /// <param name="table"  >a table containing columns to build a simple endeme set object (header)</param>
        /// <param name="row"    >the row to convert</param>
        /// <param name="dispose">whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeSet OnEndemeSet(RichDataTable table, int row, bool dispose = false)
        {
            EndemeSet enSet = new EndemeSet();
            enSet.EndemeSetId       = table.GuidValue(row, "EndemeSetId"      , Guid.Empty);
            enSet.EndemeSetCode     = table.StrValue (row, "EndemeSetCode"    , ""        );
            enSet.EndemeSetLabel    = table.StrValue (row, "EndemeSetLabel"   , ""        );
            enSet.EndemeSetDescr    = table.StrValue (row, "EndemeSetDescr"   , ""        );
            enSet.DefaultEndeme     = table.StrValue (row, "DefaultEndeme"    , ""        );
            enSet.EndemeSetResource = table.StrValue (row, "EndemeSetResource", ""        );
            enSet.EndemeSetVersion  = table.StrValue (row, "EndemeSetVersion" , ""        );

            if (dispose) table.Dispose();
            return enSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReonEndemeSet -->
        /// <summary>
        ///      Converts a list of rows from the EndemeSet and Characteristic tables into an EndemeSet (O)bject
        /// </summary>
        /// <param name="from"   >a table containing rows and columns to build a full endeme set object</param>
        /// <param name="anyRow" >the row to convert to build the header</param>
        /// <param name="dispose">whether this method should dispose the table, default: false</param>
        /// <returns>an object built fromt he rows in the table</returns>
        public EndemeSet ReonEndemeSet(RichDataTable from, int anyRow, bool dispose = false)
        {
            EndemeSet enSet = OnEndemeSet(from, anyRow, false);
            for (int characteristicRow = 0; characteristicRow < from.Count; ++characteristicRow)
                { enSet.Add(OnEndemeCharacteristic(from, characteristicRow)); }
            if (dispose) from.Dispose();
            return enSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeSet -->
        /// <summary>
        ///      Returns a table of a EndemeSet (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>a table of EndemeSet rows with their joined details</returns>
        public RichDataTable ReinEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, c.*"
                + "\r\n" + " FROM                " + EndemeAccess.LaEndemeSet  + " AS s WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + EndemeAccess.LaEndemeChar + " AS c WITH(NOLOCK) ON c.EndemeSetId = s.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND s.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeSetOfEndemeSetLabel -->
        /// <summary>
        ///      Returns a table of a EndemeSet (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <returns>a table of EndemeSet rows with their joined details</returns>
        public RichDataTable ReinEndemeSetOfEndemeSetLabel(string label, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.EndemeSetId  , c.EndemeCharLetter"
                + "\r\n" + "     , s.EndemeSetCode , c.EndemeCharCode"
                + "\r\n" + "     , s.EndemeSetLabel, c.EndemeCharLabel"
                + "\r\n" + "     , s.EndemeSetDescr, c.EndemeCharDescr, s.EndemeSetResource, s.EndemeSetVersion"
                + "\r\n" + "     , s.DefaultEndeme , c.EndemeCharIsASet"
                + "\r\n" + " FROM                " + EndemeAccess.LaEndemeSet  + " AS s WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + EndemeAccess.LaEndemeChar + " AS c WITH(NOLOCK) ON c.EndemeSetId = s.EndemeSetId"
                + "\r\n" + " WHERE s.EndemeSetLabel = @EndemeSetLabel"
                + "\r\n" + " ORDER BY c.EndemeCharLetter"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@EndemeSetLabel", label);


            RichDataTable table = new RichDataTable(cmd, "EndemeSet", "EndemeSetId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeSet: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeSet -->
        /// <summary>
        ///      Inserts an endemeSet object in(To) the database
        /// </summary>
        /// <param name="enSet">endemeSet to insert into database</param>
        /// <returns>the Id of the inserted EndemeSet</returns>
        public Guid ToEndemeSet(EndemeSet enSet, InfoAspect aspect)
        {
            enSet.EndemeSetId = Guid.NewGuid();

            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " INSERT INTO " + LaEndemeSet
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , EndemeSetCode"
                + "\r\n" + "        , EndemeSetLabel"
                + "\r\n" + "        , EndemeSetDescr"
                + "\r\n" + "        , DefaultEndeme"
                + "\r\n" + "        , EndemeSetResource"
                + "\r\n" + "        , EndemeSetVersion"
                + "\r\n" + "        )"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @EndemeSetCode"
                + "\r\n" + "        , @EndemeSetLabel"
                + "\r\n" + "        , @EndemeSetDescr"
                + "\r\n" + "        , @DefaultEndeme"
                + "\r\n" + "        , @EndemeSetResource"
                + "\r\n" + "        , @EndemeSetVersion"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeSetId"      , enSet.EndemeSetId            )
                ._AddParameter_safe("@EndemeSetCode"    , enSet.EndemeSetCode    ,    8)
                ._AddParameter_safe("@EndemeSetLabel"   , enSet.EndemeSetLabel   ,  128)
                ._AddParameter_safe("@EndemeSetDescr"   , enSet.EndemeSetDescr   ,   -1)
                ._AddParameter_safe("@DefaultEndeme"    , enSet.DefaultEndeme    ,   24)
                ._AddParameter_safe("@EndemeSetResource", enSet.EndemeSetResource,  128)
                ._AddParameter_safe("@EndemeSetVersion" , enSet.EndemeSetVersion ,   32);


            int count = cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeSet: " + cmd.Errors);
            return enSet.EndemeSetId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeSet -->
        /// <summary>
        ///     Inserts in(To) the EndemeSet table a (ne)w endemeSet built from member values
        /// </summary>
        /// <param name="endemeSetId"      ></param>
        /// <param name="endemeSetCode"    ></param>
        /// <param name="endemeSetLabel"   ></param>
        /// <param name="endemeSetDescr"   ></param>
        /// <param name="defaultEndeme"    ></param>
        /// <param name="endemeSetResource"></param>
        /// <param name="endemeSetVersion" ></param>
        /// <returns>the new EndemeSet object</returns>
        public EndemeSet ToneEndemeSet
            ( Guid     endemeSetId
            , string   endemeSetCode
            , string   endemeSetLabel
            , string   endemeSetDescr
            , string   defaultEndeme
            , string   endemeSetResource
            , string   endemeSetVersion
            , InfoAspect aspect)
        {
            EndemeSet endemeSet = NeonEndemeSet
                ( endemeSetId
                , endemeSetCode
                , endemeSetLabel
                , endemeSetDescr
                , defaultEndeme
                , endemeSetResource
                , endemeSetVersion
                );
            ToEndemeSet(endemeSet, aspect);
            return endemeSet;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeSet -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeSet
        /// </summary>
        /// <param name="endemeSetId">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public void UnEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE " + LaEndemeSet + " SET N/A = 0 WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeSet: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeSet -->
        /// <summary>
        ///      (Up)dates a row in the EndemeSet table from a EndemeSet object
        /// </summary>
        /// <param name="endemeSet">endemeSet to update</param>
        /// <returns>the count of the updated endemeSet rows"></param>
        public int UpEndemeSet(EndemeSet endemeSet, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeSetId = " + endemeSet.EndemeSetId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(LaEndemeSet, whereClause, aspect.SecondaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                            , "\r\n" + " UPDATE " + LaEndemeSet
                            + "\r\n" + " SET   EndemeSetCode     = @EndemeSetCode"
                            + "\r\n" + "     , EndemeSetLabel    = @EndemeSetLabel"
                            + "\r\n" + "     , EndemeSetDescr    = @EndemeSetDescr"
                            + "\r\n" + "     , DefaultEndeme     = @DefaultEndeme"
                            + "\r\n" + "     , EndemeSetResource = @EndemeSetResource"
                            + "\r\n" + "     , EndemeSetVersion  = @EndemeSetVersion"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeSetId"      , endemeSet.EndemeSetId           )
                            ._AddParameter_safe("@EndemeSetCode"    , endemeSet.EndemeSetCode    ,   8)
                            ._AddParameter_safe("@EndemeSetLabel"   , endemeSet.EndemeSetLabel   , 128)
                            ._AddParameter_safe("@EndemeSetDescr"   , endemeSet.EndemeSetDescr   ,  -1)
                            ._AddParameter_safe("@DefaultEndeme"    , endemeSet.DefaultEndeme    ,  24)
                            ._AddParameter_safe("@EndemeSetResource", endemeSet.EndemeSetResource, 128)
                            ._AddParameter_safe("@EndemeSetVersion" , endemeSet.EndemeSetVersion ,  32)
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeSet: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeSetId " + endemeSet.EndemeSetId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeSet table

        #region EndemeProfile table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeProfile table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeProfileTable> AtEndemeProfileTableOfEndemeProfile   (Guid     endemeProfileId , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfEndemeProfile   (endemeProfileId , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataLabel       (string   dataLabel       , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataLabel       (dataLabel       , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataValueString (string   dataValueString , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataValueString (dataValueString , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataTableName   (string   dataTableName   , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataTableName   (dataTableName   , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataRow         (int      dataRowId       , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataRow         (dataRowId       , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfDataColumnName  (string   dataColumnName  , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfDataColumnName  (dataColumnName  , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfProfileMatch    (double   profileMatch    , InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfProfileMatch    (profileMatch    , aspect)); }
        public List<EndemeProfileTable> AtEndemeProfileTableOfProfileTbdColumn(string   profileTbdColumn, InfoAspect aspect) { return AtEndemeProfileTable(InEndemeProfileTableOfProfileTbdColumn(profileTbdColumn, aspect)); }
        public void               ExEndemeProfileTable    (Guid               endemeProfileId, InfoAspect aspect) { int count = InData.DeleteUpTo(1, "FROM " + LaEndemeProfile + " WHERE EndemeProfileId = '" + endemeProfileId + "'", aspect.PrimaryConn); }
        public RichDataTable      IntoEndemeProfileTable  (EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { if (!IsEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect)) ToEndemeProfileTable(endemeProfileTable, aspect); return InEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect); }
        public EndemeProfileTable OnInEndemeProfileTable  (Guid               endemeProfileId, InfoAspect aspect) { return OnEndemeProfileTable(InEndemeProfileTable  (endemeProfileId, aspect), 0, true); }
        public EndemeProfileTable OnIntoEndemeProfileTable(EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { return OnEndemeProfileTable(IntoEndemeProfileTable(endemeProfileTable  , aspect), 0, true); }
        public Guid               UpToEndemeProfileTable  (EndemeProfileTable endemeProfileTable  , InfoAspect aspect) { if (IsEndemeProfileTable(endemeProfileTable.EndemeProfileId, aspect)) UpEndemeProfileTable(endemeProfileTable, aspect); else ToEndemeProfileTable(endemeProfileTable, aspect); return endemeProfileTable.EndemeProfileId; }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeProfileTable_test -->
        /// <summary>
        ///      Tests the ToEndemeProfileTable method
        /// </summary>
        public void ToEndemeProfileTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeProfileTableAccess";
            string adoMethod = "ToEndemeProfileTable";
            Random r = RandomSource.New().Random;
            Guid    endemeProfileId = Guid.Empty;
            EndemeAccess ende = new EndemeAccess();

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeProfileTable endemeProfileTableTo = ende.AnneEndemeProfileTable(r);
            endemeProfileId = endemeProfileTableTo.EndemeProfileId;
            try
            {
                ende.ToEndemeProfileTable(endemeProfileTableTo, _aspect);     // <-- This is what is being tested
                if (__.StringHasContent(ende.Errors)) Assert.Crash(ende.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeProfileId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeProfileTable endemeProfileTableFrom = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
            ende.AssertEqualContent(endemeProfileTableFrom, endemeProfileTableTo, adoClass, adoMethod);


            ende.ExEndemeProfileTable(endemeProfileId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeProfileTable_test -->
        /// <summary>
        ///      Tests the UpEndemeProfileTable method
        /// </summary>
        public void UpEndemeProfileTable_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass                        = "EndemeProfileTableAccess";
            string adoMethod                       = "UpEndemeProfileTable";
            Random r                        = RandomSource.New().Random;
            EndemeProfileTable endemeProfileTable1 = null;

            EndemeAccess ende = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeProfileId with a newly created endemeProfileTable
                // ----------------------------------------------------------------------
                Guid    endemeProfileId = ende.AnIdOfEndemeProfileTable(_aspect);
                endemeProfileTable1                      = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
                EndemeProfileTable  endemeProfileTable2  = ende.AnneEndemeProfileTable(r);
                endemeProfileTable2.EndemeProfileId = endemeProfileTable1.EndemeProfileId;
                ende.UpEndemeProfileTable(endemeProfileTable2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeProfileTable endemeProfileTable3 = ende.OnInEndemeProfileTable(endemeProfileId, _aspect);
                Assert.That(endemeProfileTable3.EndemeProfileId, Is.equal_to, endemeProfileTable2.EndemeProfileId, adoClass, adoMethod + " test update existing endemeProfileTable");
                ende.AssertEqualContent  (endemeProfileTable3, endemeProfileTable2, adoClass, adoMethod);
                ende.AssertUnequalContent(endemeProfileTable3, endemeProfileTable1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeProfileTable, did the update fail?
                // ----------------------------------------------------------------------
                EndemeProfileTable  endemeProfileTable4 = ende.AnneEndemeProfileTable(r);
                endemeProfileTable4.EndemeProfileId     = Guid.NewGuid();
                int                 count               = ende.UpEndemeProfileTable(endemeProfileTable4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeProfileTable");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { ende.UpEndemeProfileTable(endemeProfileTable1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeProfileTable -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeProfileTable table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public Guid AnIdOfEndemeProfileTable(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT EndemeProfileId FROM " + LaEndemeProfile + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeProfileTableId = new RichDataTable(cmd, "EndemeProfileTableId", "EndemeProfileId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeProfileTableId.Count > 0) return (Guid)endemeProfileTableId.ToList("EndemeProfileId", Guid.NewGuid())[r.Next(endemeProfileTableId.Count)];
                else return Guid.NewGuid();
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeProfileTable -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeProfileTable object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeProfileTable</returns>
        private EndemeProfileTable AnneEndemeProfileTable(Random r)
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable
                { EndemeProfileId  = Guid.NewGuid()
                , DataLabel        = r.Next(10000).ToString()
                , DataValueString  = r.Next(10000).ToString()
                , DataTableName    = r.Next(10000).ToString()
                , DataRowId        = r.Next(10000)
                , DataColumnName   = r.Next(10000).ToString()
                , ProfileMatch     = r.Next(10000)
                , ProfileTbdColumn = r.Next(10000).ToString()
                };
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable being tested</param>
        /// <param name="tgt"      >endemeProfileTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeProfileTable endemeProfileTable, EndemeProfileTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeProfileTable.DataLabel       , Is.equal_to, tgt.DataLabel       , adoClass, adoMethod + " DataLabel"       );
            Assert.That(endemeProfileTable.DataValueString , Is.equal_to, tgt.DataValueString , adoClass, adoMethod + " DataValueString" );
            Assert.That(endemeProfileTable.DataTableName   , Is.equal_to, tgt.DataTableName   , adoClass, adoMethod + " DataTableName"   );
            Assert.That(endemeProfileTable.DataRowId       , Is.equal_to, tgt.DataRowId       , adoClass, adoMethod + " DataRowId"       );
            Assert.That(endemeProfileTable.DataColumnName  , Is.equal_to, tgt.DataColumnName  , adoClass, adoMethod + " DataColumnName"  );
            Assert.That(endemeProfileTable.ProfileMatch    , Is.equal_to, tgt.ProfileMatch    , adoClass, adoMethod + " ProfileMatch"    );
            Assert.That(endemeProfileTable.ProfileTbdColumn, Is.equal_to, tgt.ProfileTbdColumn, adoClass, adoMethod + " ProfileTbdColumn");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable being tested</param>
        /// <param name="tgt"      >endemeProfileTable being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeProfileTable endemeProfileTable, EndemeProfileTable tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeProfileTable.DataLabel       , Is.not_equal_to, tgt.DataLabel       , adoClass, adoMethod + " DataLabel"       );
            Assert.That(endemeProfileTable.DataValueString , Is.not_equal_to, tgt.DataValueString , adoClass, adoMethod + " DataValueString" );
            Assert.That(endemeProfileTable.DataTableName   , Is.not_equal_to, tgt.DataTableName   , adoClass, adoMethod + " DataTableName"   );
            Assert.That(endemeProfileTable.DataRowId       , Is.not_equal_to, tgt.DataRowId       , adoClass, adoMethod + " DataRowId"       );
            Assert.That(endemeProfileTable.DataColumnName  , Is.not_equal_to, tgt.DataColumnName  , adoClass, adoMethod + " DataColumnName"  );
            Assert.That(endemeProfileTable.ProfileMatch    , Is.not_equal_to, tgt.ProfileMatch    , adoClass, adoMethod + " ProfileMatch"    );
            Assert.That(endemeProfileTable.ProfileTbdColumn, Is.not_equal_to, tgt.ProfileTbdColumn, adoClass, adoMethod + " ProfileTbdColumn");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeProfileTable -->
        /// <summary>
        ///     Returns a list of EndemeProfileTable objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeProfileTable objects</returns>
        public List<EndemeProfileTable> AtEndemeProfileTable(RichDataTable table)
        {
            List<EndemeProfileTable> list = new List<EndemeProfileTable>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeProfileTable(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeProfileTable -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeProfileTable objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeProfileTable objects"></param>
        public List<EndemeProfileTable> AtOnMareEndemeProfileTable(RichDataTable table)
        {
            List<EndemeProfileTable> endemeProfileTableList = new List<EndemeProfileTable>(table.Count);
            Dictionary<int,EndemeProfileTable> found = new Dictionary<int,EndemeProfileTable>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeProfileId = table.IntValue(row, "EndemeProfileId", -1);
                EndemeProfileTable endemeProfileTable = null;

                if (!found.ContainsKey(endemeProfileId))
                {
                    endemeProfileTable = OnEndemeProfileTable(table, row);

                    endemeProfileTable.EndemeCharacteristicList = new List<EndemeCharacteristic>();
                    endemeProfileTableList.Add(endemeProfileTable);
                    found.Add(endemeProfileId, endemeProfileTable);
                }
                else
                {
                    endemeProfileTable = found[endemeProfileId];
                }

                endemeProfileTable.EndemeCharacteristicList.Add(OnEndemeCharacteristic(table, row));
            }

            return endemeProfileTableList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeProfileTable -->
        /// <summary>
        ///      Copies an endemeProfileTable
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to copy</param>
        public static EndemeProfileTable CpEndemeProfileTable(EndemeProfileTable endemeProfileTable)
        {
            EndemeProfileTable output = new EndemeProfileTable();

            output.EndemeProfileId  = endemeProfileTable.EndemeProfileId;
            output.DataLabel        = endemeProfileTable.DataLabel;
            output.DataValueString  = endemeProfileTable.DataValueString;
            output.DataTableName    = endemeProfileTable.DataTableName;
            output.DataRowId        = endemeProfileTable.DataRowId;
            output.DataColumnName   = endemeProfileTable.DataColumnName;
            output.ProfileMatch     = endemeProfileTable.ProfileMatch;
            output.ProfileTbdColumn = endemeProfileTable.ProfileTbdColumn;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeProfileTableCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeProfile table
        /// </summary>
        /// <returns>a count of rows in the EndemeProfile table</returns>
        public int EndemeProfileTableCt(InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT COUNT(*) FROM " + LaEndemeProfile + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("CtEndemeProfileTable: " + cmd.Errors);
            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeProfileTable -->
        /// <summary>
        ///      Enables an EndemeProfileTable
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        public void DoEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + LaEndemeProfile + " SET N/A = 1 WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeProfileId", endemeProfileId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("EnEndemeProfileTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeProfileTable -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemeprofile table
        /// </summary>
        /// <returns>Maximum EndemeProfile.EndemeProfileId</returns>
//      public Guid HiIdOfEndemeProfileTable(InfoAspect aspect)
//      {
//          RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
//              , "SELECT MAX(EndemeProfileId) AS EndemeProfileId FROM " + ENDEME_PROFILE_TABLE + " WITH(NOLOCK)"
//              , Throws.Actions, "PR");


//          Guid endemeProfileId = cmd.ExecuteScalar(Guid.NewGuid());
//          if (__.StringContainsStuff(cmd.Errors))
//              throw new ApplicationException("HiIdOfEndemeProfileTable: " + cmd.Errors);
//          return endemeProfileId;
//      }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTable -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeProfile table
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeProfileId"  , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InEndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfEndemeProfile -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the EndemeProfileId column
        /// </summary>
        /// <param name="endemeProfileId">value in EndemeProfileId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeProfileId</returns>
        public RichDataTable InEndemeProfileTableOfEndemeProfile(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeProfileId" , endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf1EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataLabel column
        /// </summary>
        /// <param name="dataLabel">value in DataLabel column</param>
        /// <returns>a table of rows related to the specifed value of DataLabel</returns>
        public RichDataTable InEndemeProfileTableOfDataLabel(string dataLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE DataLabel = @DataLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@DataLabel" , dataLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf2EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataValueString -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataValueString column
        /// </summary>
        /// <param name="dataValueString">value in DataValueString column</param>
        /// <returns>a table of rows related to the specifed value of DataValueString</returns>
        public RichDataTable InEndemeProfileTableOfDataValueString(string dataValueString, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE DataValueString = @DataValueString"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@DataValueString" , dataValueString);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf3EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataTableName -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataTableName column
        /// </summary>
        /// <param name="dataTableName">value in DataTableName column</param>
        /// <returns>a table of rows related to the specifed value of DataTableName</returns>
        public RichDataTable InEndemeProfileTableOfDataTableName(string dataTableName, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE DataTableName = @DataTableName"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@DataTableName" , dataTableName);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf4EndemeProfileTable: " + cmd.Errors);
            return table;

        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataRow -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataRowId column
        /// </summary>
        /// <param name="dataRowId">value in DataRowId column</param>
        /// <returns>a table of rows related to the specifed value of DataRowId</returns>
        public RichDataTable InEndemeProfileTableOfDataRow(int dataRowId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE DataRowId = @DataRowId"
                , Throws.Actions, "PR")
                ._AddParameter_null("@DataRowId" , dataRowId);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf5EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfDataColumnName -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the DataColumnName column
        /// </summary>
        /// <param name="dataColumnName">value in DataColumnName column</param>
        /// <returns>a table of rows related to the specifed value of DataColumnName</returns>
        public RichDataTable InEndemeProfileTableOfDataColumnName(string dataColumnName, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE DataColumnName = @DataColumnName"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@DataColumnName" , dataColumnName);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf6EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfProfileMatch -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the ProfileMatch column
        /// </summary>
        /// <param name="profileMatch">value in ProfileMatch column</param>
        /// <returns>a table of rows related to the specifed value of ProfileMatch</returns>
        public RichDataTable InEndemeProfileTableOfProfileMatch(double profileMatch, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE ProfileMatch = @ProfileMatch"
                , Throws.Actions, "PR")
                ._AddParameter_null("@ProfileMatch" , profileMatch);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf7EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeProfileTableOfProfileTbdColumn -->
        /// <summary>
        ///      Returns the rows (In) the EndemeProfile table filtered by a value (Of) the ProfileTbdColumn column
        /// </summary>
        /// <param name="profileTbdColumn">value in ProfileTbdColumn column</param>
        /// <returns>a table of rows related to the specifed value of ProfileTbdColumn</returns>
        public RichDataTable InEndemeProfileTableOfProfileTbdColumn(string profileTbdColumn, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE ProfileTbdColumn = @ProfileTbdColumn"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@ProfileTbdColumn" , profileTbdColumn);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("InOf8EndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeProfileTable -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeProfile table
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "SELECT * FROM " + LaEndemeProfile + " WITH(NOLOCK) WHERE EndemeProfileId = @EndemeProfileId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeProfileId", endemeProfileId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeProfileTable with EndemeProfileId " + endemeProfileId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeProfileTable -->
        /// <summary>
        ///      Creates a (Ne)w EndemeProfileTable (o)bject from member values
        /// </summary>
        /// <param name="endemeProfileId" ></param>
        /// <param name="dataLabel"       ></param>
        /// <param name="dataValueString" ></param>
        /// <param name="dataTableName"   ></param>
        /// <param name="dataRowId"       ></param>
        /// <param name="dataColumnName"  ></param>
        /// <param name="profileMatch"    ></param>
        /// <param name="profileTbdColumn"></param>
        /// <returns>the new EndemeProfileTable object</returns>
        public EndemeProfileTable NeonEndemeProfileTable
            ( Guid     endemeProfileId
            , string   dataLabel
            , string   dataValueString
            , string   dataTableName
            , int      dataRowId
            , string   dataColumnName
            , double   profileMatch
            , string   profileTbdColumn
            )
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable
                { EndemeProfileId  = endemeProfileId
                , DataLabel        = dataLabel
                , DataValueString  = dataValueString
                , DataTableName    = dataTableName
                , DataRowId        = dataRowId
                , DataColumnName   = dataColumnName
                , ProfileMatch     = profileMatch
                , ProfileTbdColumn = profileTbdColumn
                };
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeProfileTable -->
        /// <summary>
        ///      Converts a row in the EndemeProfile data table into a EndemeProfileTable (O)bject
        /// </summary>
        /// <param name="endemeProfileTableTable">a table containing columns to build a batch object</param>
        /// <param name="row"                    >the row to convert</param>
        /// <param name="dispose"                >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeProfileTable OnEndemeProfileTable(RichDataTable endemeProfileTableTable, int row, bool dispose = false)
        {
            EndemeProfileTable endemeProfileTable = new EndemeProfileTable();
            endemeProfileTable.EndemeProfileId  = endemeProfileTableTable.GuidValue(row, "EndemeProfileId" , Guid.Empty  );
            endemeProfileTable.DataLabel        = endemeProfileTableTable.StrValue (row, "DataLabel"       , ""          );
            endemeProfileTable.DataValueString  = endemeProfileTableTable.StrValue (row, "DataValueString" , ""          );
            endemeProfileTable.DataTableName    = endemeProfileTableTable.StrValue (row, "DataTableName"   , ""          );
            endemeProfileTable.DataRowId        = endemeProfileTableTable.IntValue (row, "DataRowId"       , -1          );
            endemeProfileTable.DataColumnName   = endemeProfileTableTable.StrValue (row, "DataColumnName"  , ""          );
            endemeProfileTable.ProfileMatch     = endemeProfileTableTable.RealValue(row, "ProfileMatch"    , -1.0        );
            endemeProfileTable.ProfileTbdColumn = endemeProfileTableTable.StrValue (row, "ProfileTbdColumn", ""          );

            if (dispose) endemeProfileTableTable.Dispose();
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndemeProfileTable -->
        /// <summary>
        ///      Returns a table of a EndemeProfileTable (Re)garding all its detail table rows (in) the databse
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <returns>a table of EndemeProfileTable rows with their joined details</returns>
        public RichDataTable ReinEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " SELECT p.*, c.*"
                + "\r\n" + " FROM                " + LaEndemeProfile + " AS p WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeChar    + " AS c WITH(NOLOCK) ON c.EndemeProfileId = p.EndemeProfileId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND p.EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeProfileId", endemeProfileId);


            RichDataTable table = new RichDataTable(cmd, "EndemeProfileTable", "EndemeProfileId");
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ReinEndemeProfileTable: " + cmd.Errors);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeProfileTable -->
        /// <summary>
        ///      Inserts an endemeProfile object in(To) the database
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to insert into database</param>
        /// <returns>the Id of the inserted EndemeProfile</returns>
        public int ToEndemeProfileTable(EndemeProfileTable endemeProfile, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "\r\n" + " INSERT INTO " + LaEndemeProfile
                + "\r\n" + "        ( EndemeProfileId"
                + "\r\n" + "        , DataLabel"
                + "\r\n" + "        , DataValueString"
                + "\r\n" + "        , DataTableName"
                + "\r\n" + "        , DataRowId"
                + "\r\n" + "        , DataColumnName"
                + "\r\n" + "        , ProfileMatch"
                + "\r\n" + "        , ProfileTbdColumn"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeProfileId"
                + "\r\n" + " VALUES ( @EndemeProfileId"
                + "\r\n" + "        , @DataLabel"
                + "\r\n" + "        , @DataValueString"
                + "\r\n" + "        , @DataTableName"
                + "\r\n" + "        , @DataRowId"
                + "\r\n" + "        , @DataColumnName"
                + "\r\n" + "        , @ProfileMatch"
                + "\r\n" + "        , @ProfileTbdColumn"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeProfileId" , endemeProfile.EndemeProfileId                     )
                ._AddParameter_safe("@DataLabel"       , endemeProfile.DataLabel       ,  128)
                ._AddParameter_safe("@DataValueString" , endemeProfile.DataValueString ,   -1)
                ._AddParameter_safe("@DataTableName"   , endemeProfile.DataTableName   ,  128)
                ._AddParameter_null("@DataRowId"       , endemeProfile.DataRowId                           )
                ._AddParameter_safe("@DataColumnName"  , endemeProfile.DataColumnName  ,  128)
                ._AddParameter_null("@ProfileMatch"    , endemeProfile.ProfileMatch                        )
                ._AddParameter_safe("@ProfileTbdColumn", endemeProfile.ProfileTbdColumn,   50);


            int endemeProfileId = cmd.ExecuteScalar(-1);
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("ToEndemeProfileTable: " + cmd.Errors);
            return endemeProfileId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeProfileTable -->
        /// <summary>
        ///     Inserts in(To) the EndemeProfileTable table a (ne)w endemeProfileTable built from member values
        /// </summary>
        /// <param name="endemeProfileId" ></param>
        /// <param name="dataLabel"       ></param>
        /// <param name="dataValueString" ></param>
        /// <param name="dataTableName"   ></param>
        /// <param name="dataRowId"       ></param>
        /// <param name="dataColumnName"  ></param>
        /// <param name="profileMatch"    ></param>
        /// <param name="profileTbdColumn"></param>
        /// <returns>the new EndemeProfileTable object</returns>
        public EndemeProfileTable ToneEndemeProfileTable
            ( Guid     endemeProfileId
            , string   dataLabel
            , string   dataValueString
            , string   dataTableName
            , int      dataRowId
            , string   dataColumnName
            , double   profileMatch
            , string   profileTbdColumn
            , InfoAspect aspect)
        {
            EndemeProfileTable endemeProfileTable = NeonEndemeProfileTable
                ( endemeProfileId
                , dataLabel
                , dataValueString
                , dataTableName
                , dataRowId
                , dataColumnName
                , profileMatch
                , profileTbdColumn
                );
            ToEndemeProfileTable(endemeProfileTable, aspect);
            return endemeProfileTable;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeProfileTable -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeProfileTable
        /// </summary>
        /// <param name="endemeProfileId">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public void UnEndemeProfileTable(Guid endemeProfileId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                , "UPDATE " + LaEndemeProfile + " SET N/A = 0 WHERE EndemeProfileId = @EndemeProfileId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeProfileId", endemeProfileId);


            cmd.ExecuteNonQuery();
            if (__.StringHasContent(cmd.Errors))
                throw new ApplicationException("UnEndemeProfileTable: " + cmd.Errors);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeProfileTable -->
        /// <summary>
        ///      (Up)dates a row in the EndemeProfile table from a EndemeProfileTable object
        /// </summary>
        /// <param name="endemeProfileTable">endemeProfileTable to update</param>
        /// <returns>the count of the updated endemeProfileTable rows"></param>
        public int UpEndemeProfileTable(EndemeProfileTable endemeProfileTable, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeProfileId = " + endemeProfileTable.EndemeProfileId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(LaEndemeProfile, whereClause, aspect.PrimaryConn))
                {
                    case 0: break;
                    case 1:
                        cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                            , "\r\n" + " UPDATE " + LaEndemeProfile
                            + "\r\n" + " SET   DataLabel        = @DataLabel"
                            + "\r\n" + "     , DataValueString  = @DataValueString"
                            + "\r\n" + "     , DataTableName    = @DataTableName"
                            + "\r\n" + "     , DataRowId        = @DataRowId"
                            + "\r\n" + "     , DataColumnName   = @DataColumnName"
                            + "\r\n" + "     , ProfileMatch     = @ProfileMatch"
                            + "\r\n" + "     , ProfileTbdColumn = @ProfileTbdColumn"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeProfileId" , endemeProfileTable.EndemeProfileId                     )
                            ._AddParameter_safe("@DataLabel"       , endemeProfileTable.DataLabel       ,  128)
                            ._AddParameter_safe("@DataValueString" , endemeProfileTable.DataValueString ,   -1)
                            ._AddParameter_safe("@DataTableName"   , endemeProfileTable.DataTableName   ,  128)
                            ._AddParameter_null("@DataRowId"       , endemeProfileTable.DataRowId                           )
                            ._AddParameter_safe("@DataColumnName"  , endemeProfileTable.DataColumnName  ,  128)
                            ._AddParameter_null("@ProfileMatch"    , endemeProfileTable.ProfileMatch                        )
                            ._AddParameter_safe("@ProfileTbdColumn", endemeProfileTable.ProfileTbdColumn,   50)
                            ;


                        cmd.ExecuteNonQuery();
                        if (__.StringHasContent(cmd.Errors))
                            throw new ApplicationException("UpEndemeProfileTable: " + cmd.Errors);
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeProfileId " + endemeProfileTable.EndemeProfileId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeProfileTable table

        #region EndemeUsage table methods
        // ----------------------------------------------------------------------------------------
        //  EndemeUsage table methods
        // ----------------------------------------------------------------------------------------
        public List<EndemeUsage> AtEndemeUsageOfEndemeUsage        (int         endemeUsageId      , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfEndemeUsage        (endemeUsageId      , aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfEndemeSet          (Guid        endemeSetId        , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfEndemeSet          (endemeSetId        , aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfTableName          (string      tableName          , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfTableName          (tableName          , aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfTableEndemeFkColumn(string      tableEndemeFkColumn, InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfTableEndemeFkColumn(tableEndemeFkColumn, aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfTablePkColumn      (string      tablePkColumn      , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfTablePkColumn      (tablePkColumn      , aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfTableRowLabel      (string      tableRowLabel      , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfTableRowLabel      (tableRowLabel      , aspect)); }
        public List<EndemeUsage> AtEndemeUsageOfExtractLabelB      (string      extractLabelB      , InfoAspect aspect) { return AtEndemeUsage(InEndemeUsageOfExtractLabelB      (extractLabelB      , aspect)); }
        public Atom              AtommaEndemeUsageOf               (int         endemesetId        , InfoAspect aspect) { return MainEndemeUsageOf(endemesetId, aspect).ToAtom(); }
        public void              ExEndemeUsage                     (int         endemeUsageId      , InfoAspect aspect) { int    count = InData.DeleteUpTo(1, "FROM " + LaEndemeUsage + " WHERE EndemeUsageId = " + endemeUsageId, aspect.SecondaryConn); }
        public RichDataTable     IntoEndemeUsage                   (EndemeUsage endemeUsage        , InfoAspect aspect) { if (   IsEndemeUsage(endemeUsage.EndemeUsageId, aspect)) return InEndemeUsage(endemeUsage.EndemeUsageId, aspect); else { return InEndemeUsage(ToEndemeUsage(endemeUsage, aspect), aspect); } }
        public EndemeUsage       OnInEndemeUsage                   (int         endemeUsageId      , InfoAspect aspect) { return OnEndemeUsage(InEndemeUsage  (endemeUsageId, aspect), 0, true); }
        public EndemeUsage       OnIntoEndemeUsage                 (EndemeUsage endemeUsage        , InfoAspect aspect) { return OnEndemeUsage(IntoEndemeUsage(endemeUsage  , aspect), 0, true); }
        public int               UpToEndemeUsage                   (EndemeUsage endemeUsage        , InfoAspect aspect) { if (   IsEndemeUsage(endemeUsage.EndemeUsageId, aspect)) return UpEndemeUsage(endemeUsage        , aspect); else { return ToEndemeUsage(endemeUsage, aspect); } }

        private void AccessEndemeUsage(InfoAspect aspect)
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeUsage_test -->
        /// <summary>
        ///      Tests the ToEndemeUsage method
        /// </summary>
        public void ToEndemeUsage_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass  = "EndemeUsageAccess";
            string adoMethod = "ToEndemeUsage";
            Random r = RandomSource.New().Random;
            int    endemeUsageId = -1;
            EndemeAccess data = new EndemeAccess();

            // --------------------------------------------------------------------------
            //  Run insert method
            // --------------------------------------------------------------------------
            Assert.ThingsAbout(adoMethod);
            EndemeUsage endemeUsageTo = data.AnneEndemeUsage(r, _aspect);
            try
            {
                endemeUsageId = data.ToEndemeUsage(endemeUsageTo, _aspect);     // <-- This is what is being tested
                if (!string.IsNullOrWhiteSpace(data.Errors)) Assert.Crash(data.Errors);
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }


            // --------------------------------------------------------------------------
            //  Check results
            // --------------------------------------------------------------------------
            Assert.That(endemeUsageId, Is.greater_than, -1, adoClass, adoMethod);
            EndemeUsage endemeUsageFrom = data.OnInEndemeUsage(endemeUsageId, _aspect);
            data.AssertEqualContent(endemeUsageFrom, endemeUsageTo, adoClass, adoMethod);


            data.ExEndemeUsage(endemeUsageId, _aspect); // Cleanup
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeUsage_test -->
        /// <summary>
        ///      Tests the UpEndemeUsage method
        /// </summary>
        public void UpEndemeUsage_test()
        {
            // --------------------------------------------------------------------------
            //  Test variables
            // --------------------------------------------------------------------------
            string adoClass          = "EndemeUsageAccess";
            string adoMethod         = "UpEndemeUsage";
            Random r                 = RandomSource.New().Random;
            EndemeUsage endemeUsage1 = null;

            EndemeAccess data = new EndemeAccess();
            Assert.ThingsAbout(adoMethod);


            try
            {
                // ----------------------------------------------------------------------
                //  Update an existing endemeUsageId with a newly created endemeUsage
                // ----------------------------------------------------------------------
                int    endemeUsageId = data.AnIdOfEndemeUsage(_aspect);
                endemeUsage1               = data.OnInEndemeUsage(endemeUsageId, _aspect);
                EndemeUsage  endemeUsage2  = data.AnneEndemeUsage(r, _aspect);
                endemeUsage2.EndemeUsageId = endemeUsage1.EndemeUsageId;
                data.UpEndemeUsage(endemeUsage2, _aspect);  //  <-- this is what's being tested


                // ----------------------------------------------------------------------
                //  Did the update succeed?
                // ----------------------------------------------------------------------
                EndemeUsage endemeUsage3 = data.OnInEndemeUsage(endemeUsageId, _aspect);
                Assert.That(endemeUsage3.EndemeUsageId, Is.equal_to, endemeUsage2.EndemeUsageId, adoClass, adoMethod + " test update existing endemeUsage");
                data.AssertEqualContent  (endemeUsage3, endemeUsage2, adoClass, adoMethod);
                data.AssertUnequalContent(endemeUsage3, endemeUsage1, adoClass, adoMethod);


                // ----------------------------------------------------------------------
                //  Try to update a nonexistent endemeUsage, did the update fail?
                // ----------------------------------------------------------------------
                EndemeUsage  endemeUsage4  = data.AnneEndemeUsage(r, _aspect);
                endemeUsage4.EndemeUsageId = data.HiIdOfEndemeUsage(_aspect) + 1;
                int          count         = data.UpEndemeUsage(endemeUsage4, _aspect);  //  <-- this is what's being tested
                Assert.That(count, Is.equal_to, 0, adoClass, adoMethod + " test update non nonexisting endemeUsage");
            }
            catch (Exception ex) { Assert.Crash(ex); Is.Trash(Assert.Detail); }
            finally { data.UpEndemeUsage(endemeUsage1, _aspect); } //  Cleanup


            // --------------------------------------------------------------------------
            //  Return results
            // --------------------------------------------------------------------------
            _result += Assert.Conclusion;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnIdOfEndemeUsage -->
        /// <summary>
        ///      Looks up (An) existing random (Id) (Of) the endemeUsage table
        /// </summary>
        /// <returns>an existing id or -1</returns>
        public int AnIdOfEndemeUsage(InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT EndemeUsageId FROM " + LaEndemeUsage + " WITH(NOLOCK)"
                , Throws.Actions, "P");


            using (RichDataTable endemeUsageId = new RichDataTable(cmd, "EndemeUsageId", "EndemeUsageId"))
            {
                Random r = RandomSource.New().Random;
                if (endemeUsageId.Count > 0) return (int)endemeUsageId.ToList("EndemeUsageId", -1)[r.Next(endemeUsageId.Count)];
                else return -1;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AnneEndemeUsage -->
        /// <summary>
        ///      Creates (An) existing random (ne)w endemeUsage object
        /// </summary>
        /// <param name="r">random number source</param>
        /// <returns>a new random endemeUsage</returns>
        private EndemeUsage AnneEndemeUsage(Random r, InfoAspect aspect)
        {
            EndemeUsage endemeUsage = new EndemeUsage
              //{ EndemeUsageId       = HiIdOfEndemeUsage(aspect) + 1
                { EndemeSetId         = Guid.NewGuid()
                , TableName           = r.Next(10000).ToString()
                , TableEndemeFkColumn = r.Next(10000).ToString()
                , TablePkColumn       = r.Next(10000).ToString()
                , TableRowLabel       = r.Next(10000).ToString()
                , ExtractLabelB       = r.Next(10000).ToString()
                };
            return endemeUsage;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertEqualContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeUsage">endemeUsage being tested</param>
        /// <param name="tgt"      >endemeUsage being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertEqualContent(EndemeUsage endemeUsage, EndemeUsage tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeUsage.EndemeSetId        , Is.equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
            Assert.That(endemeUsage.TableName          , Is.equal_to, tgt.TableName          , adoClass, adoMethod + " TableName"          );
            Assert.That(endemeUsage.TableEndemeFkColumn, Is.equal_to, tgt.TableEndemeFkColumn, adoClass, adoMethod + " TableEndemeFkColumn");
            Assert.That(endemeUsage.TablePkColumn      , Is.equal_to, tgt.TablePkColumn      , adoClass, adoMethod + " TablePkColumn"      );
            Assert.That(endemeUsage.TableRowLabel      , Is.equal_to, tgt.TableRowLabel      , adoClass, adoMethod + " TableRowLabel"      );
            Assert.That(endemeUsage.ExtractLabelB      , Is.equal_to, tgt.ExtractLabelB      , adoClass, adoMethod + " ExtractLabelB"      );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AssertUnequalContent -->
        /// <summary>
        ///      Runs Assert statments to check for equality on all simple content, but not id
        /// </summary>
        /// <param name="endemeUsage">endemeUsage being tested</param>
        /// <param name="tgt"      >endemeUsage being compared against</param>
        /// <param name="adoClass" >name of class being tested</param>
        /// <param name="adoMethod">test on a method</param>
        private void AssertUnequalContent(EndemeUsage endemeUsage, EndemeUsage tgt, string adoClass, string adoMethod)
        {
            Assert.That(endemeUsage.EndemeSetId        , Is.not_equal_to, tgt.EndemeSetId        , adoClass, adoMethod + " EndemeSetId"        );
            Assert.That(endemeUsage.TableName          , Is.not_equal_to, tgt.TableName          , adoClass, adoMethod + " TableName"          );
            Assert.That(endemeUsage.TableEndemeFkColumn, Is.not_equal_to, tgt.TableEndemeFkColumn, adoClass, adoMethod + " TableEndemeFkColumn");
            Assert.That(endemeUsage.TablePkColumn      , Is.not_equal_to, tgt.TablePkColumn      , adoClass, adoMethod + " TablePkColumn"      );
            Assert.That(endemeUsage.TableRowLabel      , Is.not_equal_to, tgt.TableRowLabel      , adoClass, adoMethod + " TableRowLabel"      );
            Assert.That(endemeUsage.ExtractLabelB      , Is.not_equal_to, tgt.ExtractLabelB      , adoClass, adoMethod + " ExtractLabelB"      );
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtEndemeUsage -->
        /// <summary>
        ///     Returns a list of EndemeUsage objects from a table (At) which you can index one
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a list of EndemeUsage objects</returns>
        public List<EndemeUsage> AtEndemeUsage(RichDataTable table)
        {
            List<EndemeUsage> list = new List<EndemeUsage>();
            for (int row = 0; row < table.Count; ++row)
                list.Add(OnEndemeUsage(table, row));
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AtOnMareEndemeUsage -->
        /// <summary>
        ///      Converts a DataTable to a list of parent and detail extended endemeUsage objects
        /// </summary>
        /// <param name="table">the table to convert containin also parent and child data</param>
        /// <returns>a list of parent and detail extended endemeUsage objects"></param>
        public List<EndemeUsage> AtOnMareEndemeUsage(RichDataTable table)
        {
            List<EndemeUsage> endemeUsageList = new List<EndemeUsage>(table.Count);
            Dictionary<int,EndemeUsage> found = new Dictionary<int,EndemeUsage>();


            for (int row = 0; row < table.Count; ++row)
            {
                int endemeUsageId = table.IntValue(row, "EndemeUsageId", -1);
                EndemeUsage endemeUsage = null;

                if (!found.ContainsKey(endemeUsageId))
                {
                    endemeUsage = OnEndemeUsage(table, row);
                    endemeUsage.EndemeSet = OnEndemeSet(table, row);
                    endemeUsageList.Add(endemeUsage);
                    found.Add(endemeUsageId, endemeUsage);
                }
                else
                {
                    endemeUsage = found[endemeUsageId];
                }
            }

            return endemeUsageList;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- AxEndemeUsage -->
        /// <summary>
        ///      Deletes rows from the EndemeUsage table above a certain endemeUsageId (Axes them)
        /// </summary>
        /// <param name="endemeUsageId">Delete all above this id</param>
        /// <param name="aspect"></param>
        public int AxEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            string fromWhereClause = " FROM " + LaEndemeUsage + " WHERE EndemeUsageId >= @EndemeUsageId";
            int count = 0;
            {
                RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                    , "SELECT COUNT(*)" + fromWhereClause
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeUsageId", endemeUsageId);


                count = cmd.ExecuteScalar(-1);
            }


            if (0 < count && count < 10000)
            {
                RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                    , "DELETE" + fromWhereClause
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeUsageId", endemeUsageId);


                cmd.ExecuteNonQuery();
            }

            return count;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CpEndemeUsage -->
        /// <summary>
        ///      Copies an endemeUsage
        /// </summary>
        /// <param name="endemeUsage">endemeUsage to copy</param>
        public static EndemeUsage CpEndemeUsage(EndemeUsage endemeUsage)
        {
            EndemeUsage output = new EndemeUsage();

            output.EndemeUsageId       = endemeUsage.EndemeUsageId;
            output.EndemeSetId         = endemeUsage.EndemeSetId;
            output.TableName           = endemeUsage.TableName;
            output.TableEndemeFkColumn = endemeUsage.TableEndemeFkColumn;
            output.TablePkColumn       = endemeUsage.TablePkColumn;
            output.TableRowLabel       = endemeUsage.TableRowLabel;
            output.ExtractLabelB       = endemeUsage.ExtractLabelB;

            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- EndemeUsageCt -->
        /// <summary>
        ///      Returns a (C)oun(t) of the number of rows in the EndemeUsage table
        /// </summary>
        /// <returns>a count of rows in the EndemeUsage table</returns>
        public int EndemeUsageCt(InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT COUNT(*) FROM " + LaEndemeUsage + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int count = cmd.ExecuteScalar(-1);
            return count;
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- DoEndemeUsage -->
        /// <summary>
        ///      Enables an EndemeUsage
        /// </summary>
        /// <param name="endemeUsageId">the primary key</param>
        public int DoEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE " + LaEndemeUsage + " SET x = 1 WHERE EndemeUsageId = @EndemeUsageId"
                , Throws.Actions, "PR")

                ._AddParameter("@EndemeUsageId", endemeUsageId);

            int output = cmd.ExecuteNonQuery();
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- HiIdOfEndemeUsage -->
        /// <summary>
        ///      Returns the (Hi)ghest (Id) (Of) the endemeusage table
        /// </summary>
        /// <returns>Maximum EndemeUsage.EndemeUsageId</returns>
        public int HiIdOfEndemeUsage(InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT MAX(EndemeUsageId) AS EndemeUsageId FROM " + LaEndemeUsage + " WITH(NOLOCK)"
                , Throws.Actions, "PR");


            int endemeUsageId = cmd.ExecuteScalar(0);
            return endemeUsageId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsage -->
        /// <summary>
        ///      Returns the indicated row (In) the EndemeUsage table
        /// </summary>
        /// <param name="endemeUsageId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public RichDataTable InEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE EndemeUsageId = @EndemeUsageId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeUsageId"  , endemeUsageId);



            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfEndemeUsage -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the EndemeUsageId column
        /// </summary>
        /// <param name="endemeUsageId">value in EndemeUsageId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeUsageId</returns>
        public RichDataTable InEndemeUsageOfEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE EndemeUsageId = @EndemeUsageId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeUsageId" , endemeUsageId);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfEndemeSet -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the EndemeSetId column
        /// </summary>
        /// <param name="endemeSetId">value in EndemeSetId column</param>
        /// <returns>a table of rows related to the specifed value of EndemeSetId</returns>
        public RichDataTable InEndemeUsageOfEndemeSet(Guid endemeSetId, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId" , endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfTableName -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the TableName column
        /// </summary>
        /// <param name="tableName">value in TableName column</param>
        /// <returns>a table of rows related to the specifed value of TableName</returns>
        public RichDataTable InEndemeUsageOfTableName(string tableName, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE TableName = @TableName"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@TableName" , tableName);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfTableEndemeFkColumn -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the TableEndemeFkColumn column
        /// </summary>
        /// <param name="tableEndemeFkColumn">value in TableEndemeFkColumn column</param>
        /// <returns>a table of rows related to the specifed value of TableEndemeFkColumn</returns>
        public RichDataTable InEndemeUsageOfTableEndemeFkColumn(string tableEndemeFkColumn, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE TableEndemeFkColumn = @TableEndemeFkColumn"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@TableEndemeFkColumn" , tableEndemeFkColumn);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfTablePkColumn -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the TablePkColumn column
        /// </summary>
        /// <param name="tablePkColumn">value in TablePkColumn column</param>
        /// <returns>a table of rows related to the specifed value of TablePkColumn</returns>
        public RichDataTable InEndemeUsageOfTablePkColumn(string tablePkColumn, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE TablePkColumn = @TablePkColumn"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@TablePkColumn" , tablePkColumn);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfTableRowLabel -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the TableRowLabel column
        /// </summary>
        /// <param name="tableRowLabel">value in TableRowLabel column</param>
        /// <returns>a table of rows related to the specifed value of TableRowLabel</returns>
        public RichDataTable InEndemeUsageOfTableRowLabel(string tableRowLabel, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE TableRowLabel = @TableRowLabel"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@TableRowLabel" , tableRowLabel);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InEndemeUsageOfExtractLabelB -->
        /// <summary>
        ///      Returns the rows (In) the EndemeUsage table filtered by a value (Of) the ExtractLabelB column
        /// </summary>
        /// <param name="extractLabelB">value in ExtractLabelB column</param>
        /// <returns>a table of rows related to the specifed value of ExtractLabelB</returns>
        public RichDataTable InEndemeUsageOfExtractLabelB(string extractLabelB, InfoAspect aspect)
        {
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE ExtractLabelB = @ExtractLabelB"
                , Throws.Actions, "PR")
                ._AddParameter_safe("@ExtractLabelB" , extractLabelB);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- IsEndemeUsage -->
        /// <summary>
        ///      Checks if the indicated row (Is) present in the EndemeUsage table
        /// </summary>
        /// <param name="endemeUsageId">the primary key</param>
        /// <returns>true if the row is present</returns>
        public bool IsEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = null;
            bool present = false;
            try
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                    , "SELECT * FROM " + LaEndemeUsage + " WITH(NOLOCK) WHERE EndemeUsageId = @EndemeUsageId"
                    , Throws.Actions, "P")
                    ._AddParameter("@EndemeUsageId", endemeUsageId);


                using (RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId"))
                {
                    if (table.Count > 1) throw new AmbiguousResultException("There should not be more than one EndemeUsage with EndemeUsageId " + endemeUsageId.ToString());
                    present = (table != null && table.Count > 0);
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return present;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeUsage -->
        /// <summary>
        ///      Returns a table joined with a parent(Ma) of data from (in) an EndemeUsage
        /// </summary>
        /// <param name="endemeUsageId">the primary key</param>
        /// <returns>a table of endemeUsage rows with their joined parent data</returns>
        public RichDataTable MainEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, u.*"
                + "\r\n" + " FROM                " + LaEndemeUsage + " AS u WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet   + " AS s WITH(NOLOCK) ON s.EndemeSetId = u.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND u.EndemeUsageId = @EndemeUsageId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeUsageId", endemeUsageId);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- MainEndemeUsageOf -->
        /// <summary>
        ///      Returns a endemeUsage list (Of) a p(a)rent endemeset with its endemeset data
        /// </summary>
        /// <param name="endemesetId">the key of a row in the parent table</param>
        /// <param name="aspect">poor man's aspect oriented programming using dependency injection</param>
        /// <returns>a table of endemeUsage rows with their joined parent data</returns>
        public RichDataTable MainEndemeUsageOf(int endemeSetId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " SELECT s.*, u.*"
                + "\r\n" + " FROM                " + LaEndemeUsage + " AS u WITH(NOLOCK)"
                + "\r\n" + "     LEFT OUTER JOIN " + LaEndemeSet   + " AS s WITH(NOLOCK) ON s.EndemeSetId = u.EndemeSetId"
                + "\r\n" + " WHERE   1=1"
                + "\r\n" + "     AND u.EndemeSetId = @EndemeSetId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeSetId", endemeSetId);


            RichDataTable table = new RichDataTable(cmd, "EndemeUsage", "EndemeUsageId");
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- NeonEndemeUsage -->
        /// <summary>
        ///      Creates a (Ne)w EndemeUsage (o)bject from member values
        /// </summary>
        /// <param name="endemeUsageId"      ></param>
        /// <param name="endemeSetId"        ></param>
        /// <param name="tableName"          ></param>
        /// <param name="tableEndemeFkColumn"></param>
        /// <param name="tablePkColumn"      ></param>
        /// <param name="tableRowLabel"      ></param>
        /// <param name="extractLabelB"      ></param>
        /// <returns>the new EndemeUsage object</returns>
        public EndemeUsage NeonEndemeUsage
            ( int      endemeUsageId
            , Guid     endemeSetId
            , string   tableName
            , string   tableEndemeFkColumn
            , string   tablePkColumn
            , string   tableRowLabel
            , string   extractLabelB
            )
        {
            EndemeUsage endemeUsage = new EndemeUsage
                { EndemeUsageId       = endemeUsageId
                , EndemeSetId         = endemeSetId
                , TableName           = tableName
                , TableEndemeFkColumn = tableEndemeFkColumn
                , TablePkColumn       = tablePkColumn
                , TableRowLabel       = tableRowLabel
                , ExtractLabelB       = extractLabelB
                };
            return endemeUsage;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- OnEndemeUsage -->
        /// <summary>
        ///      Converts a row in the EndemeUsage data table into a EndemeUsage (O)bject
        /// </summary>
        /// <param name="endemeUsageTable">a table containing columns to build a batch object</param>
        /// <param name="row"             >the row to convert</param>
        /// <param name="dispose"         >whether this method should dispose the table, default: false</param>
        /// <returns>an object from the specified row</returns>
        public EndemeUsage OnEndemeUsage(RichDataTable endemeUsageTable, int row, bool dispose = false)
        {
            EndemeUsage endemeUsage = new EndemeUsage();
            endemeUsage.EndemeUsageId       = endemeUsageTable.IntValue (row, "EndemeUsageId"      , -1          );
            endemeUsage.EndemeSetId         = endemeUsageTable.GuidValue(row, "EndemeSetId"        , Guid.Empty  );
            endemeUsage.TableName           = endemeUsageTable.StrValue (row, "TableName"          , ""          );
            endemeUsage.TableEndemeFkColumn = endemeUsageTable.StrValue (row, "TableEndemeFkColumn", ""          );
            endemeUsage.TablePkColumn       = endemeUsageTable.StrValue (row, "TablePkColumn"      , ""          );
            endemeUsage.TableRowLabel       = endemeUsageTable.StrValue (row, "TableRowLabel"      , ""          );
            endemeUsage.ExtractLabelB       = endemeUsageTable.StrValue (row, "ExtractLabelB"      , ""          );

            if (dispose) endemeUsageTable.Dispose();
            return endemeUsage;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ReinEndeme -->
        /// <summary>
        ///      Gets an endeme list with attached table rows based on usage row field values
        /// </summary>
        /// <param name="endemeSetId"></param>
        /// <param name="tableName"></param>
        /// <param name="fkField"></param>
        /// <param name="pkField"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        /// <remarks>
        ///      endemeSetId, tableName and fkField make up the natural key
        /// </remarks>
        public RichDataTable ReinEndeme(Guid endemeSetId, string tableName, string fkField, string pkField, string rowLabelA, string rowLabelB, string toEndemeTable, InfoAspect aspect)
        {
            string tblName = InData.ResistSqlInjection(tableName);  if (string.IsNullOrWhiteSpace(tblName))  throw new ArgumentException("specified table may not be blank");
            string PK      = InData.ResistSqlInjection(pkField  );  if (string.IsNullOrWhiteSpace(PK     ))  throw new ArgumentException("specified primary key may not be blank");
            string FK      = InData.ResistSqlInjection(fkField  );  if (string.IsNullOrWhiteSpace(FK     ))  throw new ArgumentException("specified foreign key may not be blank");


            string labelA  = InData.ResistSqlInjection_lite(rowLabelA); // ResistSqlInjection_lite allows some simple SQL
            string labelB  = InData.ResistSqlInjection_lite(rowLabelB);
            if (string.IsNullOrWhiteSpace(labelA)) labelA = labelB;
            if (string.IsNullOrWhiteSpace(labelA)) labelA = PK;


            RichSqlCommand cmd = null;
            if (string.IsNullOrWhiteSpace(labelB))
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "\r\n" + " SELECT x." + PK + " AS ItemId, e.EndemeId, e.EndemeString, " + labelA + " AS ItemRowLabel"
                    + "\r\n" + " FROM                " + toEndemeTable + "  AS e WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN " + tblName + " AS x WITH(NOLOCK) ON x." + FK + " = e.EndemeId"
                    + "\r\n" + " WHERE e.EndemeSetId = @EndemeSetId AND x." + PK + " > 0"
                    + "\r\n" + " ORDER BY " + labelA + ", " + PK
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeSetId", endemeSetId);
            }
            else
            {
                cmd = new RichSqlCommand(CommandType.Text, aspect.PrimaryConn
                    , "\r\n" + " SELECT x." + PK + " AS ItemId, e.EndemeId, e.EndemeString, " + labelA + " AS ItemRowLabel, " + labelB + " AS ItemLabelB"
                    + "\r\n" + " FROM                " + toEndemeTable + "  AS e WITH(NOLOCK)"
                    + "\r\n" + "     LEFT OUTER JOIN " + tblName + " AS x WITH(NOLOCK) ON x." + FK + " = e.EndemeId"
                    + "\r\n" + " WHERE e.EndemeSetId = @EndemeSetId AND x." + PK + " > 0"
                    + "\r\n" + " ORDER BY " + labelA + ", " + labelB + ", " + PK
                    , Throws.Actions, "PR")
                    ._AddParameter("@EndemeSetId", endemeSetId);
            }


            RichDataTable table = new RichDataTable(cmd, tblName, PK);
            return table;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToEndemeUsage -->
        /// <summary>
        ///      Inserts an endemeUsage object in(To) the database
        /// </summary>
        /// <param name="endemeUsage">endemeUsage to insert into database</param>
        /// <returns>the Id of the inserted EndemeUsage</returns>
        public int ToEndemeUsage(EndemeUsage endemeUsage, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            // --------------------------------------------------------------------------
            //  Prepare query
            // --------------------------------------------------------------------------
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "\r\n" + " INSERT INTO " + LaEndemeUsage
                + "\r\n" + "        ( EndemeSetId"
                + "\r\n" + "        , TableName"
                + "\r\n" + "        , TableEndemeFkColumn"
                + "\r\n" + "        , TablePkColumn"
                + "\r\n" + "        , TableRowLabel"
                + "\r\n" + "        , ExtractLabelB"
                + "\r\n" + "        ) OUTPUT INSERTED.EndemeUsageId"
                + "\r\n" + " VALUES ( @EndemeSetId"
                + "\r\n" + "        , @TableName"
                + "\r\n" + "        , @TableEndemeFkColumn"
                + "\r\n" + "        , @TablePkColumn"
                + "\r\n" + "        , @TableRowLabel"
                + "\r\n" + "        , @ExtractLabelB"
                + "\r\n" + "        )"
                , Throws.Actions, "PR")
                ._AddParameter     ("@EndemeSetId"        , endemeUsage.EndemeSetId              )
                ._AddParameter_safe("@TableName"          , endemeUsage.TableName          ,   64)
                ._AddParameter_safe("@TableEndemeFkColumn", endemeUsage.TableEndemeFkColumn,   64)
                ._AddParameter_safe("@TablePkColumn"      , endemeUsage.TablePkColumn      ,   64)
                ._AddParameter_safe("@TableRowLabel"      , endemeUsage.TableRowLabel      ,  256)
                ._AddParameter_safe("@ExtractLabelB"      , endemeUsage.ExtractLabelB      ,  256);


            int endemeUsageId = cmd.ExecuteScalar(-1);
            return endemeUsageId;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToneEndemeUsage -->
        /// <summary>
        ///     Inserts in(To) the EndemeUsage table a (ne)w endemeUsage built from member values
        /// </summary>
        /// <param name="endemeUsageId"      ></param>
        /// <param name="endemeSetId"        ></param>
        /// <param name="tableName"          ></param>
        /// <param name="tableEndemeFkColumn"></param>
        /// <param name="tablePkColumn"      ></param>
        /// <param name="tableRowLabel"      ></param>
        /// <param name="extractLabelB"      ></param>
        /// <returns>the new EndemeUsage object</returns>
        public EndemeUsage ToneEndemeUsage
            ( int      endemeUsageId
            , Guid     endemeSetId
            , string   tableName
            , string   tableEndemeFkColumn
            , string   tablePkColumn
            , string   tableRowLabel
            , string   extractLabelB
            , InfoAspect aspect)
        {
            EndemeUsage endemeUsage = NeonEndemeUsage
                ( endemeUsageId
                , endemeSetId
                , tableName
                , tableEndemeFkColumn
                , tablePkColumn
                , tableRowLabel
                , extractLabelB
                );
            endemeUsage.EndemeUsageId = ToEndemeUsage(endemeUsage, aspect);
            return endemeUsage;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UnEndemeUsage -->
        /// <summary>
        ///      Disables/(Un)enables an EndemeUsage
        /// </summary>
        /// <param name="endemeUsageId">the primary key</param>
        /// <param name="disableValue">the value meaning disable</param>
        public int UnEndemeUsage(int endemeUsageId, InfoAspect aspect)
        {
            AccessEndemeUsage(aspect);
            RichSqlCommand cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                , "UPDATE " + LaEndemeUsage + " SET x = 0 WHERE EndemeUsageId = @EndemeUsageId"
                , Throws.Actions, "PR")
                ._AddParameter("@EndemeUsageId", endemeUsageId);


            int output = cmd.ExecuteNonQuery();
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- UpEndemeUsage -->
        /// <summary>
        ///      (Up)dates a row in the EndemeUsage table from a EndemeUsage object
        /// </summary>
        /// <param name="endemeUsage">endemeUsage to update</param>
        /// <returns>the count of the updated endemeUsage rows"></param>
        public int UpEndemeUsage(EndemeUsage endemeUsage, InfoAspect aspect)
        {
            string whereClause = "WHERE EndemeUsageId = " + endemeUsage.EndemeUsageId.ToString();
            RichSqlCommand cmd = null;
            int count = 0;


            try
            {
                switch (RichDataTable.RowCountIn(LaEndemeUsage, whereClause, aspect.SecondaryConn))
                {
                    case 0: break;
                    case 1:
                        AccessEndemeUsage(aspect);
                        cmd = new RichSqlCommand(CommandType.Text, aspect.SecondaryConn
                            , "\r\n" + " UPDATE " + LaEndemeUsage
                            + "\r\n" + " SET   EndemeSetId         = @EndemeSetId"
                            + "\r\n" + "     , TableName           = @TableName"
                            + "\r\n" + "     , TableEndemeFkColumn = @TableEndemeFkColumn"
                            + "\r\n" + "     , TablePkColumn       = @TablePkColumn"
                            + "\r\n" + "     , TableRowLabel       = @TableRowLabel"
                            + "\r\n" + "     , ExtractLabelB       = @ExtractLabelB"
                            + "\r\n" + whereClause
                            , Throws.Actions, "P")
                            ._AddParameter     ("@EndemeUsageId"      , endemeUsage.EndemeUsageId                          )
                            ._AddParameter     ("@EndemeSetId"        , endemeUsage.EndemeSetId                            )
                            ._AddParameter_safe("@TableName"          , endemeUsage.TableName          ,   64)
                            ._AddParameter_safe("@TableEndemeFkColumn", endemeUsage.TableEndemeFkColumn,   64)
                            ._AddParameter_safe("@TablePkColumn"      , endemeUsage.TablePkColumn      ,   64)
                            ._AddParameter_safe("@TableRowLabel"      , endemeUsage.TableRowLabel      ,  256)
                            ._AddParameter_safe("@ExtractLabelB"      , endemeUsage.ExtractLabelB      ,  256)
                            ;


                        cmd.ExecuteNonQuery();
                        count = 1;
                        break;
                    default: throw new AmbiguousResultException("too many results for endemeUsageId " + endemeUsage.EndemeUsageId.ToString());
                }
            }
            catch   { throw; }
            finally { if (cmd != null) cmd.Dispose(); }

            return count;
        }

        #endregion EndemeUsage table

    }
}