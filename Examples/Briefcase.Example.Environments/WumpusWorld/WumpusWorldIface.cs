using System;

namespace Briefcase.Example.Environments.WumpusWorld
{
    [Flags]
    public enum WumpusPercept
    {
        Breeze,
        Stench,
        Glitter
    }

    public enum WumpusAction
    {
        MoveForward,
        TurnLeft,
        TurnRight,
        Shoot,
        Grab
    }

    public enum ActionResult
    {
        // Generic success
        Success,

        // Shoot success
        Scream,

        // Generic fail
        Fail,

        // Walk fail
        Bump
    }
}