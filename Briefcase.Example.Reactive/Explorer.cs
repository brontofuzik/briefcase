using System;
using Briefcase.Agents;
using Briefcase.Example.Environments.ExplorerWorld;
using Briefcase.Example.Environments.FireWorld;

namespace Briefcase.Example.Reactive
{
    class Explorer : SituatedAgent<PlanetWorld, object, object, PlanetWorldAction, string>
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

        protected override PlanetWorldAction PerceiveAndAct(object percept)
        {
            if (collision)
            {
                MoveRandomly();
                collision = false;

                return new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Move,
                    Arg = position
                };
            }

            if (carrying && IsAtBase())
            {
                // If carrying samples and at the base, then unload samples.
                carrying = false;
                return new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Unload,
                    Arg = carriedResource
                };
            }

            if (carrying && !IsAtBase())
            {
                // If carrying samples and not at the base, then travel up gradient.
                MoveToBase();

                return new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Carry,
                    Arg = position
                };
            }

            if (sampleDetected != null)
            {
                // If you detect a sample, then pick sample up.
                carrying = true;
                carriedResource = sampleDetected;

                return new PlanetWorldAction
                {
                    Type = PlanetWorldAction.T.Load,
                    Arg = sampleDetected
                };
            }

            // If (true), then move randomly.
            MoveRandomly();

            return new PlanetWorldAction
            {
                Type = PlanetWorldAction.T.Move,
                Arg = position
            };
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

        protected override void HandleMessage(Message message)
        {
            // Do nothing.
        }
    }
}
