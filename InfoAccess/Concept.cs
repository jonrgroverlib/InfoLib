//--------------------------------------------------------------------------------------------------
// This file is part of the InfoLib version of Informationlib.
//
// InfoLib is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// InfoLib is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with InfoLib.
// If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------------------------
using InfoLib.Endemes ;               // for Endeme, EndemeDefinition, EndemeItem, EndemeList, EndemeReference, EndemeSet
using InfoLib.SoftData;               // for TreatAs
using InfoLib.Strings ;               // for __.Pluralize
using System          ;               // for Console, Exception, Guid, InvalidOperationException
using System.Collections.Generic;     // for List
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Info
{
    // --------------------------------------------------------------------------------------------
    /// <!-- Concept -->
    /// <summary>
    ///      The Concept class is level 3 construct for level 4 operations to process,
    ///      A Concept is defined as a edge-vertex graph node with active content,
    ///      A Concept may have one or more one on one relationships with other Concepts,
    ///      Level 4 programming has to do with the relationships
    /// </summary>
    /// <remarks>
    ///      Sorry about the name of the class, I could not come up with anything better than concept :(
    /// 
    /// 
    ///      Level 1 programming - Performance and data storage - SQL is a good example
    ///      Level 2 programming - Precision and object orientation - C# is a good example
    ///      Level 3 programming - Meaning and information - Endemes and Fuzzy Logic are good examples
    ///      Level 4 programming - Relationships - Ontologies and Knowledge Representation are good examples
    ///      Level 5 programming - Learning and Artificial Intelligence - Data Analytics is a good example
    ///      Level 6 programming - Computational Creativity and Decision making - no good examples yet
    ///      Level 7 programming - Communities of agents - no good examples yet
    ///      
    ///      
    ///      A Concept is a level 3 programming construct that
    ///      you can perform level 4 operatorions on.
    ///      
    ///      Concept indicates multiple relationships and that it also has content, unlike a vertex
    ///      which merely indicates multiple relationships or a Concept which merely indicates content
    ///      
    ///      Natural
    ///      Level    Operations
    ///      -------  ----------
    ///      1        Child of a    (detail rows)
    ///      1        Siblings a    (children of the same parent, With a)
    ///      1        Parents a     (header rows)
    ///       2       Was a         (Inheritance past)
    ///       2       Becomes a     (Inheritance future)
    ///       2       Has a         (compoisition)
    ///       2       Is a          (Inheritance now)
    ///       2       Queries a     (Hears a, Receives a, Views a, Sees a, directional relationship getting data)
    ///       2       Answers a     (Sends a, Tells a, directional relationship sending data)
    ///        3      Like a        (Matches a)
    ///        3      Defines a     (metadata, and information based definition using EndemeDefinition)
    ///         4     Removes a     (Breaks a)
    ///         4     Operates on a (more complex than edits a)
    ///         4     Uses a        (Does a)
    ///         4     From a        (directional relationship other than sending data)
    ///         4     Masks a       (Hides a)
    ///         4     Knows a     (simplest relationship)
    ///         4     Edits a       (Modifies a / Updates a)
    ///         4     Names a       (Labels a)
    ///         4     To a          (directional relationship other than sending data, Goes to a / Points to a)
    ///         4     Justifies a   (the opposite of masks a)
    ///          5    For a         [[[Not covered by this class]]] - this requires lots of AI which is outside the scope of this class
    ///           6   Generates a   (Builds a / creates a) - factories and computational creativity and stuff
    /// 
    /// 
    ///      This class provides a platform for implementing these relationships amongst six relationship type groups:
    ///      A)nswers, B)ecomes, C)hild, D)efines, E)dits and CRUD
    ///      --------------------------------------------------------------------------------------------------
    ///      Answers a         A          sends a, tells a
    ///      Becomes a         | B        will be a, becomes a
    ///      Child of a        | | C      details a, detail of a, follows a, part of a, subordinate to a
    ///      Defines a         | | | D    metadates a, Annotates a
    ///      Edits a       [U] | | | | E  updates a, modifies a
    ///      From a            F | | | |  comes from a, points from a
    ///      Generates a   [C] | | | G |  builds a, creates a, inserts a
    ///      Has a             | | H | |  contains a
    ///      Is a              | I | | |  see becomes a (this is the standard inheritance relationship)
    ///      Justifies a       | | | J |  validates a, enables a
    ///      Knows a           | | K | |  connected to a
    ///      Like a            | | H | |  similar to a, equals a, matches a
    ///      Masks a           | | | M |  conceals a, Hides a
    ///      Names a           | | | N |  labels a, identifies a
    ///      Operates on a     | | |   O  works on a
    ///      Parents a         | | P   |  leads a, header of a, master of a
    ///      Queries a     [R] Q | |   |  receives a, hears a, views a, reads a
    ///      Removes a     [D] | | |   R  deletes a, eliminates a, cuts a
    ///      Siblings a        | | S   |  with a, part of a group with a, groups with a, sibling of a
    ///      To a              T |     |  points to a, goes to a
    ///      Uses a              |     U  operates with a, does a
    ///      Was a               V        formerly a, previously a
    /// 
    ///
    ///      An Inner Platform
    ///      -----------------
    ///      Concept is in danger of being an inner platform anti-pattern of C#
    ///      In order to avoid this it needs functionality that C# does not have
    /// </remarks>
    public class Concept : IConceptObserver, IConceptSource // , IObserver<Concept>, IObservable<Concept>
    {
        // ----------------------------------------------------------------------------------------
        //  Noun Members (nodes, vertices, Concept types) - Concepts to operate on
        // ----------------------------------------------------------------------------------------
        public string           TargetLabel  { get; set; } // names a, builds a
/* */   public string           TargetType   { get; set; } // object of an operation (the grammarian definition of the word object, not the programmer definition)
        public int?             TargetId     { get; set; } // builds a
        public Guid             TargetGuid   { get; set; } // builds a
        public object           TargetModel  { get; set; } // builds a


        // ----------------------------------------------------------------------------------------
        //  Verb Members - edges, lines, joins, connections carrying operations
        // ----------------------------------------------------------------------------------------
/* */   public string           Operation    { get; set; } // Does a (a verb) "View", "Delete", "Add", "Edit", "Input", "Read", "None"
        public List<string>     Parameters   { get; set; } // Does a
        public List<string>     CreatesType  { get; set; } // creates a, builds a (should this go under Observer?)


        // ----------------------------------------------------------------------------------------
        //  Adjective and Adverb Members - identity denotations & Concepts to modify directly
        // ----------------------------------------------------------------------------------------
        public EndemeDefinition Self         { get; set; }              //  Adjective member
        public bool?            IsTrue       { get; set; }              //  Adjective member
        public bool             IsHidden     { get; set; } // hides a   //  Adjective member
        public bool             IsFunctional { get; set; } // Breaks a  // Adverb member
        public string           Perview      { get; set; }              // Adverb member Scope?, ToWhatExtent?, WithWhatAccess? Security?, Allowance?


        // ----------------------------------------------------------------------------------------
        //  Relationship Members - edges, lines, joins, connections (most of which are subsumed into the Observer EndemeList)
        // ----------------------------------------------------------------------------------------
        public EndemeList       Friend       { get; set; } // Like a        // Level 3, separate endeme list used (as opposed to using the Observer list below) to manage match weighting
        public EndemeList       Observer     { get; set; } // (_______) a   // Level 1,2, and especially level 4 items below:
                       // -------------------------------------------------------------------------
                       //    // Observer abstracts the following lists using the Relationship set:
                       //
                       //    // Level 1
                       //    public List<Concept> Parent       { get; set; } // follows a
                       //    public List<Concept> Child        { get; set; } // Leads a
                       //    // Level 2
                       //    public List<Concept> InheritsFrom { get; set; } // Is a, Was a, Becomes a
                       //    public List<Concept> Member       { get; set; } // Has a
                       //    // Level 4
                       //    public List<Concept> Peer         { get; set; } // knows a
                       //    public List<Concept> NamesThis    { get; set; } // names a
                       //    public List<Concept> PointsTo     { get; set; } // to a, from a
                       //    public List<Concept> PointsFrom   { get; set; } // to a, from a
                       //    public List<Concept> Sibling      { get; set; } // with a
                       //    public List<Concept> BuiltBy      { get; set; } // builds a
                       //    // Level 4 CRUD
                       //    public List<Concept> Views        { get; set; } // views a
                       //    public List<Concept> Deletes      { get; set; } // deletes a
                       //    public List<Concept> Modifies     { get; set; } // modifies a
                       //    public List<Concept> Subject      { get; set; } // sends a, tells a receives a, hears a              this Concept is the observer, it points to where it gets data from


        // ----------------------------------------------------------------------------------------
        //  Implicit conversions
        // ----------------------------------------------------------------------------------------
        public static implicit operator bool            (Concept node) { if (object.ReferenceEquals(node, null) || node.IsTrue == null) return false; else return node.IsTrue.Value; }
        public static implicit operator Guid            (Concept node) { if (object.ReferenceEquals(node, null)      ) return Guid            .Empty; else return node.TargetGuid  ; }
        public static implicit operator EndemeDefinition(Concept node) { if (object.ReferenceEquals(node, null)      ) return EndemeDefinition.Empty; else return node.Self        ; }

        delegate void Printer(string s);

        // ----------------------------------------------------------------------------------------
        //  Constructors
        // ----------------------------------------------------------------------------------------
        public Concept()
        {
            Friend   = new EndemeList("Friend"  , null        );
            Observer = new EndemeList("Standard", Relationship);
        }
        public Concept(EndemeReference enRef)
        {
            enRef.Add(Relationship);
            Friend   = new EndemeList("Friend"  , enRef, 0.95 );
            Observer = new EndemeList("Standard", Relationship);
        }


        // ----------------------------------------------------------------------------------------
        /// <!-- Relationship -->
        /// <summary>
        ///      Contains characteristics for Concept relationships:
        ///      A)nswers a B)ecomes a C)hild of a D)efines a E)dits a F)rom a G)enerates a H)as a I)s a J)ustifies a K)nows a L)ike a M)asks a N)ames a O)perates on a P)arents a Q)ueries a R)emoves a S)iblings a T)o a U)ses a W)as a
        /// </summary>
        /// <remarks>
        ///      Order for this set is not important
        ///      Try/catch to makes Concepts thread-safe
        /// </remarks>
        public static EndemeSet Relationship { get
        {   if (                 _relationship == null ||                                    // Show my work:        |Concept Relationships:
                                 _relationship.Count < 1)                                    //ABCDEFGHIJKLMNOPQRSTUV|lt Label        CRUD   Group    Other considered labels
            { try {              _relationship = new EndemeSet("Concept Relationship", false); //----------------------+----------------------------------------------------------------------------------
                                 _relationship.Add('A', "Answers a"    , "Answers a"    );   //A]               -ST- |A. Answers a         A          sends a, tells a
                                 _relationship.Add('B', "Becomes a"    , "Becomes a"    );   //[B]                 -V|B. Becomes a         | B        will be a
                                 _relationship.Add('C', "Child of a"   , "Child of a"   );   // [C]DF-       -P--S-  |C. Child of a        | | C      details a, detail of a, follows a, part of a, subordinate to a
                                 _relationship.Add('D', "Defines a"    , "Defines a"    );   //A-[D]      -M-        |D. Defines a         | | | D    metadates a, Annotates a
                                 _relationship.Add('E', "Edits a"      , "Edits a"      );   //   [E]     -M-     -U-|E. Edits a       [U] | | | | E  updates a, modifies a
                                 _relationship.Add('F', "From a"       , "From a"       );   // -C-[F]               |F. From a            F | | | |  points from a, comes from a
                                 _relationship.Add('G', "Generates a"  , "Generates a"  );   //-BC- [G]I-            |G. Generates a   [C] | | | G |  builds a, creates a, inserts a
                                 _relationship.Add('H', "Has a"        , "Has a"        );   // -C-  [H]             |H. Has a             | | H | |  contains a
                                 _relationship.Add('I', "Is a"         , "Is a"         );   //       [I]            |I. Is a              | I | | |  was a, Becomes a
                                 _relationship.Add('J', "Justifies a"  , "Justifies a"  );   //   -E-  [J]         -V|J. Justifies a       | | | J |  validates a, enables a
                                 _relationship.Add('K', "Knows a"      , "Knows a"      );   // -C-     [K]          |K. Knows a           | | K | |  connected to a
                                 _relationship.Add('L', "Like a"       , "Like a"       );   //   -E-    [L]M-  -S-  |L. Like a            | | H | |  similar to a, equals a, matches a
                                 _relationship.Add('M', "Masks a"      , "Masks a"      );   // -C-  -H-  [M]        |M. Masks a           | | | M |  conceals a, Hides a
                                 _relationship.Add('N', "Names a"      , "Names a"      );   //       -I--L[N]       |N. Names a           | | | N |  labels a, identifies a
                                 _relationship.Add('O', "Operates on a", "Operates on a");   //             [O]    -V|O. Operates on a     | | |   O  works on a
                                 _relationship.Add('P', "Parents a"    , "Parents a"    );   //      -H- -LM-[P]   -V|P. Parents a         | | P   |  leads a, header of a, master of a
                                 _relationship.Add('Q', "Queries a"    , "Queries a"    );   //      -H-      [Q]R--V|Q. Queries a     [R] Q | |   |  receives a, hears a, views a, reads a
                                 _relationship.Add('R', "Removes a"    , "Removes a"    );   // -CDE-          [R]   |R. Removes a     [D] | | |   R  deletes a, eliminates a, cuts a
                                 _relationship.Add('S', "Siblings a"   , "Siblings a"   );   //     -G-      -P-[S]-V|S. Siblings a        | | S   |  with a, part of a group with a, groups with a, sibling of a
                                 _relationship.Add('T', "To a"         , "To a"         );   //     -G-      -P- [T] |T. To a              T |     |  points to a, goes to a
                                 _relationship.Add('U', "Uses a"       , "Uses a"       );   //  -D-        -O-   [U]|U. Uses a              |     U  operates with a, does a
                                 _relationship.Add('V', "Was a"        , "Was a"        );   //    -F-       -P-   [V|V. Was a               V        formerly a, previously a
            } catch { } } return _relationship; } }                                          //----------------------+----------------------------------------------------------------------------------
        private static EndemeSet _relationship = null;                                       //ABCDEFGHIJKLMNOPQRSTUV|
        /// <summary>In SOA when you cache someConcept in a static variable you may need to add this refresh to the application refresh code</summary>
        public static void RefreshRelationshipEndemeSet() { _relationship = null; }


        // ----------------------------------------------------------------------------------------
        //  IConceptSource Interface implementation
        // ----------------------------------------------------------------------------------------
        /// <summary>
        ///      Implements a method of IConceptSource (similar to IObservable)
        /// </summary>
        /// <returns></returns>
        public Guid Subscribe(Concept observer, string label, EndemeSet enSetRelationship, Endeme enRelationship)
        {
            EndemeItem item = Observer.Add(label, enSetRelationship, enRelationship, observer, true);
            return item.ItemKey;
        }
        public Guid Subscribe(Concept observer, string label, EndemeSet enSetRelationship, Endeme enRelationship, EndemeSet enSet, Endeme en)
        {
            // --------------------------------------------------------------------------
            //  Most Concepts will be added to the Observer list
            // --------------------------------------------------------------------------
            EndemeItem standard = new EndemeItem(null);
            if (standard.ItemKey == Guid.Empty) throw new Exception("boom");
            if (enRelationship.Count > 1 || !enRelationship.Contains('L'))
                standard = Observer.Add(label, enSetRelationship, enRelationship, observer);
            if (standard.ItemKey == Guid.Empty) throw new Exception("boom");


            // --------------------------------------------------------------------------
            //  A few Concepts will be added to the Friend list
            // --------------------------------------------------------------------------
            if (enRelationship.Contains('L'))
            {
                EndemeItem friend = Friend.Add(label, enSet, en, observer);
                Observer.ChangeGuid(friend.ItemKey, standard.ItemKey);
            }
            return standard.ItemKey;
        }

        public Concept UnSubscribe(Guid observerId, EndemeSet enSetRelationship, Endeme enRelationship)
        {
            // --------------------------------------------------------------------------
            //  Simple removal if it is in the Friend list
            // --------------------------------------------------------------------------
            if (enRelationship.Contains('L'))
            {
                EndemeItem item = Friend[observerId];
                Friend.Remove(observerId);
            }


            // --------------------------------------------------------------------------
            //  More complex if the relationship is not just friendship
            // --------------------------------------------------------------------------
            if (enRelationship.Count > 1 || !enRelationship.Contains('L'))
            {
                EndemeItem item = Observer[observerId];
                if (item.EnSet.Label == enSetRelationship.Label)
                {
                    item.ItemEndeme -= enRelationship; // will this really work?
                    if (item.ItemEndeme.Count < 1)
                        Observer.Remove(observerId);
                }
                else
                    throw new InvalidOperationException("This should always be true, the endeme set should always match");
            }

            return this;
        }

        public Concept NotifySubscribers()
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        /////      Implements a method of IObservable&nbsp;Concept>
        ///// </summary>
        ///// <param name="observer"></param>
        ///// <returns></returns>
        //public IDisposable Subscribe(IObserver<Concept> observer)
        //{
        //    throw new NotImplementedException();
        //}


        // ----------------------------------------------------------------------------------------
        //  IConceptObserver Interface implementation
        // ----------------------------------------------------------------------------------------
        /// <summary>
        ///      Implements methods of IConceptObserver (similar to IObserver)
        /// </summary>
        /// <returns></returns>
        public void OnPollData(Concept value) // OnNext
        {
            Console.WriteLine("{0}: The current location is {1}", this.TargetLabel, this.TargetModel.ToString());
        }

        public void OnError(Exception ex)
        {
            Console.WriteLine("{0}: The location cannot be determined.", this.TargetLabel);
        }

        public void OnCompleted()
        {
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.TargetLabel);
        }


        // ----------------------------------------------------------------------------------------
        //  Overrides
        // ----------------------------------------------------------------------------------------
        public override string ToString()
        {
            string canOrCant = "";
            if      ( IsTrue == null) canOrCant = "";
            else if ( IsTrue.Value  ) canOrCant = "Has ";
            else if (!IsTrue.Value  ) canOrCant = "No ";


            string action = TreatAs.StrValue(Operation, "");


            string determiner = "";
            if (string.IsNullOrWhiteSpace(TargetLabel) && (TargetId == null || TargetId < 0))
                 determiner = " All";
            else determiner = "";


            string typeOf = "";
            if (string.IsNullOrWhiteSpace(TargetLabel) && (TargetId == null || TargetId < 0))
                 typeOf = " " + TreatAs.StrValue(TargetType);
            else typeOf = " " + TreatAs.StrValue(TargetType) + " " + "'" + TreatAs.StrValue(TargetLabel, TreatAs.StrValue(TargetId, "NULL")) + "'";


            string noun = "";
            if (string.IsNullOrWhiteSpace(TargetLabel) && (TargetId == null || TargetId < 0))
                 noun = " " + __.Pluralize(TreatAs.StrValue(Perview, "?"));
            else noun = " " + TreatAs.StrValue(Perview, "?");


            return canOrCant + action + determiner + typeOf + noun;
        }
    }
}
