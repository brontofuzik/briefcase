using Briefcase.Environments;
using System;
using System.Threading;
using Briefcase.Agents;

namespace Briefcase
{
    public class TurnBasedMas : MultiagentSystem
    {
        private new readonly TurnBasedEnvironment environment;
        private readonly int? maxTurns;
        private int turn;

        private readonly TimeSpan? stepTime;

        public TurnBasedMas(TurnBasedEnvironment environment = null, int? maxTurns = null, bool autoRun = false, TimeSpan? stepTime = null)
            : base(environment)
        {
            this.environment = environment;
            this.maxTurns = maxTurns;
            this.stepTime = stepTime;
        }

        public void Run()
        {
            Initialize();

            ShowEnvironment();
            Wait();

            while (!maxTurns.HasValue || turn < maxTurns)
            {
                RunTurn(turn++);
                ShowEnvironment();
                Wait();
            }
        }

        private void Wait()
        {
            if (stepTime.HasValue)
                Thread.Sleep(stepTime.Value);
            else
                Console.ReadKey();
        }

        private void RunTurn(int turn)
        {
            environment?.BeginTurn(turn);

            foreach (var agent in GetAllAgents())
                ((TurnBasedAgent)agent).Step();

            environment?.EndTurn(turn);
        }

        private void ShowEnvironment()
        {
            Console.Clear();
            Console.WriteLine(environment.Show());
        }
    }
}
