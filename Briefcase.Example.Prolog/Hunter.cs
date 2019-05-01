using System.Collections.Generic;
using System.Linq;
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
            position = (0, 0);
            direction = Direction.East;
            visited.Add(position);
        }

        public override void Step()
        {
            // Sense
            var percept = WumpusEnvironment.Perceive();
            kb.SenseBreeze(position, percept.HasFlag(WumpusPercept.Breeze));
            kb.SenseStench(position, percept.HasFlag(WumpusPercept.Stench));

            var safe = new HashSet<(int x, int y)>();
            var @unsafe = new HashSet<(int x, int y)>();
            var pits = new HashSet<(int x, int y)>();
            var wumpuses = new HashSet<(int x, int y)>();

            var candidates = visited.SelectMany(n => WumpusEnvironment.Neighbors(n)).ToList();

            foreach (var n in candidates)
            {
                if (kb.IsPit(n) && !visited.Contains(n))
                    pits.Add(n);

                if (kb.IsWumpus(n) && !visited.Contains(n))
                    wumpuses.Add(n);

                if (!kb.IsPit(n) && !kb.IsWumpus(n))
                {
                    safe.Add(n);
                }
                else
                {
                    @unsafe.Add(n);
                }
            }
        }
    }

    class KnowledgeBase
    {
        private readonly PrologEngine pl = new PrologEngine();

        public KnowledgeBase()
        {
            pl.Consult("Hunter.pl");
        }

        public void SenseBreeze((int x, int y) pos, bool breeze)
        {
            pl.ConsultFromString($"sensebreeze([{pos.x}, {pos.y}], {breeze.ToString().ToLower()})");
        }

        public void SenseStench((int x, int y) pos, bool stench)
        {
            pl.ConsultFromString($"sensestench([{pos.x}, {pos.y}], {stench.ToString().ToLower()})");
        }

        public bool IsPit((int x, int y) pos)
        {
            var result = pl.GetFirstSolution(query: $"ispit([{pos.x}, {pos.y}])");
            return result.Solved;
        }

        public bool IsWumpus((int x, int y) pos)
        {
            var result = pl.GetFirstSolution(query: $"iswumpus([{pos.x}, {pos.y}])");
            return result.Solved;
        }
    }
}
