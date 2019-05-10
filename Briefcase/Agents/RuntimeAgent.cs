using System;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    // Decorator
    internal abstract class RuntimeAgent : IRuntimeAgent
    {
        protected readonly Agent agent;

        private readonly Thread actionThread;

        private TimeSpan? stepTime;

        protected RuntimeAgent(Agent agent)
        {
            this.agent = agent;

            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            };
        }

        private async void Act()
        {
            while (true)
            {
                agent.Step();

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }

        public string Id => agent.Id;

        public virtual void Run(TimeSpan? stepTime)
        {
            this.stepTime = stepTime;
            actionThread.Start();
        }

        public virtual void Kill()
        {
            actionThread.Abort();
        }

        public abstract void Post(Message message);
    }

    interface IRuntimeAgent
    {
        string Id { get; }

        void Run(TimeSpan? stepTime);

        void Post(Message message);
    }
}
