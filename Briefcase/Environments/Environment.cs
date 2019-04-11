using System;

namespace Briefcase.Environments
{
    public abstract class Environment : IEnvironment
    {
        public IMultiagentSystem Mas { get; set; }

        public virtual void Initialize() {}

        public virtual string Show() => String.Empty;
    }
}
