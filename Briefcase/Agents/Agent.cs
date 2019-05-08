using System.Collections.Concurrent;
using System.Collections.Generic;
using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class Agent
    {
        private readonly string name;

        private readonly ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();

        protected Agent(string name)
        {
            this.name = name;
        }

        public string Id => $"{name}:{GetType().Name}";

        internal MultiagentSystem Mas { get; set; }

        public virtual void Initialize() {}

        public abstract void Step(int turn = 0);

        #region Comm

        protected void Send(string receiver, string content, string conversationId = null)
        {
            Mas.Send(new Message(name, receiver, content, conversationId));
        }

        protected void Send(IEnumerable<string> receivers, string content, string conversationId = null)
        {
            foreach (string receiver in receivers)
                Send(receiver, content, conversationId);
        }

        internal void Post(Message message)
        {
            messages.Enqueue(message);
        }

        #endregion // Comm
    }

    public abstract class SituatedAgent : Agent
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public Environment Environment { get; set; }
    }

    public abstract class SituatedAgent<TEnvironment> : SituatedAgent
        where TEnvironment : Environment
    {
        protected SituatedAgent(string name)
            : base(name)
        {
        }

        public new TEnvironment Environment
        {
            get => (TEnvironment)base.Environment;
            set => base.Environment = value;
        }
    }
}
