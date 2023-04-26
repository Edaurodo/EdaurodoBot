namespace EdaurodoBot.rsc.utils
{
    public static class EdaurodoPaths
    {
        public static string Config { get; } = Path.Combine(new[] { Directory.GetCurrentDirectory(), "config" });
        public static string Data{ get; } = Path.Combine(new[] { Directory.GetCurrentDirectory(), "data" });
    }
}

