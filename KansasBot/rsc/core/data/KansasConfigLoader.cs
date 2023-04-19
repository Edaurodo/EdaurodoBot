using KansasBot.rsc.utils;
using Newtonsoft.Json;
namespace KansasBot.rsc.core.data
{
    public sealed class KansasConfigLoader
    {
        private string ConfigPath = Path.Combine(new[] { KansasPaths.ConfigPath, "config_kansas.json" });

        public async Task<KansasConfig> LoadConfigAsync()
        {
            if (!Directory.Exists(KansasPaths.ConfigPath)) { Directory.CreateDirectory(KansasPaths.ConfigPath); }

            FileInfo file = new FileInfo(ConfigPath);
            if (file == null || !file.Exists)
            {
                string js = JsonConvert.SerializeObject(new KansasConfig(), Formatting.Indented);
                using (FileStream fs = file.Create())
                using (StreamWriter sw = new StreamWriter(fs, KansasUtilities.UTF8))
                {
                    await sw.WriteLineAsync(js);
                    await sw.FlushAsync();
                }
                throw new ArgumentException(
                    "O Arquivo de configuração é inválido ou inexistente!\n" +
                    $"O arquivo foi criado, configure o bot em:...\n{ConfigPath}");
            }

            string json = "{}";
            using (FileStream fs = file.OpenRead())
            using (StreamReader sr = new StreamReader(fs, KansasUtilities.UTF8))
                json = await sr.ReadToEndAsync();

            var config = JsonConvert.DeserializeObject<KansasConfig>(json);
            ValidateConfig(config);
            return config;
        }
        private void ValidateConfig(KansasConfig config)
        {
            if (config.Discord == null)
            {
                throw new Exception($"Falha ao verificar o arquivo de configuração, verifique o arquivo em:...\n{ConfigPath}");
            }
            if (string.IsNullOrEmpty(config.Discord.Token) || config.Discord.Token == "insert your bot token here" || config.Discord.Token.Contains(' '))
            {
                throw new Exception($"O Token do bot é inválido verifique seu token no arquivo em:...\n{ConfigPath}");
            }
        }
    }
}
