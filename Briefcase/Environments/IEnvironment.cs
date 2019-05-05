namespace Briefcase.Environments
{
    // Do we need this?
    public interface IEnvironment
    {
        IMultiagentSystem Mas { get; set; }

        void Initialize();

        string Show();
    }
}
