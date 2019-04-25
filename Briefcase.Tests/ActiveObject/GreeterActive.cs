using System.Threading;
using System.Threading.Tasks;

namespace Briefcase.ActiveObject.Test.Simple
{
    class GreeterActive : ActiveObject<Greeter>
    {
        public GreeterActive(Greeter greeter)
            : base(greeter)
        {
        }

        public Task<string> HelloAsync1(string name) 
            => CallFunction1<string>(() => Passive.Hello(name));

        public Task<string> HelloAsync2(string name)
            => CallFunction2(() => Passive.Hello(name));

        public Task IncAsync()
            => CallAction(() => Passive.Inc());
    }

    class Greeter // Passive
    {
        private readonly int delayMillis;

        public Greeter(int delayMillis)
        {
            this.delayMillis = delayMillis;
        }

        public int Counter { get; set; }

        public string Hello(string name)
        {
            Thread.Sleep(delayMillis);
            return $"Hello, {name}!";
        }

        public void Inc()
        {
            Thread.Sleep(delayMillis);
            Counter++;
        }
    }
}
