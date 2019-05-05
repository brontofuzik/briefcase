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

        public abstract void Step(int turn = 0);
    }

    public abstract class SituatedAgent : Agent
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public IEnvironment Environment { get; set; }
    }

    public abstract class SituatedAgent<TEnvironment> : SituatedAgent
        where TEnvironment : IEnvironment
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public new TEnvironment Environment { get; set; }
    }
}
