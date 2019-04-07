namespace Briefcase
{
    public interface IEnvironment
    {
        IMultiagentSystem Mas { get; set; }

        void Initialize();

        // Turn-based
        void BeginTurn(int turn);

        // Turn-based
        void EndTurn(int turn);

        string Show();
    }
}
