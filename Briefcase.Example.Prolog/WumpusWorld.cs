using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Briefcase.Environments;

namespace Briefcase.Example.Prolog
{
    class WumpusWorld : TurnBasedEnvironment<WumpusPercept, WumpusAction, ActionResult>
    {
        private const int Size = 4;

        public Terrain[,] map = new Terrain[Size, Size];

        public (int x, int y) hunterPosition;
        public Direction hunterDirection;

        // Returns None for positions outside the grid.
        private Terrain TerrainAt((int x, int y) p)
            => IsWithinBounds(p) ? map[p.x, p.y] : Terrain.None;

        private void SetTerrainAt((int x, int y) p, Terrain terrain)
        {
            if (IsWithinBounds(p))
                map[p.x, p.y] = terrain;
        }

        private static bool IsWithinBounds((int x, int y) p)
            => p.x.IsIn(0, Size) && p.y.IsIn(0, Size);

        public override void Initialize()
        {
            for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
            {
                map[x, y] = Terrain.Empty;
            }

            map[2, 0] = map[2, 2] = map[3, 3] = Terrain.Pit;
            map[0, 2] = Terrain.Wumpus;
            map[1, 2] = Terrain.Gold;

            hunterPosition = (0, 0);
            hunterDirection = Direction.East;
        }

        public override WumpusPercept Perceive()
        {
            WumpusPercept percept = 0;

            // Pit nearby -> breeze
            if (Neighbors(hunterPosition).Any(p => TerrainAt(p) == Terrain.Pit))
                percept |= WumpusPercept.Breeze;

            // Wumpus nearby -> stench
            if (Neighbors(hunterPosition).Any(p => TerrainAt(p) == Terrain.Wumpus))
                percept |= WumpusPercept.Stench;

            // Gold here -> glitter
            if (TerrainAt(hunterPosition) == Terrain.Gold)
                percept |= WumpusPercept.Glitter;

            return percept;
        }

        private IEnumerable<(int x, int y)> Neighbors((int x, int y) position)
            => Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .Select(d => PositionTo(position, d));

        private (int x, int y) PositionTo((int x, int y) position, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return (position.x, position.y + 1);
                case Direction.East:
                    return (position.x + 1, position.y);
                case Direction.South:
                    return (position.x, position.y - 1);
                case Direction.West:
                    return (position.x - 1, position.y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public override ActionResult Act(WumpusAction action)
        {
            switch (action)
            {
                case WumpusAction.Walk:
                    return Act_Walk();

                case WumpusAction.TurnLeft:
                    return Act_TurnLeft();

                case WumpusAction.TurnRight:
                    return Act_TurnRight();

                case WumpusAction.Shoot:
                    return Act_Shoot();

                case WumpusAction.Grab:
                    return Act_Grab();

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private ActionResult Act_Walk()
        {
            (int x, int y) newPosition = PositionTo(hunterPosition, hunterDirection);
            Terrain newTerrain = TerrainAt(newPosition);

            switch (newTerrain)
            {
                case Terrain.None:
                    return ActionResult.Bump;
                case Terrain.Empty:
                    return ActionResult.Ok;
                case Terrain.Pit:
                    throw new Exception("Hunter fell into a pit!");
                case Terrain.Wumpus:
                    throw new Exception("Hunter got eaten by a wumpus!");
                case Terrain.Gold:
                    return ActionResult.Ok;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ActionResult Act_TurnLeft()
        {
            hunterDirection = (Direction) (((int)hunterDirection - 1) % 4);
            return ActionResult.Ok;
        }

        private ActionResult Act_TurnRight()
        {
            hunterDirection = (Direction)(((int)hunterDirection + 1) % 4);
            return ActionResult.Ok;
        }

        private ActionResult Act_Shoot()
        {
            (int x, int y) arrowPosition = hunterPosition;
            do
            {
                arrowPosition = PositionTo(arrowPosition, hunterDirection);
            }
            while (TerrainAt(arrowPosition) == Terrain.Wumpus
                || TerrainAt(arrowPosition) == Terrain.None);

            if (TerrainAt(arrowPosition) == Terrain.Wumpus)
            {
                // Wumpus hit!
                SetTerrainAt(arrowPosition, Terrain.Empty);
                return ActionResult.Scream;
            }
            else
            {
                // No wumpus hit.
                return ActionResult.Ok;
            } 
        }

        private ActionResult Act_Grab()
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public enum Terrain
    {
        None,
        Empty,
        Pit,
        Wumpus,
        Gold
    }

    public enum Direction
    {
        North = 0,
        East,
        South,
        West
    }

    [Flags]
    enum WumpusPercept
    {
        Breeze,
        Stench,
        Glitter
    }

    enum WumpusAction
    {
        Walk,
        TurnLeft,
        TurnRight,
        Shoot,
        Grab
    }

    enum ActionResult
    {
        Ok,
        Bump,
        Scream
    }
}
