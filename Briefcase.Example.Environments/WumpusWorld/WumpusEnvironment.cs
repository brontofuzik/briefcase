using Briefcase.Environments;

namespace Briefcase.Example.Environments.WumpusWorld
{
    public class WumpusEnvironment : Environment<WumpusWorld, object, WumpusPercept, WumpusAction, ActionResult>
    {
        public WumpusEnvironment(WumpusWorld passiveWorld)
            : base(passiveWorld)
        {
        }
    }
}
