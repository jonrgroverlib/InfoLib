using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoLib.Info
{
    public interface IConceptObserver
    {
        void OnPollData (Concept value);
        void OnError    (Exception ex);
        void OnCompleted();
    }
}
