using System;
using Briefcase.Agents;

namespace Briefcase.Example.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            var mas = new MultiagentSystem();

            mas.AddAgent(new SimpleAgent("simple1"));
            //mas.AddAgent(new SimpleAgent("simple2"));
            mas.AddAgent(new Logger("logger"));

            //mas.RunTurnbased(stepTime: TimeSpan.FromSeconds(0.5));
            mas.RunRealtime();
        }
    }

    class SimpleAgent : Agent
    {
        public SimpleAgent(string name)
            : base(name)
        {
        }

        public override void Step(int turn = 0)
        {
            Send("logger", $"Hello from {Id}: {DateTime.Now}");
        }

        protected override void HandleMessage(Message message)
        {
            // Do nothing.
        }
    }

    class Logger : Agent
    {
        public Logger(string name)
            : base(name)
        {
        }

        public override void Step(int turn = 0)
        {
            // Do nothing.
        }

        protected override void HandleMessage(Message message)
        {
            Console.WriteLine(message.Content);
        }
    }
}
