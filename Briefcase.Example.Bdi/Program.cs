namespace Briefcase.Example.Bdi
{
    class Program
    {
        public static bool Debug = false;

        static void Main(string[] args)
        {
            var mas = new TurnBasedMas(new FireEnvironment());
            mas.AddAgent(new FiremanAgent("sam"));
            mas.Run();
        }
    }
}
