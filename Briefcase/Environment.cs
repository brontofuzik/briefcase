using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefcase
{
    public abstract class Environment : IEnvironment
    {
        public virtual void Initialize() {}
    }
}
