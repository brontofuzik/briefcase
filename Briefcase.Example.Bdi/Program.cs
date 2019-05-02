using System;
using Briefcase.Example.Environments.FireWorld;

namespace Briefcase.Example.Bdi
{
    class Program
    {
        public static bool Debug = false;

        static void Main(string[] args)
        {
            //RunTurnBased();
            RunRealTime();
        }

        private static void RunTurnBased()
        {
            var mas = new TurnBasedMas(new TurnBasedFireWorld(Debug));
            mas.AddAgent(new TurnBasedFireman("sam"));
            mas.Run();
        }

        private static void RunRealTime()
        {
            var mas = new RealTimeMas(new RealTimeFireWorld(), TimeSpan.FromSeconds(0.5));
            mas.AddAgent(new RealTimeFireman("sam"));
            mas.Run();
        }
    }
}
