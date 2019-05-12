using System;
using Briefcase.Agents;
using Briefcase.Example.Environments.ExplorerWorld;
using Briefcase.Example.Environments.FireWorld;

namespace Briefcase.Example.Reactive
{
    class Explorer : SituatedAgent<PlanetEnvironment>
    {
        private static readonly Random rand = new Random();

        private (int x, int y) position;

        private bool collision;
        private string sampleDetected;

        private bool carrying;
        private string carriedResource;

        public Explorer(string id)
            : base(id)
        {
        }

        public override void Initialize()
        {     
            carrying = false;

            do
            {
                position = (rand.Next(FireWorld.Size), rand.Next(FireWorld.Size));
            }
            while (IsAtBase());

            Environment.Act(Id, new PlanetWorldAction
            {
                Type = PlanetWorldAction.T.Init,
                Arg = position
            });
        }

        private bool IsAtBase()
            => position.x == FireWorld.Size / 2
            && position.y == FireWorld.Size / 2;

        public override void Step(int turn = 0)
        {
            if (collision)
            {
                MoveRandomly();
                collision = false;

                var result = Environment.Act(Id, new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Move,
                    Arg = position
                });

                // TODO Handle result.
                if (result != null)
                {
                    sampleDetected = result;
                }
            }
            else if (carrying && IsAtBase())
            {
                // If carrying samples and at the base, then unload samples.
                carrying = false;
                Environment.Act(Id, new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Unload,
                    Arg = carriedResource
                });
            }
            else if (carrying && !IsAtBase())
            {
                // If carrying samples and not at the base, then travel up gradient.
                MoveToBase();
                Environment.Act(Id, new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Carry,
                    Arg = position
                });
            }
            else if (sampleDetected != null)
            {
                // If you detect a sample, then pick sample up.
                carrying = true;
                carriedResource = sampleDetected;
                Environment.Act(Id, new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Load,
                    Arg = sampleDetected
                });
            }
            else
            {
                // If (true), then move randomly.
                MoveRandomly();

                var result = Environment.Act(Id, new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Move,
                    Arg = position
                });

                // TODO Handle result.
                if (result != null)
                {
                    sampleDetected = result;
                }
            }
        }

        protected override void HandleMessage(Message message)
        {
            // Do nothing.
        }

        private void MoveRandomly()
        {
            switch (rand.Next(4))
            {
                case 0: if (position.x > 0) position.x--; break; // West
                case 1: if (position.x < FireWorld.Size - 1) position.x++; break; // East
                case 2: if (position.y > 0) position.y--; break; // South
                case 3: if (position.y < FireWorld.Size - 1) position.y++; break; // North
            }
        }

        private void MoveToBase()
        {
            int dx = FireWorld.Size / 2 - position.x;
            int dy = FireWorld.Size / 2 - position.y;

            if (Math.Abs(dx) > Math.Abs(dy))
                position.x += Math.Sign(dx);
            else
                position.y += Math.Sign(dy);
        }
    }
}
