using System;

namespace Briefcase
{
    public abstract class Environment : IEnvironment
    {
        public virtual void Initialize() {}

        // Turn-based
        public virtual void BeginTurn(int turn)
        {
        }

        // Turn-based
        public virtual void EndTurn(int turn)
        {
        }

        public virtual string Show() => String.Empty;
    }
}
