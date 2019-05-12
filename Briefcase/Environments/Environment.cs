namespace Briefcase.Environments
{
    // TODO Rename to Environment.
    public abstract class Environment
    {
        public MultiagentSystem Mas { get; set; }

        public virtual void Initialize()
        {
        }
    }

    // TODO Rename to Environment.
    public abstract class Environment<S, P, A, R> : Environment
    {
        public abstract P Perceive(string agentId, S sensor = default);

        public R DoAct(string agentId, A action)
        {
            BeforeAct();
            var result = Act(agentId, action);
            AfterAct();
            return result;
        }

        protected virtual void BeforeAct()
        {
        }

        public abstract R Act(string agentId, A action);

        protected virtual void AfterAct()
        {
        }
    }
}
