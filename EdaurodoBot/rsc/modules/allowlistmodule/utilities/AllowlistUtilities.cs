using EdaurodoBot.rsc.utils;

namespace EdaurodoBot.rsc.modules.allowlistmodule.utilities
{
    public sealed class AllowlistUtilities
    {
        public static string PathConfig = Path.Combine(new[] { EdaurodoPaths.Config, "allowlist_config" });
        public static string PathData = Path.Combine(new[] { EdaurodoPaths.Data, "allowlist_data" });
    }
}
