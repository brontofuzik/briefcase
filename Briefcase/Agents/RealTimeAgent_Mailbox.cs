using Actress;
using Briefcase.Environments;

namespace Briefcase.Agents
{
    internal class RealTimeAgent_Mailbox : RuntimeAgent
    {
        private readonly MailboxProcessor<Message> mailbox;

        internal RealTimeAgent_Mailbox(Agent agent, RuntimeEnvironment environment)
            : base(agent, environment)
        {
            mailbox = MailboxProcessor.Start<Message>(async mb =>
            {
                while (true)
                {
                    var message = await mb.Receive();
                    agent.HandleMessage(message);
                }
            });
        }

        public override void Post(Message message)
        {
            mailbox.Post(message);
        }
    }
}
