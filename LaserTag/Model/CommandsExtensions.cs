using LaserTag.Model;

namespace LaserTag.Gun.Model
{
    public static class CommandsExtensions
    {
        public static bool HasFlag(this Commands commands, Commands flag) => commands != 0 && (commands & flag) == flag;
    }
}