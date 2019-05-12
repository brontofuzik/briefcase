using System;
using System.Collections.Generic;
using System.Linq;
using Briefcase.Utils;
using Environment = Briefcase.Environments.Environment;

namespace Briefcase.Example.Environments.WumpusWorld
{
    public class WumpusWorld : Environment
    {
        private const int Size = 4;

        public Terrain[,] map = new Terrain[Size, Size];

        // Hunter's position
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
            => p.x.IsWithinInterval(0, Size)
               && p.y.IsWithinInterval(0, Size);

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

        public override object Perceive(string agentId, object sensor = null)
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

        public IEnumerable<(int x, int y)> Neighbors((int x, int y) position)
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

        public override object Act(string agentId, object action)
        {
            var a = (WumpusAction) action;
            switch (a)
            {
                case WumpusAction.MoveForward:
                    return Act_MoveForward();

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

        private ActionResult Act_MoveForward()
        {
            (int x, int y) newPosition = PositionTo(hunterPosition, hunterDirection);
            Terrain newTerrain = TerrainAt(newPosition);

            switch (newTerrain)
            {
                case Terrain.None:
                    return ActionResult.Bump;
                case Terrain.Empty:
                    return ActionResult.Success;
                case Terrain.Pit:
                    throw new Exception("Hunter fell into a pit!"); // End
                case Terrain.Wumpus:
                    throw new Exception("Hunter got eaten by a wumpus!"); // End
                case Terrain.Gold:
                    return ActionResult.Success;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ActionResult Act_TurnLeft()
        {
            hunterDirection = (Direction) (((int)hunterDirection - 1) % 4);
            return ActionResult.Success;
        }

        private ActionResult Act_TurnRight()
        {
            hunterDirection = (Direction)(((int)hunterDirection + 1) % 4);
            return ActionResult.Success;
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
                return ActionResult.Fail;
            } 
        }

        private ActionResult Act_Grab()
            => TerrainAt(hunterPosition) == Terrain.Gold
                ? throw new Exception("Hunter found the gold!") // End
                : ActionResult.Fail;
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
}
