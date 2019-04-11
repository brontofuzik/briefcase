namespace Briefcase.Environments
{
    public interface IEnvironment
    {
        IMultiagentSystem Mas { get; set; }

        void Initialize();


        string Show();
    }
}
