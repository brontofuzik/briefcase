using System;
using System.Threading;
using System.Threading.Tasks;
using Briefcase.Environments;

namespace Briefcase.Agents
{
    internal abstract class RuntimeAgent : IRuntimeAgent
    {
        protected readonly Agent agent;
        private readonly RuntimeEnvironment environment;

        private readonly Thread actionThread;
        protected TimeSpan? stepTime;

        protected RuntimeAgent(Agent agent, RuntimeEnvironment environment)
        {
            this.agent = agent;
            this.environment = environment;

            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            };
        }

        public string Id => agent.Id;

        protected async void Act()
        {
            while (true)
            {
                //agent.Step();
                await environment.PerceiveAndAct(Id, ((SituatedAgent)agent).PerceiveAndAct);

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }

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
