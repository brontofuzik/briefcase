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
        private readonly HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
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

            // Determine safe and unsafe locations.
            var pits = new HashSet<(int x, int y)>(); // Certain pit
            var wumpuses = new HashSet<(int x, int y)>(); // Certain wumpus
            var safe = new HashSet<(int x, int y)>(); // Certainly safe
            var @unsafe = new HashSet<(int x, int y)>(); // Certainly unsafe

            var candidates = visited.SelectMany(n => WumpusEnvironment.Neighbors(n)).Distinct()
                .Where(n => !visited.Contains(n)).ToList();

            foreach (var n in candidates)
            {
                if (kb.IsPit(n))
                    pits.Add(n);

                if (kb.IsWumpus(n))
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

            if (safe.Any())
            {
                Move(safe.First());
            }
            else
            {
                
            }
        }

        private void Move((int x, int y) to)
        {
            // TODO
            throw new System.NotImplementedException();
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
