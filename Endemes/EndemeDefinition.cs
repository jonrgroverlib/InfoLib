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
using System.Collections;             // for 
using System.Collections.Generic;     // for 
using System.Linq;                    // for 
using System.Text.RegularExpressions; // for Regex

namespace InfoLib.Endemes // THIS NAMESPACE IS A PRIMITIVE!  use only System.* or InfoLib.Testing references
{
    // --------------------------------------------------------------------------------------------
    /// <!-- EndemeDefinition -->
    /// <summary>
    ///      The EndemeDefinition class contains a generalized cloud of data semantic addressed endeme objects
    /// </summary>
    /// <remarks>
    ///      EndemeObjects are not the same as objects although each one contains an object inside its EndemeValue
    ///      EndemeDefinitions are like EndemeLists on steroids
    ///      
    /// 
    ///      Endeme object/class dependency tree:    (setcount-endemecount-valuecount)*itemcount  where   count = 0, 1, n(infinity)
    ///      ------------------------------------
    ///
    ///           [-------------------- ACTION ------------------]    [-------------------------- STATE -------------------------]    [---------------------------- DATABASE ---------------------------------]           
    ///                                                                                                                                                                                                        
    ///       _   |                                              |    |                                                          |    |                                                                       |   _     _ 
    ///        |                  EndemeActuator.cs                                                                                                                            EndemeAccess                      |       |
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
    /// 
    /// 
    ///           +-------------------------------------------------------------------------------+  /
    /// Level 5       Conventional Artificial Intelligence                                          +--------------------------------------------+
    /// Programming   (machine learning, genetic algorithms, search techniques)                      \                                          /|\
    ///                                                                                                                                          |
    ///                                                                                                                                          |
    ///                                                                                                                                          |
    ///           +-------------------------------------------------------------------------------+                                              |
    /// Level 4       Relationships                                                                                                              |
    /// Programming   (ontologies, semantic web, IS-A, HAS-A and there should be many more)                                                      |
    ///                                                                                                                                          |
    ///                                                             CONJUNCTIONS                                                                 |
    ///                                                           (underdeveloped)                                                               | end run:
    ///           +-----------------------------------------------|-------------|-----------------+ 15%                                          |
    /// Level 3       Information - meaning - characterizing      |             |                                                                | warehouses
    /// Programming                                               |             |                                                                | data lakes
    ///       (undeveloped)                                   ADJECTIVES --- ADVERBS                                                             | cubes
    ///                                                       (endemes)      (?????) (commit or rollback? or whatever)                           |
    ///                                                           |             |    (threading?, spawning?, housing?, multiprocessing?)         |
    ///           +-----------------------------------------------|-------------|-----------------+ 15%                                          |
    /// Level 2       Data categorization - precision           NOUNS ------- VERBS                                                              |
    /// Programming   (C#, Java, C, F#,                         (OOP)         (FnP)                                                              |
    ///                functional pgmg etc.).                                                                                                    |
    ///      (overly developed)                                                                                                                  |
    ///                                                                                                                                          |
    ///           +-------------------------------------------------------------------------------+                                              |
    /// Level 1       Data storage + performance                                                    -------------------------------------------->+
    /// Programming   (SQL, cloud storage etc.)
    ///                
    /// 
    /// 
    ///           +-------------------------------------------------------------------------------+
    /// Dave Jourdain (1977)
    /// 
    ///      Pre-alpha code
    /// </remarks>
    public class EndemeDefinition : IEnumerable<Guid>, IHaveListMembers
    {
        // ----------------------------------------------------------------------------------------
        //  Private members
        // ----------------------------------------------------------------------------------------
        private Dictionary<Guid,EndemeObject> _list    ; // the authorititative source for content
        private List<Guid>                    _order   ; // the authorititative order
        private Dictionary<string,List<Guid>> _labelIdx; // index for the labels
        private Dictionary<string,List<Guid>> _codeIdx ; // TODO:build me
        private EndemeReference               _enRef   ;
        private string                        _label   ; // the name of the registry


