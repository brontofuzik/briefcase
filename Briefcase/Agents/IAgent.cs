namespace Briefcase.Agents
{
    // Do we need this?
    public interface IAgent
    {
        string Id { get; }

        void Initialize();

        void Step(int turn = 0);
    }
}
