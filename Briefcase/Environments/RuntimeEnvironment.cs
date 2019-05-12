using System;
using System.Threading.Tasks;

namespace Briefcase.Environments
{
    public class RuntimeEnvironment
    {
        private readonly Environment env;

        public RuntimeEnvironment(Environment env)
        {
            this.env = env;
        }

        //public virtual void Initialize()
        //{
        //}

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

    public class RuntimeEnvironment<E, S, P, A, R> : RuntimeEnvironment
        where E : Environment<S, P, A, R>
    {
        protected readonly ActiveEnvironment<S, P, A, R> activeWorld;

        protected RuntimeEnvironment(E passiveWorld)
            : base(passiveWorld)
        {
            activeWorld = new ActiveEnvironment<S, P, A, R>(passiveWorld);
        }

        //public override void Initialize()
        //{
        //    passiveWorld.Initialize();
        //}

        public virtual Task<P> PerceiveAsync(string agentId, S sensor = default)
            => activeWorld.PerceiveAsync(agentId, sensor);

        public virtual Task<R> ActAsync(string agentId, A action)
            => activeWorld.ActAsync(agentId, action);

        public Task<R> PerceiveAndAct(string agentId, Func<P, A> actOnPercept)
            => activeWorld.PerceiveAndAct(agentId, actOnPercept);
    }

    // TODO Merge into RuntimeEnvironment.
    public class ActiveEnvironment<S, P, A, R> : ActiveObject<Environment<S, P, A, R>>
    {
        public ActiveEnvironment(Environment<S, P, A, R> passive)
            : base(passive)
        {
        }

        public Task<P> PerceiveAsync(string agentId, S sensor = default)
            => CallFunction2(() => Passive.Perceive(agentId, sensor));

        public Task<R> ActAsync(string agentId, A action)
            => CallFunction2(() => Passive.Act(agentId, action));

        // Atomic
        public Task<R> PerceiveAndAct(string agentId, Func<P, A> actOnPercept)
            => CallFunction2(() => Passive.Act(agentId, actOnPercept(Passive.Perceive(agentId))));
    }
}
