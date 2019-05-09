using System;
using System.Threading;
using System.Threading.Tasks;
using Actress;

namespace Briefcase.Agents
{
    internal class RealTimeAgent_Mailbox : RuntimeAgent
    {
        private readonly Agent agent;

        private readonly Thread actionThread;
        private readonly MailboxProcessor<Message> mailbox;

        private TimeSpan? stepTime;

        public string Id => agent.Id;

        internal RealTimeAgent_Mailbox(Agent agent)
            : base(agent)
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

        public override void Post(Message message)
        {
            mailbox.Post(message);
        }

        public override void Run(TimeSpan? stepTime)
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
