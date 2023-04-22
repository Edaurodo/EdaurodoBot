﻿using DSharpPlus;
using DSharpPlus.EventArgs;
using KansasBot.rsc.core;
using KansasBot.rsc.core.data;
using KansasBot.rsc.modules.whitelistmodule.commands;
using KansasBot.rsc.modules.whitelistmodule.config;
using KansasBot.rsc.modules.whitelistmodule.data;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace KansasBot.rsc.modules.whitelistmodule.services
{
    public sealed class AllowlistService
    {
        public KansasMain Bot { get; }
        public AllowListConfig? Config { get; private set; }
        public AllowListConfigLoader? ConfigLoader { get; private set; }


        public ConcurrentDictionary<ulong, AllowlistData> Data { get; private set; }

        public AllowlistService(KansasMain bot)
        {
            Bot = bot;
            Bot.Client.Ready += Kansas_Ready;
            Data = new ConcurrentDictionary<ulong, AllowlistData>();
        }
        private Task Component_Interaction_Created(DiscordClient c, ComponentInteractionCreateEventArgs s)
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    switch (s.Id)
                    {
                        case "btn_AlStart":
                            if (!Data.ContainsKey(s.Interaction.User.Id))
                            {
                                if (Data.TryAdd(s.Interaction.User.Id, new AllowlistData(new Allowlist(this, s))))
                                {
                                    await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                    await Data[s.Interaction.User.Id].Allowlist.ExecuteAsync();
                                }
                            }
                            else
                            {
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].Allowlist.ExecuteAsync();
                            }
                            break;
                        case "btn_AlStartQuiz":
                            if (Data.ContainsKey(s.Interaction.User.Id))
                            {
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].Allowlist.ExecuteQuizAsync();
                            }
                            break;
                        case "btn_openRealInfoModal":
                            await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                            await Data[s.Interaction.User.Id].Allowlist.OpenRealInfoModal();
                            break;
                        case "btn_openCharInfoModal":
                            await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                            await Data[s.Interaction.User.Id].Allowlist.OpenCharInfoModal();
                            break;
                        case "select_AlAlternativesResponse":
                            if (Data.ContainsKey(s.Interaction.User.Id))
                            {
                                await Data[s.Interaction.User.Id].SubmitResponse(uint.Parse(s.Values[0]));
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].Allowlist.ExecuteQuizAsync();
                            }
                            break;
                    }
                });
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Message: {ex.Message}\n\n" +
                    $"Source: {ex.Source}\n\n" +
                    $"Inner Exception: {ex.InnerException}\n\n" +
                    $"Target Site: {ex.TargetSite}\n\n" +
                    $"Stack Trace: {ex.StackTrace}");
                Console.WriteLine(ex.ToString());
                return Task.CompletedTask;
            }
        }
        private Task Modal_Submitted(DiscordClient c, ModalSubmitEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    switch (s.Interaction.Data.CustomId)
                    {
                        case "modal_RealInfoModal":
                            if (Data.ContainsKey(s.Interaction.User.Id))
                            {
                                s.Values.TryGetValue("AlRealName", out string realname);
                                s.Values.TryGetValue("AlRealAge", out string realage);
                                s.Values.TryGetValue("AlExp", out string rpexp);
                                await Data[s.Interaction.User.Id].Allowlist.ChangeForm();
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].SubmitRealInfo(realname, realage, rpexp);
                                await Data[s.Interaction.User.Id].Allowlist.SendFormToUser();
                            }
                            break;
                        case "modal_CharInfoModal":
                            if (Data.ContainsKey(s.Interaction.User.Id))
                            {
                                s.Values.TryGetValue("AlCharName", out string charname);
                                s.Values.TryGetValue("AlCharAge", out string charage);
                                s.Values.TryGetValue("AlCharLore", out string charlore);
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].SubmitCharInfo(charage, charname, charlore);
                                await Data[s.Interaction.User.Id].Allowlist.FinalizeAllowlistAsync(true);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
            return Task.CompletedTask;
        }
        private async Task Kansas_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("\n DISPAROU O KANSAS_READY \n");

            ConfigLoader = new AllowListConfigLoader(Bot.Client);
            Config = await ConfigLoader.LoadConfigAsync();
            if (Config.Use == true)
            {
                if (!await ConfigLoader.ValidateConfig(Config))
                {
                    Bot.Client.Logger.LogWarning(new EventId(701, "AllowlistService"), "Não foi possível inicializar a AllowlistModule, verifique os arquivos de configuração e reinicie o Bot");
                    await Bot.Client.DisconnectAsync();
                    Bot.Client.Dispose();
                }
                else
                {
                    Bot.SlashCommands.RegisterCommands<AllowlistCommands>();
                    Bot.SlashCommands.RefreshCommands();
                    Bot.Client.ComponentInteractionCreated += Component_Interaction_Created;
                    Bot.Client.ModalSubmitted += Modal_Submitted;
                    Bot.Client.Logger.LogInformation(new EventId(700, "AllowlistService"), "AllowlistModule OK;");
                }
            }
        }
    }
}