namespace KansasBot.rsc.utils
{
    public static class KansasPaths
    {
        public static string ConfigPath { get; } = Path.Combine(new[] { Directory.GetCurrentDirectory(), "config" });
        public static string DataPath { get; } = Path.Combine(new[] { Directory.GetCurrentDirectory(), "data" });
    }
}

