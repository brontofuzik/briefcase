using Briefcase.Agents;
using Briefcase.Environments;

namespace Briefcase
{
    public class RealTimeMas : MultiagentSystem
    {
        private new readonly RealTimeEnvironment environment;

        public RealTimeMas(RealTimeEnvironment environment = null)
            : base(environment)
        {
            this.environment = environment;
        }

        public void Run()
        {
            Initialize();
            RunAgents();
        }

        private void RunAgents()
        {
            foreach (var agent in GetAllAgents())
                ((RealTimeAgent)agent).Run();
        }
    }
}