        // ------------------------------------------------------------------------------
        //  Accessors
        // ------------------------------------------------------------------------------
        public EndemeObject this[Guid           key    ] { get {                  if (_list.ContainsKey(key)        ) return _list[key]             ; else return EndemeObject.Empty; } }
        public EndemeObject this[int            idx    ] { get {                  if (0 <= idx && idx < _order.Count) return _list[_order[idx]]     ; else return EndemeObject.Empty; } }
        public EndemeObject this[EndemeProfile  profile] { get {                                                      return GetExactObject(profile);                                 } }
        public EndemeObject this[EndemeActuator enAct  ] { get { EndemeDefinition def = By(enAct); if (def.Count > 0) return def[0]                 ; else return EndemeObject.Empty; } }
        public EndemeObject this[string         profile]
        {
            get { return GetExactObject(profile); }
            set { EndemeObject obj = GetExactObject(profile); if (obj == null) InsertRegisterExactly(profile, value, true); else obj.Item = value.Item; }
        }


        // ----------------------------------------------------------------------------------------
        //  Constructor
        // ----------------------------------------------------------------------------------------
        public EndemeDefinition(string label                       ) { _label = label;                 Clear(); }
        public EndemeDefinition(string label, EndemeReference enRef) { _label = label; _enRef = enRef; Clear(); }


        // ----------------------------------------------------------------------------------------
        //  Short methods and properties
        // ----------------------------------------------------------------------------------------
        public bool              Contains     (string label) { return _labelIdx.ContainsKey(label); }
               IEnumerator IEnumerable.GetEnumerator(      ) { return           GetEnumerator()   ; }  /// <summary>Count of the number of fields</summary>
        public IEnumerator<Guid> GetEnumerator(            ) { return _order   .GetEnumerator()   ; }
        public override string   ToString     (            ) { return Label + " (" + Count + ")"  ; }
        public void              RemoveAt     (int    ord  ) { Guid guid = _order[ord]; Remove(guid); if (!Sane) throw new NullReferenceException("EndemeArray '"+Label+"' is insane."); }
        public void              Sort         (            ) {                  Set(SortedCopy()) ; }

        public        int              Capacity { get { return _order.Capacity         ; } set { _order.Capacity = value; } }
        public        int              Count    { get { return _list.Count             ; }                                  }
        public static EndemeDefinition Empty    { get { return new EndemeDefinition(""); }                                  }
        public        EndemeReference  EnRef    { get { return _enRef                  ; }                                  }  /// <summary>More than 0 fields (returned)</summary>
        public        bool             Found    { get { return (Count > 0)             ; }                                  }
        public        string           Label    { get { return _label                  ; }                                  }
        private       bool             Sane     { get { bool sane = (_list.Count >= _labelIdx.Count); if (!sane) Pause(); return sane; } }


        // ----------------------------------------------------------------------------------------
        /// <!-- Add -->
        /// <summary>
        ///      Adds an element to the EndemeArray
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="element"></param>
        /// <returns>
        ///      Probably ought to return the element not just the Guid since the Element contains the Guid
        /// </returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public Guid Add(Guid guid, EndemeObject element) // <------------------------------------+
        {                                                                                     // |
            element.ItemKey = guid;                                                           // |
            _list.Add (guid, element);                                                        // |
            _order.Add(guid);                                                                 // |
            if (_enRef == null && element.EnRef != null) _enRef = element.EnRef;              // |  warning: side effect
            return guid;                                                                      // |
        }                                                                                     // |
        public Guid         Add(EndemeObject  element                ) { return Add(Guid.NewGuid(), element);                                                                    } // <--+
        public EndemeObject Add(EndemeProfile enProfile, object value, bool rawSource) { EndemeObject enObj = new EndemeObject("", enProfile, new EndemeValue(value)); Add(enObj); return enObj; } // ---+

