using DSharpPlus;
using EdaurodoBot.rsc.modules.allowlistmodule.data;
using EdaurodoBot.rsc.modules.allowlistmodule.utilities;
using EdaurodoBot.rsc.modules.genericmodule.commands.create.embed;
using EdaurodoBot.rsc.utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class AllowListConfigLoader
    {
        private string ConfigFile = Path.Combine(new[] { AllowlistUtilities.PathConfig, "allowlist.cfg.json" });
        private string DataFile = Path.Combine(new[] { AllowlistUtilities.PathData, "allowlist.db.json" });

        private DiscordClient Client;
        public AllowListConfigLoader(DiscordClient client)
        {
            Client = client;
        }
        public async Task<List<AllowlistData>> LoadDataAsync()
        {
            if (!Directory.Exists(AllowlistUtilities.PathData)) { Directory.CreateDirectory(AllowlistUtilities.PathData); }

            FileInfo dataFile = new FileInfo(this.DataFile);

            if (!dataFile.Exists)
            {
                await SerializeNewDataFile(dataFile);
            }
            return await DeserializeData(dataFile);
        }
        public async Task<AllowlistConfig> LoadConfigAsync()
        {
            if (!Directory.Exists(AllowlistUtilities.PathConfig)) { Directory.CreateDirectory(AllowlistUtilities.PathConfig); }

            FileInfo configFile = new FileInfo(ConfigFile);

            if (!configFile.Exists)
            {
                await SerializeNewConfigFile(configFile);
                throw new Exception();
            }
            return await DeserializeConfig(configFile);
        }
        private async Task SerializeNewConfigFile(FileInfo file)
        {
            Client.Logger.LogError(new EventId(777, "ConfigLoader"), "Arquivo não existe: 'allowlist.cfg.json' criado novo arquivo de configuração, configure o modulo e reinicie a aplicação");

            AllowlistConfig config = new AllowlistConfig(
                use: false,
                changename: false,
                reprovedwaittime: 60,
                roles: new AllowlistRoles(),
                channels: new AllowlistChannels(),
                questions: new AllowlistQuestion[]
                {
                    new AllowlistQuestion(
                        question: "Escreva aqui sua pergunta!",
                        alternatives: new[] {
                            "Aqui as alternativas para sua pergunta!",
                            "Cada pergunta pode conter até no maximo 25 alternativas!",
                            "As alternativas começam a contar a partir de 1!",
                            "Esta aqui é a 4ª alternativa por exemplo!"},
                        correctanswer: 4),
                    new AllowlistQuestion(
                        question: "Está é a segunda pergunta!",
                        alternatives: new[] {
                            "Na primeira pergunta a respota 4 era a correta",
                            "Todas suas perguntas ira seguir o mesmo padrão",
                            "Lembe-se não a limite de perguntas",
                            "Mas o limite de alternativas são de 25"},
                        correctanswer: 1)
                },
                messages: new AllowlistMessages(
                    mainmessage: new MainMessage(
                        buttonlink: "https://youtu.be/Sagg08DrO5U?t=0",
                        content: null,
                        embed: null)));

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(file.Create(), EdaurodoUtilities.UTF8))
            {
                await sw.WriteAsync(json);
                await sw.FlushAsync();
                sw.Close();
            }
        }
        private async Task SerializeNewDataFile(FileInfo file)
        {
            string json = "[]";

            using (StreamWriter sw = new StreamWriter(file.Create(), EdaurodoUtilities.UTF8))
            {
                await sw.WriteAsync(json);
                await sw.FlushAsync();
                sw.Close();
            }
        }
        private async Task<AllowlistConfig> DeserializeConfig(FileInfo file)
        {
            string json = "{}";
            using (StreamReader sr = new StreamReader(file.OpenRead(), EdaurodoUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
            }
            return await ValidateConfig(JsonConvert.DeserializeObject<AllowlistConfig>(json));
        }
        private async Task<List<AllowlistData>> DeserializeData(FileInfo file)
        {
            string json = "[]";
            using (StreamReader sr = new StreamReader(file.OpenRead(), EdaurodoUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
            }
            return await ValidateData(JsonConvert.DeserializeObject<List<AllowlistData>>(json));
        }
        private Task<List<AllowlistData>> ValidateData(List<AllowlistData>? data)
        {
            if (data != null && data.Count > 0)
            {
                return Task.FromResult(data);
            }
            return Task.FromResult(new List<AllowlistData>());
        }
        public Task<AllowlistConfig> ValidateConfig(AllowlistConfig? config)
        {
            if (config != null)
            {
                if (config.Use)
                {
                    if (config.Roles != null && config.Roles.ReaderId != null && config.Roles.ApprovedId != null && config.Roles.ReprovedId != null && config.Roles.AllowlistSentId != null && config.Roles.WaitingInterviewId != null)
                    {
                        if (config.Channels != null && config.Channels.CategoryId != null && config.Channels.MainId != null && config.Channels.ApprovedId != null && config.Channels.ReprovedId != null && config.Channels.InterviewId != null)
                        {
                            if (config.Questions != null && config.Questions.Length > 0)
                            {
                                foreach (var question in config.Questions)
                                {
                                    if (question.Question == null || question.Alternatives == null || question.Alternatives.Length < 1 || question.CorrectAnswer == null || question.CorrectAnswer < 1 || question.CorrectAnswer > question.Alternatives.Length)
                                    {
                                        Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'questions' no arquivo 'allowlist.cfg.json' É NULO OU FORA DE CONTEXTO!");
                                        throw new Exception();
                                    }
                                }
                                if (config.Messages != null && config.Messages.MainMessage != null)
                                {
                                    AllowlistConfig value = new AllowlistConfig(
                                        use: config.Use,
                                        changename: config.UseChangeName,
                                        reprovedwaittime: config.ReprovedWaitTime,
                                        roles: config.Roles,
                                        channels: config.Channels,
                                        questions: config.Questions,
                                        messages: config.Messages
                                        );
                                    return Task.FromResult(value);
                                }
                                else { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'messages' no arquivo 'allowlist.cfg.json' É NULO!"); throw new Exception(); }
                            }
                            else { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'questions' no arquivo 'allowlist.cfg.json' É NULO!"); throw new Exception(); }
                        }
                        else { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'channels' no arquivo 'allowlist.cfg.json' É NULO!"); throw new Exception(); }
                    }
                    else { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'roles' no arquivo 'allowlist.cfg.json' É NULO!"); throw new Exception(); }
                }
                else { return Task.FromResult(config); }
            }
            else
            { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: NÃO FOI POSSIVEL ENCONTRAR O ARQUIVO'allowlist.cfg.json' EM \n{EdaurodoPaths.Config}"); throw new Exception(); }
        }
    }
}