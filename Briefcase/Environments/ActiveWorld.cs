using System;
using System.Threading.Tasks;

namespace Briefcase.Environments
{
    public class ActiveWorld<TSensor, TPercept, TAction, TResult> : ActiveObject<PassiveWorld<TSensor, TPercept, TAction, TResult>>
    {
        public ActiveWorld(PassiveWorld<TSensor, TPercept, TAction, TResult> passive)
            : base(passive)
        {
        }

        public Task<TPercept> PerceiveAsync(string agentId, TSensor sensor = default)
            => CallFunction2(() => Passive.Perceive(agentId, sensor));

        public Task<TResult> ActAsync(string agentId, TAction action)
            => CallFunction2(() => Passive.Act(agentId, action));

        public Task<TResult> PerceiveAndAct(string agentId, Func<TPercept, TAction> actOnPercept)
            => CallFunction2(() => Passive.Act(agentId, actOnPercept(Passive.Perceive(agentId))));
    }
}
