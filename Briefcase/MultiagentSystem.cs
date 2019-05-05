using System;
using Briefcase.Agents;
using Briefcase.Environments;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Briefcase.Utils;

namespace Briefcase
{
    public class MultiagentSystem : IMultiagentSystem
    {
        protected readonly IDictionary<string, IAgent> agents = new Dictionary<string, IAgent>();
        protected readonly IEnvironment environment;
        
        public MultiagentSystem(IEnvironment environment = null)
        {
            this.environment = environment;
            environment.Mas = this;
        }

        public void AddAgent(IAgent agent)
        {
            agents[agent.Id] = agent;
            if (agent is SituatedAgent situated)
            {
                situated.Environment = environment;
            }
        }

        public IAgent GetAgent(string id)
            => agents.GetOrDefault(id);

        // Read-only
        public IEnumerable<IAgent> GetAllAgents()
            => agents.Values;

        public void RunRealtime(TimeSpan? stepTime = null)
        {
            Initialize();
            RunAgents_Realtime(stepTime);    
        }

        private void RunAgents_Realtime(TimeSpan? stepTime = null)
        {
            var realTimeAgents = GetAllAgents()
                .Cast<Agent>() // TODO Remove IAgent?
                .Select(a => new RealTimeAgent(a));

            realTimeAgents.ForEach(a => a.Run(stepTime));
        }

        public void RunTurnbased(int? maxTurns = null, TimeSpan? stepTime = null)
        {
            Initialize();

            ShowEnvironment();
            Wait(stepTime);

            RunAgents_Turnbased(maxTurns, stepTime);
        }

        private void RunAgents_Turnbased(int? maxTurns = null, TimeSpan? stepTime = null)
        {
            int turn = 0;
            while (!maxTurns.HasValue || turn < maxTurns)
            {
                RunTurn(turn++);

                ShowEnvironment();
                Wait(stepTime);
            }
        }

        private void RunTurn(int turn)
        {
            GetAllAgents().ForEach(a => a.Step(turn));
        }

        // TODO Remove.
        private void ShowEnvironment()
        {
            Console.Clear();
            Console.WriteLine(environment.Show());
        }

        private void Wait(TimeSpan? stepTime = null)
        {
            if (stepTime.HasValue)
                Thread.Sleep(stepTime.Value);
            else
                Console.ReadKey();
        }

        protected void Initialize()
        {
            environment?.Initialize();
            GetAllAgents().ForEach(a => a.Initialize());
        }
    }
}
