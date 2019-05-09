using System;
using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.Agents
{
    // Decorator
    public class RealTimeAgent
    {
        private readonly Agent agent;

        private readonly Thread actionThread;
        private readonly Thread messageProcessingThread;

        private TimeSpan? stepTime;

        internal RealTimeAgent(Agent agent)
        {
            this.agent = agent;

            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            }; 
            
            messageProcessingThread = new Thread(ProcessMessages)
            {
                Name = $"{agent.Id}_messageProcessingThread"
            };
        }

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

                lock (agent.messages)
                {
                    var result = agent.messages.TryDequeue(out Message message);
                    if (result)
                    {
                        agent.HandleMessage(message);
                    }

                    //if (messagesQueue.Count == 0)
                    //    messagesResetEvent.Reset();
                }
            }
        }

        public void Run(TimeSpan? stepTime)
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
