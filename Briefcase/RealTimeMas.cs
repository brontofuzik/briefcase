using System;
using Briefcase.Agents;
using Briefcase.Environments;

namespace Briefcase
{
    public class RealTimeMas : MultiagentSystem
    {
        private new readonly RealTimeEnvironment environment;
        private readonly TimeSpan? stepTime;

        public RealTimeMas(RealTimeEnvironment environment = null, TimeSpan? stepTime = null)
            : base(environment)
        {
            this.environment = environment;
            this.stepTime = stepTime;
        }

        public void Run()
        {
            Initialize();
            RunAgents();
        }

        private void RunAgents()
        {
            foreach (var agent in GetAllAgents())
                ((RealTimeAgent)agent).Run(stepTime);
        }
    }
}
