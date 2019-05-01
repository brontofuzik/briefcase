using System;
using Briefcase.Agents;
using Prolog;

namespace Briefcase.Example.Prolog
{
    class Hunter : TurnBasedAgent
    {
        private readonly PrologEngine pl = new PrologEngine();

        public Hunter(string name)
            : base(name)
        {
        }

        // Shortcut
        private WumpusWorld WumpusEnvironment
            => Environment as WumpusWorld;

        public override void Initialize()
        {
            pl.Consult("Hunter.pl");
        }

        public override void Step()
        {
            var percept = WumpusEnvironment.Perceive();
        }
    }
}
