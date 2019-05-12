namespace Briefcase.Example.Environments.ExplorerWorld
{
    public class PlanetWorldAction
    {
        public T Type { get; set; }

        public object Arg;

        public enum T
        {
            Init,
            Move,
            Load,
            Carry,
            Unload
        }
    }
}
