using System;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    // Decorator
    public class RealTimeAgent
    {
        private readonly Agent agent;

        private readonly Thread thread;
        private TimeSpan? stepTime;

        internal RealTimeAgent(Agent agent)
        {
            this.agent = agent;
            thread = new Thread(Loop)
            {
                Name = $"RealTimeAgent_{agent.Id}"
            };          
        }

        private async void Loop()
        {
            while (true)
            {
                agent.Step();

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }

        public void Run(TimeSpan? stepTime)
        {
            this.stepTime = stepTime;
            thread.Start();
        }

        public void Kill()
        {
            thread.Abort();
        }
    }
}
