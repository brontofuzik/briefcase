using System.Threading;

namespace Briefcase.Agents
{
    public abstract class RealTimeAgent : Agent
    {
        private readonly Thread thread;

        public RealTimeAgent(string name)
            : base(name)
        {
            thread = new Thread(Loop);
            
        }

        private void Loop()
        {
            while (true)
            {
                Act();
                Thread.Sleep(1000);
            }
        }

        public void Run()
        {
            thread.Start();
        }

        public void Kill()
        {
            thread.Abort();
        }
    }
}
