using System;
using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new WumpusEnvironment(new WumpusWorld());
            var mas = new MultiagentSystem(env);
            mas.AddAgent(new Hunter("hunter"));
            mas.RunTurnbased(stepTime: TimeSpan.FromSeconds(0.5));
        }
    }
}
