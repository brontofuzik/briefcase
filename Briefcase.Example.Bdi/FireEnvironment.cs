using static Briefcase.Example.Bdi.Program;
using System;
using System.Text;
using System.Linq;

namespace Briefcase.Example.Bdi
{
    class FireEnvironment : Environment
    {
        public const int Size = 10;

        private readonly Random random = new Random();

        private readonly Terrain[] world = new Terrain[Size];
        private int firemanPosition;

        private int? FireLocation => world.IndexOf(t => t == Terrain.Fire);

        public override void Initialize()
        {
            ModifyTerrain((i, _) => i == 0 ? Terrain.Water : Terrain.Normal);
            firemanPosition = 0;
        }

        public override void BeginTurn(int turn)
        {
            if (Debug) Print("Environment.BeginTurn - beginning");

            // Reset water
            ModifyTerrain((_, t) => t == Terrain.GettingWater ? Terrain.Water : t);

            // Start a fire!
            if (!FireLocation.HasValue)
                world[random.Next(5, Size)] = Terrain.Fire;

            if (Debug) Print("Environment.BeginTurn - end");
        }

        public override void EndTurn(int turn)
        {
            if (Debug) Print("EndTurn");
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

        public override string Show()
        {
            const char horizontalBar = '─';
            const char verticalBar = '|';

            const string normal = " ";
            const string fire = "▒";
            const string water = "█";
            const string gettingWater = "▄";

            var ruler = new String(horizontalBar, 2 * Size + 1);

            var agentArray = Enumerable.Range(0, Size).Select(p => p == firemanPosition ? FiremanAgent.Show() : normal);
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

        // Shortcut
        private FiremanAgent FiremanAgent => Mas.GetAllAgents().Single() as FiremanAgent;

        private void ModifyTerrain(Func<int, Terrain, Terrain> setter)
        {
            for (int i = 0; i < Size; i++)
                world[i] = setter(i, world[i]);
        }

        // DEBUG
        private void Print(string message)
        {
            Console.WriteLine(message);

            Console.WriteLine($"Fireman: {firemanPosition}");

            string fireLocation = FireLocation.HasValue ? FireLocation.Value.ToString() : "None";
            Console.WriteLine($"Fire: {fireLocation}");

            string ruler = new string('-', 100);
            Console.WriteLine(ruler);
        }
    }

    public struct Percept
    {
        public Percept(int position, Terrain[] visualField)
        {
            Position = position;
            VisualField = visualField;
        }

        public int Position { get; internal set; }

        public Terrain[] VisualField { get; internal set; }
    }

    public enum Action
    {
        MoveLeft,
        MoveRight,
        GetWater,
        ExtinguishFire
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
