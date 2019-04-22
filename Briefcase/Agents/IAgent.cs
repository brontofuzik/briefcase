using Briefcase.Environments;

namespace Briefcase.Agents
{
    public interface IAgent
    {
        string Id { get; }

        IEnvironment Environment { get; set; }

        void Initialize();
    }
}
