using System;
using System.Linq;
using Briefcase.Agents;
using Prolog;

namespace Briefcase.Example.Prolog
{
    class Hunter : TurnBasedAgent
    {
        private InantsKnowledgeBase kb = new InantsKnowledgeBase();

        public Hunter(string name)
            : base(name)
        {
        }

        // Shortcut
        private WumpusWorld WumpusEnvironment
            => Environment as WumpusWorld;

        public override void Initialize()
        {
            kb.InitAgent(4, 0.2, (1, 1), 0);
        }

        public override void Step()
        {
            var percept = WumpusEnvironment.Perceive();
            var action = kb.RunAgent(percept);
            var result = WumpusEnvironment.Act(action);
        }
    }

    class InantsKnowledgeBase
    {
        private readonly PrologEngine pl = new PrologEngine();

        public InantsKnowledgeBase()
        {
            pl.Consult(@"..\..\Prolog\Inants.pl");
        }

        public void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation)
        {
            pl.GetFirstSolution($"init_agent([{size}, {pitProbability}, [{initialCell.x}, {initialCell.y}], {initialOrientation}])");
        }

        public WumpusAction RunAgent(WumpusPercept percept)
        {
            var stench = percept.HasFlag(WumpusPercept.Stench) ? "yes" : "no";
            var breeze = percept.HasFlag(WumpusPercept.Breeze) ? "yes" : "no";
            var glitter = percept.HasFlag(WumpusPercept.Glitter) ? "yes" : "no";
            var scream = "no"; // TODO
            var bump = "no"; // TODO
            string perceptPl = $"[{stench}, {breeze}, {glitter}, {scream}, {bump}]";

            var solution = pl.GetFirstSolution($"run_agent({perceptPl}, Action)");

            if (!solution.Solved)
                throw new Exception("Cannot solve run_agent(+Percept, -Action)!");

            var actionPl = solution.VarValuesIterator.Single(v => v.Name == "Action").ToString();

            switch (actionPl)
            {
                case "forward": return WumpusAction.MoveForward;
                case "turnLeft": return WumpusAction.TurnLeft;
                case "turnRight": return WumpusAction.TurnRight;
                case "shoot": return WumpusAction.Shoot;
                case "grab": return WumpusAction.Grab;
                default: throw new Exception("Unknown action returned by run_agent!");
            }
        }
    }
}
