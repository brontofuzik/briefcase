namespace Briefcase
{
    public abstract class Agent : IAgent
    {
        private readonly string name;

        public Agent(string name)
        {
            this.name = name;
        }

        public string Id => $"{name}:{GetType().Name}";

        public virtual void Initialize() {}
    }
}
