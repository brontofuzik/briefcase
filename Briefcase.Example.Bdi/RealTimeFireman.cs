using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Briefcase.Agents;
using Briefcase.Example.Environments.FireWorld;
using Briefcase.Utils;

namespace Briefcase.Example.Bdi
{
    class RealTimeFireman : RealTimeAgent
    {
        // Beliefs

        private readonly Dictionary<string, int> beliefs = new Dictionary<string, int>();

        private const string Position = "position";
        private const string Fire = "fire";
        private const string Water = "water";
        private const int UnknownPosition = -1;

        private const string HaveWater = "have-water";
        private const int True = 1;
        private const int False = 0;

        // Desires

        private readonly HashSet<string> desires = new HashSet<string>();

        private const string PatrolRight = "patrol-right";
        private const string PatrolLeft = "patrol-left";
        private const string ExtinguishFire = "extinguish-fire";

        // Intentions & plans

        private string intention = String.Empty;
        private readonly List<FireWorldAction> plan = new List<FireWorldAction>();

        public RealTimeFireman(string name)
            : base(name)
        {
        }

        // Shortcut
        private RealTimeFireWorld FireEnvironment
            => Environment as RealTimeFireWorld;

        public override void Initialize()
        {
            beliefs[Position] = 0;
            beliefs[HaveWater] = False;
            beliefs[Fire] = UnknownPosition;
            beliefs[Water] = UnknownPosition;
        }

        protected override async Task Step()
        {
            // Sense
            var percept = await FireEnvironment.Perceive();

            // BDI
            ReviseBeliefs(percept);
            GenerateDesires();
            AdoptIntention();

            // Act
            var action = NextAction();
            if (action.HasValue)
            {
                await ExecuteAction(action.Value);
            }
        }

        private void ReviseBeliefs(FireWorldPercept percept)
        {
            beliefs[Position] = percept.Position;

            // "see" and update beliefs
            if (intention == ExtinguishFire)
            {
                // Plan in progress
                if (plan.Any()) return;

                bool fireOut = percept.VisualField.All(t => t != Terrain.Fire);

                if (fireOut)
                    beliefs[Fire] = UnknownPosition;
            }

            // Any fire in sight?
            int? fireIndex = IndexOf(percept.VisualField, Terrain.Fire);
            if (fireIndex.HasValue)
                beliefs[Fire] = beliefs[Position] + fireIndex.Value - 1;

            // Any water in sight?
            int? waterIndex = IndexOf(percept.VisualField, Terrain.Water);
            if (waterIndex.HasValue)
                beliefs[Water] = beliefs[Position] + waterIndex.Value - 1;
        }

        private static int? IndexOf(Terrain[] visualField, Terrain terrain)
            => visualField.IndexOf(t => t == terrain);

        private void GenerateDesires()
        {
            // Plan to extinguish fire in progress?
            if (intention == ExtinguishFire && plan.Count > 0)
                return;

            // Where there is nothing else to do, patrol right.
            if (desires.Count == 0)
                desires.Add(PatrolRight);

            // At right end, turn left.
            if (beliefs[Position] == FireWorld.Size - 1)
            {
                desires.Remove(PatrolRight);
                desires.Add(PatrolLeft);
            }
            // At left end, turn right.
            else if (beliefs[Position] == 0)
            {
                desires.Remove(PatrolLeft);
                desires.Add(PatrolRight);
            }

            // A fire has been discovered!
            if (beliefs[Fire] != UnknownPosition)
            {
                desires.Add(ExtinguishFire);
            }
            // A fire has been extinguished.
            else
            {
                desires.Remove(ExtinguishFire);
            }
        }

        private void AdoptIntention()
        {
            string newIntention = String.Empty;

            if (desires.Contains(ExtinguishFire))
                newIntention = ExtinguishFire;
            else if (desires.Contains(PatrolLeft))
                newIntention = PatrolLeft;
            else if (desires.Contains(PatrolRight))
                newIntention = PatrolRight;

            if (newIntention != intention)
            {
                intention = newIntention;
                MakePlan();
            }
        }

        private void MakePlan()
        {
            plan.Clear();

            switch (intention)
            {
                case PatrolRight:
                    for (int i = beliefs[Position]; i < FireWorld.Size; i++)
                        plan.Add(FireWorldAction.MoveRight);
                    break;

                case PatrolLeft:
                    for (int i = beliefs[Position]; i >= 0; i--)
                        plan.Add(FireWorldAction.MoveLeft);
                    break;

                case ExtinguishFire:
                    // Move to water (left)
                    for (int i = beliefs[Position]; i > beliefs[Water]; i--)
                        plan.Add(FireWorldAction.MoveLeft);

                    // Get water
                    plan.Add(FireWorldAction.GetWater);

                    // Move to fire (right)
                    for (int i = beliefs[Water]; i < beliefs[Fire]; i++)
                        plan.Add(FireWorldAction.MoveRight);

                    // Extinguish fire
                    plan.Add(FireWorldAction.ExtinguishFire);
                    break;
            }
        }

        private FireWorldAction? NextAction()
        {
            if (!plan.Any())
                return null;

            var action = plan[0];
            plan.RemoveAt(0);

            if (!plan.Any())
                intention = String.Empty;

            return action;
        }

        private async Task ExecuteAction(FireWorldAction action)
        {
            var actionResult = await FireEnvironment.Act(action);

            // Got water successfully?
            if (action == FireWorldAction.GetWater && actionResult)
                beliefs[HaveWater] = True;

            // Extinguished water successfully?
            if (action == FireWorldAction.ExtinguishFire && actionResult)
            {
                beliefs[Fire] = UnknownPosition;
                beliefs[HaveWater] = False;
            }
        }
    }
}
