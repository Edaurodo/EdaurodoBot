using KansasBot.rsc.utils;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.config
{
    public sealed class AllowListConfigLoader
    {
        private string ConfigPath = Path.Combine(new[] { KansasPaths.ConfigPath, "config_allowlist.cfg" });

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
                throw new ArgumentException("O Arquivo de configuração é inválido ou inexistente!", nameof(file));
            }

            string json = "{}";
            using (FileStream fs = file.OpenRead())
            using (StreamReader sr = new StreamReader(fs, KansasUtilities.UTF8))
                json = await sr.ReadToEndAsync();

            AllowListConfig config = JsonConvert.DeserializeObject<AllowListConfig>(json);
            if(!await ValidateConfig(config)) { throw new Exception("Não foi possível iniciar a AllowListModule, verifique os arquivos de configuração"); }
            return config;
        }
        public Task<bool> ValidateConfig(AllowListConfig config)
        {
            if (config.ChannelConfig == null || config.RolesConfig == null)
            {
                return Task.FromResult(false);
            }
            if (config.ChannelConfig.CategoryChannelId == null || config.ChannelConfig.MainChannelId == null || config.ChannelConfig.AprovedChannelId == null || config.ChannelConfig.ReprovedChannelId == null)
            {
                return Task.FromResult(false);
            }
            if (config.RolesConfig.ReaderRoleId == null || config.RolesConfig.AprovedRoleId == null || config.RolesConfig.ReprovedRoleId == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}