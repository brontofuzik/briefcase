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

        public IEnvironment Environment { get; set; }

        public virtual void Initialize() {}
    }
}
