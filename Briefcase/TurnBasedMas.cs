﻿using System;
using System.Collections.Generic;

namespace Briefcase
{
    public class TurnBasedMas
    {
        private readonly IEnvironment environment;
        private readonly IDictionary<string, IAgent> agents = new Dictionary<string, IAgent>();

        private readonly int? maxTurns;
        private int turn;

        public TurnBasedMas(IEnvironment environment = null,
            int? maxTurns = null)
        {
            this.environment = environment;
            this.maxTurns = maxTurns;
        }

        public void AddAgent(IAgent agent)
        {
            agents[agent.Id] = agent;
        }

        public void Run()
        {
            Initialize();

            while (!maxTurns.HasValue || turn < maxTurns)
            {
                RunTurn(turn);
                turn++;
            }
        }

        private void Initialize()
        {
            environment.Initialize();

            foreach (var agent in agents.Values)
                agent.Initialize();
        }

        private void RunTurn(int turn)
        {
            environment.BeginTurn(turn);

            foreach (var agent in agents.Values)
                agent.Act(environment);

            environment.EndTurn(turn);
        }
    }
}
