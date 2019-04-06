namespace Briefcase
{
    public interface IAgent
    {
        string Id { get; }

        IEnvironment Environment { get; set; }

        void Initialize();

        void Act();
    }
}
