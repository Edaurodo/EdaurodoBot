using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using EdaurodoBot.rsc.core;
using EdaurodoBot.rsc.modules.allowlistmodule.commands;
using EdaurodoBot.rsc.modules.allowlistmodule.config;
using EdaurodoBot.rsc.modules.allowlistmodule.data;
using EdaurodoBot.rsc.modules.allowlistmodule.enums;
using EdaurodoBot.rsc.modules.allowlistmodule.utilities;
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
        public ConfigAllowlist? Config { get; private set; }

        public AllowlistService(EdaurodoMain bot)
        {
            Bot = bot;
            Bot.Client.Ready += Allowlist_Ready;
            Data = new List<AllowlistData>();
            DataInternalLock = new SemaphoreSlim(1, 1);
        }
        private async Task SaveData()
        {
            await DataInternalLock.WaitAsync();
            try
            {
                Internaldata = Data;
                DataSerialized = JsonConvert.SerializeObject(Internaldata, Formatting.Indented);
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
        private Task Allowlist_ComponentInteractionCreated(DiscordClient c, ComponentInteractionCreateEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                ulong userId = s.User.Id;
                switch (s.Id)
                {
                    case "btn_AlStart":
                        if (!Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            Data.Add(new AllowlistData(s.User));
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(s.Interaction);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == userId), Config, s.Guild);
                        }
                        else
                        {
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(s.Interaction);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == userId), Config, s.Guild);
                        }
                        break;
                    case "select_AlAlternativesResponse":
                        if (Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(s.Interaction);
                            await Data.Find(_ => _.DiscordUser.Id == userId).SubmitResponse(int.Parse(s.Values[0]));
                            await Data.Find(_ => _.DiscordUser.Id == userId).IncrementCurrentQuestion();
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == userId), Config, s.Guild);
                        }
                        break;
                    case "btn_openRealInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(s.Interaction);
                            await Allowlist.OpenRealInfoModal(Data.Find(_ => _.DiscordUser.Id == userId));
                        }
                        break;
                    case "btn_openCharInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(s.Interaction);
                            await Allowlist.OpenCharInfoModal(Data.Find(_ => _.DiscordUser.Id == userId));
                        }
                        break;
                    case "btn_AlApproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var _id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == _id).UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistApproved(Data.Find(_ => _.DiscordUser.Id == _id), Config, s.Guild);
                            Data.Remove(Data.Find(_ => _.DiscordUser.Id == _id));
                            await SaveData();
                        }
                        break;
                    case "btn_AlReproved":
                        if (ulong.TryParse(s.Channel.Name.Substring(s.Channel.Name.IndexOf('-') + 1), out var __id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == __id).UpdateInteraction(s.Interaction);
                            await Allowlist.AllowlistReprovedModal(Data.Find(_ => _.DiscordUser.Id == __id));
                        }
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private Task Allowlist_ModalSubmitted(DiscordClient c, ModalSubmitEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                ulong userId = s.Interaction.User.Id;
                DiscordGuild guild = s.Interaction.Guild;
                DiscordInteraction interaction = s.Interaction;
                switch (s.Interaction.Data.CustomId)
                {
                    case "modal_RealInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            s.Values.TryGetValue("AlRealName", out string realname);
                            s.Values.TryGetValue("AlRealAge", out string realage);
                            s.Values.TryGetValue("AlExp", out string rpexp);
                            await Data.Find(_ => _.DiscordUser.Id == userId).SubmitRealInfo(realname, realage, rpexp);
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(interaction);
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateCurrentForm(Form.Character);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == userId), Config, guild);
                        }
                        break;
                    case "modal_CharInfoModal":
                        if (Data.Exists(_ => _.DiscordUser.Id == userId))
                        {
                            s.Values.TryGetValue("AlCharName", out string charname);
                            s.Values.TryGetValue("AlCharAge", out string charage);
                            s.Values.TryGetValue("AlCharLore", out string charlore);
                            await Data.Find(_ => _.DiscordUser.Id == userId).SubmitCharInfo(charage, charname, charlore);
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateInteraction(interaction);
                            await Data.Find(_ => _.DiscordUser.Id == userId).UpdateCurrentForm(Form.None);
                            await Allowlist.ExecuteAsync(Data.Find(_ => _.DiscordUser.Id == userId), Config, guild);
                            await SaveData();
                        }
                        break;
                    case "modal_Reproved":
                        if (ulong.TryParse(interaction.Channel.Name.Substring(interaction.Channel.Name.IndexOf('-') + 1), out var _id))
                        {
                            await Data.Find(_ => _.DiscordUser.Id == _id).UpdateInteraction(interaction);
                            s.Values.TryGetValue("AlReasons", out string reasons);
                            await Allowlist.AllowlistReproved(Data.Find(_ => _.DiscordUser.Id == _id), Config, guild, reasons);
                            await SaveData();
                        }
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private Task Allowlist_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            _ = Task.Run(async () =>
            {
                var loader = new AllowListConfigLoader(Bot.Client);
                var data = await loader.LoadDataAsync();
                Config = await loader.LoadConfigAsync();
                if (Config != null && Config.Use)
                {
                    Bot.Client.GuildDownloadCompleted += Allowlist_GuildDownloadCompleted;
                    Bot.SlashCommands.RegisterCommands<AllowlistCommands>(Bot.Config.SecretConfig.GuildId);
                    Bot.SlashCommands.RefreshCommands();
                    Data = data;
                    Bot.Client.Logger.LogInformation(new EventId(777, "AllowlistService"), "AllowlistModule inicializado: OK!");
                }
            });
            return Task.CompletedTask;
        }
        private Task Allowlist_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs args)
        {
            Bot.Client.MessageCreated += Allowlist_MessageCreate;
            Bot.Client.GuildMemberUpdated += Allowlist_GuildMemberUpdated;
            Bot.Client.GuildMemberAdded += Allowlist_GuildMemberAdded;

            Bot.Client.ModalSubmitted += Allowlist_ModalSubmitted;
            Bot.Client.ComponentInteractionCreated += Allowlist_ComponentInteractionCreated;

            args.Guilds[Bot.Config.SecretConfig.GuildId].Channels[(ulong)Config.Channels.CategoryId].Children.ToList()
                .FindAll(_ => _.Type == 0 && _.Id != (ulong)Config.Channels.MainId && _.Id != (ulong)Config.Channels.ApprovedId && _.Id != (ulong)Config.Channels.ReprovedId && _.Id != (ulong)Config.Channels.InterviewId && _.Id != (ulong)Config.Channels.ChangeNameId)
                .ForEach(async _ => { await _.DeleteAsync(); });
            return Task.CompletedTask;
        }
        private Task Allowlist_MessageCreate(DiscordClient sender, MessageCreateEventArgs args)
        {
            _ = Task.Run(async () =>
            {
                if (args.Channel.Id == Config.Channels.ChangeNameId)
                {
                    await args.Guild.Members[args.Author.Id].ModifyAsync(_ =>
                    {
                        _.Nickname = args.Message.Content;
                        List<DiscordRole> roles = args.Guild.Members[args.Author.Id].Roles.ToList().FindAll(_ => _.Id != Config.Roles.ApprovedId && _.Id != Config.Roles.WaitingInterviewId);
                        roles.Add(args.Guild.Roles[(ulong)Config.Roles.ResidentId]);
                        _.Roles = roles;
                    });
                    await args.Message.CreateReactionAsync(DiscordEmoji.FromName(Bot.Client, ":white_check_mark:"));
                }
            });
            return Task.CompletedTask;
        }
        private Task Allowlist_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs args)
        {
            _ = Task.Run(async () =>
            {
                await args.Member.ModifyAsync(_ =>
                {
                    List<DiscordRole> roles = args.Member.Roles.ToList();
                    roles.Add(args.Guild.GetRole((ulong)Config.Roles.DefaultId));
                    _.Roles = roles;
                });
            });
            return Task.CompletedTask;
        }
        private Task Allowlist_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs args)
        {
            _ = Task.Run(async () =>
            {
                DiscordRole role = args.Guild.GetRole((ulong)Config.Roles.ReaderId);

                if ((!args.MemberBefore.Roles.Contains(role) && args.MemberAfter.Roles.Contains(role)) || (args.MemberBefore.Roles.Contains(role) && !args.MemberAfter.Roles.Contains(role)))
                {
                    DiscordGuild guild = args.Guild;
                    List<DiscordMember> readers = guild.Members.Values.ToList().FindAll(_ => _.Roles.Contains(role));
                    List<DiscordChannel> readerCategories = guild.Channels.Values.ToList().FindAll(_ => _.IsCategory && _.Name.StartsWith("reader-"));

                    foreach (DiscordMember reader in readers)
                    {
                        if (!readerCategories.Exists(_ => _.Name == $"reader-{reader.Id}"))
                        {
                            readerCategories.Add(await guild.CreateChannelCategoryAsync(
                                $"reader-{reader.Id}",
                                new List<DiscordOverwriteBuilder>() {
                                    new DiscordOverwriteBuilder()
                                    .For(guild.EveryoneRole)
                                    .Deny(Permissions.AccessChannels),
                                    new DiscordOverwriteBuilder()
                                    .For(reader)
                                    .Deny(Permissions.SendMessages)
                                    .Allow(Permissions.AccessChannels)
                                    .Allow(Permissions.ReadMessageHistory)
                                }));
                        }
                    }

                    foreach (DiscordChannel category in readerCategories)
                    {
                        ulong id = ulong.Parse(category.Name.Substring(category.Name.IndexOf('-') + 1));
                        if (!readers.Exists(_ => _.Id == id))
                        {
                            if (category.Children.Count > 0)
                            {
                                readerCategories.Remove(category);
                                await category.ModifyAsync(_ => _.Name = "rearranging-channels");
                                List<DiscordChannel> channels = category.Children.ToList();

                                while (channels.Count > 0)
                                {
                                    DiscordChannel tempCategory = readerCategories.First();
                                    foreach (DiscordChannel temp in readerCategories)
                                    {
                                        if (temp.Children.Count < tempCategory.Children.Count) { tempCategory = temp; }
                                    }
                                    await channels.First().ModifyAsync(_ => _.Parent = tempCategory);
                                    channels.Remove(channels.First());
                                }
                            }
                            await category.DeleteAsync();
                        }
                    }
                }
            });
            return Task.CompletedTask;
        }
    }
}