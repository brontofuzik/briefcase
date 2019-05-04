using Briefcase.Agents;
using System.Collections.Generic;

namespace Briefcase
{
    // Do we need this?
    public interface IMultiagentSystem
    {
        IAgent GetAgent(string id);

        IEnumerable<IAgent> GetAllAgents();

        void AddAgent(IAgent agent);
    }
}
