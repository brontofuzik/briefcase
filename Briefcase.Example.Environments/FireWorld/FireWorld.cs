using System;
using System.Linq;
using System.Text;
using Briefcase.Utils;

namespace Briefcase.Example.Environments.FireWorld
{
    public class FireWorld
    {
        public const int Size = 10;

        private readonly Random random = new Random();

        private readonly Terrain[] world = new Terrain[Size];

        // Agent state
        private int firemanPosition;
        private bool firemanHasWater;

        public event EventHandler AfterAct;

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

        public FireWorldPercept Perceive()
        {
            // Left edge
            if (firemanPosition == 0)
                return new FireWorldPercept(firemanPosition, new[]
                {
                        Terrain.None,
                        world[firemanPosition],
                        world[firemanPosition + 1]
                });

            // Right edge
            else if (firemanPosition == Size - 1)
                return new FireWorldPercept(firemanPosition, new[]
                {
                    world[firemanPosition - 1],
                    world[firemanPosition],
                    Terrain.None
                });

            // Middle
            else
                return new FireWorldPercept(firemanPosition, new[]
                {
                    world[firemanPosition - 1],
                    world[firemanPosition],
                    world[firemanPosition + 1]
                });
        }

        public bool Act(FireWorldAction action)
        {
            bool result;
            switch (action)
            {
                case FireWorldAction.MoveLeft:
                    if (firemanPosition > 0)
                    {
                        firemanPosition -= 1;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                case FireWorldAction.MoveRight:
                    if (firemanPosition < Size - 1)
                    {
                        firemanPosition += 1;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                case FireWorldAction.GetWater:
                    if (world[firemanPosition] == Terrain.Water)
                    {
                        world[firemanPosition] = Terrain.GettingWater;
                        firemanHasWater = true;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                case FireWorldAction.ExtinguishFire:
                    if (world[firemanPosition] == Terrain.Fire)
                    {
                        world[firemanPosition] = Terrain.Normal;
                        firemanHasWater = false;
                        result = true; // Message: fire-out
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                default:
                    result = false;
                    break;
            }

            AfterAct?.Invoke(this, new EventArgs());

            return result;
        }

        // No lock on terrain is necessary!
        private void SetTerrain(Func<int, Terrain, Terrain> setter)
        {
            for (int i = 0; i < Size; i++)
                world[i] = setter(i, world[i]);
        }

        internal string Show()
        {
            const char horizontalBar = '─';
            const char verticalBar = '|';

            const string normal = " ";
            const string fire = "▒";
            const string water = "█";
            const string gettingWater = "▄";

            var ruler = new String(horizontalBar, 2 * Size + 1);

            var agentArray = Enumerable.Range(0, Size).Select(p => p == firemanPosition ? ShowFireman() : normal);
            var agentRow = $"|{String.Join(verticalBar.ToString(), agentArray)}|";

            var worldArray = world.Select<Terrain, string>(t => t.Switch(normal,
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

        private string ShowFireman()
        {
            const string noWater = "A";
            const string withWater = "Å";

            return firemanHasWater ? withWater : noWater;
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
