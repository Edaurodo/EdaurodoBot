using EdaurodoBot.rsc.utils;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.core.config
{
    public sealed class EdaurodoConfigLoader
    {
        private string _configFile;
        private EdaurodoConfigLoader() => _configFile = Path.Combine(new[] { EdaurodoPaths.Config, "edaurodo.config.json" });

        public static async Task<ConfigEdaurodo> LoadConfigAsync()
        {
            var loader = new EdaurodoConfigLoader();

            _ = Directory.Exists(EdaurodoPaths.Config) ? null : Directory.CreateDirectory(EdaurodoPaths.Config);
            _ = Directory.Exists(EdaurodoPaths.Data) ? null : Directory.CreateDirectory(EdaurodoPaths.Data);

            FileInfo configFile = new FileInfo(loader._configFile);

            if (!configFile.Exists) { await loader.SerializeNewConfigFile(configFile); }
            return loader.DeserializeConfig(configFile).GetAwaiter().GetResult();
        }

        private async Task SerializeNewConfigFile(FileInfo file)
        {
            string json = JsonConvert.SerializeObject(new ConfigEdaurodo(null, null, null), Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(file.Create(), EdaurodoUtilities.UTF8))
            {
                await sw.WriteLineAsync(json);
                await sw.FlushAsync();
                sw.Close();
            }
        }
        private async Task<ConfigEdaurodo> DeserializeConfig(FileInfo file)
        {
            string json = "{}";
            using (StreamReader sr = new StreamReader(file.OpenRead(), EdaurodoUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
            }
            return ValidateEdaurodoConfig(JsonConvert.DeserializeObject<ConfigEdaurodo>(json)).Result;
        }
        private Task<ConfigEdaurodo> ValidateEdaurodoConfig(ConfigEdaurodo? config)
        {
            if (config is null || config.Discord is null || string.IsNullOrWhiteSpace(config.Discord.Token))
            { throw new Exception("Ocorreu um erro na verificação do arquivo de configuração do bot, verifique o arquivo e reinicie a aplicação"); }
            if (string.IsNullOrWhiteSpace(config.Discord.Token) || config.Discord.Token.Contains(' '))
            { throw new Exception("É necessário especificar um token válido para sua aplicação, você deve criar uma aplicação em: 'https://discord.com/developers/applications' para obter um token!"); }
            if (config.Music.Use && (string.IsNullOrWhiteSpace(config.Music.YoutubeApi.ApiKey) || config.Music.YoutubeApi.ApiKey.Contains(' ')))
            {
                Console.WriteLine(config.Music.YoutubeApi.ApiKey);
                throw new Exception("É necessário especificar uma apikey válida nas configurações do 'MusicModule'"); }

            return Task.FromResult(config);
        }
    }
}