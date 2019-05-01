using System;
using System.Linq;
using Briefcase.Agents;
using Prolog;
using SbsSW.SwiPlCs;

namespace Briefcase.Example.Prolog
{
    class Hunter : TurnBasedAgent
    {
        private const string filename = @"..\..\Prolog\Inants.pl";

        private readonly IKnowledgeBase kb = new SwiPlCsKnowledgeBase(filename);

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

    internal interface IKnowledgeBase
    {
        void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation);

        WumpusAction RunAgent(WumpusPercept percept);
    }

    class SwiPlCsKnowledgeBase : IKnowledgeBase
    {
        private readonly string filename;

        public SwiPlCsKnowledgeBase(string filename)
        {
            this.filename = filename;

            if (!PlEngine.IsInitialized)
            {
                PlEngine.Initialize(new[] {"-q", "-f", filename });
            }
        }

        public void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation)
        {
            PlQuery.PlCall($"init_agent([{size}, {pitProbability}, [{initialCell.x}, {initialCell.y}], {initialOrientation}])");
        }

        public WumpusAction RunAgent(WumpusPercept percept)
        {
            // Library-independent
            var stench = percept.HasFlag(WumpusPercept.Stench) ? "yes" : "no";
            var breeze = percept.HasFlag(WumpusPercept.Breeze) ? "yes" : "no";
            var glitter = percept.HasFlag(WumpusPercept.Glitter) ? "yes" : "no";
            var scream = "no"; // TODO
            var bump = "no"; // TODO
            string perceptPl = $"[{stench}, {breeze}, {glitter}, {scream}, {bump}]";

            var q = new PlQuery($"run_agent({perceptPl}, Action)");
            var actionPl = q.SolutionVariables.First()["Action"].ToString();

            // Library-independent
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

    class CsPrologKnowledgeBase : IKnowledgeBase
    {
        private readonly string filename;
        private readonly PrologEngine pl = new PrologEngine();

        public CsPrologKnowledgeBase(string filename)
        {
            this.filename = filename;
        }

        public CsPrologKnowledgeBase()
        {
            pl.Consult(filename);
        }

        public void InitAgent(int size, double pitProbability, (int x, int y) initialCell, int initialOrientation)
        {
            pl.GetFirstSolution($"init_agent([{size}, {pitProbability}, [{initialCell.x}, {initialCell.y}], {initialOrientation}])");
        }

        public WumpusAction RunAgent(WumpusPercept percept)
        {
            // Library-independent
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

            // Library-independent
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
