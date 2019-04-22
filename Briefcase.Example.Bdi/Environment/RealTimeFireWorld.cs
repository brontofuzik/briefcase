using Briefcase.Environments;
using System.Threading.Tasks;
using Briefcase.ActiveObject;
using Briefcase.ActiveObject.Dispatcher;

namespace Briefcase.Example.Bdi.Environment
{
    class RealTimeFireWorld : RealTimeEnvironment
    {
        private readonly IDispatcher dispatcher;
        private readonly ActiveFireWorld fireWorld;

        public RealTimeFireWorld()
        {
            dispatcher = new Dispatcher_NEW();
            fireWorld = new ActiveFireWorld(dispatcher);
        }

        public override void Initialize()
        {
            fireWorld.InitializeAsync().GetAwaiter().GetResult();
        }

        public Task<FireWorldPercept> Perceive()
            => fireWorld.PerceiveAsync();

        public Task<bool> Act(FireWorldAction action)
            => fireWorld.ActAsync(action);
    }
}