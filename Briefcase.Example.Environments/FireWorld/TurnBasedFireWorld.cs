using System;
using System.Runtime.CompilerServices;
using Briefcase.Environments;

namespace Briefcase.Example.Environments.FireWorld
{
    public class TurnBasedFireWorld : TurnBasedEnvironment<FireWorldPercept, FireWorldAction, bool>
    {
        private readonly FireWorld fireWorld = new FireWorld();
        private readonly bool debug;

        public TurnBasedFireWorld(bool debug)
        {
            this.debug = debug;
        }

        public override void Initialize()
        {
            fireWorld.Initialize();
        }

        public override void BeginTurn(int turn)
        {
            Debug("Beginning turn");

            fireWorld.ResetWater();
            fireWorld.StartFire();

            Debug("Began turn");
        }

        public override void EndTurn(int turn)
        {
            Debug("Ending turn");
        }

        public override FireWorldPercept Perceive() => fireWorld.Perceive();

        public override bool Act(FireWorldAction action) => fireWorld.Act(action);

        public override string Show() => fireWorld.Show();

        private void Debug(string message, [CallerMemberName] string method = null)
        {
            if (debug)
            {
                lock (Console.Out)
                {
                    Console.WriteLine($"{nameof(TurnBasedFireWorld)}.{method}:");
                    Console.WriteLine(fireWorld.Print());
                }
            }
        }
    }
}
