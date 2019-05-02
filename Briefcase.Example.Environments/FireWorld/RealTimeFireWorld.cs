using System;
using System.Threading.Tasks;
using Briefcase.ActiveObject;
using Briefcase.Environments;
using Briefcase.Utils;

namespace Briefcase.Example.Environments.FireWorld
{
    public class RealTimeFireWorld : RealTimeEnvironment
    {
        private readonly ActiveFireWorld active;

        public RealTimeFireWorld()
        {
            active = new ActiveFireWorld(new FireWorld());
            active.Passive.AfterAct += AfterAct;
        }

        private void AfterAct(object sender, EventArgs e)
        {
            active.Passive.ResetWater();
            active.Passive.StartFire();
            ShowEnvironment();
        }

        private void ShowEnvironment()
        {
            Console.Clear();
            Console.WriteLine(active.Passive.Show());
        }

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
            => CallAction(() => Passive.Initialize());

        public Task ResetWaterAsync()
            => CallAction(() => Passive.ResetWater());

        public Task StartFireAsync()
            => CallAction(() => Passive.StartFire());

        #endregion // Internal

        #region Sense & act

        public Task<FireWorldPercept> PerceiveAsync()
            => CallFunction2(() => Passive.Perceive());

        public Task<bool> ActAsync(FireWorldAction action)
            => CallFunction2(() => Passive.Act(action));

        #endregion // Sense & act
    }
}