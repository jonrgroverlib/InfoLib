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
using InfoLib.Endemes;         // for 
using System;                         // for 
using System.Collections.Generic;     // for Dictionary
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- TransformField -->
    /// <summary>
    ///      The TransformField class maps zero or more fields from the extract to zero or one fields in the load,
    ///      A)dd only, B)lank, C)omplex, D)uplicate, E)mpty, F)K, G)rab any, I)dentity, K)ey, L)ookup,
    ///      M)eaning, N)ot used, O)ptional, P)arent, R)eal Id, S)ource Id, T)rim, U)pdate only, V)alue (literal)
    /// </summary>
    /// <remarks>alpha toy code - used once in production, expected to be deprecated</remarks>
    public class TransformField
    {
        // ----------------------------------------------------------------------------------------
        //  Proposed Members (mostly deprecated)
        //  ABCDEFGHIJKLMNOPQRSTUV|
        //  ----------------------+
        //    C         M     ST  |S. Copy/Staging/Mirror/Temp X
        //    C  F       N        |C. Column (field name)
        //      E                 |E. Error
        //       F           R    |F. Field requirement
        //          I             |I. Import X
        //             L      S   |L. Length (size)
        //              M         |M. Mapping
        //                O       |O. Order
        //       F         P      |P. Pattern (format)
        //             L     R    |R. xRef (lookup)
        //                    S   |S. Storage X
        //       F             T  |T. Table (or file)
        //                 P     V|V. Param/Variable
        //  ----------------------+
        //  ABCDEFGHIJKLMNOPQRSTUV|
        // ----------------------------------------------------------------------------------------
/*IX*/  public string ExtractKey     { get; set; }
/*CX*/  public string MirrorKey      { get; set; }
/*SX*/  public string LoadKey        { get; set; }

        public string XrefLookup     { get; set; }


        // ------------------------------------------------------------------------------
        //  Actions endeme set
        // ------------------------------------------------------------------------------
        // "A" "Add only"        , "Add/Insert only (for load)"                                             
        // "B" "Basic/blank"     , "Basic/blank - \"\" for strings, 0 for numbers, 'DateTime.Now' for dates"
        // "C" "Complex"         , "Complex"                                                                
        // "D" "Duplicate"       , "Duplicate"                                                              
        // "E" "Empty"           , "set loaded value to null"                                               
        // "F" "Foreign key"     , "Foreign key"                                                            
        // "G" "Grab Any"        , "Random lookup on table"                                                 
        // "H" ""                , ""                                                                       
        // "I" "Identity column" , "Identity column, determined by the system"                              
        // "J" ""                , ""                                                                       
        // "K" "Key, simple"     , "Key but not to be treated as identity column"                           
        // "L" "Lookup, numeric" , "Lookup table (numeric)"                                                 
        // "M" "Meaning lookup"  , "Lookup table (string)"                                                  
        // "N" "Not used"        , "Not active field or non-existent (don't insert or update)"              
        // "O" "??Optional? vs Required?", "??Optional? vs Required?"                                       
        // "P" "Parent lookup"   , "Lookup table (parent)"                                                  
        // "Q" ""                , ""                                                                       
        // "R" "Real ID"         , "database id loaded into db D?R?"                                        
        // "S" "Source ID"       , "Source key (foreign key to the extract source) - source id"             
        // "T" "Trim"            , "Trim string"                                                            
        // "U" "Update only"     , "Update only (for load)"                                                 
        // "V" "Value"           , "Literal value to set column"                                            
        // ------------------------------------------------------------------------------
        public string ExtractActions { get; set; } // unusual characteristics related to this field translation
        public string MirrorActions  { get; set; } // unusual characteristics related to this field translation
        public string LoadActions    { get; set; } // unusual characteristics related to this field translation

        public static EndemeSet ActionSet { get
        {
            if (_actionSet == null)
            {
                _actionSet = new EndemeSet("Common Transformation Operations");
                _actionSet.Add('A', "Add only"        , "Add/Insert only (for load)"                                             );
                _actionSet.Add('B', "Basic/blank"     , "Basic/blank - \"\" for strings, 0 for numbers, 'DateTime.Now' for dates");
                _actionSet.Add('C', "Complex"         , "Complex"                                                                );
                _actionSet.Add('D', "Duplicate"       , "Duplicate"                                                              );
                _actionSet.Add('E', "Empty"           , "set loaded value to null"                                               );
                _actionSet.Add('F', "Foreign key"     , "Foreign key"                                                            );
                _actionSet.Add('G', "Grab Any"        , "Random lookup on table"                                                 ); // TODO: convert this to "L"+"I"
                _actionSet.Add('H', ""                , ""                                                                       );
                _actionSet.Add('I', "Identity column" , "Identity column, determined by the system"                              );
                _actionSet.Add('J', ""                , ""                                                                       );
                _actionSet.Add('K', "Key, simple"     , "Key but not to be treated as identity column"                           );
                _actionSet.Add('L', "Lookup, numeric" , "Lookup table (numeric)"                                                 );
                _actionSet.Add('M', "Meaning lookup"  , "Lookup table (string)"                                                  );
                _actionSet.Add('N', "Not used"        , "Not active field or non-existent (don't insert or update)"              );
                _actionSet.Add('O', "??Optional? vs Required?", "??Optional? vs Required?"                                       );
                _actionSet.Add('P', "Parent lookup"   , "Lookup table (parent)"                                                  );
                _actionSet.Add('Q', ""                , ""                                                                       );
                _actionSet.Add('R', "Real ID"         , "database id loaded into db D?R?"                                        );
                _actionSet.Add('S', "Source ID"       , "Source key (foreign key to the extract source) - source id"             );
                _actionSet.Add('T', "Trim"            , "Trim string"                                                            );
                _actionSet.Add('U', "Update only"     , "Update only (for load)"                                                 );
                _actionSet.Add('V', "Value"           , "Literal value to set column"                                            );
            }
            return _actionSet;
        } }
        private static EndemeSet _actionSet;


        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public TransformField() { Init("","","","","","",""); }
        /// <summary>A)dd/Insert only, C)omplex, D)uplicate, F)oreign key, I)dentity column, K)ey not treated as identity column, L)ookup table, N)ot active, T)rim string, U)pdate only</summary>
        /// <param name="extractActions"></param>
        /// <param name="mirrorActions"></param>
        /// <param name="loadActions"></param>
        /// <param name="xrefLookup"></param>
        /// <param name="extractKey"></param>
        /// <param name="mirrorKey"></param>
        /// <param name="loadKey"></param>
        public TransformField(string extractActions, string mirrorActions, string loadActions, string xrefLookup, string extractKey, string mirrorKey, string loadKey) { Init(extractActions, mirrorActions, loadActions, xrefLookup, extractKey, mirrorKey, loadKey  ); }

        private void Init(string extractActions, string mirrorActions, string loadActions, string xrefLookup, string extractKey, string mirrorKey, string loadKey)
        {
            ExtractKey     = extractKey    ;
            MirrorKey      = mirrorKey     ;
            LoadKey        = loadKey       ;

            ExtractActions = extractActions;
            MirrorActions  = mirrorActions ;
            LoadActions    = loadActions   ;

            XrefLookup = xrefLookup;
        }
       
    }
}
