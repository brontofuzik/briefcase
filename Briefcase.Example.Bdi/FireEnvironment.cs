using System;
using System.Linq;

namespace Briefcase.Example.Bdi
{
    class FireEnvironment : Environment
    {
        public const int Size = 15;

        private readonly Random random = new Random();

        private readonly Terrain[] world = new Terrain[Size];
        private int firemanPosition;

        public override void Initialize()
        {
            ChangeTerrain((i, _) => i == 0 ? Terrain.Water : Terrain.Normal);
            firemanPosition = 0;
        }

        public override void BeginTurn(int turn)
        {
            // Reset water
            ChangeTerrain((_, t) => t == Terrain.GettingWater ? Terrain.Water : t);

            // Start a fire!
            if (!world.Any(t => t == Terrain.Fire))
                world[random.Next(1, Size)] = Terrain.Fire;
        }

        public override void EndTurn(int turn)
        {
            // Do nothing?
        }

        public Percept Perceive()
        {
            if (firemanPosition == 0)
            {
                // Left edge
                return new Percept
                {
                    Left = Terrain.None,
                    Center = world[firemanPosition],
                    Right = world[firemanPosition + 1]
                };
            }
            else if (firemanPosition == Size - 1)
            {
                // Right edge
                return new Percept
                {
                    Left = world[firemanPosition - 1],
                    Center = world[firemanPosition],
                    Right = Terrain.None
                };
            }
            else
            {
                // Middle
                return new Percept
                {
                    Left = world[firemanPosition - 1],
                    Center = world[firemanPosition],
                    Right = world[firemanPosition + 1]
                };
            }
        }

        public bool Act(Action action)
        {
            switch (action.Type)
            {
                case "move":
                    firemanPosition += action.Params == "right" ? +1 : -1;
                    return true;

                case "get-water":
                    if (world[firemanPosition] == Terrain.Water)
                    {
                        world[firemanPosition] = Terrain.GettingWater;
                        return true; // Message: got-water
                    }
                    return false;

                case "drop-water":
                    if (world[firemanPosition] == Terrain.Fire)
                    {
                        world[firemanPosition] = Terrain.Normal;
                        return true; // Message: fire-out
                    }
                    return false;

                default:
                    return false;
            }
        }

        private void ChangeTerrain(Func<int, Terrain, Terrain> setter)
        {
            for (int i = 0; i < Size; i++)
                world[i] = setter(i, world[i]);
        }
    }

    public struct Percept
    {
        public Terrain Left { get; internal set; }

        public Terrain Center { get; internal set; }

        public Terrain Right { get; internal set; }
    }

    public struct Action
    {
        // move, get-water, drop-water
        public string Type { get; internal set; }

        // left, right
        public string Params { get; internal set; }
    }

    public enum Terrain
    {
        Normal,
        Fire,
        Water,
        GettingWater,
        None
    }
}
