using System;
using System.Linq;
using System.Text;

namespace Briefcase.Example.Bdi.Environment
{
    class FireWorld
    {
        public const int Size = 10;

        private readonly Random random = new Random();

        private readonly Terrain[] world = new Terrain[Size];
        private int firemanPosition;

        // Shortcut
        public int? FireLocation => world.IndexOf(t => t == Terrain.Fire);

        public void Initialize()
        {
            // Water at location 0.
            SetTerrain((i, _) => i == 0 ? Terrain.Water : Terrain.Normal);
            firemanPosition = 0;
        }

        public void ResetWater()
        {
            // Change getting-water to water.
            SetTerrain((_, t) => t == Terrain.GettingWater ? Terrain.Water : t);
        }

        public void StartFire()
        {
            if (!FireLocation.HasValue)
                world[random.Next(5, Size)] = Terrain.Fire;
        }

        public Percept Perceive()
        {
            // Left edge
            if (firemanPosition == 0)
                return new Percept(firemanPosition, new[]
                {
                        Terrain.None,
                        world[firemanPosition],
                        world[firemanPosition + 1]
                });

            // Right edge
            else if (firemanPosition == Size - 1)
                return new Percept(firemanPosition, new[]
                {
                    world[firemanPosition - 1],
                    world[firemanPosition],
                    Terrain.None
                });

            // Middle
            else
                return new Percept(firemanPosition, new[]
                {
                    world[firemanPosition - 1],
                    world[firemanPosition],
                    world[firemanPosition + 1]
                });
        }

        public bool Act(Action action)
        {
            switch (action)
            {
                case Action.MoveLeft:
                    if (firemanPosition > 0)
                    {
                        firemanPosition -= 1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Action.MoveRight:
                    if (firemanPosition < Size - 1)
                    {
                        firemanPosition += 1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Action.GetWater:
                    if (world[firemanPosition] == Terrain.Water)
                    {
                        world[firemanPosition] = Terrain.GettingWater;
                        return true; // Message: got-water
                    }
                    return false;

                case Action.ExtinguishFire:
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

        // No lock on terrain is necessary!
        private void SetTerrain(Func<int, Terrain, Terrain> setter)
        {
            for (int i = 0; i < Size; i++)
                world[i] = setter(i, world[i]);
        }

        internal string Show(string firemanShow)
        {
            const char horizontalBar = '─';
            const char verticalBar = '|';

            const string normal = " ";
            const string fire = "▒";
            const string water = "█";
            const string gettingWater = "▄";

            var ruler = new String(horizontalBar, 2 * Size + 1);

            var agentArray = Enumerable.Range(0, Size).Select(p => p == firemanPosition ? firemanShow : normal);
            var agentRow = $"|{String.Join(verticalBar.ToString(), agentArray)}|";

            var worldArray = world.Select(t => t.Switch(normal,
                (Terrain.Fire, fire),
                (Terrain.Water, water),
                (Terrain.GettingWater, gettingWater)));
            var worldRow = $"|{String.Join(verticalBar.ToString(), worldArray)}|";

            return new StringBuilder()
                .AppendLine(ruler)
                .AppendLine(agentRow)
                .AppendLine(ruler)
                .AppendLine(worldRow)
                .Append(ruler)
                .ToString();
        }

        // DEBUG
        internal string Print()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Fireman: {firemanPosition}");

            string fireLocation = FireLocation.HasValue ? FireLocation.Value.ToString() : "None";
            sb.AppendLine($"Fire: {fireLocation}");

            return sb.ToString();
        }
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
