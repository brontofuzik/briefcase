using Briefcase.Environments;
using System.Threading.Tasks;

namespace Briefcase.Example.Bdi.Environment
{
    class RealTimeFireWorld : RealTimeEnvironment
    {
        public Task<bool> Act(Action action)
        {
            Enqueue(action);
            return Task.FromResult(true); // TODO
        }

        protected override void Handle(object action)
        {
            throw new System.NotImplementedException();
        }
    }
}