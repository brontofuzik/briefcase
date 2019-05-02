using Briefcase.Example.Environments.WumpusWorld;

namespace Briefcase.Example.Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            var mas = new TurnBasedMas(new WumpusWorld());
            mas.AddAgent(new Hunter("hunter"));
            mas.Run();
        }
    }
}
