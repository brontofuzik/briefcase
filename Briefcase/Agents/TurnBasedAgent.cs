namespace Briefcase.Agents
{
    public abstract class TurnBasedAgent : Agent
    {
        protected TurnBasedAgent(string name)
            : base(name)
        {
        }

        public abstract void Step();
    }
}
