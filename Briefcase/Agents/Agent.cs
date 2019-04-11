using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class Agent : IAgent
    {
        private readonly string name;

        public Agent(string name)
        {
            this.name = name;
        }

        public string Id => $"{name}:{GetType().Name}";

        public IEnvironment Environment { get; set; }

        public virtual void Initialize() {}

        public abstract void Act();
    }
}
