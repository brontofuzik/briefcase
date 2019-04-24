using System;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    public abstract class RealTimeAgent : Agent
    {
        private readonly Thread thread;

        protected RealTimeAgent(string name)
            : base(name)
        {
            thread = new Thread(Loop)
            {
                Name = $"RealTimeAgent_{name}"
            };          
        }

        private async void Loop()
        {
            while (true)
            {
                await Act();
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        protected abstract Task Act();

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
