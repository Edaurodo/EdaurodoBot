using DSharpPlus;
using KansasBot.rsc.utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.config
{
    public sealed class AllowListConfigLoader
    {
        private string ConfigPath = Path.Combine(new[] { KansasPaths.ConfigPath, "config_allowlist.json" });
        private DiscordClient Client;

        public AllowListConfigLoader(DiscordClient client)
        {
            this.Client = client;
        }
        public async Task<AllowListConfig> LoadConfigAsync()
        {
            FileInfo file = new FileInfo(ConfigPath);
            if (file == null || !file.Exists)
            {
                string js = JsonConvert.SerializeObject(new AllowListConfig(), Formatting.Indented);
                using (FileStream fs = file.Create())
                using (StreamWriter sw = new StreamWriter(fs, KansasUtilities.UTF8))
                {
                    await sw.WriteLineAsync(js);
                    await sw.FlushAsync();
                }
                //throw new ArgumentException("O Arquivo de configuração é inválido ou inexistente!", nameof(file));
            }

            string json = "{}";
            using (FileStream fs = file.OpenRead())
            using (StreamReader sr = new StreamReader(fs, KansasUtilities.UTF8))
                json = await sr.ReadToEndAsync();

            AllowListConfig config = JsonConvert.DeserializeObject<AllowListConfig>(json);
            return config;
        }
        public Task<bool> ValidateConfig(AllowListConfig? config)
        {
            if (config == null)
            {
                Client.Logger.LogWarning(new EventId(702, "AllowlistConfig"), $"Arquivo de configuração é nulo, verifique os arquivos de configuração em:\n {ConfigPath}");
                return Task.FromResult(false);
            }

            if (config.Channels == null || config.Roles == null)
            {
                Client.Logger.LogWarning(new EventId(703, "AllowlistConfig"), $"ChannelConfig ou RolesConfig é nulo, verifique os arquivos de configuração em:\n {ConfigPath}");
                return Task.FromResult(false);
            }

            if (config.Channels.CategoryChannelId == null || config.Channels.MainChannelId == null || config.Channels.ApprovedChannelId == null || config.Channels.ReprovedChannelId == null)
            {
                Client.Logger.LogWarning(new EventId(704, "AllowlistConfig"), $"Algum argumento em ChannelConfig é nulo, verifique os arquivos de configuração em:\n {ConfigPath}");
                return Task.FromResult(false);
            }

            if (config.Roles.ReaderRoleId == null || config.Roles.ApprovedRoleId == null || config.Roles.ReprovedRoleId == null)
            {
                Client.Logger.LogWarning(new EventId(705, "AllowlistConfig"), $"Algum argumento em RolesConfig é nulo, verifique os arquivos de configuração em:\n {ConfigPath}");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}