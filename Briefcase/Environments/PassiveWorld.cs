namespace Briefcase.Environments
{
    public abstract class PassiveWorld<TSensor, TPercept, TAction, TResult>
    {
        public virtual void Initialize()
        {
        }

        public abstract TPercept Perceive(TSensor sensor = default);

        public TResult DoAct(TAction action)
        {
            BeforeAct();
            var result = Act(action);
            AfterAct();
            return result;
        }

        protected virtual void BeforeAct()
        {
        }

        public abstract TResult Act(TAction action);

        protected virtual void AfterAct()
        {
        }
    }
}
