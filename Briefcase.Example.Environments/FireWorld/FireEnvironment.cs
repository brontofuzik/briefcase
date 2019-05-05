using Briefcase.Environments;

namespace Briefcase.Example.Environments.FireWorld
{
    public class FireEnvironment : Environment<FireWorld, object, FireWorldPercept, FireWorldAction, bool>
    {
        public FireEnvironment(FireWorld passiveWorld)
            : base(passiveWorld)
        {
        }

        public override string Show()
            => passiveWorld.Show();

        //private void Debug(string message, [CallerMemberName] string method = null)
        //{
        //    if (debug)
        //    {
        //        lock (Console.Out)
        //        {
        //            Console.WriteLine($"{nameof(FireEnvironment)}.{method}:");
        //            Console.WriteLine(passive.Print());
        //        }
        //    }
        //}
    }
}