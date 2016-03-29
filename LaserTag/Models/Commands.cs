using System;

namespace LaserTag.Models
{
    [Flags]
    public enum Commands
    {
        RdsPower = 1,
        Reload = 2,
        Respawn = 4
    }

    public static class CommandsExtensions
    {
        public static bool HasFlag(this Commands commands, Commands flag)
        {
            return commands != 0 && (commands & flag) == flag;
        }
    }
}
