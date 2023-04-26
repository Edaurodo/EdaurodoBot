using EdaurodoBot.rsc.utils;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.core.data
{
    public sealed class EdaurodoConfigLoader
    {
        private string ConfigFile = Path.Combine(new[] { EdaurodoPaths.Config, "edaurodo.config.json" });

        private EdaurodoConfigLoader() { }

        public static async Task<EdaurodoConfig> LoadConfigAsync()
        {
            var loader = new EdaurodoConfigLoader();

            if (!Directory.Exists(EdaurodoPaths.Config)) { Directory.CreateDirectory(EdaurodoPaths.Config); }
            if (!Directory.Exists(EdaurodoPaths.Data)){ Directory.CreateDirectory(EdaurodoPaths.Data); }

            FileInfo configFile = new FileInfo(loader.ConfigFile);

            if (!configFile.Exists)
            {
                await loader.SerializeNewConfigFile(configFile);
                throw new Exception($"Um novo arquivo de configuração foi criado, configure o bot a reinicie o aplicativo!\n{configFile.FullName}");
            }

            return loader.DeserializeConfig(configFile).GetAwaiter().GetResult();
        }

        private async Task SerializeNewConfigFile(FileInfo file)
        {
            string json = JsonConvert.SerializeObject(new EdaurodoConfig(), Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(file.Create(), EdaurodoUtilities.UTF8))
            {
                await sw.WriteLineAsync(json);
                await sw.FlushAsync();
                sw.Close();
            }
        }
        private async Task<EdaurodoConfig> DeserializeConfig(FileInfo file)
        {
            string json = "{}";
            using (StreamReader sr = new StreamReader(file.OpenRead(), EdaurodoUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
            }
            return ValidateEdaurodoConfig(JsonConvert.DeserializeObject<EdaurodoConfig>(json)).Result;
        }
        private Task<EdaurodoConfig> ValidateEdaurodoConfig(EdaurodoConfig? config)
        {
            if (config != null && config.Discord != null && config.Discord.MessageCacheSize != null && config.Discord.UseInteractivity != null && config.Discord.Token != null)
            {
                if (!string.IsNullOrWhiteSpace(config.Discord.Token) && !config.Discord.Token.Contains(' '))
                {
                        return Task.FromResult(config);
                }
                else { throw new Exception("É necessário adicionar um token para seu bot, você deve criar uma aplicação em: https://discord.com/developers/applications"); }
            }
            else { throw new Exception("Ocorreu um erro na verificação do arquivo de configuração do bot, verifique o arquivo e reinicie a aplicação"); }
        }
    }
}