using System;
using System.Threading;
using System.Threading.Tasks;
using Actress;

namespace Briefcase.Agents
{
    // Decorator
    public class RealTimeAgent_Mailbox
    {
        private readonly Agent agent;

        private readonly Thread actionThread;
        private readonly MailboxProcessor<Message> mailbox;

        private TimeSpan? stepTime;

        internal RealTimeAgent_Mailbox(Agent agent)
        {
            this.agent = agent;

            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            };

            mailbox = MailboxProcessor.Start<Message>(async mb =>
            {
                while (true)
                {
                    var message = await mb.Receive();
                    agent.HandleMessage(message);
                }
            });
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

        public void Run(TimeSpan? stepTime)
        {
            this.stepTime = stepTime;

            actionThread.Start();
        }

        public void Kill()
        {
            actionThread.Abort();
        }
    }
}
