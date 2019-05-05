using Briefcase.Environments;

namespace Briefcase.Agents
{
    public interface IAgent
    {
        string Id { get; }

        void Initialize();

        void Step();
    }
}
