﻿using Briefcase.Agents;
using Briefcase.Environments;
using System.Collections.Generic;

namespace Briefcase
{
    public abstract class MultiagentSystem : IMultiagentSystem
    {
        protected readonly IEnvironment environment;
        protected readonly IDictionary<string, IAgent> agents = new Dictionary<string, IAgent>();

        protected MultiagentSystem(IEnvironment environment = null)
        {
            this.environment = environment;
            environment.Mas = this;
        }

        protected void Initialize()
        {
            environment?.Initialize();

            foreach (var agent in GetAllAgents())
                agent.Initialize();
        }

        public IAgent GetAgent(string id)
        {
            agents.TryGetValue(id, out IAgent agent);
            return agent;
        }

        public IEnumerable<IAgent> GetAllAgents() => agents.Values;

        public void AddAgent(IAgent agent)
        {
            agents[agent.Id] = agent;
            agent.Environment = environment;
        }
    }
}
