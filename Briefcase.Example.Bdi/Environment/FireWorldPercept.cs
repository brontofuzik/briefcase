namespace Briefcase.Example.Bdi.Environment
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
}
