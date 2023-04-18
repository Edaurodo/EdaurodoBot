using KansasBot.rsc.utils;
using Newtonsoft.Json;
namespace KansasBot.rsc.core.data
{
    internal class KansasConfigLoader
    {
        private string ConfigPath = Path.Combine(new[] { KansasPaths.ConfigPath, "config_kansas.cfg" });

        public async Task<KansasConfig> LoadConfig()
        {
            FileInfo file = new FileInfo(ConfigPath);
            string json = "{}";
            if (!file.Exists || file == null) { CreateConfigFile(); }

            using (var sr = new StreamReader(file.OpenRead(), KansasUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
                sr.Dispose();
            }
            KansasConfig config = JsonConvert.DeserializeObject<KansasConfig>(json);
            ValidateConfig(config);
            return config;
        }
        private void ValidateConfig(KansasConfig config)
        {
            if (config.Discord == null)
            {
                throw new Exception($"Falha ao verificar o arquivo de configuração, verifique o arquivo em {ConfigPath}");
            }
            if (string.IsNullOrEmpty(config.Discord.Token) || config.Discord.Token == "insert your bot token here" || config.Discord.Token.Contains(' '))
            {
                throw new Exception($"TOKEN inválido verifique seu token no arquivo em {ConfigPath}");
            }
        }
        private async void CreateConfigFile()
        {
            if (!Directory.Exists(KansasPaths.ConfigPath)) { CreateConfigDirectory(); }
            string json = JsonConvert.SerializeObject(new KansasConfig(), Formatting.Indented);

            using (var sw = new StreamWriter(new FileInfo(ConfigPath).Create(), KansasUtilities.UTF8))
            {
                await sw.WriteAsync(json);
                await sw.FlushAsync();
                sw.Close();
                sw.Dispose();
            }
        }
        private void CreateConfigDirectory()
        {
            Directory.CreateDirectory(KansasPaths.ConfigPath);
        }
    }
}
