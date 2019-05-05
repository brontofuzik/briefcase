using System;
using System.Threading.Tasks;

namespace Briefcase.Environments
{
    public abstract class Environment : IEnvironment
    {
        public IMultiagentSystem Mas { get; set; }

        public virtual void Initialize()
        {
        }

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

    public abstract class Environment<TPassiveWorld, TSensor, TPercept, TAction, TResult> : Environment
        where TPassiveWorld : PassiveWorld<TSensor, TPercept, TAction, TResult>
    {
        protected readonly TPassiveWorld passiveWorld;
        protected readonly ActiveWorld<TSensor, TPercept, TAction, TResult> activeWorld;

        protected Environment(TPassiveWorld passiveWorld)
        {
            this.passiveWorld = passiveWorld;
            this.activeWorld = new ActiveWorld<TSensor, TPercept, TAction, TResult>(passiveWorld);
        }

        public override void Initialize()
        {
            passiveWorld.Initialize();
        }

        public virtual TPercept Perceive(TSensor sensor = default)
            => passiveWorld.Perceive();

        // Real-time
        public virtual Task<TPercept> PerceiveAsync(TSensor arg = default)
            => activeWorld.PerceiveAsync();

        // Turn-based
        public virtual TResult Act(TAction action)
            => passiveWorld.DoAct(action);

        // Real-time
        public virtual Task<TResult> ActAsync(TAction action)
            => activeWorld.ActAsync(action);
    }
}
