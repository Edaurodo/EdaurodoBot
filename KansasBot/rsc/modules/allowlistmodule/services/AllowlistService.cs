using DSharpPlus;
using DSharpPlus.EventArgs;
using KansasBot.rsc.core;
using KansasBot.rsc.modules.allowlistmodule.commands;
using KansasBot.rsc.modules.allowlistmodule.config;
using KansasBot.rsc.modules.allowlistmodule.data;
using KansasBot.rsc.modules.allowlistmodule.enums;
using KansasBot.rsc.modules.allowlistmodule.utilities;
using KansasBot.rsc.utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

namespace KansasBot.rsc.modules.allowlistmodule.services
{
    public sealed class AllowlistService
    {
        public ConcurrentDictionary<ulong, AllowlistData> Data { get; private set; }
        private ConcurrentDictionary<ulong, AllowlistData>? InternalData;
        public KansasMain Bot { get; }
        public AllowlistConfig? Config { get; private set; }
        public AllowListConfigLoader? ConfigLoader { get; private set; }

        public AllowlistService(KansasMain bot)
        {
            Bot = bot;
            Bot.Client.Ready += Kansas_Ready;
            Data = new ConcurrentDictionary<ulong, AllowlistData>();
        }

        public Task SaveAllowlistData()
        {
            lock (InternalData)
            {
                InternalData = Data;

                string json = JsonConvert.SerializeObject(InternalData, Formatting.Indented);
                FileInfo file = new FileInfo(Path.Combine(new[] { AllowlistUtilities.DataPath, "allowlist.db.json" }));
                if (file == null || !file.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(file.Create(), Encoding.Unicode))
                    {
                        sw.WriteLine(json);
                        sw.Flush();
                        sw.Close();
                    }
                    return Task.CompletedTask;
                }
                using (StreamWriter sw = new StreamWriter(file.OpenWrite(), Encoding.Unicode))
                {
                    sw.WriteLine(json);
                    sw.Flush();
                    sw.Close();
                }
                return Task.CompletedTask;
            }
        }
        private Task Component_Interaction_Created(DiscordClient c, ComponentInteractionCreateEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                switch (s.Id)
                {
                    case "btn_AlStart":
                        if (!Data.ContainsKey(s.Interaction.User.Id))
                        {
                            if (Data.TryAdd(s.Interaction.User.Id, new AllowlistData(s.Interaction)))
                            {
                                await Allowlist.ExecuteAsync(Data[s.Interaction.User.Id], Config);
                            }
                        }
                        else
                        {
                            await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                            await Allowlist.ExecuteAsync(Data[s.Interaction.User.Id], Config);
                        }
                        break;
                    case "btn_AlApproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var id))
                        {
                            await Data[id].UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistApproved(Data[id], Config);
                        }
                        break;
                    case "btn_AlReproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var idr))
                        {
                            await Data[idr].UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistReprovedModal(Data[idr]);
                        }
                        break;
                    case "btn_openRealInfoModal":
                        await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                        await Allowlist.OpenRealInfoModal(s.Interaction);
                        break;
                    case "btn_openCharInfoModal":
                        await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                        await Allowlist.OpenCharInfoModal(s.Interaction);
                        break;
                    case "select_AlAlternativesResponse":
                        if (Data.ContainsKey(s.Interaction.User.Id))
                        {
                            await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                            await Data[s.Interaction.User.Id].SubmitResponse(int.Parse(s.Values[0]));
                            await Data[s.Interaction.User.Id].IncrementCurrentQuestion();
                            await Allowlist.ExecuteAsync(Data[s.Interaction.User.Id], Config);
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
                        if (Data.ContainsKey(s.Interaction.User.Id))
                        {
                            s.Values.TryGetValue("AlRealName", out string realname);
                            s.Values.TryGetValue("AlRealAge", out string realage);
                            s.Values.TryGetValue("AlExp", out string rpexp);
                            await Data[s.Interaction.User.Id].UpdateCurrentForm(Form.Character);
                            await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                            await Data[s.Interaction.User.Id].SubmitRealInfo(realname, realage, rpexp);
                            await Allowlist.ExecuteAsync(Data[s.Interaction.User.Id], Config);
                        }
                        break;
                    case "modal_CharInfoModal":
                        if (Data.ContainsKey(s.Interaction.User.Id))
                        {
                            s.Values.TryGetValue("AlCharName", out string charname);
                            s.Values.TryGetValue("AlCharAge", out string charage);
                            s.Values.TryGetValue("AlCharLore", out string charlore);
                            await Data[s.Interaction.User.Id].UpdateCurrentForm(Form.None);
                            await Data[s.Interaction.User.Id].UpdateInteraction(s.Interaction);
                            await Data[s.Interaction.User.Id].SubmitCharInfo(charage, charname, charlore);
                            await Allowlist.ExecuteAsync(Data[s.Interaction.User.Id], Config);
                        }
                        break;
                    case "modal_Reproved":
                        if (ulong.TryParse(s.Interaction.Channel.Name.Substring(s.Interaction.Channel.Name.IndexOf('-') + 1), out var id))
                        {
                            if (Data.ContainsKey(id))
                            {
                                await Data[id].UpdateInteraction(s.Interaction);
                                s.Values.TryGetValue("AlReasons", out string reasons);
                                await Allowlist.AllowlistReproved(Data[id], Config, reasons);
                            }
                        }
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private async Task Kansas_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            ConfigLoader = new AllowListConfigLoader(Bot.Client);
            Config = await ConfigLoader.LoadConfigAsync();
            if (Config != null)
            {
                if (Config.Use)
                {
                    Bot.SlashCommands.RegisterCommands<AllowlistCommands>();
                    Bot.SlashCommands.RefreshCommands();
                    Bot.Client.ComponentInteractionCreated += Component_Interaction_Created;
                    Bot.Client.ModalSubmitted += Modal_Submitted;
                    Bot.Client.Logger.LogInformation(new EventId(700, "AllowlistService"), "AllowlistModule OK;");
                }
            }
            else
            {
                Bot.Client.Logger.LogCritical(new EventId(777, "AllowlistService"), $"ERRO NAS CONFIGURAÇÔES: NÃO FOI POSSIVEL ENCONTRAR O ARQUIVO'allowlist.cfg.json' EM \n{KansasPaths.ConfigPath}");
                await Bot.Client.DisconnectAsync();
                Bot.Client.Dispose();
                throw new Exception();
            }
        }
    }
}