using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.whitelistmodule.services;

namespace KansasBot.rsc.modules.whitelistmodule.commands.update
{
    public sealed class UpdateReadersChannels
    {
        private InteractionContext Context;
        private AllowlistService Service;
        private DiscordGuild Guild;
        private List<DiscordMember>? Readers;
        private List<DiscordChannel>? ReadersCategoryes;
        private DiscordRole Role;
        public UpdateReadersChannels(InteractionContext context, AllowlistService service)
        {
            Context = context;
            Service = service;
            Guild = Context.Guild;
            Role = Guild.GetRole((ulong)Service.Config.Roles.ReaderRoleId);
            Readers = Guild.GetAllMembersAsync().GetAwaiter().GetResult().ToList().FindAll(_ => _.Roles.Contains(Role));
            ReadersCategoryes = Guild.GetChannelsAsync().GetAwaiter().GetResult().ToList().FindAll(_ => _.IsCategory == true && _.Name.StartsWith("reader-"));
        }
        public async Task ExecuteAsync()
        {
            await Context.DeferAsync(true);
            await Context.DeleteResponseAsync();
            await ExecuteUpdate();
        }

        private async Task ExecuteUpdate()
        {
            foreach(var category in ReadersCategoryes)
            {
                ulong id = ulong.Parse(category.Name.Substring(category.Name.IndexOf('-') + 1));
                if (!Readers.Exists(_ => _.Id == id))
                {
                    if(category.Children.Count > 0)
                    {
                        ReadersCategoryes.Remove(category);
                        await category.ModifyAsync(_ => _.Name = "rearranging-channels");
                        await RearrangeChannels(category.Children.ToList());
                    }
                    await category.DeleteAsync();
                }
            }

            foreach (var reader in Readers)
            {
                if (!ReadersCategoryes.Exists(_ => _.Name == $"reader-{reader.Id}"))
                {
                    await Guild.CreateChannelCategoryAsync(
                        $"reader-{reader.Id}",
                        new List<DiscordOverwriteBuilder>() {
                        new DiscordOverwriteBuilder()
                        .For(Guild.EveryoneRole)
                        .Deny(Permissions.AccessChannels),
                        new DiscordOverwriteBuilder()
                        .For(reader)
                        .Allow(Permissions.AccessChannels)
                        .Allow(Permissions.ReadMessageHistory)
                        .Deny(Permissions.SendMessages)
                        });
                }
            }
        }
        private async Task RearrangeChannels(List<DiscordChannel> channels)
        {
            while (channels.Count > 0)
            {
                var tempReaderCategory = ReadersCategoryes.First();
                foreach (var category in ReadersCategoryes)
                {
                    if (category.Children.Count < tempReaderCategory.Children.Count) { tempReaderCategory = category; }
                }
                await channels.First().ModifyAsync(_ => _.Parent = tempReaderCategory);
                channels.RemoveAt(0);
            }
        }
    }
}
