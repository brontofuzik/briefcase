using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefcase.Example.Bdi.Environment
{
    /*
    class FireWorld
    {
        public const int Size = 10;

        private readonly Random random = new Random();

        private readonly Terrain[] world = new Terrain[Size];
        private int firemanPosition;

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
    }
    */

    public enum Terrain
    {
        Normal,
        Fire,
        Water,
        GettingWater,
        None
    }
}
