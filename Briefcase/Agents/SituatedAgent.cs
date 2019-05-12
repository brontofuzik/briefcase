using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class SituatedAgent : Agent
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public Environment Environment { get; set; }
    }

    public abstract class SituatedAgent<TEnvironment> : SituatedAgent
        where TEnvironment : Environment
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public new TEnvironment Environment
        {
            get => (TEnvironment)base.Environment;
            set => base.Environment = value;
        }
    }
}