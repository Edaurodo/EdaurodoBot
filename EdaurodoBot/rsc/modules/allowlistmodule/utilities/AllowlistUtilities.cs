using EdaurodoBot.rsc.utils;

namespace EdaurodoBot.rsc.modules.allowlistmodule.utilities
{
    public sealed class AllowlistUtilities
    {
        public static string ConfigPath = Path.Combine(new[] { EdaurodoPaths.Config, "allowlist_config" });
        public static string DataPath = Path.Combine(new[] { EdaurodoPaths.Data, "allowlist_data" });
    }
}
