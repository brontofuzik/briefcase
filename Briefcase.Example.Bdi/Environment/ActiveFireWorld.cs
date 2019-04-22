using System;
using System.Threading.Tasks;
using Briefcase.ActiveObject;
using Briefcase.ActiveObject.Futures;

namespace Briefcase.Example.Bdi.Environment
{
    // TODO Can be auto-generated?
    class ActiveFireWorld : PassiveObjectActiveProxy<FireWorld>
    {
        public ActiveFireWorld(IDispatcher dispatcher)
            : base(dispatcher)
        {
        }

        #region Internal

        public Task<int> InitializeAsync()
            => Execute<int>(nameof(FireWorld.Initialize)).AsTask();

        public Task<int> ResetWaterAsync()
            => Execute<int>(nameof(FireWorld.ResetWater)).AsTask();

        public Task<int> StartFireAsync()
            => Execute<int>(nameof(FireWorld.StartFire)).AsTask();

        #endregion // Internal

        #region Sense & act

        public Task<FireWorldPercept> PerceiveAsync()
            => Execute<FireWorldPercept>(nameof(FireWorld.Perceive)).AsTask();

        public Task<bool> ActAsync(FireWorldAction action)
            => Execute<bool>(nameof(FireWorld.Perceive), action).AsTask();

        #endregion // Sense & act
    }
}
