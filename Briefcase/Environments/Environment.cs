namespace Briefcase.Environments
{
    public abstract class Environment
    {
        public MultiagentSystem Mas { get; set; }

        public virtual void Initialize()
        {
        }

        public abstract object Perceive(string agentId, object sensor = null);

        public object DoAct(string agentId, object action)
        {
            BeforeAct();
            var result = Act(agentId, action);
            AfterAct();
            return result;
        }

        protected virtual void BeforeAct()
        {
        }

        public abstract object Act(string agentId, object action);

        protected virtual void AfterAct()
        {
        }
    }
}
