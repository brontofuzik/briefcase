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
}
