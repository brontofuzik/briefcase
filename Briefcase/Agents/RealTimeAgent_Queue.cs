using System;
using System.Collections.Concurrent;
using System.Threading;
using Briefcase.Environments;
using Environment = Briefcase.Environments.Environment;

namespace Briefcase.Agents
{
    // Decorator
    internal class RealTimeAgent_Queue<E, S, P, A, R> : RuntimeAgent<E, S, P, A, R>
        where E : Environment<S, P, A, R>
    {
        private readonly ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();
        private readonly ManualResetEvent messagesResetEvent = new ManualResetEvent(false);
        private readonly Thread messageProcessingThread;

        internal RealTimeAgent_Queue(Agent agent, RuntimeEnvironment<E, S, P, A, R> environment)
            : base(agent, environment)
        {        
            messageProcessingThread = new Thread(ProcessMessages)
            {
                Name = $"{agent.Id}_messageProcessingThread"
            };
        }

        private void ProcessMessages()
        {
            while (true)
            {
                messagesResetEvent.WaitOne();

                lock (messages)
                {
                    var result = messages.TryDequeue(out Message message);
                    if (result)
                    {
                        agent.HandleMessage(message);
                    }

                    if (messages.Count == 0)
                        messagesResetEvent.Reset();
                }
            }
        }

        public override void Post(Message message)
        {
            messages.Enqueue(message);
        }

        public override void Run(TimeSpan? stepTime)
        {
            base.Run(stepTime);
            messageProcessingThread.Start();
        }

        public override void Kill()
        {
            base.Kill();
            messageProcessingThread.Abort();
        }
    }
}
