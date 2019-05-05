using Briefcase.Agents;
using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Hunter : Agent<WumpusEnvironment>
    {
        private readonly IKnowledgeBase kb = null; // TODO

        public Hunter(string name)
            : base(name)
        {
        }

        public override void Initialize()
        {
            kb.InitAgent(4, 0.2, (1, 1), 0);
        }

        public override void Step()
        {
            var percept = Environment.Perceive();
            var action = kb.RunAgent(percept);
            var result = Environment.Act(action);
        }
    }

    internal interface IKnowledgeBase
    {
        void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation);

        WumpusAction RunAgent(WumpusPercept percept);
    }
}
