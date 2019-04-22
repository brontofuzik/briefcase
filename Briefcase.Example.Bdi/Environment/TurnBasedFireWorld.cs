using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Briefcase.Example.Bdi.Environment
{
    class TurnBasedFireWorld : Environments.TurnBasedEnvironment
    {
        private readonly FireWorld fireWorld = new FireWorld();

        // Shortcut
        private Fireman FiremanAgent => Mas.GetAllAgents().Single() as Fireman;

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

        public FireWorldPercept Perceive() => fireWorld.Perceive();

        public bool Act(FireWorldAction action) => fireWorld.Act(action);

        public override string Show() => fireWorld.Show(FiremanAgent.Show());

        private void Debug(string message, [CallerMemberName] string method = null)
        {
            if (Program.Debug)
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
