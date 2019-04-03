using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefcase
{
    public interface IAgent
    {
        string Id { get; }

        void Initialize();
    }
}
