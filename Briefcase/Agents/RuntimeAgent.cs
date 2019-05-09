using System;

namespace Briefcase.Agents
{
    // Decorator
    internal abstract class RuntimeAgent : IRuntimeAgent
    {
        protected readonly Agent agent;

        protected RuntimeAgent(Agent agent)
        {
            this.agent = agent;
        }

        public string Id => agent.Id;

        public abstract void Run(TimeSpan? stepTime);

        public abstract void Post(Message message);
    }

    interface IRuntimeAgent
    {
        string Id { get; }

        void Run(TimeSpan? stepTime);

        void Post(Message message);
    }
}
