using System;
using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            var mas = new MultiagentSystem();

            mas.SetEnvironment(new WumpusWorld());

            mas.AddAgent(new Hunter("hunter"));

            mas.RunTurnbased(stepTime: TimeSpan.FromSeconds(0.5));
        }
    }
}
