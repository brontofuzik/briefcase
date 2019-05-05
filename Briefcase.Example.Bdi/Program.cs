using System;
using System.ComponentModel.Design.Serialization;
using Briefcase.Example.Environments.FireWorld;

namespace Briefcase.Example.Bdi
{
    class Program
    {
        public static bool Debug = false;

        static void Main(string[] args)
        {
            var env = new FireEnvironment(new FireWorld());
            var mas = new MultiagentSystem(env);
            mas.AddAgent(new Fireman("sam"));

            // Run real-time or turn-based?
            mas.RunRealtime(TimeSpan.FromSeconds(0.5));
            mas.RunTurnbased(stepTime: TimeSpan.FromSeconds(0.5));
        }
    }
}
