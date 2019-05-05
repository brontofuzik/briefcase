using System;
using Briefcase.Agents;
using System.Collections.Generic;

namespace Briefcase
{
    // Do we need this?
    public interface IMultiagentSystem
    {
        void AddAgent(IAgent agent);

        IAgent GetAgent(string id);

        IEnumerable<IAgent> GetAllAgents();

        void RunRealtime(TimeSpan? stepTime = null);

        void RunTurnbased(int? maxTurns = null, TimeSpan? stepTime = null);
    }
}
