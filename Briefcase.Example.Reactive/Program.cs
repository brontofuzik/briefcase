using System;
using Briefcase.Example.Environments.ExplorerWorld;

namespace Briefcase.Example.Reactive
{
    class Program
    {
        private const int Explorers = 5;

        static void Main(string[] args)
        {
            var mas = new MultiagentSystem();

            mas.SetEnvironment(new PlanetWorld());

            for (int i = 1; i <= Explorers; i++)
                mas.AddAgent(new Explorer($"explorer{i}"));

            mas.RunRealtime<PlanetWorld>(stepTime: TimeSpan.FromSeconds(0.5));
        }
    }
}
