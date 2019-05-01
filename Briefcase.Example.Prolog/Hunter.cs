using System.Collections.Generic;
using Briefcase.Agents;
using Prolog;

namespace Briefcase.Example.Prolog
{
    class Hunter : TurnBasedAgent
    {
        private const int MaxArrows = 1;    

        private readonly KnowledgeBase kb = new KnowledgeBase();
        private readonly List<(int x, int y)> visited = new List<(int x, int y)>();
        private int arrows = MaxArrows;

        // Hunter's position
        private (int x, int y) position;
        private Direction direction;

        public Hunter(string name)
            : base(name)
        {
        }

        // Shortcut
        private WumpusWorld WumpusEnvironment
            => Environment as WumpusWorld;

        public override void Initialize()
        {          
            visited.Add((0, 0));
        }

        public override void Step()
        {
            // Sense
            var percept = WumpusEnvironment.Perceive();
            kb.SenseBreeze(position, percept.HasFlag(WumpusPercept.Breeze));
            kb.SenseStench(position, percept.HasFlag(WumpusPercept.Stench));



        }
    }

    class KnowledgeBase
    {
        private readonly PrologEngine pl = new PrologEngine();

        public KnowledgeBase()
        {
            pl.Consult("Hunter.pl");
        }

        public void SenseBreeze((int x, int y) position, bool breeze)
        {
            pl.ConsultFromString($"sensebreeze([{position.x}, {position.y}], {breeze.ToString().ToLower()})");
        }

        public void SenseStench((int x, int y) position, bool stench)
        {
            pl.ConsultFromString($"sensestench([{position.x}, {position.y}], {stench.ToString().ToLower()})");
        }
    }
}
