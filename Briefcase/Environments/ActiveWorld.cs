using System.Threading.Tasks;
using Briefcase.Utils;

namespace Briefcase.Environments
{
    public class ActiveWorld<TSensor, TPercept, TAction, TResult> : ActiveObject<PassiveWorld<TSensor, TPercept, TAction, TResult>>
    {
        public ActiveWorld(PassiveWorld<TSensor, TPercept, TAction, TResult> passive)
            : base(passive)
        {
        }

        public Task<TPercept> PerceiveAsync()
            => CallFunction2(() => Passive.Perceive());

        public Task<TResult> ActAsync(TAction action)
            => CallFunction2(() => Passive.Act(action));
    }
}
