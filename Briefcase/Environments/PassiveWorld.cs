namespace Briefcase.Environments
{
    public abstract class PassiveWorld<TSensor, TPercept, TAction, TResult>
    {
        public virtual void Initialize()
        {
        }

        public abstract TPercept Perceive(string agentId, TSensor sensor = default);

        public TResult DoAct(string agentId, TAction action)
        {
            BeforeAct();
            var result = Act(agentId, action);
            AfterAct();
            return result;
        }

        protected virtual void BeforeAct()
        {
        }

        public abstract TResult Act(string agentId, TAction action);

        protected virtual void AfterAct()
        {
        }
    }
}
