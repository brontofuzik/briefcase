using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class SituatedAgent : Agent
    {
        protected SituatedAgent(string id)
            : base(id)
        {
        }

        internal Environment Environment { get; set; }
    }

    public abstract class SituatedAgent<E, S, P, A, R> : SituatedAgent
        where E : Environment<S, P, A, R>
    {
        protected SituatedAgent(string id)
            : base(id)
        {
        }

        protected new E Environment
        {
            get => (E)base.Environment;
            set => base.Environment = value;
        }

        public override void Step(int turn = 0)
        {
            var percept = Environment.Perceive(Id);
            var action = PerceiveAndAct(percept);
            var result = Environment.Act(Id, action);
        }

        protected abstract A PerceiveAndAct(P arg);
    }
}