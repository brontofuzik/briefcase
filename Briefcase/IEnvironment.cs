namespace Briefcase
{
    public interface IEnvironment
    {
        void Initialize();

        // Turn-based
        void BeginTurn(int turn);

        // Turn-based
        void EndTurn(int turn);

        string Show();
    }
}
