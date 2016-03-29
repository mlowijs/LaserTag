using System;

namespace LaserTag.Model
{
    [Flags]
    public enum Commands
    {
        RdsPower = 1,
        Reload = 2,
        Respawn = 4
    }
}
