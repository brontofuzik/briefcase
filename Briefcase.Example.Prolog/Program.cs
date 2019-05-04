using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            var world = new WumpusWorld();
            var env = new WumpusEnvironment(world);
            var mas = new TurnBasedMas(env);
            mas.AddAgent(new Hunter("hunter"));
            mas.Run();
        }
    }
}
