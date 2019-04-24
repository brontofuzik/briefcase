using Briefcase.Environments;
using System.Threading.Tasks;
using Briefcase.ActiveObject;
using Briefcase.Utils;

namespace Briefcase.Example.Bdi.Environment
{
    class RealTimeFireWorld : RealTimeEnvironment
    {
        private readonly ActiveFireWorld fireWorld;

        public RealTimeFireWorld()
        {
            fireWorld = new ActiveFireWorld(new FireWorld());
        }

        public override void Initialize()
        {
            fireWorld.InitializeAsync().Await();
        }

        public Task<FireWorldPercept> Perceive()
            => fireWorld.PerceiveAsync();

        public Task<bool> Act(FireWorldAction action)
            => fireWorld.ActAsync(action);
    }

    class ActiveFireWorld : ActiveObject<FireWorld>
    {
        public ActiveFireWorld(FireWorld passive)
            : base(passive)
        {
        }

        #region Internal

        public Task InitializeAsync()
            => CallAction(() => obj.Initialize());

        public Task ResetWaterAsync()
            => CallAction(() => obj.ResetWater());

        public Task StartFireAsync()
            => CallAction(() => obj.StartFire());

        #endregion // Internal

        #region Sense & act

        public Task<FireWorldPercept> PerceiveAsync()
            => CallFunction2(() => obj.Perceive());

        public Task<bool> ActAsync(FireWorldAction action)
            => CallFunction2(() => obj.Act(action));

        #endregion // Sense & act
    }
}