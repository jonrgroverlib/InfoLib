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
using System;                         // for DateTime, Convert, Console
using System.Collections.Generic;     // for Dictionary
using System.Data;                    // for DataRow, DataTable
using System.Data.SqlClient;          // for SqlCommand, SqlException, SqlDataReader
using System.Data.SqlTypes;           // for SqlDateTime
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- ParameterizedLocation -->
    /// <summary>
    ///      The ParameterizedLocation class implements a parameterized data access layer for
    ///      4 location tables: Level_1, Level_2, Agency_Master, location_Master
    ///      uses the old id for each location
    /// </summary>
    /// <remarks>
    ///      SELECT p.location_ID
    ///          , p.Agency_ID
    ///          , p.location_Local_ID
    ///          , p.Network_ID
    ///          , CASE WHEN p.location_Active       IS NULL THEN '0' ELSE '''' + CONVERT(varchar, p.location_Active)                       END AS location_Active
    ///          , CASE WHEN p.Create_Date           IS NULL THEN ''  ELSE '''' + CONVERT(varchar, p.Create_Date          , 121)            END AS Create_Date
    ///          , CASE WHEN p.location_Phone        IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Phone       , CHAR(9), ' '))) END AS location_Phone
    ///          , CASE WHEN p.location_Name         IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Name        , CHAR(9), ' '))) END AS location_Name
    ///          , CASE WHEN p.location_Address      IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Address     , CHAR(9), ' '))) END AS location_Address
    ///          , CASE WHEN p.location_City         IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_City        , CHAR(9), ' '))) END AS location_City
    ///          , CASE WHEN p.location_Zip          IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Zip         , CHAR(9), ' '))) END AS location_Zip
    ///          , CASE WHEN p.location_Contact_Name IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Contact_Name, CHAR(9), ' '))) END AS location_Contact_Name
    ///          , CASE WHEN p.location_Email        IS NULL THEN ''  ELSE     LTRIM(RTRIM(REPLACE(p.location_Email       , CHAR(9), ' '))) END AS location_Email
    ///      FROM [CANVaS].dbo.[location_Master] AS p
    ///      WHERE p.Agency_ID = 1
    ///      
    /// 
    ///      Debugging advice:
    ///      -----------------
    ///      duplicate entries of the same column can be caused by not having the load or extract keys in the transform entry right
    ///      
    ///      alpha toy code - used once in production, expected to be deprecated
    /// </remarks>
    public class ParameterizedLocation : EtlModule
    {
        // ----------------------------------------------------------------------------------------
        //  Location constructor (five target tables, not just one)
        // ----------------------------------------------------------------------------------------
        public ParameterizedLocation() : base("Location", 50)
        {
            // --------------------------------------------------------------------------
            //  Set general module values
            // --------------------------------------------------------------------------
            ImportItemTable = "Location_Import_Item";
            RealIdColumn    = "RealLocationId";
            MainTable       = "location_Master";
            MainTablePk     = "location_ID";


            try
            {
                // --------------------------------------------------------------------------
                //  Set ETL extract dictionary parameters, Set error codes for input columns
                // --------------------------------------------------------------------------
                Extract.Add("location_ID"        , new ExtractField("location_ID"          ,   2,  1, "^[0-9]+$"                                 , "int"   ,   10, "1805", "1806", "1807", "1808", "1809")); // start location_Master

                Extract.Add("Agency_ID"          , new ExtractField("location_ID"          , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Agency_Name"        , new ExtractField("location_Name"        , 0  , -1, ""                                         , "string",   80, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Agency_Active"      , new ExtractField("location_Active"      , 0  , -1, "^[01]$"                                   , "bit"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Create_Date"        , new ExtractField("Create_Date"          , 0  , -1, "^'?(19|20)[0-9][0-9]-[01][0-9]-[0-3][0-9]", "date"  ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_2_ID2"        , new ExtractField("location_ID"          , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Address1"           , new ExtractField("location_Address"     , 0  , -1, ""                                         , "string",   50, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Address2"           , new ExtractField(""                     , 0  , -1, ""                                         , "string",   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("City"               , new ExtractField("location_City"        , 0  , -1, ""                                         , "string",   50, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("State"              , new ExtractField(""                     , 0  , -1, ""                                         , "string",   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Zip"                , new ExtractField("location_Zip"         , 0  , -1, ""                                         , "string",   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Phone"              , new ExtractField("location_Phone"       , 0  , -1, ""                                         , "string",   13, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Contact_Name"       , new ExtractField("location_Contact_Name", 0  , -1, ""                                         , "string",   50, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Email_Address"      , new ExtractField("location_Email"       , 0  , -1, ""                                         , "string", 4000, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Active"             , new ExtractField("location_Active"      , 0  , -1, "^[01]$"                                   , "bit"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("County_Id"          , new ExtractField(""                     , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Agency_code"        , new ExtractField(""                     , 0  , -1, ""                                         , "string",   10, "1800", "1800", "1800", "1800", "1800"));

                Extract.Add("Level_1_ID"         , new ExtractField("location_ID"          , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_1_name"       , new ExtractField("location_Name"        , 0  , -1, ""                                         , "string",  128, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_1_active"     , new ExtractField("location_Active"      , 0  , -1, "^[01]$"                                   , "bit"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_1_create_date", new ExtractField("Level_1_create_date"  , 0  , -1, "^'?(19|20)[0-9][0-9]-[01][0-9]-[0-3][0-9]", "date"  ,   10, "1800", "1800", "1800", "1800", "1800"));

                Extract.Add("Level_2_ID"         , new ExtractField("location_ID"          , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_1_ID2"        , new ExtractField(""                     , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_2_name"       , new ExtractField("location_Name"        , 0  , -1, ""                                         , "string",   80, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_2_active"     , new ExtractField("location_Active"      , 0  , -1, "^[01]$"                                   , "bit"   ,   10, "1800", "1800", "1800", "1800", "1800"));
                Extract.Add("Level_21_createDate", new ExtractField("Create_Date"          , 0  , -1, "^'?(19|20)[0-9][0-9]-[01][0-9]-[0-3][0-9]", "date"  ,   10, "1800", "1800", "1800", "1800", "1800"));

                Extract.Add("Agency_ID1"         , new ExtractField("location_ID"          , 0  , -1, "^[0-9]+$"                                 , "int"   ,   10, "1815", "1816", "1817", "1818", "1819"));
                Extract.Add("Agency_ID2"         , new ExtractField("Agency_ID"            ,  1 ,  2, "^[0-9]+$"                                 , "int"   ,   10, "1825", "1826", "1827", "1828", "1829"));
                Extract.Add("location_Name"      , new ExtractField("location_Name"        ,   2,  8, ""                                         , "string",   80, "1835", "1836", "1837", "1838", "1839"));
                Extract.Add("location_Address"   , new ExtractField("location_Address"     ,  1 ,  9, ""                                         , "string",   50, "1845", "1846", "1847", "1848", "1849"));
                Extract.Add("location_City"      , new ExtractField("location_City"        ,  1 , 10, ""                                         , "string",   50, "1855", "1856", "1857", "1858", "1859"));
                Extract.Add("location_Zip"       , new ExtractField("location_Zip"         ,  1 , 11, "^[0-9]{5}(-[0-9]{4})?$"                   , "string",   10, "1865", "1866", "1867", "1868", "1869"));
                Extract.Add("location_Phone"     , new ExtractField("location_Phone"       ,  1 ,  7, "^[-0-9.() extEXT]+$"                      , "string",   13, "1875", "1876", "1877", "1878", "1879"));
                Extract.Add("locationContactName", new ExtractField("location_Contact_Name",  1 , 12, ""                                         , "string",   50, "1885", "1886", "1887", "1888", "1889"));
                Extract.Add("location_Active"    , new ExtractField("location_Active"      ,   2,  5, "^[01]$"                                   , "bit"   ,    1, "1895", "1896", "1897", "1898", "1899"));
                Extract.Add("location_Local_ID"  , new ExtractField("location_Local_ID"    ,  1 ,  3, "^[0-9]+$"                                 , "int"   ,   10, "1905", "1906", "1907", "1908", "1909"));
                Extract.Add("locationCreateDate" , new ExtractField("Create_Date"          ,  1 ,  6, "^'?(19|20)[0-9][0-9]-[01][0-9]-[0-3][0-9]", "date"  ,   24, "1915", "1916", "1917", "1918", "1919"));
                Extract.Add("location_Email"     , new ExtractField("location_Email"       ,  1 , 13, ""                                         , "string", 4000, "1925", "1926", "1927", "1928", "1929"));
                Extract.Add("Network_ID"         , new ExtractField("Network_ID"           ,  1 ,  4, "^[0-9]+$"                                 , "int"   ,   10, "1935", "1936", "1937", "1938", "1939"));
                Extract.Add("locationCountyId"   , new ExtractField(""                     , 0  , -1, ""                                         , "string",   10, "1945", "1946", "1947", "1948", "1949"));
                Extract.Add("locationState"      , new ExtractField(""                     , 0  , -1, ""                                         , "string",    2, "1955", "1956", "1957", "1958", "1959"));

                ExtractField field = Extract["locationState"];
            }
            catch { }


            try
            {
                // --------------------------------------------------------------------------
                //  Set ETL archive dictionary parameters
                // --------------------------------------------------------------------------
                Mirror.Add("LocationImportId"   , new MirrorField("Location_Import_ID"    , "@LocationImportId"   , true , "int"   ,   10));
                Mirror.Add("ImportSummaryId"    , new MirrorField("Import_Summary_ID"     , "@ImportSummaryId"    , true , "int"   ,   10));
                Mirror.Add("ImportStatus"       , new MirrorField("Import_Status"         , "@ImportStatus"       , true , "string",   16));
                                                
                Mirror.Add("DB_Location_ID"     , new MirrorField("location_ID"           , "@location_ID"        , true , "int"   ,   10)); // location_ID
              //Mirror.Add("location_ID"        , new MirrorField("location_ID"           , "@location_ID"        , true , "int"   ,   10)); // location_ID

                //Mirror.Add("Agency_ID"          , new MirrorField("location_ID"          , "@Agency_ID"          , true , "int"   ,   10));
                //Mirror.Add("Agency_Name"        , new MirrorField("location_Name"        , "@Agency_Name"        , true , "string",   80));
                //Mirror.Add("Agency_Active"      , new MirrorField("location_Active"      , "@Agency_Active"      , true , "bit"   ,   10));
                //Mirror.Add("Create_Date"        , new MirrorField("Create_Date"          , "@Create_Date"        , true , "date"  ,   10));
                //Mirror.Add("Level_2_ID2"        , new MirrorField("location_ID"          , "@Level_2_ID2"        , true , "int"   ,   10));
                //Mirror.Add("Address1"           , new MirrorField("location_Address"     , "@Address1"           , true , "string",   50));
                //Mirror.Add("Address2"           , new MirrorField(""                     , "@Address2"           , true , "string",   10));
                //Mirror.Add("City"               , new MirrorField("location_City"        , "@City"               , true , "string",   50));
                //Mirror.Add("State"              , new MirrorField(""                     , "@State"              , true , "string",   10));
                //Mirror.Add("Zip"                , new MirrorField("location_Zip"         , "@Zip"                , true , "string",   10));
                //Mirror.Add("Phone"              , new MirrorField("location_Phone"       , "@Phone"              , true , "string",   13));
                //Mirror.Add("Contact_Name"       , new MirrorField("location_Contact_Name", "@Contact_Name"       , true , "string",   50));
                //Mirror.Add("Email_Address"      , new MirrorField("location_Email"       , "@Email_Address"      , true , "string", 4000));
                //Mirror.Add("Active"             , new MirrorField("location_Active"      , "@Active"             , true , "bit"   ,   10));
                //Mirror.Add("County_Id"          , new MirrorField(""                     , "@County_Id"          , true , "int"   ,   10));
                //Mirror.Add("Agency_code"        , new MirrorField(""                     , "@Agency_code"        , true , "string",   10));

                //Mirror.Add("Level_1_ID"         , new MirrorField("location_ID"          , "@Level_1_ID"         , true , "int"   ,   10));
                //Mirror.Add("Level_1_name"       , new MirrorField("location_Name"        , "@Level_1_name"       , true , "string",  128));
                //Mirror.Add("Level_1_active"     , new MirrorField("location_Active"      , "@Level_1_active"     , true , "bit"   ,   10));
                //Mirror.Add("Level_1_create_date", new MirrorField("Level_1_create_date"  , "@Level_1_create_date", true , "date"  ,   10));

                //Mirror.Add("Level_2_ID"         , new MirrorField("location_ID"          , "@Level_2_ID"         , true , "int"   ,   10));
                //Mirror.Add("Level_1_ID2"        , new MirrorField(""                     , "@Level_1_ID2"        , true , "int"   ,   10));
                //Mirror.Add("Level_2_name"       , new MirrorField("location_Name"        , "@Level_2_name"       , true , "string",   80));
                //Mirror.Add("Level_2_active"     , new MirrorField("location_Active"      , "@Level_2_active"     , true , "bit"   ,   10));
                //Mirror.Add("Level_21_createDate", new MirrorField("Create_Date"          , "@Level_21_createDate", true , "date"  ,   10));

                //Mirror.Add("Agency_ID1"         , new MirrorField("location_ID"          , "@Agency_ID1"         , true , "int"   ,   10));
                Mirror.Add("Agency_ID2"         , new MirrorField("Agency_ID"            , "@Agency_ID2"         , true , "int"   ,   10)); // Agency_ID
                Mirror.Add("location_Name"      , new MirrorField("location_Name"        , "@location_Name"      , true , "string",   80)); // location_Name
                Mirror.Add("location_Address"   , new MirrorField("location_Address"     , "@location_Address"   , true , "string",   50)); // location_Address
                Mirror.Add("location_City"      , new MirrorField("location_City"        , "@location_City"      , true , "string",   50)); // location_City
                Mirror.Add("location_Zip"       , new MirrorField("location_Zip"         , "@location_Zip"       , true , "string",   10)); // location_Zip
                Mirror.Add("location_Phone"     , new MirrorField("location_Phone"       , "@location_Phone"     , true , "string",   13)); // location_Phone
                Mirror.Add("locationContactName", new MirrorField("location_Contact_Name", "@locationContactName", true , "string",   50)); // location_Contact_Name
                Mirror.Add("location_Active"    , new MirrorField("location_Active"      , "@location_Active"    , true , "bit"   ,    1)); // location_Active
                Mirror.Add("location_Local_ID"  , new MirrorField("location_Local_ID"    , "@location_Local_ID"  , true , "int"   ,   10)); // location_Local_ID
                Mirror.Add("locationCreateDate" , new MirrorField("Create_Date"          , "@locationCreateDate" , true , "date"  ,   24)); // Create_Date
                Mirror.Add("location_Email"     , new MirrorField("location_Email"       , "@location_Email"     , true , "string", 4000)); // location_Email;
                Mirror.Add("Network_ID"         , new MirrorField("Network_ID"           , "@Network_ID"         , true , "int"   ,   10)); // Network_ID
                //Mirror.Add("locationCountyId"   , new MirrorField(""                     , "@locationCountyId"   , true , "string",   10))
                //Mirror.Add("locationState"      , new MirrorField(""                     , "@locationState"      , true , "string",    2));
            }
            catch { }


            try
            {
                // --------------------------------------------------------------------------
                //  Set ETL transform dictionary parameters
                // --------------------------------------------------------------------------
                Transform.Add("LocationImportId"   , new TransformField("N", "I", "N"        , ""          , ""                   , "LocationImportId"   , ""                   ));
                Transform.Add("ImportSummaryId"    , new TransformField("N", "" , "N"        , ""          , ""                   , "ImportSummaryId"    , ""                   ));
                Transform.Add("ImportStatus"       , new TransformField("N", "" , "N"        , ""          , ""                   , "ImportStatus"       , ""                   ));
                                                                                                                       
                Transform.Add("location_ID"        , new TransformField("K", "R", "I"+"K"    , ""          , "location_ID"        , "DB_Location_ID"     , "location_ID"        ));

                Transform.Add("Agency_ID"          , new TransformField("D", "N", "I"+"K"    , ""          , "Agency_ID"          , ""                   , "Agency_ID"          ));
                Transform.Add("Agency_Name"        , new TransformField("D", "N", ""         , ""          , "Agency_Name"        , ""                   , "Agency_Name"        ));
                Transform.Add("Agency_Active"      , new TransformField("D", "N", ""         , ""          , "Agency_Active"      , ""                   , "Agency_Active"      ));
                Transform.Add("Create_Date"        , new TransformField("D", "N", ""         , ""          , "Create_Date"        , ""                   , "Create_Date"        ));
                Transform.Add("Level_2_ID2"        , new TransformField("D", "N", ""         , ""          , "Level_2_ID2"        , ""                   , "Level_2_ID2"        ));
                Transform.Add("Address1"           , new TransformField("D", "N", ""         , ""          , "Address1"           , ""                   , "Address1"           ));
                Transform.Add("Address2"           , new TransformField("N", "N", "B"        , ""          , "Address2"           , ""                   , "Address2"           ));
                Transform.Add("City"               , new TransformField("D", "N", ""         , ""          , "City"               , ""                   , "City"               ));
                Transform.Add("State"              , new TransformField("N", "N", "V"        , "VA"        , "State"              , ""                   , "State"              ));
                Transform.Add("Zip"                , new TransformField("D", "N", ""         , ""          , "location_Zip"       , ""                   , "Zip"                ));
                Transform.Add("Phone"              , new TransformField("D", "N", ""         , ""          , "Phone"              , ""                   , "Phone"              ));
                Transform.Add("Contact_Name"       , new TransformField("D", "N", ""         , ""          , "Contact_Name"       , ""                   , "Contact_Name"       ));
                Transform.Add("Email_Address"      , new TransformField("D", "N", ""         , ""          , "Email_Address"      , ""                   , "Email_Address"      ));
                Transform.Add("Active"             , new TransformField("D", "N", ""         , ""          , "Active"             , ""                   , "Active"             ));
                Transform.Add("County_Id"          , new TransformField("N", "N", "B"        , ""          , "County_Id"          , ""                   , "County_Id"          ));
                Transform.Add("Agency_code"        , new TransformField("N", "N", "B"        , ""          , "Agency_code"        , ""                   , "Agency_code"        ));

                Transform.Add("Level_1_ID"         , new TransformField("D", "N", "I"+"K"+"V", "55"        , "Level_1_ID"         , ""                   , "Level_1_ID"         ));
                Transform.Add("Level_1_name"       , new TransformField("D", "N", "V"        , "Virginia"  , "Level_1_name"       , ""                   , "Level_1_name"       ));
                Transform.Add("Level_1_active"     , new TransformField("D", "N", "V"        , "1"         , "Level_1_active"     , ""                   , "Level_1_active"     ));
                Transform.Add("Level_1_create_date", new TransformField("D", "N", "V"        , "3/17/2015" , "Level_1_create_date", ""                   , "Level_1_create_date"));

                Transform.Add("Level_2_ID"         , new TransformField("D", "N", "I"+"K"    , ""          , "Level_2_ID"         , ""                   , "Level_2_ID"         ));
                Transform.Add("Level_1_ID2"        , new TransformField("N", "N", "V"        , "55"        , "Level_1_ID2"        , ""                   , "Level_1_ID2"        ));
                Transform.Add("Level_2_name"       , new TransformField("D", "N", ""         , ""          , "Level_2_name"       , ""                   , "Level_2_name"       ));
                Transform.Add("Level_2_active"     , new TransformField("D", "N", ""         , ""          , "Level_2_active"     , ""                   , "Level_2_active"     ));
                Transform.Add("Level_21_createDate", new TransformField("D", "N", ""         , ""          , "Level_21_createDate", ""                   , "Level_21_createDate"));

                Transform.Add("Agency_ID1"         , new TransformField("D", "N", ""         , ""          , "location_ID"        , ""                   , "Agency_ID1"         ));
                Transform.Add("Agency_ID2"         , new TransformField("" , "" , "N"        , ""          , "Agency_ID2"         , "Agency_ID2"         , "Agency_ID2"         ));
                Transform.Add("location_Name"      , new TransformField("" , "" , ""         , ""          , "location_Name"      , "location_Name"      , "location_Name"      ));
                Transform.Add("location_Address"   , new TransformField("" , "" , ""         , ""          , "location_Address"   , "location_Address"   , "location_Address"   ));
                Transform.Add("location_City"      , new TransformField("" , "" , ""         , ""          , "location_City"      , "location_City"      , "location_City"      ));
                Transform.Add("location_Zip"       , new TransformField("" , "" , ""         , ""          , "location_Zip"       , "location_Zip"       , "location_Zip"       ));
                Transform.Add("location_Phone"     , new TransformField("" , "" , ""         , ""          , "location_Phone"     , "location_Phone"     , "location_Phone"     ));
                Transform.Add("locationContactName", new TransformField("" , "" , ""         , ""          , "locationContactName", "locationContactName", "locationContactName"));
                Transform.Add("location_Active"    , new TransformField("" , "" , ""         , ""          , "location_Active"    , "location_Active"    , "location_Active"    ));
                Transform.Add("location_Local_ID"  , new TransformField("" , "" , ""         , ""          , "location_Local_ID"  , "location_Local_ID"  , "location_Local_ID"  ));
                Transform.Add("locationCreateDate" , new TransformField("" , "" , ""         , ""          , "locationCreateDate" , "locationCreateDate" , "locationCreateDate" ));
                Transform.Add("location_Email"     , new TransformField("" , "" , ""         , ""          , "location_Email"     , "location_Email"     , "location_Email"     ));
                Transform.Add("Network_ID"         , new TransformField("" , "" , ""         , ""          , "Network_ID"         , "Network_ID"         , "Network_ID"         ));
                Transform.Add("locationCountyId"   , new TransformField("N", "N", ""         , ""          , "locationCountyId"   , ""                   , "locationCountyId"   ));
                Transform.Add("locationState"      , new TransformField("" , "N", "V"        , "VA"        , "State"              , ""                   , "locationState"      ));
            }
            catch { }
                                                                                             

            try
            {
                // --------------------------------------------------------------------------
                //  Set ETL load dictionary parameters
                // --------------------------------------------------------------------------
                Load.Add("Agency_ID"            , new LoadField("Agency_Master"  , "Agency_ID"            ,   2, "@Agency_ID"            , "int"   ,   10));
                Load.Add("Agency_Name"          , new LoadField("Agency_Master"  , "Agency_Name"          ,   2, "@Agency_Name"          , "string", 4000));
                Load.Add("Agency_Active"        , new LoadField("Agency_Master"  , "Agency_Active"        ,   2, "@Agency_Active"        , "bit"   ,   10));
                Load.Add("Create_Date"          , new LoadField("Agency_Master"  , "Create_Date"          ,   2, "@Create_Date"          , "date"  ,   10));
                Load.Add("Level_2_ID2"          , new LoadField("Agency_Master"  , "Level_2_ID"           ,  1 , "@Level_2_ID2"          , "int"   ,   10));
                Load.Add("Address1"             , new LoadField("Agency_Master"  , "Address1"             ,  1 , "@Address1"             , "string",  100));
                Load.Add("Address2"             , new LoadField("Agency_Master"  , "Address2"             ,  1 , "@Address2"             , "string",  100));
                Load.Add("City"                 , new LoadField("Agency_Master"  , "City"                 ,  1 , "@City"                 , "string",  100));
                Load.Add("State"                , new LoadField("Agency_Master"  , "State"                ,  1 , "@State"                , "string",    2));
                Load.Add("Zip"                  , new LoadField("Agency_Master"  , "Zip"                  ,  1 , "@Zip"                  , "string",   10));
                Load.Add("Phone"                , new LoadField("Agency_Master"  , "Phone"                ,  1 , "@Phone"                , "string",   20));
                Load.Add("Contact_Name"         , new LoadField("Agency_Master"  , "Contact_Name"         ,  1 , "@Contact_Name"         , "string",  100));
                Load.Add("Email_Address"        , new LoadField("Agency_Master"  , "Email_Address"        ,  1 , "@Email_Address"        , "string",  100));
                Load.Add("Active"               , new LoadField("Agency_Master"  , "Active"               ,   2, "@Active"               , "bit"   ,    1));
                Load.Add("County_Id"            , new LoadField("Agency_Master"  , "County_Id"            ,  1 , "@County_Id"            , "int"   ,   10));
                Load.Add("Agency_code"          , new LoadField("Agency_Master"  , "Agency_code"          ,  1 , "@Agency_code"          , "string",   10));

                Load.Add("Level_1_ID"           , new LoadField("Level_1"        , "Level_1_ID"           ,   2, "@Level_1_ID"           , "int"   ,   10));
                Load.Add("Level_1_name"         , new LoadField("Level_1"        , "Level_1_name"         ,   2, "@Level_1_name"         , "string",  100));
                Load.Add("Level_1_active"       , new LoadField("Level_1"        , "Level_1_active"       ,   2, "@Level_1_active"       , "bit"   ,    1));
                Load.Add("Level_1_create_date"  , new LoadField("Level_1"        , "Level_1_create_date"  ,   2, "@Level_1_create_date"  , "date"  ,   10));

                Load.Add("Level_2_ID"           , new LoadField("Level_2"        , "Level_2_ID"           ,   2, "@Level_2_ID"           , "int"   ,   10));
                Load.Add("Level_1_ID2"          , new LoadField("Level_2"        , "Level_1_ID"           ,   2, "@Level_1_ID2"          , "int"   ,   10));
                Load.Add("Level_2_name"         , new LoadField("Level_2"        , "Level_2_name"         ,   2, "@Level_2_name"         , "string",  100));
                Load.Add("Level_2_active"       , new LoadField("Level_2"        , "Level_2_active"       ,   2, "@Level_2_active"       , "bit"   ,    1));
                Load.Add("Level_21_createDate"  , new LoadField("Level_2"        , "Level_21_create_date" ,   2, "@Level_21_createDate"  , "date"  ,   24));

                Load.Add("location_ID"          , new LoadField("location_Master", "location_ID"          ,   2, "@location_ID"          , "int"   ,   10));
                Load.Add("Agency_ID1"           , new LoadField("location_Master", "Agency_ID"            ,  1 , "@Agency_ID1"           , "int"   ,   10));
                Load.Add("Agency_ID2"           , new LoadField("location_Master", "Agency_ID"            , 0  , "@Agency_ID2"           , "int"   ,   10));
                Load.Add("location_Name"        , new LoadField("location_Master", "location_Name"        ,   2, "@location_Name"        , "string",   80));
                Load.Add("location_Address"     , new LoadField("location_Master", "location_Address"     ,  1 , "@location_Address"     , "string",   50));
                Load.Add("location_City"        , new LoadField("location_Master", "location_City"        ,  1 , "@location_City"        , "string",   50));
                Load.Add("location_Zip"         , new LoadField("location_Master", "location_Zip"         ,  1 , "@location_Zip"         , "string",   10));
                Load.Add("location_Phone"       , new LoadField("location_Master", "location_Phone"       ,  1 , "@location_Phone"       , "string",   13));
                Load.Add("locationContactName"  , new LoadField("location_Master", "location_Contact_Name",  1 , "@locationContactName"  , "string",   50));
                Load.Add("location_Active"      , new LoadField("location_Master", "location_Active"      ,   2, "@location_Active"      , "bit"   ,    1));
                Load.Add("location_Local_ID"    , new LoadField("location_Master", "location_Local_ID"    ,  1 , "@location_Local_ID"    , "int"   ,   10));
                Load.Add("locationCreateDate"   , new LoadField("location_Master", "Create_Date"          ,  1 , "@Create_Date"          , "date"  ,   24));
                Load.Add("location_Email"       , new LoadField("location_Master", "location_Email"       ,  1 , "@location_Email"       , "string", 4000));
                Load.Add("Network_ID"           , new LoadField("location_Master", "Network_ID"           ,  1 , "@Network_ID"           , "int"   ,   10));
                Load.Add("locationCountyId"     , new LoadField("location_Master", "County_Id"            ,  1 , "@County_Id"            , "int"   ,   10));
                Load.Add("locationState"        , new LoadField("location_Master", "[State]"              ,  1 , "@State"                , "string",   50));
            }
            catch { }

            this.CheckDictionaryConnections();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CheckElementExists -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="exportIdColumName"></param>
        /// <param name="id"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected bool CheckElementExists(string tableName, string exportIdColumName, object id, InfoAspect aspect)
        {
            bool found = false;
            if (!InData.IsNull(id))
            {
                try
                {
                    string query = " SELECT * FROM "+tableName+" WHERE "+exportIdColumName+" = " + id.ToString(); // TODO: parameterize this
                    DataTable table = InData.GetTable(tableName, query, aspect.SecondaryConnection);
                    found = (table.Rows.Count > 0);
                }
                catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }
            else
                throw new DataException("null key not findable");
            return found;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ElementExists -->
        /// <summary>
        ///      Looks for element exists on direct id transfer transformations (instructions)
        /// </summary>
        /// <param name="module"></param>
        /// <param name="field"></param>
        /// <param name="tableName"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        protected bool ElementExists(string tableName, DataRow field, InfoAspect aspect)
        {
            bool exists = false;

            try
            {
                if (tableName == "Level_1") // "V"
                {
                    TransformField transform = Transform["Level_1_ID"];
                    SqlInt32 extractPkValue  = InData.GetSqlInt32(transform.XrefLookup);
                    string   loadPkColumn    = ColumnName_LoadId (tableName, aspect.Enter()); aspect--;
                    exists                   = CheckElementExists(tableName, loadPkColumn, extractPkValue, aspect.Enter()); aspect--;
                }
                else
                {
                    // ------------------------------------------------------------------
                    //  The normal direct 1 to 1 conversion case
                    // ------------------------------------------------------------------
                    string   extractPkColumn = ColumnName_ExtractPkFromLoad(tableName, aspect.Enter()); aspect--;
                    SqlInt32 extractPkValue  = InData.GetSqlInt32(field, extractPkColumn);
                    string   loadPkColumn    = ColumnName_LoadId  (tableName, aspect.Enter()); aspect--;
                    exists                   = CheckElementExists(tableName, loadPkColumn, extractPkValue, aspect.Enter()); aspect--;
                }
            }
            catch (Exception ex) { Pause(); aspect.Rethrow(ex); }

            return exists;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Import -->
        /// <summary>
        ///      Imports a DataTable of location elements into one of the location tables
        /// </summary>
        /// <param name="importData"></param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public DataTable Import(DataTable importData, string tableName, InfoAspect aspect)
        {
            // --------------------------------------------------------------------------
            //  Initialize variables
            // --------------------------------------------------------------------------
            DataTable importedData = ImportDalCommon.InitializeImportedDataTable(importData, tableName+"DbId", aspect.Enter()); aspect--; // the output table
            string    insertQuery  = InsertQuery(tableName, aspect.Enter()); aspect--;
            string    updateQuery  = UpdateQuery(tableName, aspect.Enter()); aspect--;


            for (int row = 0; row < importData.Rows.Count; ++row)
            {
                DataRow field = importData.Rows[row];
                try
                {
                    // ------------------------------------------------------------------
                    //  Insert or Update a valid location record in one of four location tables
                    // ------------------------------------------------------------------
                    bool isValid = (bool)field["IsValid"];
                    SqlInt32 id = SqlInt32.Null;
                    if (isValid)
                    {
                        bool isUpdate = (bool)field["IsUpdate"];
                        if (!isUpdate) { isUpdate = ElementExists(      tableName, field, aspect.Enter()); aspect--; }
                        if (isUpdate)  { id       = Update(             tableName, field, importedData, aspect.Enter(tableName,"Update")); aspect--; }
                        else           { id       = Insert(insertQuery, tableName, field, importedData, aspect.Enter(tableName,"Insert")); aspect--; }
                    }
                }
                catch (Exception ex) { Pause(); aspect.Rethrow(ex); }
            }

            return importedData;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Insert -->
        /// <summary>
        ///      If the location element does not exist then insert into one of the four location tables
        /// </summary>
        /// <param name="insertQuery"></param>
        /// <param name="field"></param>
        /// <param name="importedData">adds row to this table if the location element is actually saved</param>
        /// <param name="aspect"></param>
        /// <returns></returns>
        private SqlInt32 Insert(string insertQuery, string tableName, DataRow field, DataTable importedData, InfoAspect aspect)
        {
            SqlInt32 id = SqlInt32.Null;
            object obj = null;

            using (SqlCommand cmd = new SqlCommand(insertQuery, aspect.SecondaryConnection))
            {
                try
                {
                    // ------------------------------------------------------------------
                    //  Fill the command
                    // ------------------------------------------------------------------
                  //InsertQueryFill(cmd, tableName, field, aspect.Enter(Name,"QueryFill")); aspect--;
                    foreach (string transformKey in Transform.Keys)
                    {
                        AddParameterToLoad(cmd, tableName, field, transformKey, aspect.Enter()); aspect--;
                    }


                    // ------------------------------------------------------------------
                    //  Sanity check
                    // ------------------------------------------------------------------
                    string err = ParameterSanityCheck(cmd);
                    if (!string.IsNullOrEmpty(err.Trim()))
                        throw new Exception(err);


                    // ------------------------------------------------------------------
                    //  Perform the insert
                    // ------------------------------------------------------------------
                    obj = ExecuteScalar(cmd, tableName, aspect.Enter()); aspect--;


                    string report = InData.AsciiNewQuery(cmd);


                    if (obj != null)
                    {
                        id = InData.GetSqlInt32(obj);


                        // ------------------------------------------------------------------
                        //  Record the insert
                        // ------------------------------------------------------------------
                        if (id > 0)
                        {
                            int row = importedData.Rows.Count;
                            importedData.ImportRow(field);
                            importedData.Rows[row][tableName + "DbId"] = id;
                            field[RealIdColumn] = id;
                        }
                    }
                    else
                    {
                        string report2 = InData.AsciiNewQuery(cmd);
                    }
                }
                catch (SqlException ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
                catch (Exception    ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
                finally
                {
                    // ------------------------------------------------------------------
                    //  Make sure the database identity structure is set properly
                    // ------------------------------------------------------------------
                    string     query = " SET IDENTITY_INSERT ["+tableName+"] OFF;";
                    SqlCommand reset = new SqlCommand(query, aspect.SecondaryConnection);
                    reset.ExecuteNonQuery();
                }
            }

            return id;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Update -->
        /// <summary>
        ///      Updates a row in one of the location related tables
        /// </summary>
        /// <param name="updateQuery"></param>
        /// <param name="field"></param>
        /// <param name="importedData"></param>
        /// <param name="aspect"></param>
        private SqlInt32 Update(string tableName, DataRow field, DataTable importedData, InfoAspect aspect)
        {
            if (tableName == "Instructions")
            {
                string updateQuery = "";
                object obj = null;

                using (SqlCommand cmd = new SqlCommand(updateQuery, aspect.SecondaryConnection))
                {
                    try
                    {

                        // --------------------------------------------------------------
                        //  Fill the command
                        // --------------------------------------------------------------
                      //UpdateQueryFill(cmd, tableName, field, aspect.Enter(Name, "QueryFill")); aspect--;
                        foreach (string transformKey in Transform.Keys)
                        {
                            AddParameterToLoad(cmd, tableName, field, transformKey, aspect.Enter()); aspect--;
                        }


                        // --------------------------------------------------------------
                        //  Perform the insert
                        // --------------------------------------------------------------
                        obj = ExecuteScalar(cmd, tableName, aspect.Enter()); aspect--;

                        string report = InData.AsciiNewQuery(cmd);


                        if (obj != null)
                        {
                            SqlInt32 id = (SqlInt32)obj;


                            // ------------------------------------------------------------------
                            //  Record the insert
                            // ------------------------------------------------------------------
                            if (id > 0)
                            {
                                int row = importedData.Rows.Count;
                                importedData.ImportRow(field);
                                importedData.Rows[row][tableName + "DbId"] = id;
                                field[RealIdColumn] = id;
                            }
                        }
                        else
                        {
                            string report2 = InData.AsciiNewQuery(cmd);
                        }
                    }
                    catch (SqlException ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
                    catch (Exception    ex) { Pause(); string report = InData.AsciiNewQuery(cmd); aspect.Rethrow(ex); }
                }

            }
            return 0;
        }
    }
}
