using System;
using System.Threading;

namespace Briefcase
{
    public class TurnBasedMas : MultiagentSystem
    {
        private readonly int? maxTurns;
        private int turn;

        private readonly bool autoRun;
        private readonly TimeSpan stepTime;

        public TurnBasedMas(IEnvironment environment = null, int? maxTurns = null, bool autoRun = false, TimeSpan? stepTime = null)
            : base(environment)
        {
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

        private void Initialize()
        {
            environment?.Initialize();

            foreach (var agent in GetAllAgents())
                agent.Initialize();
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
