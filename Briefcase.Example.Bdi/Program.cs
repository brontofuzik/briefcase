﻿using Briefcase.Example.Bdi.Environment;

namespace Briefcase.Example.Bdi
{
    class Program
    {
        public static bool Debug = false;

        static void Main(string[] args)
        {
            RunTurnBased();
        }

        private static void RunTurnBased()
        {
            var mas = new TurnBasedMas(new TurnBasedFireWorld());
            mas.AddAgent(new Fireman("sam"));
            mas.Run();
        }

        private static void RunRealTime()
        {
            var mas = new RealTimeMas(new RealTimeFireWorld());
            mas.AddAgent(new Fireman("sam"));
            mas.Run();
        }
    }
}
