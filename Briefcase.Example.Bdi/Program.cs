using System;
using Briefcase.Example.Environments.FireWorld;

namespace Briefcase.Example.Bdi
{
    class Program
    {
        public static bool Debug = false;

        static void Main(string[] args)
        {
            var env = new FireEnvironment(new FireWorld());

            //RunTurnBased(env);
            RunRealTime(env);
        }

        private static void RunTurnBased(FireEnvironment env)
        {
            var mas = new TurnBasedMas(env);
            mas.AddAgent(new Fireman("sam"));
            mas.Run();
        }

        // TODO
        private static void RunRealTime(FireEnvironment env)
        {
            var mas = new RealTimeMas(env, TimeSpan.FromSeconds(0.5));
            mas.AddAgent(new Fireman("sam"));
            mas.Run();
        }
    }
}
