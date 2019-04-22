using System;

namespace Briefcase.Environments
{
    public abstract class Environment : IEnvironment
    {
        public IMultiagentSystem Mas { get; set; }

        public abstract void Initialize();

        public virtual string Show() => String.Empty;
    }
}
