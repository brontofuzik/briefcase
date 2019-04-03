namespace Briefcase
{
    public abstract class Environment : IEnvironment
    {
        public virtual void Initialize() {}

        // Turn-based
        public abstract void BeginTurn(int turn);

        // Turn-based
        public abstract void EndTurn(int turn);
    }
}
