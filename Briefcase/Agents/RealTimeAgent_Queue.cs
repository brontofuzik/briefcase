using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    // Decorator
    internal class RealTimeAgent_Queue : RuntimeAgent
    {
        private readonly ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();

        private readonly Thread actionThread;
        private readonly Thread messageProcessingThread;

        private TimeSpan? stepTime;

        internal RealTimeAgent_Queue(Agent agent)
            :base(agent)
        {
            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            }; 
            
            messageProcessingThread = new Thread(ProcessMessages)
            {
                Name = $"{agent.Id}_messageProcessingThread"
            };
        }

        public string Id => agent.Id;

        private async void Act()
        {
            while (true)
            {
                agent.Step();

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }

        private void ProcessMessages()
        {
            while (true)
            {
                //messagesResetEvent.WaitOne();

                lock (messages)
                {
                    var result = messages.TryDequeue(out Message message);
                    if (result)
                    {
                        agent.HandleMessage(message);
                    }

                    //if (messagesQueue.Count == 0)
                    //    messagesResetEvent.Reset();
                }
            }
        }

        public override void Post(Message message)
        {
            messages.Enqueue(message);
        }

        public override void Run(TimeSpan? stepTime)
        {
            this.stepTime = stepTime;

            actionThread.Start();
            messageProcessingThread.Start();
        }

        public void Kill()
        {
            actionThread.Abort();
            messageProcessingThread.Abort();
        }
    }
}
