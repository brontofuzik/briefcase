using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefcase.Example.Bdi
{
    class Program
    {
        static void Main(string[] args)
        {
            var mas = new TurnBasedMas(new FireEnvironment());
            mas.AddAgent(new FiremanAgent("sam"));
            mas.Run();
        }
    }
}
