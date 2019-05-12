using Briefcase.Agents;
using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Hunter : SituatedAgent<WumpusEnvironment>
    {
        private readonly IKnowledgeBase kb = null; // TODO

        public Hunter(string id)
            : base(id)
        {
        }

        public override void Initialize()
        {
            kb.InitAgent(4, 0.2, (1, 1), 0);
        }

        public override void Step(int turn = 0)
        {
            var percept = Environment.Perceive(Id);
            var action = kb.RunAgent(percept);
            var result = Environment.Act(Id, action);
        }

        protected override void HandleMessage(Message message)
        {
            throw new System.NotImplementedException();
        }
    }

    internal interface IKnowledgeBase
    {
        void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation);

        WumpusAction RunAgent(WumpusPercept percept);
    }
}
