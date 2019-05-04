namespace Briefcase.Example.Environments.FireWorld
{
    public struct FireWorldPercept
    {
        public FireWorldPercept(int position, Terrain[] visualField)
        {
            Position = position;
            VisualField = visualField;
        }

        public int Position { get; internal set; }

        public Terrain[] VisualField { get; internal set; }
    }

    public enum FireWorldAction
    {
        MoveLeft,
        MoveRight,
        GetWater,
        ExtinguishFire
    }
}
