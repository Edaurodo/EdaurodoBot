using DSharpPlus;
using KansasBot.rsc.modules.allowlistmodule.utilities;
using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using KansasBot.rsc.utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.allowlistmodule.config
{
    public sealed class AllowListConfigLoader
    {
        private string ConfigArchive = Path.Combine(new[] { AllowlistUtilities.ConfigPath, "allowlist.cfg.json" });
        private DiscordClient Client;

        public AllowListConfigLoader(DiscordClient client)
        {
            this.Client = client;
        }
        public async Task<AllowlistConfig> LoadConfigAsync()
        {
            if (!Directory.Exists(AllowlistUtilities.ConfigPath)) { Directory.CreateDirectory(AllowlistUtilities.ConfigPath); }
            if (!Directory.Exists(AllowlistUtilities.DataPath)) { Directory.CreateDirectory(AllowlistUtilities.DataPath); }


            FileInfo file = new FileInfo(ConfigArchive);
            if (file == null || !file.Exists)
            {
                await SerializeNewConfig(file);
                throw new Exception();
            }
            else { return await DeserializeConfig(file); }
        }
        private async Task SerializeNewConfig(FileInfo file)
        {
            Client.Logger.LogError(new EventId(777, "ConfigLoader"), "Arquivo não existe: 'allowlist.cfg.json' criado novo arquivo de configuração, configure o modulo e reinicie a aplicação");

            AllowlistConfig config = new AllowlistConfig(
                use: true,
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
                        embed: new Embed()
                        {
                            Color = "#2B2D31",
                            Title = new EmbedTitle() { Text = "Você ainda não configurou uma mensage para iniciar a AllowList" },
                            Description =
                            "### `Esta é a mensagem padrão da aplicação`\n\n" +
                            "> Navege pelos arquivos de configuração da aplicação\n" +
                            "> Encontre o arquivo 'allowlist.cfg.json' e procure o campo\n" +
                            "> 'messages' -> 'main_fixed_message' para altertar esta mensagem\n\n" +
                            "## `Use o comando \"/Create Embed\" para criar e obter o JSON`"
                        })
                ));

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(file.Create(), KansasUtilities.UTF8))
            {
                await sw.WriteLineAsync(json);
                await sw.FlushAsync();
                sw.Close();
            }
        }
        private async Task<AllowlistConfig> DeserializeConfig(FileInfo file)
        {
            string json = "{}";
            using (StreamReader sr = new StreamReader(file.OpenRead(), KansasUtilities.UTF8))
            {
                json = await sr.ReadToEndAsync();
                sr.Close();
            }
            return await ValidateConfig(JsonConvert.DeserializeObject<AllowlistConfig>(json));
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
                                    if (question.Question == null || question.Alternatives == null || question.Alternatives.Length < 1 || question.CorrectAnswer == null || (question.CorrectAnswer < 1 || question.CorrectAnswer > question.Alternatives.Length))
                                    {
                                        Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: ALGUM CAMPO EM 'questions' no arquivo 'allowlist.cfg.json' É NULO OU FORA DE CONTEXTO!");
                                        throw new Exception();
                                    }
                                }
                                if (config.Messages != null && config.Messages.MainMessage != null)
                                {
                                    AllowlistConfig value = new AllowlistConfig(
                                        use: config.Use,
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
            { Client.Logger.LogCritical(new EventId(777, "ConfigLoader"), $"ERRO NAS CONFIGURAÇÔES: NÃO FOI POSSIVEL ENCONTRAR O ARQUIVO'allowlist.cfg.json' EM \n{KansasPaths.ConfigPath}"); throw new Exception(); }
        }
    }
}