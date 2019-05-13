using System;
using System.Threading.Tasks;

namespace Briefcase.Environments
{
    public class RuntimeEnvironment : IRuntimeEnvironment
    {
        private readonly Environment passive;
        protected readonly ActiveEnvironment active;

        public RuntimeEnvironment(Environment env)
        {
            passive = env;
            active = new ActiveEnvironment(env);
        }

        public void Initialize()
        {
            passive.Initialize();
        }

        public virtual Task<object> PerceiveAsync(string agentId, object sensor = default)
            => active.PerceiveAsync(agentId, sensor);

        public virtual Task<object> ActAsync(string agentId, object action)
            => active.ActAsync(agentId, action);

        public Task<object> PerceiveAndAct(string agentId, Func<object, object> actOnPercept)
            => active.PerceiveAndAct(agentId, actOnPercept);

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

    public interface IRuntimeEnvironment
    {
        void Initialize();
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
