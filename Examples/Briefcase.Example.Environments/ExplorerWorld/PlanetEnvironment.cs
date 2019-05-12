using Briefcase.Environments;

namespace Briefcase.Example.Environments.ExplorerWorld
{
    public class PlanetEnvironment : Environment<PlanetWorld, object, object, PlanetWorldAction, string>
    {
        public PlanetEnvironment(PlanetWorld passiveWorld)
            : base(passiveWorld)
        {
        }
    }
}
