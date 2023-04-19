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
                        case "btn_startwl":
                            if (!Data.ContainsKey(s.Interaction.User.Id))
                            {
                                Data.TryAdd(s.Interaction.User.Id, new AllowlistData(new Allowlist(this, s)));
                                Data[s.Interaction.User.Id].Allowlist.ExecuteAsync();
                                await s.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("oi").AsEphemeral(true));
                            }
                            else
                            {
                                await Data[s.Interaction.User.Id].Allowlist.ExecuteAsync();
                                await s.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("oi").AsEphemeral(true));
                            }
                            break;
                        case "btn_startquiz":
                            if (Data.ContainsKey(s.Interaction.User.Id))
                            {
                                Console.WriteLine("ENTROU NO IF");
                                await Data[s.Interaction.User.Id].Allowlist.UpdateInteraction(s.Interaction);
                                await Data[s.Interaction.User.Id].Allowlist.ExecuteQuizAsync();
                            }
                            else { Console.WriteLine("ENTROU NO ELSE"); }
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
                    Bot.Client.Logger.LogInformation(new EventId(700, "AllowlistService"), "AllowlistModule OK;");
                }
            }
        }
    }
}