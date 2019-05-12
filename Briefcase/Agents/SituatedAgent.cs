using Briefcase.Environments;

namespace Briefcase.Agents
{
    public abstract class SituatedAgent : Agent
    {
        protected SituatedAgent(string id)
            : base(id)
        {
        }

        protected internal Environment Environment { get; set; }

        public override void Step(int turn = 0)
        {
            var percept = Environment.Perceive(Id);
            var action = PerceiveAndAct(percept);
            var result = Environment.Act(Id, action);
        }

        protected internal abstract object PerceiveAndAct(object percept);
    }
}