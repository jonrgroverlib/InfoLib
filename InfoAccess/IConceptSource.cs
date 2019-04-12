using InfoLib.Endemes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoLib.Info
{
    /// <summary>
    ///      Not exactly the observer pattern because it contains bi-directional links
    /// </summary>
    public interface IConceptSource
    {
        EndemeList Observer { get; set; } // 'bi-directional link' back to observers

        Guid    Subscribe        (Concept observer, string label, EndemeSet enSetRelationship, Endeme enRelationship);
        Guid    Subscribe        (Concept observer, string label, EndemeSet enSetRelationship, Endeme enRelationship, EndemeSet enSet, Endeme en);
        Concept UnSubscribe      (Guid    observerId            , EndemeSet enSetRelationship, Endeme enRelationship);
        Concept NotifySubscribers();
    }
}
