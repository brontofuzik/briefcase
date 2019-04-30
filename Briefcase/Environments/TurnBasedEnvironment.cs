namespace Briefcase.Environments
{
    public abstract class TurnBasedEnvironment : Environment
    {
        public virtual void BeginTurn(int turn)
        {
        }

        public virtual void EndTurn(int turn)
        {
        }
    }

    public abstract class TurnBasedEnvironment<TPercept, TAction, TResult> : TurnBasedEnvironment
    {
        public abstract TPercept Perceive();

        public abstract TResult Act(TAction action);
    }
}
