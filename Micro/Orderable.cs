//-------------------------------------------------------------------------------------------------
// This file is part of the InfoLibCsLesserGpl version of Infolib.
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
//-------------------------------------------------------------------------------------------------
using InfoLib.Data   ;  // for EndemeAccess
using InfoLib.Endemes;  // for EndemeSet, InfoAspect
using System         ;  // for Guid

namespace InfoLib.Micro
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Orderable -->
    /// <summary>
    ///      The Orderable class contains orderable endeme sets (all endemes are orderings of endeme sets)
    /// </summary>
    public class Orderable
    {
        // ----------------------------------------------------------------------------------------
        //  Constants
        // ----------------------------------------------------------------------------------------
        public const string MentalCharacteristicsLabel = "Mental Characteristics";


        // ----------------------------------------------------------------------------------------
        /// <!-- MentalCharacteristics -->
        /// <summary>
        ///      Contains user access characteristics:
        ///      A)dd, D)elete, E)dit, L)ogs, N)one, O)rg, R)ead, S)ub-, T)op, U)ser etc.
        /// </summary>
        /// <remarks>
        ///      Order is important
        ///      Try/catch to makes things more thread-safe
        /// </remarks>
        public static EndemeSet MentalCharacteristics
        { get { if (             _mind == null ||
                                 _mind.Count < 1)                                                               //ABCDEFGHIJKLMNOPQRSTUV|   Characteristic  Description
            { try {              _mind = new EndemeSet(MentalCharacteristicsLabel, "MIND", true);               //----------------------+----------------------------------------------
                                 _mind.Add('A', "ADVOCATE", "Advocacy"       , "making something look good ");  //A]                    |A. Advocacy        making something look good 
                                 _mind.Add('B', "BRAVE"   , "Bravery"        , "taking and dealing pressure");  //[B]    -I-    -P-     |B. Bravery         taking and dealing pressure
                                 _mind.Add('C', "CREATE"  , "Creativity"     , "making something unthought ");  // [C]   -I-            |C. Creativity      making something unthought 
                                 _mind.Add('D', "DEDUCEIQ", "Deductive IQ"   , "reason from prcpl to detail");  //  [D]     -L-         |D. Deductive IQ    reason from prcpl to detail
                                 _mind.Add('E', "EQ"      , "Emotional IQ"   , "seeing and knowing emotions");  //   [E]                |E. Emotional IQ    seeing and knowing emotions
                                 _mind.Add('F', "FINANCE" , "Financial IQ"   , "manage budget and resources");  //-B- [F]    -M-        |E. Financial IQ    manage budget and resources
                                 _mind.Add('G', "GROWTH"  , "Growth"         , "teaching, learning, growing");  //     [G]  -L-     -T- |G. Growth          teaching, learning, growing
                                 _mind.Add('H', "PRESENCE", "Presence"       , "ontological effectiveness  ");  //      <H>    -OP-     |E. Presence        ontological effectiveness  
                                 _mind.Add('I', "INDUCEIQ", "Inductive IQ"   , "reason from detail to prcpl");  //  -D-  [I]            |I. Inductive IQ    reason from detail to prcpl
                                 _mind.Add('J', "PHYSICAL", "Physical IQ"    , "body strength and precision");  //        <J>   -P-     |E. Physical IQ     body strength and precision
                                 _mind.Add('K', "KINETIC" , "Kinetic IQ"     , "multi-tasking and awareness");  //         [K]M-        |K. Kinetic IQ      multi-tasking and awareness
                                 _mind.Add('L', "LANGUAGE", "Language"       , "using language effectively ");  //          [L]         |L. Language        using language effectively 
                                 _mind.Add('M', "MECHANIC", "Mechanical IQ"  , "hand, tool and machine use ");  //           [M]        |E. Mechanical IQ   hand, tool and machine use 
                                 _mind.Add('N', "NETWORK" , "Network/Nav"    , "finding way through network");  //            [N]       |N. Network/Nav     finding way through network
                                 _mind.Add('O', "ORGANIZE", "Organize People", "assignnemnt and feedback   ");  //             [O]      |O. Organize People assignnemnt and feedback   
                                 _mind.Add('P', "PRACTICE", "Practicality"   , "doing, expedience, politics");  //    -E-       [P]     |P. Practicality    doing, expedience, politics
                                 _mind.Add('Q', "RECOGNIZ", "Recognition"    , "seeing useful applications ");  //A-         -M- <Q>R-  |Q. Recognition     seeing useful applications 
                                 _mind.Add('R', "REASON"  , "Reason"         , "reason from cause to effect");  //       -I-      [R]   |R. Reason          reason from cause to effect
                                 _mind.Add('S', "SERVICE" , "Service"        , "knowing and meeting needs  ");  //      -H-        [S]  |S. Service         knowing and meeting needs  
                                 _mind.Add('T', "TEAMWORK", "Teamwork"       , "group influence/interaction");  //     -G-          [T] |T. Teamwork        group influence/interaction
                                 _mind.Add('U', "UTILITY" , "Utility"        , "filling and filing forms   ");  //             -O- -T[U]|U. Utility         filling and filing forms   
                                 _mind.Add('V', "VISUAL"  , "Visual IQ"      , "graphic, 3d, volumetric IQ ");  //                    [V|V. Visual IQ       graphic, 3d, volumetric IQ 
            } catch { } } return _mind; } }                                                                     //----------------------+----------------------------------------------
        private static EndemeSet _mind = null;                                                                  //ABCDEFGHIJKLMNOPQRSTUV|


        // ------------------------------------------------------------------------------
        //  Methods
        // ------------------------------------------------------------------------------
        public  static Guid      MentalCharacteristics_id     (InfoAspect aspect) {               return new EndemeAccess().InEndemeSetOfEndemeSetLabel(MentalCharacteristicsLabel, aspect).GuidValue(0, "EndemeSetId", Guid.NewGuid()); }
        public  static int       MentalCharacteristics_update (InfoAspect aspect) { _mind = null; return new EndemeAccess().ReUpEndemeSet              (MentalCharacteristics.WithId(MentalCharacteristics_id(aspect)), aspect);         }
        public  static EndemeSet MentalCharacteristics_refresh(                 ) { _mind = null; return                                                MentalCharacteristics;                                                           } // In SOA when you cache something in a static variable you may need to add this to the application refresh code
        public  static Guid      MentalCharacteristics_insert (InfoAspect aspect) { _mind = null; return new EndemeAccess().ToteEndemeSet            (MentalCharacteristics.WithId(Guid.NewGuid()), aspect.SecondaryConn);             }
    }
}
