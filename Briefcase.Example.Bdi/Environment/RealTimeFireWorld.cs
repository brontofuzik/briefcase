using System;
using System.Linq;
using Briefcase.Environments;
using System.Threading.Tasks;
using Briefcase.ActiveObject;
using Briefcase.Example.Bdi.Agents;
using Briefcase.Utils;

namespace Briefcase.Example.Bdi.Environment
{
    class RealTimeFireWorld : RealTimeEnvironment
    {
        private readonly FireWorld passive;
        private readonly ActiveFireWorld active;

        public RealTimeFireWorld()
        {
            passive = new FireWorld();
            active = new ActiveFireWorld(passive);
            passive.AgentActed += AgentActed;
        }

        private void AgentActed(object sender, EventArgs e)
        {
            passive.ResetWater();
            passive.StartFire();
            ShowEnvironment();
        }

        private void ShowEnvironment()
        {
            Console.Clear();
            Console.WriteLine(passive.Show(FiremanAgent.Show()));
        }

        // Shortcut
        private RealTimeFireman FiremanAgent => Mas.GetAllAgents().Single() as RealTimeFireman;

        public override void Initialize()
        {
            active.InitializeAsync().Await();
        }

        public Task<FireWorldPercept> Perceive()
            => active.PerceiveAsync();

        public Task<bool> Act(FireWorldAction action)
            => active.ActAsync(action);
    }

    // Proxy
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