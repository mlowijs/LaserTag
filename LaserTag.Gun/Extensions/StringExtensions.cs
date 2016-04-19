namespace LaserTag.Gun.Extensions
{
    public static class StringExtensions
    {
        public static string PadLeft(this string s, int totalWidth, char paddingChar)
        {
            if (s.Length >= totalWidth)
                return s;

            int toPad = totalWidth - s.Length;

            for (int i = 0; i < toPad; i++)
                s = paddingChar.ToString() + s;

            return s;
        }
    }
}
