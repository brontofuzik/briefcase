using System;
using System.Threading.Tasks;

namespace Briefcase.Environments
{
    public class RuntimeEnvironment
    {
        protected readonly ActiveEnvironment activeWorld;

        public RuntimeEnvironment(Environment env)
        {
            activeWorld = new ActiveEnvironment(env);
        }

        //public override void Initialize()
        //{
        //    passiveWorld.Initialize();
        //}

        public virtual Task<object> PerceiveAsync(string agentId, object sensor = default)
            => activeWorld.PerceiveAsync(agentId, sensor);

        public virtual Task<object> ActAsync(string agentId, object action)
            => activeWorld.ActAsync(agentId, action);

        public Task<object> PerceiveAndAct(string agentId, Func<object, object> actOnPercept)
            => activeWorld.PerceiveAndAct(agentId, actOnPercept);

        // Real-time & turn-based
        public virtual void BeforeAct()
        {
        }

        // Real-time & turn-based
        public virtual void AfterAct()
        {
        }

        // Turn-based only
        public virtual void BeginTurn(int turn)
        {
        }

        // Turn-based only
        public virtual void EndTurn(int turn)
        {
        }

        public virtual string Show() => String.Empty;
    }

    // TODO Merge into RuntimeEnvironment.
    public class ActiveEnvironment : ActiveObject<Environment>
    {
        public ActiveEnvironment(Environment passive)
            : base(passive)
        {
        }

        public Task<object> PerceiveAsync(string agentId, object sensor = null)
            => CallFunction2(() => Passive.Perceive(agentId, sensor));

        public Task<object> ActAsync(string agentId, object action)
            => CallFunction2(() => Passive.Act(agentId, action));

        // Atomic
        public Task<object> PerceiveAndAct(string agentId, Func<object, object> actOnPercept)
            => CallFunction2(() => Passive.Act(agentId, actOnPercept(Passive.Perceive(agentId))));
    }
}
