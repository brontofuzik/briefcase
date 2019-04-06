using static Briefcase.Example.Bdi.Program;
using static System.String;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Briefcase.Example.Bdi
{
    class FiremanAgent : Agent
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

        private string intention = Empty;
        private readonly List<Action> plan = new List<Action>();
        private bool needToReplan;

        public FiremanAgent(string name)
            : base(name)
        {
        }

        private FireEnvironment FireEnvironment
            => Environment as FireEnvironment;

        public override void Initialize()
        {
            beliefs[Position] = 0;
            beliefs[HaveWater] = False;
            beliefs[Fire] = UnknownPosition;
            beliefs[Water] = UnknownPosition;
        }

        public override void Act()
        {
            if (Debug) Print("Agent.Act - beginning");

            var percept = FireEnvironment.Perceive();

            ReviseBeliefs(percept);
            if (Debug) Print($"Agent.Act - {nameof(ReviseBeliefs)}");

            GenerateDesires();
            if (Debug) Print($"Agent.Act - {nameof(GenerateDesires)}");

            AdoptIntetion();
            if (Debug) Print($"Agent.Act - {nameof(AdoptIntetion)}");

            // New intention adopted?
            if (needToReplan)
            {
                MakePlan();
                if (Debug) Print($"Agent.Act - {nameof(MakePlan)}");
            }

            ExecuteAction();
            if (Debug) Print($"Agent.Act - {nameof(ExecuteAction)}");

            if (Debug) Print("Agent.Act - end");
        }

        private void ReviseBeliefs(Percept percept)
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
            // TODO Shouldn't this happen only after the intention check below?
            if (desires.Count == 0)
                desires.Add(PatrolRight);

            // Plan to extinguish fire in progress?
            if (intention == ExtinguishFire && plan.Count > 0) 
                return;

            // At right end, turn left.
            if (beliefs[Position] == FireEnvironment.Size - 1) 
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

        private void AdoptIntetion()
        {
            string newIntention = Empty;

            if (desires.Contains(ExtinguishFire))
                newIntention = ExtinguishFire;
            else if (desires.Contains(PatrolLeft))
                newIntention = PatrolLeft;
            else if (desires.Contains(PatrolRight))
                newIntention = PatrolRight;

            if (newIntention != intention)
            {
                intention = newIntention;
                needToReplan = true;
            }
        }

        private void MakePlan()
        {
            plan.Clear();
            
            switch (intention)
            {
                case PatrolRight:
                    for (int i = beliefs[Position]; i < FireEnvironment.Size; i++)
                        plan.Add(Action.MoveRight);
                    break;

                case PatrolLeft:
                    for (int i = beliefs[Position]; i >= 0; i--)
                        plan.Add(Action.MoveLeft);
                    break;

                case ExtinguishFire:
                    // Move to water (left)
                    for (int i = beliefs[Position]; i > beliefs[Water]; i--)
                        plan.Add(Action.MoveLeft);

                    // Get water
                    plan.Add(Action.GetWater);

                    // Move to fire (right)
                    for (int i = beliefs[Water]; i < beliefs[Fire]; i++)
                        plan.Add(Action.MoveRight);

                    // Extinguish fire
                    plan.Add(Action.ExtinguishFire);
                    break;

                default:
                    break;
            }

            needToReplan = false;
        }

        private void ExecuteAction()
        {
            // TODO Why?
            if (plan.Count == 0)
                intention = Empty;

            var action = plan[0];
            plan.RemoveAt(0);

            // TODO What to do with the action result?
            var actionResult = FireEnvironment.Act(action);

            // Got water successfully?
            if (action == Action.GetWater && actionResult)
                beliefs[HaveWater] = True;

            // Extinguished water successfully?
            if (action == Action.ExtinguishFire && actionResult)
                beliefs[HaveWater] = False;
        }

        // DEBUG
        private void Print(string message)
        {
            Console.WriteLine(message);

            string beliefsStr = String.Join("; ", beliefs.Select(kvp => $"({kvp.Key}, {kvp.Value})"));
            Console.WriteLine($"Beliefs: {beliefsStr}");

            string desiresStr = String.Join(", ", desires);
            Console.WriteLine($"Desires: {desiresStr}");

            Console.WriteLine($"Intention: {intention}");

            string planStr = String.Join(", ", plan);
            Console.WriteLine($"Plan: {planStr}");

            string ruler = new string('-', 100);
            Console.WriteLine(ruler);
        }
    }
}
