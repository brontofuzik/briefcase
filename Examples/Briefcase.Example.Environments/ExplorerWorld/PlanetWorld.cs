using System;
using System.Collections.Generic;
using Environment = Briefcase.Environments.Environment;

namespace Briefcase.Example.Environments.ExplorerWorld
{
    public class PlanetWorld : Environment
    {
        private const int Size = 11;
        private const int Resources = 10;

        private static readonly Random rand = new Random();

        // Key: agent id
        internal Dictionary<string, (int, int)> ExplorerPositions { get; set; }
            = new Dictionary<string, (int, int)>();

        // Key: resource name
        internal Dictionary<string, (int, int)> ResourcePositions { get; set; }
            = new Dictionary<string, (int, int)>();

        private Dictionary<string, string> Loads { get; }
            = new Dictionary<string, string>();

        public override void Initialize()
        {
            var usedPositions = new HashSet<(int, int)>();

            // Do not use the base position.
            usedPositions.Add((Size / 2, Size / 2));

            for (int i = 0; i < Resources; i++)
            {
                (int, int) newPosition;
                do
                {
                    newPosition = (rand.Next(Size), rand.Next(Size));
                }
                while (usedPositions.Contains(newPosition));

                ResourcePositions.Add($"resource{i}", newPosition);
                usedPositions.Add(newPosition);
            }
        }

        public override object Perceive(string agentId, object sensor = default)
        {
            throw new NotImplementedException();
        }

        public override object Act(string agentId, object action)
        {
            var a = (PlanetWorldAction)action;
            switch (a.Type)
            {
                case PlanetWorldAction.T.Init:
                    return Act_Init(agentId, ((int, int))a.Arg);

                case PlanetWorldAction.T.Move:
                    return Act_Move(agentId, ((int, int))a.Arg);

                case PlanetWorldAction.T.Load:
                    return Act_Load(agentId, (string)a.Arg);

                case PlanetWorldAction.T.Carry:
                    return Act_Carry(agentId, ((int, int))a.Arg);

                case PlanetWorldAction.T.Unload:
                    return Act_Unload(agentId, (string)a.Arg);
            }

            return null;
        }

        private string Act_Init(string agentId, (int, int) position)
        {
            ExplorerPositions.Add(agentId, position);
            return null;
        }

        private string Act_Move(string agentId, (int, int) position)
        {
            ExplorerPositions[agentId] = position;

            // Block
            foreach (string explorer in ExplorerPositions.Keys)
            {
                if (explorer == agentId)
                    continue;

                if (ExplorerPositions[explorer] == position)
                    return "Block!";
            }

            // Sample
            foreach (string sample in ResourcePositions.Keys)
            {
                if (ResourcePositions[sample] == position)
                    return sample;
            }

            return null;
        }

        private string Act_Load(string agentId, string sampleId)
        {
            Loads[agentId] = sampleId; 
            return null;
        }

        private string Act_Carry(string agentId, (int, int) position)
        {
            ExplorerPositions[agentId] = position;
            string sampleId = Loads[agentId];
            ResourcePositions[sampleId] = position;

            return null;
        }

        private string Act_Unload(string agentId, string sampleId)
        {
            Loads.Remove(agentId);
            return null;
        }
    }
}
