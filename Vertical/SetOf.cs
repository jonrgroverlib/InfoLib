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
using InfoLib.Endemes;         // for EndemeSet
using System;                         // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Vertical
{
    // --------------------------------------------------------------------------------------------
    /// <!-- SetOf -->
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>to be deprecated</remarks>
    public static class SetOf
    {
        public static EndemeSet ConsumerRelocators
        {
            get
            {
                if (_consumerRelocators == null)
                {
                    _consumerRelocators = new EndemeSet("Consumer Relocator");

                    _consumerRelocators.Add('A', "Assessor of consumer" , "The Consumer's assessor - users that have done an assessment for this consumer");
                    _consumerRelocators.Add('B', "Both locations"       , "someone with access to both source and destination locations"                  );
                    _consumerRelocators.Add('C', "Consumer focused"     , "someone with permissions most focused on working with consumers (modes etc)"   );
                    _consumerRelocators.Add('D', "Destination"          , "a high level supervisor associated with the destination region"                );
                    _consumerRelocators.Add('E', "Eldest employee"      , "seniority, one of the users that's been around the longest"                    );
                    _consumerRelocators.Add('F', "Flunkie"              , "very low level employee (data entry)"                                          );
                    _consumerRelocators.Add('G', "Global admin"         , "a state administrator"                                                         );
                    _consumerRelocators.Add('H', "Highest permissions"  , "one of the users with the most permissions"                                    );
                    _consumerRelocators.Add('I', "Idle user"            , "someone who is not in the middle of an assessment right now"                   );
                    _consumerRelocators.Add('J', "J-a level 2 admin"    , "subregion administrator"                                                       );
                    _consumerRelocators.Add('K', "K-a level 3 admin"    , "agency administrator or local administrator"                                   );
                    _consumerRelocators.Add('L', "Located with consumer", "one of the users in the most recent consumer location"                         );
                    _consumerRelocators.Add('M', "Movement engineer"    , "someone whose job it is to move consumers around"                              );
                    _consumerRelocators.Add('N', "Now logged in"        , "one of the users logged in right now"                                          );
                    _consumerRelocators.Add('O', "Open level admin"     , "users with location administration close to the level where access is relaxed" );
                    _consumerRelocators.Add('P', "Proper Permissions"   , "one of the users with the proper permissions to move the consumer"             );
                    _consumerRelocators.Add('Q', "Quiescent mover"      , "the person who least recently performed a move"                                );
                    _consumerRelocators.Add('R', "Recent mover"         , "the person who most recently performed a move"                                 );
                    _consumerRelocators.Add('S', "Supervisor"           , "one of the supervisors for that consumer's location"                           );
                    _consumerRelocators.Add('T', "Top level admin"      , "a level 1 administrator"                                                       );
                    _consumerRelocators.Add('U', "User log recent"      , "one of the users with the most recent user log info"                           );
                    _consumerRelocators.Add('V', "Vendor/RCR"           , "vendor/RCR"                                                                    );
                }

                return _consumerRelocators;
            }
        }
        private static EndemeSet _consumerRelocators;


        // ----------------------------------------------------------------------------------------
        /// <!-- ConsumerMergeMatchElements -->
        /// <summary>
        ///      Similarity calculation policy priorities set for merging consumers
        /// </summary>
        public static EndemeSet ConsumerMergeMatchElements
        {
            get
            {
                if (_matchPolicies == null)
                {
                    _matchPolicies = new EndemeSet("Match Policy");

                    _matchPolicies.Add('A', "Assessments", "Assessments"          );
                    _matchPolicies.Add('B', "BirthDate"  , "Birth Dates"          );
                    _matchPolicies.Add('C', "ZipCode"    , "ZipCodes"             );
                    _matchPolicies.Add('D', "DistaffName", "Mother's Maiden Names");
                    _matchPolicies.Add('E', "Ethnicity"  , "Ethnicities"          );
                    _matchPolicies.Add('F', "FirstName"  , "First Names"          );
                    _matchPolicies.Add('G', "Gender"     , "Genders"              );
                    _matchPolicies.Add('H', "HasMedicaid", "Has Medicaid YN's"    );
                    _matchPolicies.Add('I', "IdMedicaid" , "Medicaid Id's"        );
                    _matchPolicies.Add('L', "LastName"   , "Last Names"           );
                    _matchPolicies.Add('M', "MiddleName" , "Middle Names"         );
                    _matchPolicies.Add('P', "Providers"  , "Providers"            );
                    _matchPolicies.Add('R', "Race"       , "Races"                );
                    _matchPolicies.Add('S', "SystemId"   , "System Id's"          );
                }

                return _matchPolicies;
            }
        }
        private static EndemeSet _matchPolicies;
    }
}