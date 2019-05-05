using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class Agent : IAgent
    {
        private readonly string name;

        protected Agent(string name)
        {
            this.name = name;
        }

        public string Id => $"{name}:{GetType().Name}";

        public virtual void Initialize() {}

        public abstract void Step();
    }

    public abstract class Agent<TEnvironment> : Agent
        where TEnvironment : IEnvironment
    {
        protected Agent(string name)
            : base(name)
        {
        }

        public TEnvironment Environment { get; set; }
    }
}
