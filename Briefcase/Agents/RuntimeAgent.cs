﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Briefcase.Environments;
using Environment = Briefcase.Environments.Environment;

namespace Briefcase.Agents
{
    internal abstract class RuntimeAgent : IRuntimeAgent
    {
        protected readonly Agent agent;

        private readonly Thread actionThread;
        protected TimeSpan? stepTime;

        protected RuntimeAgent(Agent agent)
        {
            this.agent = agent;

            actionThread = new Thread(Act)
            {
                Name = $"{agent.Id}_actionThread"
            };
        }

        public string Id => agent.Id;

        protected abstract void Act();

        public virtual void Run(TimeSpan? stepTime)
        {
            this.stepTime = stepTime;
            actionThread.Start();
        }

        public virtual void Kill()
        {
            actionThread.Abort();
        }

        public abstract void Post(Message message);
    }

    internal abstract class RuntimeAgent<TEnv> : RuntimeAgent
        where TEnv : Environment
    {      
        private readonly RuntimeEnvironment<TEnv> environment;

        protected RuntimeAgent(Agent agent, RuntimeEnvironment<TEnv> environment)
            : base(agent)
        {
            this.environment = environment;
        }

        protected override async void Act()
        {
            while (true)
            {
                //agent.Step();
                await environment.PerceiveAndAct(Id, ((SituatedAgent<TEnv>)agent).PerceiveAndAct);

                if (stepTime.HasValue)
                    await Task.Delay(stepTime.Value);
            }
        }
    }

    interface IRuntimeAgent
    {
        string Id { get; }

        void Run(TimeSpan? stepTime);

        void Post(Message message);
    }
}
