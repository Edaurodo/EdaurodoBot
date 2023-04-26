using KansasBot.rsc.utils;

namespace KansasBot.rsc.modules.allowlistmodule.utilities
{
    public sealed class AllowlistUtilities
    {
        public static string DataPath = Path.Combine(new[] { KansasPaths.DataPath, "allowlist_data"});
        public static string ConfigPath = Path.Combine(new[] { KansasPaths.ConfigPath, "allowlist_config" });
    }
}
