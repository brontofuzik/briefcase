using System;
using Briefcase.Agents;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Briefcase.Utils;
using Environment = Briefcase.Environments.Environment;

namespace Briefcase
{
    public class MultiagentSystem
    {
        protected readonly IDictionary<string, Agent> agents = new Dictionary<string, Agent>();
        protected readonly Environment environment;
        
        public MultiagentSystem(Environment environment = null)
        {
            this.environment = environment;
            environment.Mas = this;
        }

        public void AddAgent(Agent agent)
        {
            agents[agent.Id] = agent;
            agent.Mas = this;
            if (agent is SituatedAgent situated)
            {
                situated.Environment = environment;
            }
        }

        public Agent GetAgent(string id)
            => agents.GetOrDefault(id);

        // Read-only
        public IEnumerable<Agent> GetAllAgents()
            => agents.Values;

        #region Real-time

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

        #endregion // Real-time

        #region Turn-based

        public void RunTurnbased(int? maxTurns = null, TimeSpan? stepTime = null)
        {
            Initialize();
            Wait(stepTime);

            RunAgents_Turnbased(maxTurns, stepTime);
        }

        private void RunAgents_Turnbased(int? maxTurns = null, TimeSpan? stepTime = null)
        {
            int turn = 0;
            while (!maxTurns.HasValue || turn < maxTurns)
            {
                RunTurn(turn++);
                Wait(stepTime);
            }
        }

        private void RunTurn(int turn)
        {
            GetAllAgents().ForEach(a => a.Step(turn));
        }

        private void Wait(TimeSpan? stepTime = null)
        {
            if (stepTime.HasValue)
                Thread.Sleep(stepTime.Value);
            else
                Console.ReadKey();
        }

        #endregion // Turn-based

        protected void Initialize()
        {
            environment?.Initialize();
            GetAllAgents().ForEach(a => a.Initialize());
        }

        public void Send(Message message)
        {
            GetAgent(message.Receiver)?.Post(message);
        }
    }
}
