using System;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    public abstract class RealTimeAgent : Agent
    {
        private readonly Thread thread;
        private TimeSpan? stepTime;

        protected RealTimeAgent(string name)
            : base(name)
        {
            thread = new Thread(Loop)
            {
                Name = $"RealTimeAgent_{name}"
            };          
        }

        private async void Loop()
        {
            while (true)
            {
                await Step();

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }

        protected abstract Task Step();

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
