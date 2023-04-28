using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using EdaurodoBot.rsc.core;
using EdaurodoBot.rsc.modules.allowlistmodule.commands;
using EdaurodoBot.rsc.modules.allowlistmodule.config;
using EdaurodoBot.rsc.modules.allowlistmodule.data;
using EdaurodoBot.rsc.modules.allowlistmodule.enums;
using EdaurodoBot.rsc.modules.allowlistmodule.utilities;
using EdaurodoBot.rsc.utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.services
{
    public sealed class AllowlistService
    {
        private SemaphoreSlim DataInternalLock;
        private List<AllowlistData>? Internaldata;
        private string? DataSerialized;

        public List<AllowlistData> Data { get; private set; }

        public EdaurodoMain Bot { get; }
        public AllowlistConfig? Config { get; private set; }
        public AllowListConfigLoader? ConfigLoader { get; private set; }

        public AllowlistService(EdaurodoMain bot)
        {
            Bot = bot;
            Bot.Client.Ready += Edaurodo_Ready;
            Bot.Client.GuildDownloadCompleted += Edaurodo_GuildDownloadCompleted;
            Data = new List<AllowlistData>();
            DataInternalLock = new SemaphoreSlim(1, 1);
        }
        private async Task SaveData()
        {
            await DataInternalLock.WaitAsync();
            try
            {
                Internaldata = Data;
                DataSerialized = JsonConvert.SerializeObject(Data, Formatting.Indented);
                FileInfo file = new FileInfo(Path.Combine(new[] { AllowlistUtilities.PathData, "allowlist.db.json" }));
                file.Delete();
                using (StreamWriter sw = file.CreateText())
                {
                    await sw.WriteAsync(DataSerialized);
                    await sw.FlushAsync();
                    sw.Close();
                }
                Internaldata = null;
                DataSerialized = null;
            }
            finally
            {
                DataInternalLock.Release();
            }
        }
        private Task Component_Interaction_Created(DiscordClient c, ComponentInteractionCreateEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                switch (s.Id)
                {
                    case "btn_AlStart":
                        if (!Data.Exists(_ => _.DiscordUser.Id == s.Interaction.User.Id))
                        {
                            Data.Add(new AllowlistData(s.Interaction.User));
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id), Config, s.Guild);
                        }
                        else
                        {
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id), Config, s.Guild);
                        }
                        break;
                    case "btn_AlApproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == id).UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistApproved(Data.Find(_ => _.DiscordUser.Id == id), Config, s.Guild);
                            Data.Remove(Data.Find(_ => _.DiscordUser.Id == id));
                            await SaveData();
                        }
                        break;
                    case "btn_AlReproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var id2))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == id2).UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistReprovedModal(Data.Find(_ => _.DiscordUser.Id == id2));
                        }
                        break;
                    case "btn_openRealInfoModal":
                        await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                        await Allowlist.OpenRealInfoModal(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id));
                        break;
                    case "btn_openCharInfoModal":
                        await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                        await Allowlist.OpenCharInfoModal(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id));
                        break;
                    case "select_AlAlternativesResponse":
                        if (Data.Exists(_ => _.DiscordUser.Id == s.Interaction.User.Id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).SubmitResponse(int.Parse(s.Values[0]));
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).IncrementCurrentQuestion();
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id), Config, s.Guild);
                        }
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private Task Modal_Submitted(DiscordClient c, ModalSubmitEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                switch (s.Interaction.Data.CustomId)
                {
                    case "modal_RealInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == s.Interaction.User.Id))
                        {
                            s.Values.TryGetValue("AlRealName", out string realname);
                            s.Values.TryGetValue("AlRealAge", out string realage);
                            s.Values.TryGetValue("AlExp", out string rpexp);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateCurrentForm(Form.Character);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).SubmitRealInfo(realname, realage, rpexp);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id), Config, s.Interaction.Guild);
                        }
                        break;
                    case "modal_CharInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == s.Interaction.User.Id))
                        {
                            s.Values.TryGetValue("AlCharName", out string charname);
                            s.Values.TryGetValue("AlCharAge", out string charage);
                            s.Values.TryGetValue("AlCharLore", out string charlore);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateCurrentForm(Form.None);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).UpdateInteraction(s.Interaction);
                            await Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id).SubmitCharInfo(charage, charname, charlore);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == s.Interaction.User.Id), Config, s.Interaction.Guild);
                            await SaveData();
                        }
                        break;
                    case "modal_Reproved":
                        if (ulong.TryParse(s.Interaction.Channel.Name.Substring(s.Interaction.Channel.Name.IndexOf('-') + 1), out var id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == id).UpdateInteraction(s.Interaction);
                            s.Values.TryGetValue("AlReasons", out string reasons);
                            await Allowlist.AllowlistReproved(Data.Find(_ => _.DiscordUser.Id == id), Config, s.Interaction.Guild, reasons);
                            await SaveData();
                        }
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private async Task Edaurodo_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            ConfigLoader = new AllowListConfigLoader(Bot.Client);
            Config = await ConfigLoader.LoadConfigAsync();
            var data = await ConfigLoader.LoadDataAsync();
            if (Config != null)
            {
                if (Config.Use)
                {
                    Bot.SlashCommands.RegisterCommands<AllowlistCommands>(Bot.Config.InternalConfig.GuildId);
                    Bot.SlashCommands.RefreshCommands();
                    Bot.Client.ComponentInteractionCreated += Component_Interaction_Created;
                    Bot.Client.ModalSubmitted += Modal_Submitted;
                    Data = data;
                    Bot.Client.Logger.LogInformation(new EventId(777, "AllowlistService"), "AllowlistModule inicializado: tudo OK;");
                }
            }
            else
            {
                Bot.Client.Logger.LogCritical(new EventId(777, "AllowlistService"), $"ERRO NAS CONFIGURAÇÔES: NÃO FOI POSSIVEL ENCONTRAR O ARQUIVO 'allowlist.cfg.json' EM \n{EdaurodoPaths.Config}");
                await Bot.Client.DisconnectAsync();
                Bot.Client.Dispose();
                throw new Exception();
            }
        }
        private Task Edaurodo_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs args)
        {
            args.Guilds[Bot.Config.InternalConfig.GuildId].Channels[(ulong)Config.Channels.CategoryId].Children
                .ToList()
                .FindAll(_ => _.Type == ChannelType.Text && _.Id != Config.Channels.MainId && _.Id != Config.Channels.ApprovedId && _.Id != Config.Channels.ReprovedId && _.Id != Config.Channels.InterviewId)
                .ForEach(_ =>
                {
                    if (_ != null)
                    {
                        _.Guild.Channels[(ulong)Config.Channels.MainId]
                        .DeleteOverwriteAsync(_.Guild.Members[ulong.Parse(_.Name.Substring(_.Name.IndexOf('-') + 1))]);
                        _.DeleteAsync();
                    }
                });

            return Task.CompletedTask;
        }
    }
}