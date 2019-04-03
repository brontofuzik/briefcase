namespace Briefcase
{
    public interface IAgent
    {
        string Id { get; }

        void Initialize();

        void Act(IEnvironment environment);
    }
}
