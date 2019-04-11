using Briefcase.Environments;
using System;
using System.Threading;

namespace Briefcase
{
    public class TurnBasedMas : MultiagentSystem
    {
        private new readonly TurnBasedEnvironment environment;
        private readonly int? maxTurns;
        private int turn;

        private readonly bool autoRun;
        private readonly TimeSpan stepTime;

        public TurnBasedMas(TurnBasedEnvironment environment = null, int? maxTurns = null, bool autoRun = false, TimeSpan? stepTime = null)
            : base(environment)
        {
            this.environment = environment;
            this.maxTurns = maxTurns;
            this.autoRun = autoRun;
            this.stepTime = stepTime ?? TimeSpan.FromSeconds(1);
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
            if (autoRun)
                Thread.Sleep(stepTime);
            else
                Console.ReadKey();
        }

        private void RunTurn(int turn)
        {
            environment?.BeginTurn(turn);

            foreach (var agent in GetAllAgents())
                agent.Act();

            environment?.EndTurn(turn);
        }

        private void ShowEnvironment()
        {
            Console.Clear();
            Console.WriteLine(environment.Show());
        }
    }
}
