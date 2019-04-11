using System.Collections.Concurrent;
using System.Threading;

namespace Briefcase.Environments
{
    public abstract class RealTimeEnvironment : Environment
    {
        private readonly ConcurrentQueue<object> queue = new ConcurrentQueue<object>();

        private readonly Thread thread;

        protected RealTimeEnvironment()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                var result = queue.TryDequeue(out object action);
                if (result)
                    Handle(action);
            }
        }

        protected void Enqueue(object action)
        {
            queue.Enqueue(action);
        }

        protected abstract void Handle(object action);
    }
}