        // ----------------------------------------------------------------------------------------
        /// <!-- AddToIndex -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="element"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        private void AddToIndex(Guid guid, EndemeObject element)
        {
            string label = element.ItemLabel;
            if (!_labelIdx.ContainsKey(label))
                _labelIdx.Add(label,new List<Guid>());
            _labelIdx[label].Add(guid);
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- By -->
        /// <summary>
        ///      Returns a list of endeme objects by one or more of (filter, order, group?)
        /// </summary>
        /// <param name="actuator"></param>
        /// <returns></returns>
        /// <remarks>
        ///      algortihm:
        ///      
        ///      - Go through each HasChar factor and look to see if that is covered by the item profile
        ///         - Look to see if each hashchar factor is covered by the item profile
        /// </remarks>
        public EndemeDefinition By(EndemeActuator actuator)
        {
            actuator.InitWeights();

            EndemeDefinition output = new EndemeDefinition(this.Label + "_" + actuator.ToString(), this.EnRef);


            // --------------------------------------------------------------------------
            //  Go through each item and see whether to add it to the out put list
            // --------------------------------------------------------------------------
            foreach (EndemeObject item in _list.Values)
            {
                // ----------------------------------------------------------------------
                //  Check whether the item's profile covers all of the HasChar factors
                // ----------------------------------------------------------------------
                double charMatch = actuator.HasChar.CharMatch(item.ItemProfile);


                double setMatch = actuator.HasSets.SetMatch(item.ItemProfile);


                double orderMatch = actuator.Ordered.OrderMatch(item.ItemProfile);


                // ----------------------------------------------------------------------
                //  Check for (AND) all the defined value factors
                // ----------------------------------------------------------------------
                bool valFound = true;
                if (charMatch > -0.5 && setMatch > -0.5 && orderMatch > -0.5)
                    foreach (ValueFactor val in actuator.HasVals)
                    {
                        valFound &= val.FoundInProfile(item.ItemProfile.Segment);
                    }


                // ----------------------------------------------------------------------
                //  Check for (OR) all the weight factors
                // ----------------------------------------------------------------------
                double weightMatch = 1.0;
                if (charMatch > -0.5 && setMatch > -0.5 && orderMatch > -0.5)
                    foreach (WeightFactor wgt in actuator.Weights)
                    {
                        double factorWeight = wgt.CalculateFactorWeight(item.ItemProfile.Segment);
                        double normalized   = (factorWeight - wgt.WorstMatch)/(wgt.BestMatch - wgt.WorstMatch);
                        weightMatch *= normalized;
                    }


                // ----------------------------------------------------------------------
                //  Add the item if its profile covers all of the defined factor groups
                // ----------------------------------------------------------------------
                if (charMatch > -0.5 && setMatch > -0.5 && orderMatch > -0.5 && valFound)
                {
                    EndemeObject copy = item.Copy();
                    copy.TempMatch = charMatch + weightMatch;
                    output.Add(copy);
                }

            }


            // --------------------------------------------------------------------------
            //  Order output by weight
            // --------------------------------------------------------------------------
            if (output.Count > 1) return output.SortedCopy(); else return output;
        }
        public EndemeDefinition For (EndemeActuator actuator) { return By(actuator); }
        public EndemeDefinition From(EndemeActuator actuator) { return By(actuator); }
        public EndemeDefinition When(EndemeActuator actuator) { return By(actuator); }


        // ----------------------------------------------------------------------------------------
        /// <!-- Clear -->
        /// <summary>
        ///      Clears the endeme list without changing its name or detatching the endeme reference instance
        /// </summary>
        public void Clear()
        {
            _list     = new Dictionary<Guid,EndemeObject>();
            _order    = new List<Guid>();
            _labelIdx = new Dictionary<string,List<Guid>>();
            _codeIdx  = new Dictionary<string,List<Guid>>();
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SortedCopy -->
        /// <summary>
        ///      Returns an endeme definition sorted by its object's TempMatch
        /// </summary>
        /// <returns></returns>
        public EndemeDefinition SortedCopy()
        {
            List<KeyValuePair<Guid, EndemeObject>> sorted = _list.OrderByDescending(o => o.Value.TempMatch).ToList();
            EndemeDefinition output = new EndemeDefinition(Label, EnRef);
            for (int i = 0; i < sorted.Count; ++i)
                output.Add(sorted[i].Key, sorted[i].Value);
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Set -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enDef"></param>
        public void Set(EndemeDefinition enDef)
        {
            _list     = enDef._list    ;
            _order    = enDef._order   ;
            _labelIdx = enDef._labelIdx;
            _codeIdx  = enDef._codeIdx ;
            _enRef    = enDef._enRef   ;
            _label    = enDef._label   ;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- CopyAll -->
        /// <summary>
        ///      Makes a copy preserving the keys
        /// </summary>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeDefinition CopyAll()
        {
            EndemeDefinition list = new EndemeDefinition(this.Label);
            foreach (Guid key in this)
                list.Add(key, this[key].Copy_simple());
            if (!list.Sane) throw new NullReferenceException("EndemeList '"+list.Label+"' is insane.");
            return list;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Educe -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public EndemeObject Educe(EndemeTermKey profile, bool rawSource)
        {
            EndemeProfile enProfile = new EndemeProfile(profile, _enRef);
            for (int i = 0; i < enProfile.Count; ++i)  _enRef.Educe(enProfile[i].ItemEndeme);
            EndemeObject enObject = this[enProfile];
            if (enObject == null)
            {
                enObject = this.Add(enProfile, null, rawSource);
            }
            return enObject;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetExactObject -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enProfile"></param>
        /// <returns></returns>
        public EndemeObject GetExactObject(EndemeProfile enProfile)
        {
            if (enProfile == null || enProfile.Count == 0) return null;
            else
            {
                EndemeObject enObject = ExactObject(enProfile);
                return enObject;
            }
        }
        public EndemeObject GetExactObject(string profile) { return GetExactObject(new EndemeProfile(profile, _enRef)); }

        // ----------------------------------------------------------------------------------------
        /// <!-- GetSubsetObject -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enProfile"></param>
        /// <returns></returns>
        public EndemeObject GetSubsetObject(EndemeProfile enProfile)
        {
            if (enProfile == null || enProfile.Count == 0) return null;
            else
            {
                EndemeObject enObject = SubsetObject(enProfile);
                return enObject;
            }
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Insert -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ord"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <remarks>beta code - nearly production ready</remarks>
        public EndemeObject Insert(int ord, EndemeObject element)
        {
            Guid guid = Guid.NewGuid();
            _list.Add(guid, element);
            _order.Insert(ord, guid);
            AddToIndex(guid, element);

            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");
            return element;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- InsertRegisterExactly -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enProfile"></param>
        /// <param name="value"></param>
        public void InsertRegisterExactly(EndemeProfile enProfile, object value, bool rawSource)
        {
            // --------------------------------------------------------------------------
            //  Preprocess the endeme
            // --------------------------------------------------------------------------
            if (enProfile == null)
                return; // maybe throw, maybe sliding scale on application
            else
            {
                // ----------------------------------------------------------------------
                //  Figure out what value to store and where to store the value
                // ----------------------------------------------------------------------
                EndemeObject enObject = ExactObject(enProfile);
                if (enObject == null) Add(enProfile, value, rawSource);
                else enObject.Item = EndemeObject.Simple(new EndemeValue(value));


                //string str = AsciiValue;
            }
        }
        private void InsertRegisterExactly(string profile, object value, bool rawSource) { InsertRegisterExactly(new EndemeProfile(profile, _enRef), value, rawSource); }

        // ----------------------------------------------------------------------------------------
        /// <!-- OrderBy -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortprofile"></param>
        /// <returns></returns>
        /// <remarks>alpha code</remarks>
        public EndemeDefinition OrderBy(EndemeProfile sortprofile)
        {
            // --------------------------------------------------------------------------
            //  Put the keys in order, preserving the keys
            // --------------------------------------------------------------------------
            EndemeDefinition toOrder = this.CopyAll();
            foreach (Guid key in toOrder)
            {
                EndemeProfile profile = toOrder[key].ItemProfile;
                toOrder[key].ItemProfile.Match(sortprofile);
            }


            // --------------------------------------------------------------------------
            //  Order the register by match using Linq
            // --------------------------------------------------------------------------
            List<Guid> orderedKeys = toOrder._list.OrderBy(x => x.Value.TempMatch).Select(x => x.Key).ToList();
            EndemeDefinition ordered = new EndemeDefinition(Label, EnRef);
            foreach (Guid key in orderedKeys)
                ordered.Add(key, toOrder[key]);
            return ordered;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PartHaving -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <returns></returns>
        internal EndemeDefinition PartHaving(EndemeSet enSet)
        {
            EndemeDefinition output = new EndemeDefinition(this.Label, this.EnRef);
            foreach (Guid key in this._order)
            {
                EndemeProfile profile = this[key].ItemProfile;
                if (profile.Contains(enSet))
                    output.Add(key, _list[key]);
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- PartNotHaving -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enSet"></param>
        /// <returns></returns>
        internal EndemeDefinition PartNotHaving(EndemeSet enSet)
        {
            EndemeDefinition output = new EndemeDefinition(this.Label, this.EnRef);
            foreach (Guid key in this._order)
            {
                EndemeProfile profile = this[key].ItemProfile;
                if (!profile.Contains(enSet))
                    output.Add(key, _list[key]);
            }
            return output;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Pause -->
        /// <summary>
        /// 
        /// </summary>
        private void Pause()
        {
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Remove -->
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <remarks>beta code - nearly production ready</remarks>
        public void Remove(Guid guid)
        {
            string label = _list[guid].ItemLabel;

            List<Guid> guids = _labelIdx[label];
            guids.Remove(guid);
            if (guids.Count == 0)
                _labelIdx.Remove(label);

            _order.Remove(guid);
            _list.Remove(guid);
            if (!Sane) throw new NullReferenceException("EndemeList '"+Label+"' is insane.");
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- Subset -->
        /// <summary>
        ///      Returns an endeme object only if it has exactly the same endeme
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeObject Subset(EndemeProfile enProfile)
        {
            int count = 0;
            EndemeObject enObject = null;
            for (int i = 0; i < this.Count; ++i)
            {
                EndemeProfile myProfile = this[i].ItemProfile;
                if (enProfile.IsSubsetOf(myProfile))
                {
                    count++;
                    enObject = this[i];
                }
            }
            if (count > 1) throw new Exception("boom");
            return enObject;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ExactObject -->
        /// <summary>
        ///      Returns an endeme object only if it has exactly the same endeme
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeObject ExactObject(EndemeProfile enProfile)
        {
            int count = 0;
            EndemeObject enObject = EndemeObject.Empty;
            for (int i = 0; i < this.Count; ++i)
            {
                EndemeProfile myProfile = this[i].ItemProfile;
                if (enProfile == myProfile)
                {
                    count++;
                    enObject = this[i];
                }
            }
            if (count > 1) throw new Exception("boom");
            return enObject;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- SubsetObject -->
        /// <summary>
        ///      Returns an endeme object only if it has exactly the same endeme
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EndemeObject SubsetObject(EndemeProfile enProfile)
        {
            int count = 0;
            EndemeObject enObject = null;
            for (int i = 0; i < this.Count; ++i)
            {
                EndemeProfile myProfile = this[i].ItemProfile;
                if (enProfile.IsSubsetOf(myProfile))
                {
                    count++;
                    enObject = this[i];
                }
            }
            if (count > 1) throw new Exception("boom");
            return enObject;
        }

        // ----------------------------------------------------------------------------------------
        /// <!-- ToList -->
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<IAmAnEndemeItem> ToList()
        {
            List<IAmAnEndemeItem> list = new List<IAmAnEndemeItem>();
            for (int i = 0; i < Count; ++i) list.Add(this[i]);
            return list;
        }
    }
}
