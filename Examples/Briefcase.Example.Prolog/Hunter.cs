using Briefcase.Agents;
using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Hunter : SituatedAgent<WumpusWorld, object, WumpusPercept, WumpusAction, ActionResult>
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

        protected override WumpusAction PerceiveAndAct(WumpusPercept percept)
            => kb.RunAgent(percept);

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
